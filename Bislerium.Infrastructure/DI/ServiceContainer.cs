using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;



namespace Bislerium.Infrastructure.DI
{
    public static class ServiceContainer
    {
        public static IServiceCollection InfrastructureServices(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ServiceContainer).Assembly.FullName)),
                ServiceLifetime.Scoped);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "AuthToken";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // or Always if using HTTPS
                options.ExpireTimeSpan = TimeSpan.FromHours(1); // Adjust expiration time as needed
                options.LoginPath = "/Account/Login"; // Adjust login path if needed
                options.AccessDeniedPath = "/Account/AccessDenied"; // Adjust access denied path if needed
                options.SlidingExpiration = true; // Extend expiration on activity
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
            });
            services.AddScoped<IUser, UserService>();
            return services;
        }
    }
}
