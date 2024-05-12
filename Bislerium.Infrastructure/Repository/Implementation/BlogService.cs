using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.SignalHubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;


namespace Bislerium.Infrastructure.Repository.Implementation
{
    public class BlogService : IBlog
    {
        private readonly AppDbContext appDbContext;

        public BlogService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<BlogResponse> CreateBlogAsync(BlogDTO blogDTO, int userId)
        {
            try
            {
                // Validate userId, ensure it corresponds to a valid user in your system
                var user = await appDbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    return new BlogResponse(false, "Invalid user ID");
                }

                // Check if the blog image exceeds the file size limit
                if (blogDTO.BlogImage != null && blogDTO.BlogImage.Length > 3 * 1024 * 1024) // 3 MB in bytes
                {
                    return new BlogResponse(false, "Blog image file size exceeds the limit of 3 Megabytes (MB)");
                }

                // Create a new blog entity
                var newBlog = new Blog
                {
                    Title = blogDTO.Title,
                    BlogImage = blogDTO.BlogImage,
                    PublishedBy = userId,
                    PublishedDate = DateTime.Now,
                    Description = blogDTO.Description,
                    UpVoteCount = 0, // Initialize counts if needed
                    Comments = "", // Initialize comments if needed
                    DownVoteCount = 0 // Initialize counts if needed
                };

                // Add the new blog to the database
                appDbContext.Blogs.Add(newBlog);
                await appDbContext.SaveChangesAsync();

                return new BlogResponse(true, "Blog created successfully");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return new BlogResponse(false, $"Error creating blog: {ex.Message}");
            }
        }

        public async Task<BlogResponse> UpdateBlogAsync(int blogId, BlogDTO blogDTO, int userId)
        {
            try
            {
                // Retrieve the blog entity to update
                var blogToUpdate = await appDbContext.Blogs.FindAsync(blogId);
                if (blogToUpdate == null)
                {
                    return new BlogResponse(false, "Blog not found");
                }

                // Check if the user is authorized to update the blog (only the creator can update)
                if (blogToUpdate.PublishedBy != userId)
                {
                    return new BlogResponse(false, "You are not authorized to update this blog");
                }

                // Check if the blog image exceeds the file size limit
                if (blogDTO.BlogImage != null && blogDTO.BlogImage.Length > 3 * 1024 * 1024) // 3 MB in bytes
                {
                    return new BlogResponse(false, "Blog image file size exceeds the limit of 3 Megabytes (MB)");
                }

                // Update the blog entity with the new details
                blogToUpdate.Title = blogDTO.Title;
                blogToUpdate.BlogImage = blogDTO.BlogImage;
                blogToUpdate.Description = blogDTO.Description;
                // Update other properties as needed

                // Save changes to the database
                await appDbContext.SaveChangesAsync();

                return new BlogResponse(true, "Blog updated successfully");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return new BlogResponse(false, $"Error updating blog: {ex.Message}");
            }
        }

        public async Task<BlogResponse> DeleteBlogAsync(int blogId, int userId)
        {
            try
            {
                // Retrieve the blog entity to delete
                var blogToDelete = await appDbContext.Blogs.FindAsync(blogId);
                if (blogToDelete == null)
                {
                    return new BlogResponse(false, "Blog not found");
                }

                // Check if the current user is the creator of the blog
                if (blogToDelete.PublishedBy != userId)
                {
                    return new BlogResponse(false, "You are not authorized to delete this blog");
                }

                // Remove the blog entity from the database
                appDbContext.Blogs.Remove(blogToDelete);
                await appDbContext.SaveChangesAsync();

                return new BlogResponse(true, "Blog deleted successfully");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return new BlogResponse(false, $"Error deleting blog: {ex.Message}");
            }
        }

        public async Task<IEnumerable<BlogDTO>> GetAllBlogsAsync()
        {
            try
            {
                // Retrieve all blogs from the database
                var blogs = await appDbContext.Blogs.ToListAsync();

                // Map the blogs to BlogDTO objects
                var blogDTOs = blogs.Select(blog => new BlogDTO
                {
                    Title = blog.Title,
                    BlogImage = blog.BlogImage,
                    Description = blog.Description,
                    PublishedDate = blog.PublishedDate
                });

                return blogDTOs;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error getting all blogs: {ex.Message}");
                return null; // Or handle the error and return an empty collection
            }
        }


        public async Task<BlogDTO> GetBlogByIdAsync(int blogId)
        {
            try
            {
                // Retrieve the blog from the database by its ID
                var blog = await appDbContext.Blogs.FindAsync(blogId);

                if (blog == null)
                {
                    return null; // Or handle the case where the blog is not found
                }

                // Map the blog to a BlogDTO object
                var blogDTO = new BlogDTO
                {
                    Title = blog.Title,
                    BlogImage = blog.BlogImage,
                    Description = blog.Description,
                    PublishedDate = blog.PublishedDate
                };

                return blogDTO;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error getting blog by ID: {ex.Message}");
                return null; // Or handle the error appropriately
            }
        }


        public async Task<IEnumerable<BlogDTO>> GetBlogsByUserIdAsync(int userId)
        {
            try
            {
                // Retrieve blogs from the database by userId
                var blogs = await appDbContext.Blogs.Where(blog => blog.PublishedBy == userId).ToListAsync();

                // Map the blogs to BlogDTO objects
                var blogDTOs = blogs.Select(blog => new BlogDTO
                {
                    Title = blog.Title,
                    BlogImage = blog.BlogImage,
                    Description = blog.Description,
                    PublishedDate = blog.PublishedDate
                });

                return blogDTOs;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error getting blogs by user ID: {ex.Message}");
                return null; // Or handle the error and return an empty collection
            }
        }

        public async Task<BlogResponse> UpVoteBlogAsync(int blogId)
        {
            try
            {
                // Initialize SignalR connection
                var hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:3000/voteHub")
                    .Build();

                // Start the connection
                await hubConnection.StartAsync();

                // Invoke the UpvoteBlog method on the VoteHub
                await hubConnection.InvokeAsync("UpvoteBlog", blogId);

                return new BlogResponse(true, "Blog upvoted successfully");
            }
            catch (Exception ex)
            {
                return new BlogResponse(false, "Error upvoting blog: " + ex.Message);
            }
        }

        public async Task<BlogResponse> DownVoteBlogAsync(int blogId)
        {
            var blog = await appDbContext.Blogs.FindAsync(blogId);
            if (blog == null)
            {
                return new BlogResponse(false, "Blog not found");
            }

            // Increment the downvote count
            blog.DownVoteCount++;

            // Save changes to the database
            await appDbContext.SaveChangesAsync();

            return new BlogResponse(true, "Blog downvoted successfully");
        }

        public async Task<BlogResponse> AddCommentAsync(int blogId, CommentDTO commentDTO, int userId)
        {
            // Check if the blog exists
            var blog = await appDbContext.Blogs.FindAsync(blogId);
            if (blog == null)
            {
                return new BlogResponse(false, "Blog not found");
            }

            // Create the comment structure
            var newComment = $"{DateTime.UtcNow}: {commentDTO.Content} (User: {userId})";

            // Add the comment to the blog's comments
            if (blog.Comments == null)
            {
                blog.Comments = newComment;
            }
            else
            {
                blog.Comments += $"\n{newComment}";
            }

            // Save changes to the database
            await appDbContext.SaveChangesAsync();

            return new BlogResponse(true, "Comment added successfully");
        }

        public async Task<BlogResponse> DeleteCommentAsync(int blogId, int userId)
        {
            try
            {
                // Find the blog in the database
                var blog = await appDbContext.Blogs.FindAsync(blogId);
                if (blog == null)
                {
                    return new BlogResponse(false, "Blog not found");
                }

                // Check if the user is authorized to delete comments on this blog
                if (blog.PublishedBy != userId)
                {
                    return new BlogResponse(false, "Unauthorized to delete comments on this blog");
                }

                // Check if the blog has any comments to delete
                if (string.IsNullOrEmpty(blog.Comments))
                {
                    return new BlogResponse(false, "No comments to delete");
                }

                // Optionally, you can clear all comments or implement specific logic to delete a comment
                blog.Comments = ""; // Clears all comments

                // Save changes to the database
                await appDbContext.SaveChangesAsync();

                return new BlogResponse(true, "Comments deleted successfully");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return new BlogResponse(false, $"Error deleting comments: {ex.Message}");
            }
        }
    }
}
