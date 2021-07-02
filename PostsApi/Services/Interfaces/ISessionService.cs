﻿using PostsApi.Models.JsonWebToken;
using PostsApi.Models.ViewModels.User;
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
        public Task<UserToken> Login(UserLoginViewModel userLoginViewModel, bool isModelStateValid);
    }
}
