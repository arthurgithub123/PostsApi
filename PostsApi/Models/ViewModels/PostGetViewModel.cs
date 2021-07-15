using System;

namespace PostsApi.Models.ViewModels
{
    public class PostGetViewModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? RecomendDate { get; set; }
        public bool CanEdit { get; set; }
    }
}
