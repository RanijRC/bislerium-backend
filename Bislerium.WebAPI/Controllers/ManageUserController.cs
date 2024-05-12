using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bislerium.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageUserController : ControllerBase
    {
        private readonly IUser userService;

        public ManageUserController(IUser userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
        {
            var users = await userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetUsername(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            string username = user.Username;
            if (user == null)
            {
                return NotFound();
            }

            return Ok(username);
        }


        [HttpPost]
        public async Task<ActionResult<RegisterResponse>> CreateUser(RegisterDTO registerDTO)
        {
            var result = await userService.RegisterUserAsync(registerDTO);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, RegisterDTO updateDTO)
        {
            var success = await userService.UpdateUserAsync(id, updateDTO);
            if (!success)
            {
                return NotFound(); // User not found
            }

            return NoContent(); // Update successful
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound(); // User not found
            }

            return NoContent(); // Deletion successful
        }

        [HttpPut("role")]
        [Authorize] // Requires authentication
        public async Task<IActionResult> UpdateUserRole([FromBody] UserUpdateDTO userUpdateDTO)
        {
            // Get the current user's ID from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            Console.WriteLine(userIdClaim);
            if (userIdClaim == null)
            {
                return Unauthorized(); // Token doesn't contain user ID
            }

            int loggedInUserId = int.Parse(userIdClaim.Value);

            // Check if the current user is an admin
            var loggedInUser = await userService.GetUserByIdAsync(loggedInUserId);
            if (loggedInUser == null || !loggedInUser.Role.Equals(SystemRole.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(); // User is not authorized to update roles
            }

            // Update the user's role
            var success = await userService.UpdateUserRoleAsync(userUpdateDTO.Id, userUpdateDTO.NewRole);
            if (!success)
            {
                return NotFound(); // User not found or role update failed
            }

            return NoContent();
        }
    }
}
