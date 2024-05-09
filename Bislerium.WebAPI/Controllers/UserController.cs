using Bislerium.Application.DTOs;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser user;
        private readonly AppDbContext appDbContext;

        public UserController(IUser user, AppDbContext appDbContext)
        {
            this.user = user;
            this.appDbContext = appDbContext;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponse>> LogUserIn(LoginDTO loginDTO)
        {
            if (string.IsNullOrEmpty(loginDTO.UsernameOrEmail) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return BadRequest(new { message = "Username/Email and password are required." });
            }

            try
            {
                var result = await user.LoginUserAsync(loginDTO, HttpContext);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                return StatusCode(500, new { message = "An error occurred during login process." });
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<LoginResponse>> RegisterUser(RegisterDTO registerDTO)
        {
            var result = await user.RegisterUserAsync(registerDTO);

            if (!result.Flag)
            {
                // User registration failed due to validation error
                return BadRequest(new { message = result.Message });
            }

            // User registered successfully
            return Ok(result);
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required for password reset");
            }

            var users = await user.ForgotPassword(email);
            if (users != null)
            {
                // Generate the password reset token for the user
                var resetToken = await user.GeneratePasswordResetTokenAsync(users);

                // Store the reset token in the database (assuming you have a ResetToken field in your user model)
                users.ResetToken = resetToken;
                users.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1); // Set token expiration time (e.g., 1 hour)
                await appDbContext.SaveChangesAsync();

                // Send the password reset email
                string subject = "Password Reset Request";
                string callbackUrl = $"http://yourwebsite.com/reset-password?token={resetToken}";

                // Construct the email body with the password reset link
                string body = $"Click the link below to reset your password:\n{callbackUrl}";

                // Send the email using a method or service for email sending
                await SendEmailAsync(email, subject, body);

                return Ok("Password reset email sent successfully");
            }
            else
            {
                // User not found or password reset could not be initiated
                return BadRequest("User not found or password reset could not be initiated");
            }
        }

        // Method for sending email (example implementation)
        private async Task SendEmailAsync(string email, string subject, string body)
        {
            // Implementation for sending email using your preferred email sending method or service
            // For example, you can use SendGrid, SMTP, or any other email service
        }
    }
}

