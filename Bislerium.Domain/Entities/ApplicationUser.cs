using Bislerium.Domain.Shared;


namespace Bislerium.Domain.Entities
{
    public class ApplicationUser : BaseEntity
    {
        public string? Firstname { get; set; }
        public string? Middlename { get; set; } = null;
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
