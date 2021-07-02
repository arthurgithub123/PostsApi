using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostsApi.Models.JsonWebToken;
using PostsApi.Models.ViewModels.User;
using PostsApi.Services.Interfaces;
using System.Threading.Tasks;

namespace PostsApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/session")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        
        private readonly ISessionService _sessionService;
        
        [Authorize(Roles = "Administrator")]
        [HttpPost("administrator")]
        public async Task<ActionResult> CreateAdministrator(AdminCreateViewModel adminCreateViewModel)
        {
            await _sessionService.CreateAdministrator(adminCreateViewModel, ModelState.IsValid);

            return StatusCode(StatusCodes.Status201Created, new
            {
                StatusCode = 201,
                Message = "Administrador cadastrado com sucesso"
            });
        }
        
        [HttpPost("common")]
        public async Task<ActionResult<UserToken>> CreateCommon([FromForm] UserCreateViewModel userCreateViewModel)
        {
            UserToken userToken = await _sessionService.CreateCommon(userCreateViewModel, ModelState.IsValid);

            return StatusCode(StatusCodes.Status201Created, userToken);
        }

        [HttpPost("password")]
        public async Task<IActionResult> CreatePassword(PasswordCreateViewModel passwordCreateViewModel)
        {
            await _sessionService.CreatePassword(passwordCreateViewModel, ModelState.IsValid);
            
            return Ok(new
            {
                StatusCode = 200,
                Message = "Senha criada com sucesso"
            });
        }

        [HttpGet("password/forgot/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            await _sessionService.ForgotPassword(email);
        
            return Ok(new
            {
                StatusCode = 200,
                Message = "E-mail com link para nova senha enviado com sucesso"
            });
        }

        [HttpPost("password/change")]
        public async Task<IActionResult> ChangePassword(PasswordChangeViewModel passwordChangeViewModel)
        {
            await _sessionService.ChangePassword(passwordChangeViewModel, this.User);

            return Ok(new
            {
                StatusCode = 200,
                Message = "Senha alterada com sucesso"
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login(UserLoginViewModel userLoginViewModel)
        {
            UserToken userToken = await _sessionService.Login(userLoginViewModel, ModelState.IsValid);
            
            return Ok(userToken);
        }
    }
}
