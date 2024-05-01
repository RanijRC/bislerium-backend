﻿using Bislerium.Application.DTOs;
using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO)
        {
            var getUser = await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.UsernameOrEmail || u.Username == loginDTO.UsernameOrEmail);
            if (getUser == null) return new LoginResponse(false, "User not found, sorry");

            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (checkPassword)
            {
                //Generate JWT Token
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
                Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password!)
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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Email, user.Email!),
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
    }
}