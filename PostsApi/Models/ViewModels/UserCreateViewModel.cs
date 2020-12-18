using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PostsApi.Models.ViewModels
{
    public class UserCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
