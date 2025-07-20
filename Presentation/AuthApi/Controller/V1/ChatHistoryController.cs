using Asp.Versioning;
using Domain;
using Domain.Request.AiChatHistory;
using Domain.RoutePaths;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersistenceMongo;
using PersistenceMongo.Entities;

namespace AuthApi.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
public class ChatHistoryController(ChatHistoryMdb chatHistoryMdb): ControllerBase
{

    [Authorize]
    [HttpGet(RoutePaths.ChGetAllSession)]
    public async Task<Result<List<ChatSessionModel>>> GetSessions() =>
        await chatHistoryMdb.GetSessions(GetId()).SuccessResult();
    
    [Authorize]
    [HttpPost(RoutePaths.ChCreateSession)]
    public async Task<Result<ChatSessionModel>> ChCreateSession() =>
        await chatHistoryMdb.CreateSession(GetId()).SuccessResult();
    
    [Authorize]
    [HttpPost(RoutePaths.ChAddHistory)]
    public async Task<Result> ChAddHistory([FromBody] AddChatHistoryReq req) =>
        await chatHistoryMdb.AddHistory(req).SuccessResult();
    
    private int GetId()=>
        int.Parse(User.Claims.First(mm => mm.Type == "Id").Value);
}