using Bislerium.Application.DTOs;
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

        public UserController(IUser user)
        {
            this.user = user;
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
            return Ok(result);
        }

        [HttpPost]
        [Route("resetpassword")]
        public async Task<ActionResult<RegisterResponse>> ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email is required for password reset." });
            }

            try
            {
                // Request password reset and send email
                var resetResult = await user.RequestPasswordReset(email);
                return Ok(resetResult);
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                return StatusCode(500, new { message = "An error occurred during password reset request." });
            }
        }

    }
}
