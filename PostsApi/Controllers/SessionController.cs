﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostsApi.Models.Token;
using PostsApi.Models.ViewModels.User;
using PostsApi.Services.Interfaces;
using System.Threading.Tasks;

namespace PostsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        
        private readonly ISessionService _sessionService;
        
        [Authorize(Roles = "Administrator")]
        [HttpPost("Administrator/Create")]
        public async Task<ActionResult> CreateAdministrator(AdminCreateViewModel adminCreateViewModel)
        {
            await _sessionService.CreateAdministrator(adminCreateViewModel, ModelState.IsValid);

            return StatusCode(StatusCodes.Status201Created, new
            {
                StatusCode = 201,
                Message = "Administrador cadastrado com sucesso"
            });
        }

        [HttpPost("Commom/Create")]
        public async Task<ActionResult<UserToken>> CreateCommom([FromForm] UserCreateViewModel userCreateViewModel)
        {
            UserToken userToken = await _sessionService.CreateCommom(userCreateViewModel, ModelState.IsValid);

            return StatusCode(StatusCodes.Status201Created, userToken);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login(UserLoginViewModel userLoginViewModel)
        {
            UserToken userToken = await _sessionService.Login(userLoginViewModel, ModelState.IsValid);
            
            return Ok(userToken);
        }
    }
}
