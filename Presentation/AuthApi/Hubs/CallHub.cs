using System.Runtime.InteropServices.ComTypes;
using Appilcation.IRepository;
using AuthApi.Hubs.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuthApi.Hubs;

[Authorize]
public class CallHub(ICallMemory memory,IUserRepository repository) : Hub<ICallClient>
{


    public async Task StartOfferVideoCall(string userId,string guid)
    {
        var callerEmail = Context.User?.Claims.First(mm => mm.Type ==UserClaimType.Email).Value;
        if (callerEmail == null)
            return;
        
        var claims = await repository.GetClaims(Convert.ToInt32( userId), "");
        var name= claims.FirstOrDefault(mm => mm.Type == UserClaimType.Name)?.Value??callerEmail;
        var photo= claims.FirstOrDefault(mm => mm.Type == UserClaimType.ProfilPictur)?.Value??"";
        var callerId = Context.User?.Claims.First(mm => mm.Type ==UserClaimType.Id).Value;;
        
        var receiver= memory.GetConnectionById(userId);
       
        memory.AddVideoCall(new()
        {
            Guid = guid,
            FromUserId = callerId,
            ToUserId = userId,
            Status = "Pending"
        });
        
        await Clients.Client(receiver.ConnectionId).VideoCallOfferCome(name,photo,callerId,guid);
    }

    public async Task EndOfferVideoCall(string guid)
    {
       var d= memory.GetVideoCall(guid);
        
        var fromUser= memory.GetConnectionById(d.FromUserId);
        var toUser= memory.GetConnectionById(d.ToUserId);
        await Clients.Client(fromUser.ConnectionId).VideoCallOfferEnd();
        await Clients.Client(toUser.ConnectionId).VideoCallOfferEnd();
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
        
        memory.AddUser(new CallUserModel()
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