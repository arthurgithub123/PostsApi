using System;
using System.ComponentModel.DataAnnotations;

namespace PostsApi.Models.Entities
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public Guid? EditorId { get; set; }
        public DateTime? ExcludedAt { get; set; }
        public Guid? ExcludeId { get; set; }
    }
}
