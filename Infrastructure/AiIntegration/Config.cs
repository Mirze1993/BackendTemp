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
            .AddKernel()
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