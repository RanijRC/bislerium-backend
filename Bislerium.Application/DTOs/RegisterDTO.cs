using System.ComponentModel.DataAnnotations;

namespace Bislerium.Application.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string? Firstname { get; set; } = string.Empty;
        public string? Middlename { get; set; } = null;
        [Required]
        public string? Lastname { get; set; } = string.Empty;
        [Required]
        public string? Username { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string? Email { get; set; } = string.Empty;
        [Required]
        public string? Password { get; set; } = string.Empty;
        [Required, Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}
