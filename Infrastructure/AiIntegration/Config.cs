using System.ClientModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using OpenAI;


namespace AiIntegration;

public static class Config
{
    public static  void AddAiIntegration(
        this IServiceCollection service,IConfiguration configuration
    )
    {
        service
            .AddKernel().Services
            .AddOpenAIChatCompletion(
                modelId: configuration.GetValue ("OpenAIChatCompletion:ModelId",""),
                openAIClient:
                new OpenAIClient(credential:
                    new ApiKeyCredential(configuration.GetValue ("OpenAIChatCompletion:Key","") ),
                    options: new OpenAIClientOptions()
                    {
                        Endpoint = new Uri(configuration.GetValue ("OpenAIChatCompletion:Endpoint",""))
                    }
                )
            );
    }
}