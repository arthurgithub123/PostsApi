using PostsApi.Models.Pagination;
using PostsApi.Models.ViewModels;
using System;

namespace PostsApi.Services.Interfaces
{
    public interface IPostService
    {
        public void CreateOrRecommend(Guid userId, string userRole, PostCreateViewModel postCreateViewModel);
        public void Accept(Guid id, Guid userId);
        public PaginationResponse<PostViewModel> GetAll(Guid userId, string userRole, string filter, string requestHost, string requestPathBase, PaginationQueryParams paginationQueryParams, string paginationUrl);
        public PostGetViewModel GetById(Guid id, Guid userId, string userRole, string requestHost, string requestPathBase);
        public void Edit(Guid id, Guid userId, string userRole, PostViewModel postViewModel);
    }
}
