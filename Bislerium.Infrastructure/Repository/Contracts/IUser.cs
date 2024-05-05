using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Infrastructure.Repository.Contracts
{
    public interface IUser
    {
        Task<RegisterResponse> RegisterUserAsync(RegisterDTO registerDTO);
        Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO);
        Task<ApplicationUser> GetUserByIdAsync(int userId);
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
        Task<bool> UpdateUserAsync(int userId, RegisterDTO updateDTO);
        Task<bool> DeleteUserAsync(int userId);
    }
}
