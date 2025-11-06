using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Appilcation.ExtensionMethods;

public static class JwtAuthentication
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection serviceProvider,
        IConfiguration configuration)
    {
        serviceProvider.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    RoleClaimType = "Role",
                    NameClaimType = "Name",
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWTOption:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWTOption:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        configuration["JWTOption:Key"] ?? ""))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken)
                            &&
                            (path.StartsWithSegments("/chat") || path.StartsWithSegments("/callChat")))
                            // Read the token out of the query string
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });

        serviceProvider.AddAuthorization();
        
        return serviceProvider;
    }
    
    public static void UseAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}