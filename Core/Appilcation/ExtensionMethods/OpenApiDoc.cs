using Domain.ConfigOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Appilcation.ExtensionMethods;

public static class OpenApiDoc
{
    public static IServiceCollection  AddOpenApiCustomer(this IServiceCollection  serviceProvider,IConfiguration configuration)
    {
        
        var config = new OpenApiConfig();
        configuration.GetSection(OpenApiConfig.Name).Bind(config);
        
        serviceProvider.Get
        
        serviceProvider.AddOpenApi(options =>
        {
            foreach (var VARIABLE in COLLECTION)
            {
                
            }
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = config.Title,
                    Version = config.Version,
                    Description = config.Description,
                };
                return Task.CompletedTask;
            });
        });
        
        return serviceProvider;
    }

    public static void MapOpenApiCust(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
        });
    }
    
    
}