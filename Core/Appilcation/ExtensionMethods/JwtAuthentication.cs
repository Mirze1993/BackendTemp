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