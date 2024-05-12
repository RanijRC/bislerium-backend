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
        Task<bool> UpdateUserRoleAsync(int userId, string newRole);

        Task<ApplicationUser?> FindUserByEmailOrUsername(string emailOrUsername);
        Task<string> GeneratePasswordResetToken(ApplicationUser user);
        Task<ApplicationUser> ForgotPassword(string email);
        Task<ChangePasswordResponse> ChangePassword(string token, string newPassword);
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
        Task<bool> UpdateUserAsync(int userId, RegisterDTO updateDTO);
        Task<bool> DeleteUserAsync(int userId);
    }
}
