using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
