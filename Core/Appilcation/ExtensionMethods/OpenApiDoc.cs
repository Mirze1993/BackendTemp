using System.Threading.Tasks;
using Appilcation.ConfigureOptions;
using Asp.Versioning.ApiExplorer;
using Domain.ConfigOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    // var versionedDescriptionProvider = context
                    //     .ApplicationServices
                    //     .GetService<IApiVersionDescriptionProvider>();

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
}