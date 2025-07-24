using System.Text;
using Domain.HubModels;
using Domain.Request.AiChatHistory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PersistenceMongo;

namespace AiAgentApi.Hubs;

public class ChatHub:Hub<IChatClient>
{
    public async Task SendMessageAi(ChatMessageReq message,
        [FromServices] IChatCompletionService chatCompletionService,
        [FromServices] Kernel kernel,
        [FromServices] ChatHistoryMdb chatHistoryMdb
        )
    {
        await chatHistoryMdb.AddHistory(new AddChatHistoryReq()
        {
            SessionId = message.SessionId, Content = message.Content, Role = "user"
        });
        OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
        var resp = new StringBuilder();
        await foreach (var item in chatCompletionService.GetStreamingChatMessageContentsAsync(message.Content
                           ,executionSettings: openAiPromptExecutionSettings
                           ,kernel:kernel
                       ))
        {
            resp.Append(item);
            await Clients.Caller.ReceiveMessage( item.ToString(),false);
        }
        await Clients.Caller.ReceiveMessage( "",true);
        await chatHistoryMdb.AddHistory(new AddChatHistoryReq()
        {
            SessionId = message.SessionId, Content = resp.ToString(), Role = "assistant"
        });
       
    }
        
}