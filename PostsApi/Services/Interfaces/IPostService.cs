using PostsApi.Models.ViewModels;
using System;
using System.Linq;

namespace PostsApi.Services.Interfaces
{
    public interface IPostService
    {
        public void CreateOrRecommend(Guid userId, string userRole, PostViewModel postViewModel);
        public void Accept(Guid id, Guid userId);
        public IQueryable<PostViewModel> GetAll(Guid userId, string userRole, string filter, string requestHost, string requestPathBase);
        public PostViewModel GetById(Guid id, Guid userId, string userRole, string requestHost, string requestPathBase);
        public void Edit(Guid id, Guid userId, string userRole, PostViewModel postViewModel);
    }
}
