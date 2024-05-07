using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bislerium.Infrastructure.Repository.Implementation
{
    public class UserService : IUser
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration configuration;
        private readonly ILogger<UserService> logger;

        public UserService(AppDbContext appDbContext, IConfiguration configuration, ILogger<UserService> logger)
        {
            this.appDbContext = appDbContext;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO)
        {
            var getUser = await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.UsernameOrEmail || u.Username == loginDTO.UsernameOrEmail);
            if (getUser == null)
                return new LoginResponse(false, "User not found, sorry");

            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (checkPassword)
            {
                // Generate JWT Token
                string jwtToken = GenerateJWTToken(getUser);
                return new LoginResponse(true, "Login successfully", jwtToken);
            }
            else
            {
                return new LoginResponse(false, "Invalid credentials");
            }
        }


        public async Task<RegisterResponse> RegisterUserAsync(RegisterDTO registerDTO)
        {
            var existingUsersCount = await appDbContext.Users.CountAsync();
            string role = existingUsersCount == 0 ? SystemRole.Admin : SystemRole.Blogger;

            var getUser = await FindUserByEmailOrUsername(registerDTO.Email!);
            if (getUser != null)
                return new RegisterResponse(false, "User with this email already exists");

            getUser = await FindUserByEmailOrUsername(registerDTO.Username!);
            if (getUser != null)
                return new RegisterResponse(false, "User with this username already exists");

            appDbContext.Users.Add(new ApplicationUser()
            {
                Firstname = registerDTO.Firstname!,
                Middlename = registerDTO.Middlename,
                Lastname = registerDTO.Lastname!,
                Email = registerDTO.Email!,
                Username = registerDTO.Username!,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password!),
                Role = role
            });
            await appDbContext.SaveChangesAsync();
            return new RegisterResponse(true, "Registration completed");
        }

        private string GenerateJWTToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Include user ID claim
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role!)
            };
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private async Task<ApplicationUser?> FindUserByEmailOrUsername(string emailOrUsername)
        {
            return await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == emailOrUsername || u.Username == emailOrUsername);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(int userId)
        {
            return await appDbContext.Users.FindAsync(userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await appDbContext.Users.ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(int userId, RegisterDTO updateDTO)
        {
            var user = await appDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            // Update user properties
            user.Firstname = updateDTO.Firstname!;
            user.Middlename = updateDTO.Middlename;
            user.Lastname = updateDTO.Lastname!;
            user.Email = updateDTO.Email!;
            user.Username = updateDTO.Username!;

            await appDbContext.SaveChangesAsync();
            return true; // Update successful
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await appDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            appDbContext.Users.Remove(user);
            await appDbContext.SaveChangesAsync();
            return true; // Deletion successful
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, string newRole)
        {
            var user = await appDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            return user.Role == newRole;
        }

        public Task<ApplicationUser> RequestPasswordReset(string email)
        {
            throw new NotImplementedException();
        }

        



        //public async Task RequestPasswordResetLink(ApplicationUser user)
        //{
        //    var code = GenerateJWTToken(user);


        //    var link = $"{webClientBaseUrl.DeepLinksSettings.WebClient}/change-password?email={user.Email}&activationToken={code}";

        //    var emailContent = "Your email content that contains the above link";

        //    await emailSender.SendEmailAsync(user.Email, "Password Reset", emailContent);

        //    logger.LogInformation($"A password reset email was sent to {user.Email}");
        //}
    }
}
