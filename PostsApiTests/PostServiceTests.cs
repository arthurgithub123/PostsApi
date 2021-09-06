using PostsApi.Models.ViewModels;
using Moq;
using PostsApi.Services.Interfaces;
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

namespace PostsApiTests
{
    public class PostServiceTests
    {
        #region CreateOrRecommend Method Tests
        #region Exceptions Tests
        [Fact]
        public void CreateOrRecommend_WithNullPostCreateViewModel_ReturnsHttpResponseException()
        {
            var httpResponseException = new HttpResponseException(400, "Objeto post não pode ser nulo");

            var postService = new Mock<IPostService>();
            postService
                    .Setup(setup => setup.CreateOrRecommend(It.IsAny<Guid>(), It.IsAny<string>(), null))
                    .Throws(httpResponseException);

            Assert.Throws<HttpResponseException>(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", null)
            );
        }

        [Fact]
        public void CreateOrRecommend_WithNullDescriptionAndNullImage_ReturnsHttpResponseException()
        {
            var postCreateViewModel = new PostCreateViewModel { Description = null, Image = null };
            
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup => setup.CreateOrRecommend(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.Is<PostCreateViewModel>(postCreateViewModelObject =>
                        postCreateViewModelObject.Description == null &&
                        postCreateViewModelObject.Image == null
                    ))
                )
                .Throws(httpResponseException);

            Assert.Throws<HttpResponseException>(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
            );
        }

        [Fact]
        public void CreateOrRecommend_WithEmptyDescriptionAndNullImage_ReturnsHttpResponseException()
        {
            var httpResponseException = new HttpResponseException(400, "Preencha, pelo menos, a descrição ou a imagem");

            var postCreateViewModel = new PostCreateViewModel { Description = "", Image = null };
            
            var postService = new Mock<IPostService>();
            postService
                .Setup(setup =>
                    setup.CreateOrRecommend(
                        It.IsAny<Guid>(),
                        It.IsAny<string>(),
                        It.Is<PostCreateViewModel>(postCreateViewModelSetup => 
                            postCreateViewModelSetup.Description == "" &&
                            postCreateViewModelSetup.Image == null
                        )
                    )
                ).Throws(httpResponseException);

            Assert.Throws<HttpResponseException>(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
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

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup => setup.CreateOrRecommend(
                    It.IsAny<Guid>(), 
                    It.IsAny<string>(), 
                    It.Is<PostCreateViewModel>(postCreateViewModelSetup =>
                        postCreateViewModelSetup.Description == "" &&
                        (postCreateViewModelSetup.Image != null && postCreateViewModelSetup.Image.Length == 0)
                    ))
                )
                .Throws(httpResponseException);

            Assert.Throws<HttpResponseException>(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
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

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup => setup.CreateOrRecommend(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.Is<PostCreateViewModel>(postCreateViewModelSetup => 
                        postCreateViewModelSetup.Description == null &&
                        (postCreateViewModelSetup.Image != null && postCreateViewModelSetup.Image.Length == 0)
                    ))
                )
                .Throws(httpResponseException);

            Assert.Throws<HttpResponseException>(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
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

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup => setup.CreateOrRecommend(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.Is<PostCreateViewModel>(postCreateViewModelSetup =>
                        string.IsNullOrEmpty(postCreateViewModelSetup.Description) &&
                        (
                            postCreateViewModelSetup.Image == null ||
                            (postCreateViewModelSetup.Image != null && postCreateViewModelSetup.Image.Length == 0)
                        )
                    ))
                )
                .Throws(httpResponseException);

            var createOrRecommendException = Record.Exception(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
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

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup =>
                    setup.CreateOrRecommend(
                        It.IsAny<Guid>(),
                        It.IsAny<string>(),
                        It.Is<PostCreateViewModel>(postCreateViewModelSetup =>
                            string.IsNullOrEmpty(postCreateViewModelSetup.Description) &&
                            (
                                postCreateViewModelSetup.Image == null ||
                                (postCreateViewModelSetup.Image != null && postCreateViewModelSetup.Image.Length == 0)
                            )
                        )
                    )
                )
                .Throws(httpResponseException);

            var createOrRecommendException = Record.Exception(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
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

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup =>
                    setup.CreateOrRecommend(
                        It.IsAny<Guid>(),
                        It.IsAny<string>(),
                        It.Is<PostCreateViewModel>(postCreateViewModelSetup =>
                            string.IsNullOrEmpty(postCreateViewModelSetup.Description) &&
                            (
                                postCreateViewModelSetup.Image == null ||
                                (postCreateViewModelSetup.Image != null && postCreateViewModelSetup.Image.Length == 0)
                            )
                    ))
                )
                .Throws(httpResponseException);

            var createOrRecommendException = Record.Exception(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
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

            var postService = new Mock<IPostService>();
            postService.Setup(setup =>
                setup.CreateOrRecommend(
                    It.IsAny<Guid>(), 
                    It.IsAny<string>(),
                    It.Is<PostCreateViewModel>(postCreateViewModelSetup =>
                        string.IsNullOrEmpty(postCreateViewModelSetup.Description) &&
                        (
                            postCreateViewModelSetup.Image == null ||
                            (postCreateViewModelSetup.Image != null && postCreateViewModelSetup.Image.Length == 0)
                        )
                    )
                )
            )
            .Throws(httpResponseException);

            var createOrRecommendException = Record.Exception(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
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

            var postService = new Mock<IPostService>();
            postService.Setup(setup =>
                setup.CreateOrRecommend(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.Is<PostCreateViewModel>(postCreateViewModelSetup =>
                        string.IsNullOrEmpty(postCreateViewModelSetup.Description) &&
                        (
                            postCreateViewModelSetup.Image == null ||
                            (postCreateViewModelSetup.Image != null && postCreateViewModelSetup.Image.Length == 0)
                        )
                    )
                )
            )
            .Throws(httpResponseException);

            var createOrRecommendException = Record.Exception(() =>
                postService.Object.CreateOrRecommend(Guid.NewGuid(), "some role", postCreateViewModel)
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

            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(setup => setup.GetById(It.IsAny<Guid>()))
                .Callback<Guid>(passedInPostIdToGetById => passedInPostId = passedInPostIdToGetById)
                .Returns(() => {
                    return passedInPostId == invalidPostId
                        ? null
                        : new Post();
                });

            var postService = new PostService(postRepository.Object, null);

            Assert.Throws<HttpResponseException>(() =>
                postService.Accept(invalidPostId, userId)
            );
        }
        #endregion

        #region Success Tests
        [Fact]
        public void Accept_WithExistingPostIdInDatabase_ReturnsNothing()
        {
            Guid invalidPostId = Guid.NewGuid();
            Guid validPostId = Guid.NewGuid();
            Guid passedInPostId = Guid.Empty;

            var userId = Guid.NewGuid();

            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(setup => setup.GetById(It.IsAny<Guid>()))
                .Callback<Guid>(passedInPostIdToGetById => passedInPostId = passedInPostIdToGetById)
                .Returns(() => {
                    return passedInPostId == invalidPostId
                        ? null
                        : new Post();
                });

            var postService = new PostService(postRepository.Object, null);

            var acceptException = Record.Exception(() => postService.Accept(validPostId, userId));

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

            var postService = new PostService(null, null);

            Assert.Throws<HttpResponseException>(() =>
                postService.GetAll(userId, "some role", searchFilter, "requestHost", "requestPath", new PaginationQueryParams(), "paginationUrl")
            );
        }

        [Fact]
        public void GetAll_WithEmptySearchFilter_ReturnsHttpResponseException()
        {
            Guid userId = Guid.NewGuid();
            string searchFilter = "";

            var postService = new PostService(null, null);

            Assert.Throws<HttpResponseException>(() =>
                postService.GetAll(userId, "some role", searchFilter, "requestHost", "requestPath", new PaginationQueryParams(), "paginationUrl")
            );
        }
        #endregion

        #region Success Tests
        [Fact]
        public void GetAll_WithSearchFilter_ReturnsPaginationResponsePostViewModel()
        {
            Guid userId = Guid.NewGuid();
            string searchFilter = "all";

            var postRepository = new Mock<IGenericRepository<Post>>();

            PaginationResponse<PostViewModel> paginationResponse = null;

            var postService = new PostService(postRepository.Object, null);
            
            Exception getAllException = Record.Exception(() =>
                paginationResponse =
                    postService.GetAll(userId, "some role", searchFilter, "requestHost", "requestPath", new PaginationQueryParams(), "paginationUrl")
            );
            
            Assert.Null(getAllException);
            Assert.NotNull(paginationResponse);
        }
        #endregion
        #endregion

        #region GetById Method Tests
        #region Exceptions Tests
        [Fact]
        public void GetById_WithNonExistingPostIdWithSameUserIdAndPostCreatorId_ReturnsHttpResponseException()
        {
            Guid invalidPostId = Guid.NewGuid();
            Guid passedInPostId = Guid.Empty;

            var userId = Guid.NewGuid();

            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(setup => setup.GetById(It.IsAny<Guid>()))
                .Callback<Guid>(passedInPostIdToGetById => passedInPostId = passedInPostIdToGetById)
                .Returns(() => {
                    return passedInPostId == invalidPostId
                        ? null
                        : new Post { CreatorId = userId };
                });

            var postService = new PostService(postRepository.Object, null);

            Assert.Throws<HttpResponseException>(() =>
                postService.GetById(invalidPostId, userId, "some role", "requestHost", "requestPath")
            );
        }
        
        [Fact]
        public void GetById_WithExistingPostIdWithUserIdDifferentFromPostCreatorId_ReturnsHttpResponseException()
        {
            Guid userId = Guid.NewGuid();

            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(setup => setup.GetById(It.IsAny<Guid>()))
                .Returns(() => new Post { CreatorId = Guid.NewGuid() });

            var postService = new PostService(postRepository.Object, null);

            Assert.Throws<HttpResponseException>(() =>
                postService.GetById(Guid.NewGuid(), userId, "some role", "requestHost", "requestPath")
            );
        }
        #endregion

        #region Success Tests
        [Fact]
        public void GetById_WithExistingPostIdAndSameUserIdAndPostCreatorId_ReturnsNothing()
        {
            Guid userId = Guid.NewGuid();

            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(setup => setup.GetById(It.IsAny<Guid>()))
                .Returns(() => new Post { CreatorId = userId });

            var postService = new PostService(postRepository.Object, null);

            var getByIdException = Record.Exception(() =>
                postService.GetById(Guid.NewGuid(), userId, "some role", "requestHost", "requestPath")
            );

            Assert.Null(getByIdException);
        }
        #endregion
        #endregion

        #region Edit Method Tests
        #region Exceptions Tests
        [Fact]
        public void Edit_WithNonExistingPostId_ReturnsHttpResponseException()
        {
            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(setup => setup.GetById(It.IsAny<Guid>()))
                .Returns(() => null);

            var postService = new PostService(postRepository.Object, null);

            Assert.Throws<HttpResponseException>(() =>
                postService.Edit(Guid.NewGuid(), Guid.NewGuid(), "some role", new PostViewModel())
            );
        }

        [Fact]
        public void Edit_WithExistingPostIdAndUserIdDifferentFromPostCreatorId_ReturnsHttpResponseException()
        {
            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(setup => setup.GetById(It.IsAny<Guid>()))
                .Returns(() => new Post { CreatorId = Guid.NewGuid() });

            var postService = new PostService(postRepository.Object, null);

            Assert.Throws<HttpResponseException>(() =>
                postService.Edit(Guid.NewGuid(), Guid.NewGuid(), "some role", new PostViewModel { Description = "some description" })
            );
        }

        [Fact]
        public void Edit_WithExistingPostIdAndSameUserIdPostCreatorIdWithBothPostViewModelDescriptionAndImageNull_ReturnsHttpResponseException()
        {
            Guid userId = Guid.NewGuid();

            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(setup => setup.GetById(It.IsAny<Guid>()))
                .Returns(() => new Post { CreatorId = userId });

            var postService = new PostService(postRepository.Object, null);

            Assert.Throws<HttpResponseException>(() =>
                postService.Edit(Guid.NewGuid(), userId, "some role", new PostViewModel())
            );
        }

        [Fact]
        public void Edit_WithExistingPostIdAndSameUserIdPostCreatorIdWithPostViewModelEmptyDescriptionAndImageNull_ReturnsHttpResponseException()
        {
            Guid userId = Guid.NewGuid();

            var postRepository = new Mock<IGenericRepository<Post>>();
            postRepository
                .Setup(setup => setup.GetById(It.IsAny<Guid>()))
                .Returns(() => new Post { CreatorId = userId });

            var postService = new PostService(postRepository.Object, null);

            Assert.Throws<HttpResponseException>(() =>
                postService.Edit(Guid.NewGuid(), userId, "some role", new PostViewModel { Description = "" })
            );
        }
        #endregion

        #region Success Tests
        [Fact]
        public void Edit_WithDescriptionAndImage_ReturnsNothing()
        {
            Guid validPostId = Guid.NewGuid();
            Guid validUserId = Guid.NewGuid();

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup =>
                    setup.Edit(
                        It.Is<Guid>(postIdToPass => postIdToPass == validPostId),
                        It.Is<Guid>(userIdToPass => userIdToPass == validUserId),
                        It.IsAny<string>(),
                        It.Is<PostViewModel>(postViewModelToPass =>
                            !string.IsNullOrEmpty(postViewModelToPass.Description) &&
                            (postViewModelToPass.Image != null && postViewModelToPass.Image.Length > 0)
                        )
                    )
                )
                .Verifiable("PostService Edit must be called passing: valid post id, valid user id, not null or empty description " +
                    "for PostViewModel and not null image with its length greater than 0 for PostViewModel");

            postService.Object.Edit(
                validPostId,
                validUserId,
                "some role",
                new PostViewModel
                {
                    Description = "some description",
                    Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 100, "Data", "image.png")
                }
            );

            postService.VerifyAll();
        }

        [Fact]
        public void Edit_WithDescriptionAndNullImage_ReturnsNothing()
        {
            Guid validPostId = Guid.NewGuid();
            Guid validUserId = Guid.NewGuid();

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup =>
                    setup.Edit(
                        validPostId,
                        validUserId,
                        It.IsAny<string>(),
                        It.Is<PostViewModel>(postViewModelToPass =>
                            !string.IsNullOrEmpty(postViewModelToPass.Description) &&
                            postViewModelToPass.Image == null
                        )
                    )
                );

            postService.Object.Edit(
                validPostId,
                validUserId,
                "some role",
                new PostViewModel { Description = "some description", Image = null }
            );

            postService.VerifyAll();
        }

        [Fact]
        public void Edit_WithDescriptionAndImageWithNoLength_ReturnsNothing()
        {
            Guid validPostId = Guid.NewGuid();
            Guid validUserId = Guid.NewGuid();

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup =>
                    setup.Edit(
                        validPostId,
                        validUserId,
                        It.IsAny<string>(),
                        It.Is<PostViewModel>(postViewModelSetup =>
                            !string.IsNullOrEmpty(postViewModelSetup.Description) &&
                            (postViewModelSetup.Image != null && postViewModelSetup.Image.Length == 0)
                        )
                    )
                );

            postService.Object.Edit(
                validPostId,
                validUserId,
                "some role",
                new PostViewModel { 
                    Description = "some description", 
                    Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 0, "Data", "image.png")
                }
            );

            postService.VerifyAll();
        }

        [Fact]
        public void Edit_WithNullDescriptionWithImage_ReturnsNothing()
        {
            Guid validPostId = Guid.NewGuid();
            Guid validUserId = Guid.NewGuid();

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup =>
                    setup.Edit(
                        validPostId,
                        validUserId,
                        It.IsAny<string>(),
                        It.Is<PostViewModel>(postViewModelSetup =>
                            postViewModelSetup.Description == null &&
                            (postViewModelSetup.Image != null && postViewModelSetup.Image.Length > 0)
                        )
                    )
                );

            postService.Object.Edit(
                validPostId,
                validUserId,
                "some role",
                new PostViewModel
                {
                    Description = null,
                    Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 100, "Data", "image.png")
                }
            );

            postService.VerifyAll();
        }

        [Fact]
        public void Edit_WithEmptyDescriptionWithImage_ReturnsNothing()
        {
            Guid validPostId = Guid.NewGuid();
            Guid validUserId = Guid.NewGuid();

            var postService = new Mock<IPostService>();
            postService
                .Setup(setup =>
                    setup.Edit(
                        validPostId,
                        validUserId,
                        It.IsAny<string>(),
                        It.Is<PostViewModel>(postViewModelSetup =>
                            postViewModelSetup.Description == "" &&
                            (postViewModelSetup.Image != null && postViewModelSetup.Image.Length > 0)
                        )
                    )
                );

            postService.Object.Edit(
                validPostId,
                validUserId,
                "some role",
                new PostViewModel
                {
                    Description = "",
                    Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("lorem ipsum")), 0, 100, "Data", "image.png")
                }
            );

            postService.VerifyAll();
        }
        #endregion
        #endregion
    }
}
