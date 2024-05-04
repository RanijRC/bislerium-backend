using Bislerium.Application.SignalHubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;

namespace Bislerium.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        protected readonly IHubContext<CommentHub> comment;
        public CommentController([NotNull] IHubContext<CommentHub> comment)
        {
            this.comment = comment;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentPost commentPost)
        {
            await comment.Clients.All.SendAsync("SendToReact");
            return Ok();
        }

        public class CommentPost
        {
            public virtual string? Comment { get; set; }
        }
    }
}
