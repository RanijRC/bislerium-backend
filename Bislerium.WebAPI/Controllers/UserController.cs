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
        private readonly IUser userService;
        private readonly IEmail emailService;

        public UserController(IUser userService, IEmail emailService)
        {
            this.userService = userService;
            this.emailService = emailService;
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
                var result = await userService.LoginUserAsync(loginDTO, HttpContext);
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
            var result = await userService.RegisterUserAsync(registerDTO);

            if (!result.Flag)
            {
                // User registration failed due to validation error
                return BadRequest(new { message = result.Message });
            }

            // User registered successfully
            return Ok(result);
        }

        [HttpPost]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email exists in the database
                var user = await userService.FindUserByEmailOrUsername(model.Email);
                if (user != null)
                {
                    // Generate a password reset token
                    string resetToken = await userService.GeneratePasswordResetToken(user);

                    // Send the password reset email
                    string subject = "Password Reset Request";
                    string body = $"Click the link below to reset your password:\nhttp://localhost:3000/resetpassword?token={resetToken}";
                    await emailService.SendEmailAsync(user.Email, subject, body);

                    return Ok("Password reset link sent successfully.");
                }

                // User not found, return a bad request
                return BadRequest("User not found.");
            }

            // Invalid model state, return bad request
            return BadRequest(ModelState);
        }
    }
}

