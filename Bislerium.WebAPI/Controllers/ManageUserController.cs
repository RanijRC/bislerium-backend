using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.Repository.Implementation;
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
        public async Task<ActionResult<ApplicationUser>> GetUser(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
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

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string newRole)
        {
            // Get the currently logged-in user ID (you may have this information in the request)
            int loggedInUserId = GetCurrentUserId();

            bool isAdmin = await userService.UpdateUserRoleAsync(loggedInUserId, SystemRole.Admin);
            if (!isAdmin)
            {
                return Unauthorized(); // User is not authorized to update roles
            }

            var success = await userService.UpdateUserRoleAsync(id, newRole);
            if (!success)
            {
                return NotFound(); // User not found or role update failed
            }

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            // Handle the case where the user ID is not available in the token
            throw new InvalidOperationException("User ID not available in JWT token");
        }
    }
}
