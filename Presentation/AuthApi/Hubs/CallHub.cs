using Appilcation.IRepository;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuthApi.Hubs;

[Authorize]
public class CallHub(ICallMemory memory,IUserRepository repository) : Hub<ICallClient>
{


    public async Task StartOfferVideoCall(string userId)
    {
        var callerEmail = Context.User?.Claims.First(mm => mm.Type ==UserClaimType.Email).Value;
        if (callerEmail == null)
            return;
        
        var claims = await repository.GetClaims(Convert.ToInt32( userId), "");
        var name= claims.FirstOrDefault(mm => mm.Type == UserClaimType.Name)?.Value??callerEmail;
        var photo= claims.FirstOrDefault(mm => mm.Type == UserClaimType.ProfilPictur)?.Value??"";
        
        var receiver= memory.GetConnectionById(userId);
        await Clients.Client(receiver.ConnectionId).VideoCallOfferCome(name,photo);
    }
    

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Claims.First(mm => mm.Type ==UserClaimType.Id).Value;
        if (userId == null)
            return;
        
        
        var claims = await repository.GetClaims(Convert.ToInt32( userId), "");
        var name= claims.FirstOrDefault(mm => mm.Type == UserClaimType.Name)?.Value;
        var photo= claims.FirstOrDefault(mm => mm.Type == UserClaimType.ProfilPictur)?.Value;
        
        var email = Context.User?.Claims.First(mm => mm.Type ==UserClaimType.Email).Value;
        
        memory.AddUser(new CallUserModel
        {
            UserId = userId,
            Name =  name??"",
            Picture =  photo??"",
            Email = email??"",
            ConnectionId = Context.ConnectionId
        });
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        if (connectionId != null)
            memory.RemoveUser(connectionId) ;
        return base.OnDisconnectedAsync(exception);
    }
}