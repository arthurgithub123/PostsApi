using System.ComponentModel.DataAnnotations;

namespace PostsApi.Models.ViewModels.User
{
    public class PasswordCreateViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
