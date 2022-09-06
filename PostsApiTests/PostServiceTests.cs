using PostsApi.Models.ViewModels;
using Moq;
using System;
using Xunit;
using PostsApi.GlobalErrorHandling;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using PostsApi.Models.Entities;
using PostsApi.Repositories.Generic;
using PostsApi.Services.Implementations;
using PostsApi.Models.Pagination;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace PostsApiTests
{
    public class PostServiceTests
    {
        public PostServiceTests()
        {
            webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.SetupProperty(property =>
                property.ContentRootPath,
                "C:\\Users\\arthu\\source\\repos\\PostsApi\\PostsApi"
            );

            postRepository = new Mock<IGenericRepository<Post>>();

            postService = new PostService(postRepository.Object, webHostEnvironment.Object);
        }

        private readonly Mock<IWebHostEnvironment> webHostEnvironment;
        private readonly Mock<IGenericRepository<Post>> postRepository;
        private readonly PostService postService;

        #region CreateOrRecommend Method Tests
        #region Exceptions Tests
        [Fact]
        public void CreateOrRecommend_WithNullPostCreateViewModel_ReturnsHttpResponseException()
        {
            var httpResponseException = new HttpResponseException(400, "Objeto post não pode ser nulo");

            Assert.Throws<HttpResponseException>(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", null)
            );
        }

        [Fact]
        public void CreateOrRecommend_WithNullDescriptionAndNullImage_ReturnsHttpResponseException()
        {
            var postCreateViewModel = new PostCreateViewModel { Description = null, Image = null };
            
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            Assert.Throws<HttpResponseException>(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );
        }

        [Fact]
        public void CreateOrRecommend_WithEmptyDescriptionAndNullImage_ReturnsHttpResponseException()
        {
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var postCreateViewModel = new PostCreateViewModel { Description = "", Image = null };

            Assert.Throws<HttpResponseException>(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );
        }

        [Fact]
        public void CreateOrRecommend_WithEmptyDescriptionAndImageWithNoLength_ReturnsHttpResponseException()
        {
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var postCreateViewModel = new PostCreateViewModel
            {
                Description = "",
                Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 0, "Data", "image.png")
            };

            Assert.Throws<HttpResponseException>(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );
        }

        [Fact]
        public void CreateOrRecommend_WithNullDescriptionAndImageWithNoLength_ReturnsHttpResponseException()
        {
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var postCreateViewModel = new PostCreateViewModel
            {
                Description = null,
                Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 0, "Data", "image.png")
            };

            Assert.Throws<HttpResponseException>(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );
        }
        #endregion

        #region Success Tests
        [Fact]
        public void CreateOrRecommend_WithDescriptionAndImage_ReturnsNothing()
        {
            var postCreateViewModel = new PostCreateViewModel
            {
                Description = "some description",
                Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 100, "Data", "image.png")
            };

            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var createOrRecommendException = Record.Exception(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );

            Assert.Null(createOrRecommendException);
        }
        
        [Fact]
        public void CreateOrRecommend_WithDescriptionAndNullImage_ReturnsNothing()
        {
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var postCreateViewModel = new PostCreateViewModel
            {
                Description = "some description",
                Image = null
            };

            var createOrRecommendException = Record.Exception(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );

            Assert.Null(createOrRecommendException);
        }

        [Fact]
        public void CreateOrRecommend_WithDescriptionAndImageWithNoLength_ReturnsNothing()
        {
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var postCreateViewModel = new PostCreateViewModel
            {
                Description = "some description",
                Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 0, "Data", "image.png")
            };

            var createOrRecommendException = Record.Exception(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );

            Assert.Null(createOrRecommendException);
        }

        [Fact]
        public void CreateOrRecommend_WithNullDescriptionAndImage_ReturnsNothing()
        {
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var postCreateViewModel = new PostCreateViewModel
            {
                Description = null,
                Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 100, "Data", "image.png")
            };

            var createOrRecommendException = Record.Exception(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );

            Assert.Null(createOrRecommendException);
        }

        [Fact]
        public void CreateOrRecommend_WithEmptyDescriptionAndImage_ReturnsNothing()
        {
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var postCreateViewModel = new PostCreateViewModel
            {
                Description = "",
                Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 100, "Data", "image.png")
            };

            var createOrRecommendException = Record.Exception(() =>
                postService.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );

            Assert.Null(createOrRecommendException);
        }
        #endregion
        #endregion

        #region Accept Method Tests
        #region Exceptions Tests
        [Fact]
        public void Accept_WithPostIdWichDoesNotExistsInDatabase_ReturnsHttpResponseException()
        {
            Guid invalidPostId = Guid.NewGuid();
            Guid passedInPostId = Guid.Empty;

            var userId = Guid.NewGuid();

            Assert.Throws<HttpResponseException>(() =>
                postService.Accept(invalidPostId, userId)
            );
        }
        #endregion

        #region Success Tests
        [Fact]
        public void Accept_WithExistingPostIdInDatabase_ReturnsNothing()
        {
            List<Post> posts = new List<Post>();
            Guid postId = Guid.Empty;
            Guid userId = Guid.NewGuid();
            Post fetchedPost = null;

            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.SetupProperty(property =>
                property.ContentRootPath,
                "C:\\Users\\arthu\\source\\repos\\PostsApi\\PostsApi"
            );

            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(property =>
                    property.Create(It.IsAny<Post>())
                )
                .Callback<Post>(passedInPost =>
                {
                    passedInPost.Id = Guid.NewGuid();
                    passedInPost.IsCreatedByAdmin = false;

                    postId = passedInPost.Id;

                    posts.Add(passedInPost);
                });

            postRepository
                .Setup(property =>
                    property.GetById(It.IsAny<Guid>())
                )
                .Callback<Guid>(passedInPostId =>
                {
                    fetchedPost = posts.Find(post => post.Id.Equals(passedInPostId));
                })
                .Returns(() => fetchedPost);

            postRepository
                .Setup(property => property.Update(It.IsAny<Post>()))
                .Callback<Post>(passedInPost =>
                {
                    int index = posts.FindIndex(post => post.Id.Equals(passedInPost.Id));
                    posts[index].AcceptedUserId = userId;
                    posts[index].AcceptedAt = DateTime.UtcNow;
                });

            var postService = new PostService(
                postRepository.Object,
                webHostEnvironment.Object
            );

            postService.CreateOrRecommend(
                userId,
                "Customer",
                new PostCreateViewModel
                {
                    Description =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                    Image = null
                }
            );

            var acceptException = Record.Exception(() => postService.Accept(postId, userId));

            Assert.Null(acceptException);
        }
        #endregion
        #endregion

        #region GetAll Method Tests
        #region Exceptions Tests
        [Fact]
        public void GetAll_WithNullSearchFilter_ReturnsHttpResponseException()
        {
            Guid userId = Guid.NewGuid();
            string searchFilter = null;

            Assert.Throws<HttpResponseException>(() =>
                postService.GetAll(
                    userId,
                    "Customer",
                    searchFilter,
                    "requestHost",
                    "requestPath",
                    new PaginationQueryParams(),
                    "paginationUrl"
               )
            );
        }

        [Fact]
        public void GetAll_WithEmptySearchFilter_ReturnsHttpResponseException()
        {
            Guid userId = Guid.NewGuid();
            string searchFilter = "";

            Assert.Throws<HttpResponseException>(() =>
                postService.GetAll(
                    userId,
                    "Customer",
                    searchFilter,
                    "requestHost",
                    "requestPath",
                    new PaginationQueryParams(),
                    "paginationUrl"
               )
            );
        }
        #endregion

        public int CheckPaginationPostsListEqualValues(int length, List<PostViewModel> expectedPosts, List<PostViewModel> paginationPosts)
        {
            int equalAmount = 0;

            if (expectedPosts.Count == length)
            {
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        if (expectedPosts[i].Id == paginationPosts[j].Id &&
                           expectedPosts[i].Description == paginationPosts[j].Description &&
                           expectedPosts[i].CanEdit == paginationPosts[j].CanEdit &&
                           expectedPosts[i].CreatedAt == paginationPosts[j].CreatedAt
                        )
                        {
                            equalAmount++;
                        }
                    }
                }
            }

            return equalAmount;
        }

        #region Success Tests
        [Fact]
        public void GetAll_WithSearchFilter_ReturnsPaginationResponsePostViewModel()
        {
            List<Post> posts = new List<Post>();
            Guid postId = Guid.Empty;
            Guid userId = Guid.NewGuid();
            Post fetchedPost = null;

            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.SetupProperty(property =>
                property.ContentRootPath,
                "C:\\Users\\arthu\\source\\repos\\PostsApi\\PostsApi"
            );

            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(property => property.Create(It.IsAny<Post>()))
                .Callback<Post>(passedInPost =>
                {
                    passedInPost.Id = Guid.NewGuid();
                    passedInPost.IsCreatedByAdmin = false;

                    postId = passedInPost.Id;

                    posts.Add(passedInPost);
                });

            postRepository
                .Setup(property => property.GetAll())
                .Returns(() => posts.AsQueryable<Post>());

            postRepository
                .Setup(property => property.Update(It.IsAny<Post>()))
                .Callback<Post>(passedInPost =>
                {
                    int index = posts.FindIndex(post => post.Id.Equals(passedInPost.Id));
                    posts[index].AcceptedUserId = userId;
                    posts[index].AcceptedAt = DateTime.UtcNow;
                });

            postRepository
                .Setup(property => property.GetById(It.IsAny<Guid>()))
                .Callback<Guid>(passedInPostId =>
                {
                    fetchedPost = posts.Find(post => post.Id.Equals(passedInPostId));
                })
                .Returns(() => fetchedPost);

            var postService = new PostService(
                postRepository.Object,
                webHostEnvironment.Object
            );

            // Insert some data
            Guid[] insertedPostsIds = new Guid[4];

            postService.CreateOrRecommend(userId, "Common", new PostCreateViewModel { Description = "Lorem", Image = null });
            insertedPostsIds[0] = postId;
            postService.CreateOrRecommend(userId, "Common", new PostCreateViewModel { Description = "Ipsum", Image = null });
            insertedPostsIds[1] = postId;
            postService.CreateOrRecommend(userId, "Common", new PostCreateViewModel { Description = "Dolor", Image = null });
            insertedPostsIds[2] = postId;
            postService.CreateOrRecommend(userId, "Common", new PostCreateViewModel { Description = "Sit", Image = null });
            insertedPostsIds[3] = postId;

            for(int i=0; i < insertedPostsIds.Length; i++)
            {
                postService.Accept(insertedPostsIds[i], userId);
            }

            string searchFilter = "all";

            PaginationResponse<PostViewModel> paginationResponse = null;

            Exception getAllException = Record.Exception(() =>
                paginationResponse = postService.GetAll(
                    userId,
                    "Common",
                    searchFilter,
                    "requestHost",
                    "requestPath",
                    new PaginationQueryParams(),
                    "paginationUrl"
               )
            );

            var expectedPosts = new List<PostViewModel>();

            for (int i=0; i < posts.Count; i++)
            {
                expectedPosts.Add
                (
                    new PostViewModel
                    {
                        Id = posts[i].Id,
                        Description = posts[i].Description,
                        CanEdit = true,
                        CreatedAt = posts[i].CreatedAt
                    }
                );
            }

            int equalAmount = CheckPaginationPostsListEqualValues(
                paginationResponse.TotalRecords,
                expectedPosts,
                paginationResponse.Data.ToList()
            );

            Assert.Null(getAllException);
            Assert.Equal(4, paginationResponse.TotalRecords);
            Assert.Null(paginationResponse.NextPage);
            Assert.Null(paginationResponse.PreviousPage);
            Assert.Equal(4, equalAmount);
        }
        #endregion
        #endregion
    }
}
