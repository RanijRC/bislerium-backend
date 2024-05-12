using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.SignalR;


namespace Bislerium.Infrastructure.SignalHubs
{
    public class VoteHub : Hub
    {
        private readonly AppDbContext appDbContext;

        public VoteHub(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task UpvoteBlog(int blogId)
        {
            var blog = await appDbContext.Blogs.FindAsync(blogId);
            if (blog == null)
            {
                // Handle the case where the blog is not found
                return;
            }

            // Increment the upvote count
            blog.UpVoteCount++;

            // Save changes to the database
            await appDbContext.SaveChangesAsync();

            // Notify clients about the upvote
            await Clients.All.SendAsync("ReceiveUpvote", blogId);
        }

        //public async Task DownvoteBlog(int blogId)
        //{

        //    // Notify clients about the downvote
        //    await Clients.All.SendAsync("ReceiveDownvote", blogId);
        //}
    }
}
