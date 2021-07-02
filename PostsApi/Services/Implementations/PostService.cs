using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PostsApi.GlobalErrorHandling;
using PostsApi.Models.Entities;
using PostsApi.Models.Pagination;
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
            
            post.CreatedAt = DateTime.UtcNow;
            post.CreatorId = userId;
            
            if(userRole == "Administrator")
            {
                post.IsCreatedByAdmin = true;
            }
            
            if(userRole == "Common")
            {
                post.IsCreatedByAdmin = false;
            }
            
            _postRepository.Create(post);
        }
        
        public void Accept(Guid id, Guid userId)
        {
            Post post = _postRepository.GetById(id);
            post.AcceptedUserId = userId;
            post.AcceptedAt = DateTime.UtcNow;

            _postRepository.Update(post);
        }

        public PaginationResponse<PostViewModel> GetAll(Guid userId, string userRole, string filter, string requestHost, string requestPathBase, PaginationQueryParams paginationQueryParams, string paginationUrl)
        {
            if (String.IsNullOrEmpty(filter))
            {
                throw new HttpResponseException(400, "O filtro da pesquisa não pode estar vazio");
            }

            IQueryable<Post> posts = _postRepository.GetAll();

            if (filter == "all")
            {
                posts = posts
                    .Where(post =>
                        post.IsCreatedByAdmin ||
                        (!post.IsCreatedByAdmin && post.AcceptedAt.HasValue)
                    )
                    .OrderByDescending(post => post.CreatedAt);
            }
            else if (filter == "onlyAdministratorsPosts")
            {
                posts = posts
                    .Where(post => post.IsCreatedByAdmin)
                    .OrderByDescending(post => post.CreatedAt);
            }
            else
            {
                if (userRole == "Administrator")
                {
                    if (filter == "recomendedNotAcceptedYet")
                    {
                        posts = posts
                            .Where(post =>
                                !post.IsCreatedByAdmin &&
                                !post.AcceptedAt.HasValue &&
                                !post.RejectedAt.HasValue
                            )
                            .OrderByDescending(post => post.CreatedAt);
                    }

                    if (filter == "recomendedAndAccepted")
                    {
                        posts = posts
                            .Where(post =>
                                !post.IsCreatedByAdmin &&
                                post.AcceptedAt.HasValue
                            )
                            .OrderByDescending(post => post.CreatedAt);
                    }

                    if (filter == "recomendDate")
                    {
                        posts = posts
                            .Where(post => !post.IsCreatedByAdmin)
                            .OrderByDescending(post => post.CreatedAt);
                    }

                    if (filter == "acceptedDate")
                    {
                        posts = posts
                            .Where(post => post.AcceptedAt.HasValue)
                            .OrderByDescending(post => post.AcceptedAt);
                    }
                }

                if (userRole == "Common")
                {
                    if (filter == "accepted")
                    {
                        posts = posts
                            .Where(post =>
                                post.AcceptedAt.HasValue &&
                                post.CreatorId == userId
                            )
                            .OrderByDescending(post => post.AcceptedAt);
                    }

                    if (filter == "notAccepted")
                    {
                        posts = posts
                            .Where(post =>
                                post.RejectedAt.HasValue &&
                                !post.AcceptedAt.HasValue &&
                                post.CreatorId == userId
                            )
                            .OrderByDescending(post => post.CreatedAt);
                    }
                    
                    if(filter == "myCreatedPosts")
                    {
                        posts = posts
                            .Where(post =>
                                !post.AcceptedAt.HasValue &&
                                !post.RejectedAt.HasValue &&
                                post.CreatorId == userId
                            )
                            .OrderByDescending(post => post.CreatedAt);
                    }

                    if (filter == "onlyCommonUsersPosts")
                    {
                        posts = posts
                            .Where(post =>
                                !post.IsCreatedByAdmin &&
                                post.AcceptedAt.HasValue
                            )
                            .OrderByDescending(post => post.CreatedAt);
                    }
                }
            }

            var totalRecords = posts.Count();

            var postsList = posts
                .Skip((paginationQueryParams.Page - 1) * paginationQueryParams.Per_Page)
                .Take(paginationQueryParams.Per_Page)
                .Select(post => new PostViewModel
                {
                    Id =  post.Id,
                    Description = post.Description,
                    CreatedAt = post.CreatedAt,
                    ImageUrl = !String.IsNullOrEmpty(post.ImageName) ? "https://" + requestHost + requestPathBase + "/Assets/Posts/Images/" + post.ImageName : null,
                    CanEdit = post.CreatorId == userId ? true : false
                });

            PaginationResponse<PostViewModel> paginationResponse = PaginationHelper<PostViewModel>.CreateResponseWithPagination(
                postsList,
                totalRecords,
                paginationQueryParams,
                paginationUrl
            );
            
            return paginationResponse;
        }

        public PostViewModel GetById(Guid id, Guid userId, string userRole, string requestHost, string requestPathBase)
        {
            Post post = _postRepository.GetById(id);

            if(post == null)
            {
                throw new HttpResponseException(400, "Post inexistente");
            }

            if(userRole == "Common")
            {
                if (post.CreatorId != userId)
                {
                    throw new HttpResponseException(400, "Não é possível alterar posts de outra pessoa");
                }
            }

            return new PostViewModel
            {
                Id = post.Id,
                Description = !String.IsNullOrEmpty(post.Description)
                    ? post.Description
                    : null,
                CreatedAt = post.CreatedAt,
                ImageUrl = !String.IsNullOrEmpty(post.ImageName)
                    ? "https://" + requestHost + requestPathBase + "/Assets/Posts/Images/" + post.ImageName
                    : null
            };
        }

        public void Edit(Guid id, Guid userId, string userRole, PostViewModel postViewModel)
        {
            Post post = _postRepository.GetById(id);

            if (post == null)
            {
                throw new HttpResponseException(400, "Post inexistente");
            }

            if (post.CreatorId != userId)
            {
                throw new HttpResponseException(400, "Não é possível alterar posts de outra pessoa");
            }

            if (String.IsNullOrEmpty(postViewModel.Description) &&
                String.IsNullOrEmpty(postViewModel.ImageUrl)
            )
            {
                throw new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");
            }

            post.Description = postViewModel.Description;

            if (!String.IsNullOrEmpty(postViewModel.ImageUrl) &&
                postViewModel.ImageUrl != "notChanged"
            )
            {
                if (!String.IsNullOrEmpty(post.ImageName))
                {
                    post.ImageName = DeleteCurrentImageAndSaveNew(postViewModel.Image, post.ImageName);
                }
                else
                {
                    post.ImageName = SaveImage(postViewModel.Image);
                }
            }

            if (String.IsNullOrEmpty(postViewModel.ImageUrl) &&
                !String.IsNullOrEmpty(post.ImageName)
            )
            {
                string fileFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Assets", "Posts", "Images");

                System.IO.File.Delete(Path.Combine(fileFolderPath, post.ImageName));

                post.ImageName = null;
            }

            post.EditorId = userId;
            post.EditedAt = DateTime.UtcNow;

            _postRepository.Update(post);
        }

        private string SaveImage(IFormFile formFile)
        {
            string fileFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Assets", "Posts", "Images");

            string imageFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;

            string folderPathAndImageFileName = Path.Combine(fileFolderPath, imageFileName);

            using (var fileStream = new FileStream(folderPathAndImageFileName, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

            return imageFileName;
        }

        private string DeleteCurrentImageAndSaveNew(IFormFile formFile, string existingUserProfileImage)
        {
            string fileFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Assets", "Posts", "Images");

            System.IO.File.Delete(Path.Combine(fileFolderPath, existingUserProfileImage));

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
