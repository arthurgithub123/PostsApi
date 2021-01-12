using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PostsApi.ErrorHandling;
using PostsApi.Models.Entities;
using PostsApi.Models.ViewModels;
using PostsApi.Repositories.Generic;
using PostsApi.Services.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace PostsApi.Services.Implementations
{
    public class PostService : IPostService
    {
        public PostService(IGenericRepository<Post> postRepository, IWebHostEnvironment webHostEnvironment)
        {
            _postRepository = postRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        private readonly IGenericRepository<Post> _postRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public void CreateOrRecommend(Guid userId, string userRole, PostViewModel postViewModel)
        {
            if(postViewModel == null)
            {
                throw new HttpResponseException(400, "Objeto post não pode ser nulo");
            }
            
            if(
                String.IsNullOrEmpty(postViewModel.Description) && 
                (
                 postViewModel.Image == null || (postViewModel.Image != null && postViewModel.Image.Length == 0)
                )
            )
            {
                throw new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");
            }

            Post post = new Post();

            post.Description = postViewModel.Description;

            if (postViewModel.Image != null && postViewModel.Image.Length > 0)
            {
                post.ImageName = SaveImage(postViewModel.Image);
            }
            
            if(userRole == "Administrator")
            {
                post.CreatedAt = DateTime.UtcNow;
                post.CreatorId = userId;
            }
            
            if(userRole == "Commom")
            {
                post.RecomendDate = DateTime.UtcNow;
                post.RecomendUserId = userId;
            }
            
            _postRepository.Create(post);
        }
        
        public IQueryable<PostViewModel> GetAll(Guid userId, string userRole, string filter, string requestHost, string requestPathBase)
        {
            if (String.IsNullOrEmpty(filter))
            {
                throw new HttpResponseException(400, "O filtro da pesquisa não pode estar vazio");
            }

            IQueryable<Post> posts = _postRepository.GetAll();
            
            if (userRole == "Administrator")
            {
                if (filter == "all")
                {
                    posts.OrderByDescending(post => post.CreatedAt);
                }

                if(filter == "recomendedNotAccepted")
                {
                    posts = posts
                        .Where(post => 
                            post.RecomendDate.HasValue && 
                            !post.AcceptedAt.HasValue
                        )
                        .OrderByDescending(post => post.RecomendDate);
                }

                if(filter == "recomendedAndAccepted")
                {
                    posts = posts
                        .Where(post => 
                            post.RecomendDate.HasValue && 
                            post.AcceptedAt.HasValue
                        )
                        .OrderByDescending(post => post.RecomendDate);
                }

                if(filter == "recomendDate")
                {
                    posts = posts
                        .Where(post => post.RecomendDate.HasValue)
                        .OrderByDescending(post => post.RecomendDate);
                }

                if(filter == "acceptedDate")
                {
                    posts = posts
                        .Where(post => post.AcceptedAt.HasValue)
                        .OrderByDescending(post => post.AcceptedAt);
                }

                if(filter == "onlyAdministratorsPosts")
                {
                    posts = posts
                        .Where(post => post.CreatedAt.HasValue)
                        .OrderByDescending(post => post.CreatedAt);
                }
            }

            if (userRole == "Commom")
            {
                if(filter == "all")
                {
                    posts = posts
                        .Where(post =>
                            post.CreatedAt.HasValue ||
                            post.AcceptedAt.HasValue
                        )
                        .OrderByDescending(post => post.CreatedAt);
                }

                if(filter == "accepted")
                {
                    posts = posts
                        .Where(post => 
                            post.AcceptedAt.HasValue && 
                            post.RecomendUserId.HasValue && post.RecomendUserId.Value == userId
                        )
                        .OrderByDescending(post => post.AcceptedAt);
                }

                if(filter == "notAccepted")
                {
                    posts = posts
                        .Where(post =>
                            !post.AcceptedAt.HasValue &&
                            post.RecomendUserId.HasValue && post.RecomendUserId == userId
                        )
                        .OrderByDescending(post => post.RecomendDate);
                }

                if(filter == "onlyCommomUsersPosts")
                {
                    posts = posts
                        .Where(post =>
                            post.RecomendDate.HasValue &&
                            post.AcceptedAt.HasValue
                        )
                        .OrderByDescending(post => post.RecomendDate);
                }

                if (filter == "onlyAdministratorsPosts")
                {
                    posts = posts
                        .Where(post => post.CreatedAt.HasValue)
                        .OrderByDescending(post => post.CreatedAt);
                }
            }

            var postsList = posts.Select(post => new PostViewModel
            {
                Id =  post.Id,
                Description = post.Description,
                CreatedAt = post.CreatedAt.HasValue ? post.CreatedAt : null,
                RecomendDate = post.RecomendDate.HasValue ? post.RecomendDate : null,
                ImageUrl = "https://" + requestHost + requestPathBase + "/Assets/Posts/Images/" + post.ImageName
            });

            return postsList;
        }

        private string SaveImage(IFormFile formFile)
        {
            string fileFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Assets");

            string imageFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;

            string folderPathAndImageFileName = Path.Combine(fileFolderPath, imageFileName);

            using (var fileStream = new FileStream(folderPathAndImageFileName, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

            return imageFileName;
        }
    }
}
