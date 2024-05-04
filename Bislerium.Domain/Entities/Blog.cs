using Bislerium.Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Bislerium.Domain.Entities
{
    public class Blog : BaseEntity
    {
        [Required]
        public string? Title { get; set; }
        public string? BlogImage { get; set; } //stores blog image 
        [Required]
        public string? PublishedBy { get; set; }
        [Required]
        public string? PublishedDate { get; set; }
        public string? Comments { get; set; }
        public int Likes { get; set; }
    }
}
