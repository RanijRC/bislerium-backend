using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Http;
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

        public async Task<BlogResponse> CreateBlogAsync(BlogDTO blogDTO, string userId)
        {
            if(blogDTO == null) 
            {
                return new BlogResponse(false, "Blog data is required");
            }

            // Validate file size of the image
            if (blogDTO.BlogImage != null && blogDTO.BlogImage.Length > 3 * 1024 * 1024) // 3 MB limit
            {
                return new BlogResponse(false, "The image file size must not exceed 3 MB.");
            }

            // Create a new blog post
            var blog = new Blog
            {
                Title = blogDTO.Title,
                BlogImage = blogDTO.BlogImage != null ? await SaveImage(blogDTO.BlogImage) : null,
                PublishedBy = userId, // Associate the post with the current user
                PublishedDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            };

            appDbContext.Blogs.Add(blog);
            await appDbContext.SaveChangesAsync();

            return new BlogResponse(true, "Blog created successfully.");
        }

        public Task<BlogResponse> DeleteBlogAsync(int blogId)
        {
            throw new NotImplementedException();
        }

        public async Task<BlogResponse> UpdateBlogAsync(int blogId, BlogDTO blogDTO, string userId)
        {
            var blog = await appDbContext.Blogs.FindAsync(blogId);
            if (blog == null)
            {
                return new BlogResponse(false, "Blog not found.");
            }

            // Check if the currently authenticated user is the owner of the blog post
            if (blog.PublishedBy != userId)
            {
                return new BlogResponse(false, "You are not authorized to update this blog post.");
            }

            // Validate file size of the image
            if (blogDTO.BlogImage != null && blogDTO.BlogImage.Length > 3 * 1024 * 1024) // 3 MB limit
            {
                return new BlogResponse(false, "The image file size must not exceed 3 MB.");
            }

            // Update the blog post
            blog.Title = blogDTO.Title;
            if (blogDTO.BlogImage != null)
            {
                blog.BlogImage = await SaveImage(blogDTO.BlogImage);
            }

            await appDbContext.SaveChangesAsync();

            return new BlogResponse(true, "Blog updated successfully.");
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            // Implement logic to save the image file, e.g., to a storage service or disk
            // Here's a simple example assuming you are saving to a folder named "uploads" in your project directory
            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return "/uploads/" + uniqueFileName; // Return the URL or path of the saved image
        }
    }
}
