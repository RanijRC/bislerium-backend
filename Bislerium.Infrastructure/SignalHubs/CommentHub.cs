//using Bislerium.Infrastructure.Data;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace Bislerium.Infrastructure.SignalHubs
//{
//    public class CommentHub : Hub
//    {
//        private readonly AppDbContext appDbContext; // Inject your DbContext here

//        public CommentHub(AppDbContext appDbContext)
//        {
//            this.appDbContext = appDbContext;
//        }

//        public async Task UpdateComment(int blogId, string updatedContent)
//        {
//            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the userId from claims

//            // Find the blog owned by the user
//            var blog = appDbContext.Blogs.FirstOrDefault(b => b.Id == blogId && b.UserId.ToString() == userId);
//            if (blog != null)
//            {
//                // Update the comment within the blog
//                blog.Comments = updatedContent;

//                // Save changes to the database
//                await appDbContext.SaveChangesAsync();

//                // Notify clients about the updated comment
//                await Clients.All.SendAsync("CommentUpdated", blogId, updatedContent);
//            }
//        }

//        public async Task DeleteComment(int blogId)
//        {
//            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the userId from claims

//            // Find the blog owned by the user
//            var blog = appDbContext.Blogs.FirstOrDefault(b => b.Id == blogId && b.UserId.ToString() == userId);
//            if (blog != null)
//            {
//                // Remove the comment from the blog
//                blog.Comments = null; // Or set it to an empty string if needed

//                // Save changes to the database
//                await appDbContext.SaveChangesAsync();

//                // Notify clients about the deleted comment
//                await Clients.All.SendAsync("CommentDeleted", blogId);
//            }
//        }
//    }
//}
