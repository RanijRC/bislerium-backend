using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.Repository.Implementation;
using Bislerium.Infrastructure.SignalHubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Bislerium.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlog blogService;
        private readonly IUser userService;
        private readonly VoteHub voteContext;

        public BlogController(IBlog blogService, IUser userService, VoteHub voteContext)
        {
            this.blogService = blogService;
            this.userService = userService;
            this.voteContext = voteContext;
        }

        [HttpPost]
        [Route("createblog")]
        public async Task<IActionResult> CreateBlog([FromBody] BlogDTO blogDTO)
        {
            // Get the logged-in user's ID from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            Console.WriteLine(userIdClaim);
            if (userIdClaim == null)
            {
                return Unauthorized(); // User ID not found in the token
            }

            int userId = int.Parse(userIdClaim.Value);

            // Use the UserService to retrieve the user details
            var user = await userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Use the BlogService to create the blog using the user's ID
            var response = await blogService.CreateBlogAsync(blogDTO, userId);

            if (response.Flag)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPut("update/{blogId}")]
        [Authorize] // Add authorization attribute to ensure the user is authenticated
        public async Task<IActionResult> UpdateBlog(int blogId, [FromBody] BlogDTO blogDTO)
        {
            try
            {
                // Get the userId from the authenticated user's claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new BlogResponse(false, "Unauthorized"));
                }

                // Parse the userId from the claim (assuming it's an integer)
                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new BlogResponse(false, "Unauthorized"));
                }

                // Call the UpdateBlogAsync method in your UserService or BlogService
                var response = await blogService.UpdateBlogAsync(blogId, blogDTO, userId);

                // Check the response and return appropriate IActionResult
                if (response.Flag)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BlogResponse(false, $"Error updating blog: {ex.Message}"));
            }
        }



        [HttpDelete("delete/{blogId}")]
        [Authorize] // Add authorization attribute to ensure the user is authenticated
        public async Task<IActionResult> DeleteBlog(int blogId)
        {
            try
            {
                // Get the userId from the authenticated user's claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new BlogResponse(false, "Unauthorized"));
                }

                // Parse the userId from the claim (assuming it's an integer)
                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new BlogResponse(false, "Unauthorized"));
                }

                // Call the DeleteBlogAsync method in your BlogService or UserService
                var response = await blogService.DeleteBlogAsync(blogId, userId);

                // Check the response and return appropriate IActionResult
                if (response.Flag)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BlogResponse(false, $"Error deleting blog: {ex.Message}"));
            }
        }



        [HttpGet]
        [Route("allblogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            try
            {
                // Call your service method to get all blogs
                var blogs = await blogService.GetAllBlogsAsync();

                // Check if the result is not null
                if (blogs != null)
                {
                    // Return the list of blogs
                    return Ok(blogs);
                }
                else
                {
                    // Handle the case where the result is null
                    return NotFound("No blogs found");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting all blogs: {ex.Message}");
            }
        }

        [HttpGet("{blogId}")]
        public async Task<IActionResult> GetBlogById(int blogId)
        {
            try
            {
                // Call your service method to get the blog by ID
                var blog = await blogService.GetBlogByIdAsync(blogId);

                // Check if the blog is found
                if (blog != null)
                {
                    // Return the blog DTO
                    return Ok(blog);
                }
                else
                {
                    // Handle the case where the blog is not found
                    return NotFound("Blog not found");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting blog by ID: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBlogsByUserId(int userId)
        {
            try
            {
                // Call your service method to get blogs by user ID
                var blogs = await blogService.GetBlogsByUserIdAsync(userId);

                // Check if any blogs are found
                if (blogs != null && blogs.Any())
                {
                    // Return the list of blog DTOs
                    return Ok(blogs);
                }
                else
                {
                    // Handle the case where no blogs are found for the user
                    return NotFound("No blogs found for the user");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error getting blogs by user ID: {ex.Message}");
            }
        }

        [HttpPost("upvote/{blogId}")]
        public async Task<IActionResult> UpvoteBlog(int blogId)
        {
            try
            {
                // Invoke the upvote logic in the service layer
                var upvoteResult = await blogService.UpVoteBlogAsync(blogId);

                // Check if the upvote was successful
                if (upvoteResult.Flag)
                {
                    // Notify clients about the upvote event
                    await voteContext.Clients.All.SendAsync("ReceiveUpvote", blogId);

                    // Return 200 OK status code indicating success along with the success message
                    return Ok(upvoteResult.Message);
                }
                else
                {
                    // Return 400 Bad Request status code indicating failure along with the error message
                    return BadRequest(upvoteResult.Message);
                }
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error status code for exceptions
                return StatusCode(500, $"Error upvoting blog: {ex.Message}");
            }
        }

        [HttpPost("downvote/{blogId}")]
        public async Task<IActionResult> DownvoteBlog(int blogId)
        {
            try
            {
                // Call the DownVoteBlogAsync method in your service layer
                var downvoteResult = await blogService.DownVoteBlogAsync(blogId);

                // Check if the downvote was successful
                if (downvoteResult.Flag)
                {
                    // Notify clients about the downvote event if needed
                    await voteContext.Clients.All.SendAsync("ReceiveDownvote", blogId);

                    // Return 200 OK status code indicating success along with the success message
                    return Ok(downvoteResult.Message);
                }
                else
                {
                    // Return 404 Not Found status code indicating that the blog was not found
                    return NotFound(downvoteResult.Message);
                }
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error status code for exceptions
                return StatusCode(500, $"Error downvoting blog: {ex.Message}");
            }
        }

    }

}
