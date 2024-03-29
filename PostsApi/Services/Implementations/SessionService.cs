﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PostsApi.GlobalErrorHandling;
using PostsApi.Models.Email;
using PostsApi.Models.Entities.Identity;
using PostsApi.Models.JsonWebToken;
using PostsApi.Models.Pagination;
using PostsApi.Models.ViewModels.User;
using PostsApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PostsApi.Services.Implementations
{
    public class SessionService : ISessionService
    {
        public SessionService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IConfiguration configuration,
            IUserService userService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _userService = userService;
        }
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public async Task CreateAdministrator(AdminCreateViewModel adminCreateViewModel, bool isModelStateValid)
        {
            if (!isModelStateValid)
            {
                throw new HttpResponseException(400, "E-mail e senha devem ser informados");
            }

            ApplicationUser emailExists = await _userManager.FindByEmailAsync(adminCreateViewModel.Email);

            if (emailExists != null)
            {
                throw new HttpResponseException(400, "Já existe uma conta com o e-mail informado");
            }

            ApplicationUser applicationUser = new ApplicationUser
            {
                Name = adminCreateViewModel.Name,
                Email = adminCreateViewModel.Email,
                UserName = adminCreateViewModel.Email
            };

            if (adminCreateViewModel.Avatar != null && adminCreateViewModel.Avatar.Length > 0)
            {
                string imageFileName = _userService.SaveProfileImage(adminCreateViewModel.Avatar);

                applicationUser.Avatar = imageFileName;
            }

            var createResult = await _userManager.CreateAsync(applicationUser, Guid.NewGuid().ToString() + "A-Z");

            if (!createResult.Succeeded)
            {
                throw new HttpResponseException(400, "Usuário ou senha inválidos");
            }
            else
            {
                await _userManager.AddToRoleAsync(applicationUser, "Administrator");
            }

            await GeneratePasswordResetTokenAndEmail(applicationUser);
        }

        public async Task CreatePassword(PasswordCreateViewModel passwordCreateViewModel, bool isModelStateValid)
        {
            if (!isModelStateValid)
            {
                throw new HttpResponseException(400, "E-mail, senha e token são necessários");
            }
            
            var applicationUser = await _userManager.FindByEmailAsync(passwordCreateViewModel.Email);

            var result = await _userManager.ResetPasswordAsync(applicationUser, passwordCreateViewModel.Token, passwordCreateViewModel.Password);
            
            if (!result.Succeeded)
            {
                throw new HttpResponseException(500, "Erro ao criar senha");
            }
        }
        
        public async Task<UserToken> CreateCommon(UserCreateViewModel userCreateViewModel, bool isModelStateValid)
        {
            if (!isModelStateValid)
            {
                throw new HttpResponseException(400, "E-mail e senha devem ser informados");
            }

            var emailExists = await _userManager.FindByEmailAsync(userCreateViewModel.Email);

            if (emailExists != null)
            {
                throw new HttpResponseException(400, "Já existe uma conta com o e-mail informado");
            }

            var applicationUser = new ApplicationUser
            {
                Name = userCreateViewModel.Name,
                Email = userCreateViewModel.Email,
                UserName = userCreateViewModel.Email
            };

            if (userCreateViewModel.Avatar != null && userCreateViewModel.Avatar.Length > 0)
            {
                string imageFileName = _userService.SaveProfileImage(userCreateViewModel.Avatar);

                applicationUser.Avatar = imageFileName;
            }

            var createResult = await _userManager.CreateAsync(applicationUser, userCreateViewModel.Password);

            if (!createResult.Succeeded)
            {
                throw new HttpResponseException(400, "Usuário ou senha inválidos");
            }
            else
            {
                await _userManager.AddToRoleAsync(applicationUser, "Common");

                var signInResult = await _signInManager.PasswordSignInAsync(
                    userCreateViewModel.Email,
                    userCreateViewModel.Password,
                    isPersistent: false,
                    lockoutOnFailure: false
                );

                if (!signInResult.Succeeded)
                {
                    throw new HttpResponseException(400, "Login inválido");
                }
                else
                {
                    IList<string> userRoles = await _userManager.GetRolesAsync(applicationUser);
                    string jwtSecretKey = _configuration["JWT:Key"];

                    return _tokenService.BuildToken(applicationUser, userRoles[0], jwtSecretKey);
                }
            }
        }

        public async Task ForgotPassword(string userEmail)
        {
            ApplicationUser applicationUser = await _userManager.FindByEmailAsync(userEmail);

            if (applicationUser == null)
            {
                throw new HttpResponseException(400, "Não existe um usuário esse e-mail");
            }

            await GeneratePasswordResetTokenAndEmail(applicationUser);
        }

        public async Task ChangePassword(PasswordChangeViewModel passwordChangeViewModel, ClaimsPrincipal user)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(user);
            
            if(applicationUser == null)
            {
                throw new HttpResponseException(400, "Usuário inválido. Entre na sua conta para realizar a alteração");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(applicationUser, passwordChangeViewModel.CurrentPassword, passwordChangeViewModel.NewPassword);
            
            if (!changePasswordResult.Succeeded)
            {
                throw new HttpResponseException(400, "Informe a senha atual e a nova corretamente de acordo com as regras para a criação de senha");
            }
        }

        public PaginationResponse<UserViewModel> GetAll(Guid userId, string filter, PaginationQueryParams paginationQueryParams, string paginationUrl)
        {
            if (String.IsNullOrEmpty(filter))
            {
                throw new HttpResponseException(400, "O filtro da pesquisa não pode estar vazio");
            }

            if(filter != "common" && filter != "administrator")
            {
                throw new HttpResponseException(400, "O filtro da pesquisa deve ser common ou administrator");
            }
            
            if (filter == "common") filter = "Common";
            else filter = "Administrator";

            var totalRecords = _userManager.Users
                .Where(user => user.UserRoles.First().Role.Name == filter && user.Id != userId)
                .Count();

            var users = _userManager.Users
                .Where(user => user.UserRoles.First().Role.Name == filter && user.Id != userId)
                .Skip((paginationQueryParams.Page - 1) * paginationQueryParams.Per_Page)
                .Take(paginationQueryParams.Per_Page)
                .Select(user => new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                });

            PaginationResponse<UserViewModel> paginationResponse = PaginationHelper<UserViewModel>.CreateResponseWithPagination(
                users,
                totalRecords,
                paginationQueryParams,
                paginationUrl
             );

            return paginationResponse;
        }

        public PaginationResponse<UserViewModel> Search(Guid userId, string filter, PaginationQueryParams paginationQueryParams, string paginationUrl)
        {
            if (String.IsNullOrEmpty(filter))
            {
                throw new HttpResponseException(400, "O filtro da pesquisa não pode estar vazio");
            }

            var totalRecords = _userManager.Users
                .Where(user =>
                    user.Name.ToLower().Contains(filter.ToLower()) ||
                    user.Email.ToLower().Contains(filter.ToLower()) &&
                    user.Id != userId
                )
                .Count();

            var users = _userManager.Users
                .Where(user =>
                    user.Name.ToLower().Contains(filter.ToLower()) ||
                    user.Email.ToLower().Contains(filter.ToLower()) &&
                    user.Id != userId
                )
                .Skip((paginationQueryParams.Page - 1) * paginationQueryParams.Per_Page)
                .Take(paginationQueryParams.Per_Page)
                .Select(user => new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                });

            PaginationResponse<UserViewModel> paginationResponse = PaginationHelper<UserViewModel>.CreateResponseWithPagination(
                users,
                totalRecords,
                paginationQueryParams,
                paginationUrl
             );

            return paginationResponse;
        }

        public async Task TurnUserAdministrator(Guid userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString());

            if(user == null)
            {
                throw new HttpResponseException(400, "Não há um administrator com esse id");
            }
            
            await _userManager.AddToRoleAsync(user, "Administrator");
            await _userManager.RemoveFromRoleAsync(user, "Common");
        }

        public async Task<UserToken> Login(UserLoginViewModel userLoginViewModel, bool isModelStateValid)
        {
            if (!isModelStateValid)
            {
                throw new HttpResponseException(400, "E-mail e senha devem ser informados");
            }

            var loginResult = await _signInManager.PasswordSignInAsync(
                userLoginViewModel.Email,
                userLoginViewModel.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!loginResult.Succeeded)
            {
                throw new HttpResponseException(400, "Login inválido");
            }

            ApplicationUser applicationUser = await _userManager.FindByEmailAsync(userLoginViewModel.Email);

            IList<string> userRoles = await _userManager.GetRolesAsync(applicationUser);

            string jwtSecretKey = _configuration["JWT:Key"];

            return _tokenService.BuildToken(applicationUser, userRoles[0], jwtSecretKey);
        }

        private async Task GeneratePasswordResetTokenAndEmail(ApplicationUser applicationUser)
        {
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);

            if (string.IsNullOrEmpty(resetToken))
            {
                throw new HttpResponseException(500, "Erro ao gerar token para criar a senha");
            }

            string resetPageWithTokenAndEmail = string.Concat("http://localhost:3000/users/create_password", "?token=", resetToken, "&email=", applicationUser.Email);

            string email = _configuration["Email:Email"];
            string password = _configuration["Email:Password"];
            string name = _configuration["Email:Name"];

            string subject = "Posts API - Criação de senha";

            string shortLinkDescription = resetPageWithTokenAndEmail.Substring(0, 80);

            string htmlBody = string.Concat(
                "<div style='text-align: center;'>",
                    "<p>Olá, ", applicationUser.Name, "</p>",
                    "<p>Clique no link abaixo para criar sua senha: <p>",
                    "<p><a href='", resetPageWithTokenAndEmail, "'>", shortLinkDescription, "</a></p>",
                    "<p>Obrigado, </p>",
                    "<p>Equipe Posts API</p>",
                "</div>"
            );

            EmailHelper.SendEmail(name, email, applicationUser.Name, applicationUser.Email, subject, htmlBody, email, password);
        }
    }
}
