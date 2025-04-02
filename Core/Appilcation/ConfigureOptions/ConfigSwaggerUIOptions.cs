
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Appilcation.ConfigureOptions;

public class ConfigSwaggerUiOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    : IConfigureOptions<SwaggerUIOptions>
{

    public void Configure(SwaggerUIOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
            options.SwaggerEndpoint($"/openapi/{description.GroupName}.json", $"{description.GroupName}");
    }
}