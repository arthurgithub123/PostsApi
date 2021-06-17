using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PostsApi.ErrorHandling;
using PostsApi.Models.Entities.Identity;
using PostsApi.Models.Token;
using PostsApi.Models.ViewModels.User;
using PostsApi.Services.Interfaces;
using System.Collections.Generic;
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

        public async Task<UserToken> CreateAdministrator(UserCreateViewModel userCreateViewModel, bool isModelStateValid)
        {
            if (!isModelStateValid)
            {
                throw new HttpResponseException(400, "E-mail e senha devem ser informados");
            }

            ApplicationUser emailExists = await _userManager.FindByEmailAsync(userCreateViewModel.Email);

            if (emailExists != null)
            {
                throw new HttpResponseException(400, "Já existe uma conta com o e-mail informado");
            }

            ApplicationUser applicationUser = new ApplicationUser
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
                await _userManager.AddToRoleAsync(applicationUser, "Administrator");

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

        public async Task<UserToken> CreateCommom(UserCreateViewModel userCreateViewModel, bool isModelStateValid)
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
                await _userManager.AddToRoleAsync(applicationUser, "Commom");

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
    }
}
