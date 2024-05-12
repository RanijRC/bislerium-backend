using Bislerium.Domain.Shared;
using System.ComponentModel.DataAnnotations;


namespace Bislerium.Domain.Entities
{
    public class ApplicationUser 
    {
        [Key]
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Middlename { get; set; } = null;
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
        public string? ResetToken { get; set; }
        public DateTime ResetTokenExpiresAt { get; set; }
        public string? Role { get; set; }

        public List<Blog> Blogs { get; set; }
    }
}
