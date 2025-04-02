using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning.ApiExplorer;
using Domain.ConfigOptions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Appilcation.ConfigureOptions;

public class ConfigOpenApiOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    : IConfigureOptions<OpenApiOptions>
{
    public void Configure(OpenApiOptions options)
    {
        var config = new OpenApiConfig();
        configuration.GetSection(OpenApiConfig.Name).Bind(config);

        foreach (var item in provider.ApiVersionDescriptions.Reverse())
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = config.Title,
                    Version = item.ApiVersion.ToString(),
                    Description = config.Description,
                };
                return Task.CompletedTask;
            });
    }
}