using Bislerium.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Bislerium.Infrastructure.Repository.Contracts
{
    public interface IUser
    {
        Task<RegisterResponse> RegisterUserAsync(RegisterDTO registerDTO);
        Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO);

    }
}
