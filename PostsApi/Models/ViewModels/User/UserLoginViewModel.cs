using System.ComponentModel.DataAnnotations;

namespace PostsApi.Models.ViewModels.User
{
    public class UserLoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
