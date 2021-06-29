using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PostsApi.Models.ViewModels.User
{
    public class AdminCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
