using System.ComponentModel.DataAnnotations;

namespace PostsApi.Models.ViewModels.User
{
    public class PasswordChangeViewModel
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
