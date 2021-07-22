using PostsApi.Models.JsonWebToken;
using PostsApi.Models.Pagination;
using PostsApi.Models.ViewModels.User;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PostsApi.Services.Interfaces
{
    public interface ISessionService
    {
        public Task CreateAdministrator(AdminCreateViewModel adminCreateViewModel, bool isModelStateValid);
        public Task<UserToken> CreateCommon(UserCreateViewModel userCreateViewModel, bool isModelStateValid);
        public Task CreatePassword(PasswordCreateViewModel passwordCreateViewModel, bool isModelStateValid);
        public Task ForgotPassword(string email);
        public Task ChangePassword(PasswordChangeViewModel passwordChangeViewModel, ClaimsPrincipal user);
        public PaginationResponse<UserViewModel> GetAll(Guid userId, string userRole, string filter, string requestHost, string requestPathBase, PaginationQueryParams paginationQueryParams, string paginationUrl);
        public Task TurnUserAdministrator(Guid userId);
        public Task<UserToken> Login(UserLoginViewModel userLoginViewModel, bool isModelStateValid);
    }
}
