using Microsoft.AspNetCore.Http;

namespace PostsApi.Models.ViewModels
{
    public class PostCreateViewModel
    {
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
