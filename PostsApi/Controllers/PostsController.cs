using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PostsApi.Models.Entities.Identity;
using PostsApi.Models.Pagination;
using PostsApi.Models.ViewModels;
using PostsApi.Services.Interfaces;
using System;
using System.Linq;

namespace PostsApi.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Authorize(Roles = "Administrator, Common")]
    [Route("api/posts")]
    [ApiController]
    public class PostsController : Controller
    {
        public PostsController(IPostService postService, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }

        private readonly IPostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUser ApplicationUser { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ApplicationUser = _userManager.GetUserAsync(this.User).Result;
        }

        [HttpPost("")]
        public IActionResult CreateOrRecommend([FromForm] PostCreateViewModel postCreateViewModel)
        {
            string userRole = _userManager.GetRolesAsync(ApplicationUser).Result[0];

            _postService.CreateOrRecommend(ApplicationUser.Id, userRole, postCreateViewModel);

            return StatusCode(StatusCodes.Status201Created, new
            {
                StatusCode = 201,
                Message = "Post cadastrado com sucesso"
            });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("accept/{id}")]
        public IActionResult Accept(Guid id)
        {
            _postService.Accept(id, ApplicationUser.Id);

            return Ok(new
            {
                StatusCode = 200,
                Message = "Post aceito com sucesso"
            });
        }

        [HttpGet("list/{filter}")]
        public IActionResult List(string filter, [FromQuery] PaginationQueryParams paginationQueryParams)
        {
            string userRole = _userManager.GetRolesAsync(ApplicationUser).Result[0];

            string paginationUrl = string.Concat(Request.Scheme, "://", Request.Host, Request.Path);
            
            PaginationResponse<PostViewModel> paginationResponse = _postService.GetAll(
                ApplicationUser.Id, 
                userRole, 
                filter, 
                Request.Host.ToString(), 
                Request.PathBase, 
                paginationQueryParams, 
                paginationUrl
           );

            return Ok(paginationResponse);
        }
        
        [HttpGet("list_by_id/{id}")]
        public IActionResult ListById(Guid id)
        {
            string userRole = _userManager.GetRolesAsync(ApplicationUser).Result[0];

            PostViewModel postViewModel = _postService.GetById(id, ApplicationUser.Id, userRole, Request.Host.ToString(), Request.PathBase);
            
            return Ok(postViewModel);
        }

        [HttpPut("edit/{id}")]
        public IActionResult Edit(Guid id, [FromForm] PostViewModel postViewModel)
        {
            string userRole = _userManager.GetRolesAsync(ApplicationUser).Result[0];

            _postService.Edit(id, ApplicationUser.Id, userRole, postViewModel);

            return Ok();
        }
    }
}
