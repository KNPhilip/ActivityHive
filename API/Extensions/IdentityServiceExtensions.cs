using System.Text;
using API.Services.AuthService;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

// using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, 
            IConfiguration config)
        {
            services.AddIdentityCore<User>(options => {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.User.RequireUniqueEmail = true;
                // Email verification is disabled in production
                // options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

            services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero

                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => 
                    {
                        StringValues accessToken = context.Request.Query["access_token"];
                        PathString path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options => {
                options.AddPolicy("IsActivityHost", policy => {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}