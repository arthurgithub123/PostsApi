using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PostsApi.Models.Entities.Identity;
using PostsApi.Models.Token;
using PostsApi.Models.ViewModels;
using PostsApi.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        public SessionController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IConfiguration configuration,
            IUserService userService)
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

        [Authorize(Roles = "Administrator")]
        [HttpPost("Administrator/Create")]
        public async Task<ActionResult<UserToken>> CreateAdministrator(UserCreateViewModel userCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("E-mail e senha devem ser informados");
            }

            ApplicationUser emailExists = await _userManager.FindByEmailAsync(userCreateViewModel.Email);

            if (emailExists != null)
            {
                return BadRequest("Já existe uma conta com o e-mail informado");
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
                return BadRequest("Usuário ou senha inválidos");
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
                    return BadRequest("Login inválido");
                }
                else
                {
                    IList<string> userRoles = await _userManager.GetRolesAsync(applicationUser);
                    string jwtSecretKey = _configuration["JWT:Key"];

                    return _tokenService.BuildToken(applicationUser, userRoles[0], jwtSecretKey);
                }
            }
        }

        [HttpPost("Commom/Create")]
        public async Task<ActionResult<UserToken>> CreateCommom([FromForm] UserCreateViewModel userCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("E-mail e senha devem ser informados");
            }

            var emailExists = await _userManager.FindByEmailAsync(userCreateViewModel.Email);

            if (emailExists != null)
            {
                return BadRequest("Já existe uma conta com o e-mail informado");
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
                return BadRequest("Usuário ou senha inválidos");
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
                    return BadRequest("Login inválido");
                }
                else
                {
                    IList<string> userRoles = await _userManager.GetRolesAsync(applicationUser);
                    string jwtSecretKey = _configuration["JWT:Key"];

                    return _tokenService.BuildToken(applicationUser, userRoles[0], jwtSecretKey);
                }
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login(UserLoginViewModel userLoginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("E-mail e senha devem ser informados");
            }

            var loginResult = await _signInManager.PasswordSignInAsync(
                userLoginViewModel.Email,
                userLoginViewModel.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!loginResult.Succeeded)
            {
                return BadRequest("Login inválido");
            }

            ApplicationUser applicationUser = await _userManager.FindByEmailAsync(userLoginViewModel.Email);

            IList<string> userRoles = await _userManager.GetRolesAsync(applicationUser);

            string jwtSecretKey = _configuration["JWT:Key"];

            return _tokenService.BuildToken(applicationUser, userRoles[0], jwtSecretKey);
        }
    }
}
