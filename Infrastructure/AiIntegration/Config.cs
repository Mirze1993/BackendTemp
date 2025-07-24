using System.ClientModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using OpenAI;

namespace AiIntegration;

public static class Config
{
    public static  void AddAiIntegration(
        this IServiceCollection service
    )
    {
        service
            .AddKernel().Services
            .AddOpenAIChatCompletion(
                modelId: "openai/gpt-3.5-turbo-0613",
                openAIClient:
                new OpenAIClient(credential:
                    new ApiKeyCredential("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"),
                    options: new OpenAIClientOptions()
                    {
                        Endpoint = new Uri("https://openrouter.ai/api/v1")
                    }
                )
            );
    }
}