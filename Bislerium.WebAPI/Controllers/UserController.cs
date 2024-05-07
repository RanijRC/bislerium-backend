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
                var result = await user.LoginUserAsync(loginDTO);
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

        //[HttpPost]
        //[Route("reset-link")]
        //public async Task<IActionResult> SendResetLink([FromQuery] string email)
        //{
        //    if (email.IsEmpty())
        //    {
        //        return BadRequest(new Error()
        //        {
        //            Code = "400",
        //            Description = "Invalid Email!"
        //        });
        //    }

        //    await user.
        //}
    }
}
