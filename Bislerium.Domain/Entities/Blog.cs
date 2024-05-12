using Bislerium.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bislerium.Domain.Entities
{
    public class Blog 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string BlogImage { get; set; }

        [Required]
        public int PublishedBy { get; set; }

        [ForeignKey(nameof(PublishedBy))] // This attribute establishes the foreign key relationship
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        [Required]
        public string Description { get; set; }

        public int UpVoteCount { get; set; }

        public string Comments { get; set; }

        public int DownVoteCount { get; set; }
    }
}
