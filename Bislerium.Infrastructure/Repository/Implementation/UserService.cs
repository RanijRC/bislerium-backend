using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Bislerium.Infrastructure.Repository.Implementation
{
    public class UserService : IUser
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration configuration;

        public UserService(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.appDbContext = appDbContext;
            this.configuration = configuration;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO, HttpContext httpContext)
        {
            var getUser = await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.UsernameOrEmail || u.Username == loginDTO.UsernameOrEmail);
            if (getUser == null)
                return new LoginResponse(false, "User not found");

            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (checkPassword)
            {
                // Generate JWT Token
                string jwtToken = GenerateJWTToken(getUser);

                // Store the JWT token in the token field of the user
                getUser.Token = jwtToken;

                // Update the user entity in the database
                await appDbContext.SaveChangesAsync();

                // Create a cookie with the JWT token
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(5), // Example: Cookie expires in 5 days
                    Secure = true, // Set to true if your application uses HTTPS
                    SameSite = SameSiteMode.Strict // Adjust SameSiteMode as needed
                };
                httpContext.Response.Cookies.Append("JwtCookie", jwtToken, cookieOptions);

                return new LoginResponse(true, "Login successfully", jwtToken, getUser.Id); // Return UserId along with token
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


        public async Task<ApplicationUser?> FindUserByEmailOrUsername(string emailOrUsername)
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

        public async Task<ApplicationUser> ForgotPassword(string email)
        {
            // Find the user by email
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                // User not found, return null or throw an exception
                return null;
            }

            // Generate the password reset token
            string resetToken = await GeneratePasswordResetToken(user);

            // Update the user entity with the reset token and its expiration
            user.ResetToken = resetToken;
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1); // Set token expiration time (e.g., 1 hour)

            // Save changes to the database
            await appDbContext.SaveChangesAsync();

            // Return the updated user object
            return user;
        }


        public async Task<string> GeneratePasswordResetToken(ApplicationUser user)
        {
            // Generate a unique secure token (e.g., using GUID)
            string resetToken = Guid.NewGuid().ToString();

            // Update the user entity with the reset token and its expiration
            user.ResetToken = resetToken;
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1); // Set token expiration time (e.g., 1 hour)

            // Save changes to the database asynchronously
            await appDbContext.SaveChangesAsync();

            // Return the generated reset token
            return resetToken;
        }
    }
}
