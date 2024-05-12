//using Bislerium.Infrastructure.Data;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Bislerium.Infrastructure.SignalHubs
//{
//    public class PushNotification : Hub
//    {
//        private readonly AppDbContext appDbContext; // Inject your DbContext here

//        public PushNotification(AppDbContext appDbContext)
//        {
//            this.appDbContext = appDbContext;
//        }

//        public async Task SendReactionNotification(int postId, string message)
//        {
//            // Get the author's userId from the post
//            var post = appDbContext.Blogs.FirstOrDefault(p => p.Id == postId);
//            if (post != null)
//            {
//                var authorUserId = post.UserId; // Assuming UserId represents the author's identifier

//                // Send the notification to the author
//                await Clients.User(authorUserId.ToString()).SendAsync("ReceiveNotification", message);
//            }
//        }

//        public async Task SendCommentNotification(int postId, string message)
//        {
//            // Get the author's userId from the post
//            var post = appDbContext.Blogs.FirstOrDefault(p => p.Id == postId);
//            if (post != null)
//            {
//                var authorUserId = post.UserId; // Assuming UserId represents the author's identifier

//                // Send the notification to the author
//                await Clients.User(authorUserId.ToString()).SendAsync("ReceiveNotification", message);
//            }
//        }
//    }
//}
