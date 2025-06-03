using System.Threading.Tasks;
using Appilcation.ConfigureOptions;
using Asp.Versioning.ApiExplorer;
using Domain.ConfigOptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Appilcation.ExtensionMethods;

public static class OpenApiDoc
{
    public static IServiceCollection AddOpenApiCustomer(this IServiceCollection serviceProvider,
        IConfiguration configuration)
    {
        var config = new OpenApiConfig();
        configuration.GetSection(OpenApiConfig.Name).Bind(config);
        foreach (var item in config.Version)
            serviceProvider.AddOpenApi(item, options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = config.Title,
                        Version = item,
                        Description = config.Description,
                    };
                    return Task.CompletedTask;
                });
            });

        serviceProvider.ConfigureOptions<ConfigSwaggerUiOptions>();
        return serviceProvider;
    }

    public static void MapOpenApiCust(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwaggerUI();
    }
    
    
    internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            {
                // Add the security scheme at the document level
                var requirements = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer", // "bearer" refers to the header name here
                        In = ParameterLocation.Header,
                        BearerFormat = "Json Web Token"
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = requirements;

                // Apply it as a requirement for all operations
                foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
                {
                    operation.Value.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }] = Array.Empty<string>()
                    });
                }
            }
        }
    }
}