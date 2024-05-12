using Bislerium.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Repository.Contracts
{
    public interface IBlog
    {
        Task<BlogResponse> CreateBlogAsync(BlogDTO blogDTO, int userId);
        Task<BlogResponse> UpdateBlogAsync(int blogId, BlogDTO blogDTO, int userId);
        Task<BlogResponse> DeleteBlogAsync(int blogId, int userId);
        Task<BlogDTO> GetBlogByIdAsync(int blogId);
        Task<IEnumerable<BlogDTO>> GetBlogsByUserIdAsync(int userId);
        Task<IEnumerable<BlogDTO>> GetAllBlogsAsync();
        Task<BlogResponse> UpVoteBlogAsync(int blogId);
        Task<BlogResponse> DownVoteBlogAsync(int blogId);
        Task<BlogResponse> AddCommentAsync(int blogId, CommentDTO commentDTO, int userId);
        Task<BlogResponse> DeleteCommentAsync(int commentId, int userId);
    }
}
