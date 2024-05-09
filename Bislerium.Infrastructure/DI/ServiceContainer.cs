using Bislerium.Domain.Entities;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Repository.Contracts;
using Bislerium.Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
                options.Cookie.Name = "JwtCookie"; // Same as the cookie name you used in UserService
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(5); // Example: Cookie expires in 5 days
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.Strict; // Adjust SameSiteMode as needed
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Set to Always if your application uses HTTPS
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
            });
            services.AddSignalR();
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IBlog, BlogService>();
            services.AddTransient<IEmail, EmailService>();
            return services;
        }
    }
}
