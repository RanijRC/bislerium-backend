using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.SignalR;


namespace Bislerium.Infrastructure.SignalHubs
{
    public class VoteHub : Hub
    {
        private readonly BlogService blogService ;

        public VoteHub(BlogService blogService)
        {
            this.blogService = blogService;
        }

        public async Task UpdateVoteCount(int postId, int upVotes, int downVotes)
        {
            // Update the vote counts in the database using the injected service/repository
            await blogService.UpdateVoteCounts(postId, upVotes, downVotes);

            // Broadcast the updated counts to all clients
            await Clients.All.SendAsync("ReceiveVoteCount", postId, upVotes, downVotes);
        }
    }
}
