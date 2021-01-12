using Microsoft.AspNetCore.Http;
using System;

namespace PostsApi.Models.ViewModels
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? RecomendDate { get; set; }
    }
}
