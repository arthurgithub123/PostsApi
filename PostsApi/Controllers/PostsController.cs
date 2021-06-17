using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PostsApi.Models.Entities.Identity;
using PostsApi.Models.ViewModels;
using PostsApi.Services.Interfaces;
using System;
using System.Linq;

namespace PostsApi.Controllers
{
    [Authorize(Roles = "Administrator, Commom")]
    [Route("api/Posts")]
    [ApiController]
    public class PostsController : Controller
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

        public ApplicationUser ApplicationUser { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ApplicationUser = _userManager.GetUserAsync(this.User).Result;
        }

        [HttpPost("")]
        public IActionResult AdministratorCreate([FromForm] PostViewModel postViewModel)
        {
            string userRole = _userManager.GetRolesAsync(ApplicationUser).Result[0];

            _postService.CreateOrRecommend(ApplicationUser.Id, userRole, postViewModel);

            return StatusCode(StatusCodes.Status201Created, new
            {
                StatusCode = 201,
                Message = "Post cadastrado com sucesso"
            });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("Accept/{id}")]
        public IActionResult Accept(Guid id)
        {
            _postService.Accept(id, ApplicationUser.Id);

            return Ok(new
            {
                StatusCode = 200,
                Message = "Post aceito com sucesso"
            });
        }

        [HttpGet("List/{filter}")]
        public IActionResult List(string filter)
        {
            string userRole = _userManager.GetRolesAsync(ApplicationUser).Result[0];
            
            IQueryable<PostViewModel> posts = _postService.GetAll(ApplicationUser.Id, userRole, filter, Request.Host.ToString(), Request.PathBase);

            return Ok(posts);
        }
    
        [HttpGet("ListById/{id}")]
        public IActionResult ListById(Guid id)
        {
            string userRole = _userManager.GetRolesAsync(ApplicationUser).Result[0];

            PostViewModel postViewModel = _postService.GetById(id, ApplicationUser.Id, userRole, Request.Host.ToString(), Request.PathBase);
            
            return Ok(postViewModel);
        }

        [HttpPut("Edit/{id}")]
        public IActionResult Edit(Guid id, [FromForm] PostViewModel postViewModel)
        {
            ApplicationUser applicationUser = _userManager.GetUserAsync(this.User).Result;
            string userRole = _userManager.GetRolesAsync(applicationUser).Result[0];

            _postService.Edit(id, applicationUser.Id, userRole, postViewModel);

            return Ok();
        }
    }
}
