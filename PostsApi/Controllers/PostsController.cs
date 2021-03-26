using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PostsApi.Models.Entities.Identity;
using PostsApi.Models.ViewModels;
using PostsApi.Services.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace PostsApi.Controllers
{
    [Authorize(Roles = "Administrator, Commom")]
    [Route("api/Posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        public PostsController(IPostService postService, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _postService = postService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        private readonly IPostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [HttpPost("Create")]
        public IActionResult AdministratorCreate([FromForm] PostViewModel postViewModel)
        {
             ApplicationUser applicationUser = _userManager.GetUserAsync(this.User).Result;

            Guid userId = applicationUser.Id;
            string userRole = _userManager.GetRolesAsync(applicationUser).Result[0];

            _postService.CreateOrRecommend(userId, userRole, postViewModel);

            return Ok(new
            {
                StatusCode = 201, 
                Message = "Post cadastrado com sucesso" 
            });
        }

        [HttpGet("List/{filter}")]
        public IActionResult List(string filter)
        {
            ApplicationUser applicationUser = _userManager.GetUserAsync(this.User).Result;
            string userRole = _userManager.GetRolesAsync(applicationUser).Result[0];

            IQueryable<PostViewModel> posts = _postService.GetAll(applicationUser.Id, userRole, filter, Request.Host.ToString(), Request.PathBase);

            return Ok(posts);
        }
    
        [HttpGet("ListById/{id}")]
        public IActionResult ListById(Guid id)
        {
            ApplicationUser applicationUser = _userManager.GetUserAsync(this.User).Result;
            string userRole = _userManager.GetRolesAsync(applicationUser).Result[0];

            PostViewModel postViewModel = _postService.GetById(id, applicationUser.Id, userRole, Request.Host.ToString(), Request.PathBase);
            
            return Ok(postViewModel);
        }
    }
}
