using Bislerium.Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.SignalHubs
{
    public class CommentHub : Hub
    {
        public async Task SendCommentUpdate(int blogId)
        {
            await Clients.All.SendAsync("ReceiveCommentUpdate", blogId);
        }
    }
}
