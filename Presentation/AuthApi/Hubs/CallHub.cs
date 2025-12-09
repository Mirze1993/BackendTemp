using System.Runtime.InteropServices.ComTypes;
using Appilcation.IRepository;
using AuthApi.Hubs.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AuthApi.Hubs;

[Authorize]
public class CallHub(ICallMemory memory, IUserRepository repository) : Hub<ICallClient>
{
    public async Task StartOfferVideoCall(string userId, string guid)
    {
        var callerEmail = Context.User?.Claims.First(mm => mm.Type == UserClaimType.Email).Value;
        if (callerEmail == null)
            return;

        var claims = await repository.GetClaims(Convert.ToInt32(userId), "");
        var name = claims.FirstOrDefault(mm => mm.Type == UserClaimType.Name)?.Value ?? callerEmail;
        var photo = claims.FirstOrDefault(mm => mm.Type == UserClaimType.ProfilPictur)?.Value ?? "";
        var callerId = Context.User?.Claims.First(mm => mm.Type == UserClaimType.Id).Value;


        var receiver = memory.GetConnectionById(userId);
        if (receiver == null)
        {
            await Clients.Caller.EndOfferVideoCallHandle("NotActive");
            return;
        }


        memory.AddVideoCall(new()
        {
            Guid = guid,
            FromUserId = callerId,
            ToUserId = userId,
            Status = "Pending"
        });

        await Clients.Client(receiver.ConnectionId).StartOfferVideoCallHandle(name, photo, callerId, guid);
    }

    public async Task EndOfferVideoCall(string guid, string result)
    {
        var d = memory.GetVideoCall(guid);

        var fromUser = memory.GetConnectionById(d.FromUserId);
        var toUser = memory.GetConnectionById(d.ToUserId);
        if (fromUser != null)
            await Clients.Client(fromUser.ConnectionId).EndOfferVideoCallHandle(result);
        if (toUser != null)
            await Clients.Client(toUser.ConnectionId).EndOfferVideoCallHandle(result);
    }

    public async Task AcceptVideoCall(string guid)
    {
        var d = memory.GetVideoCall(guid);

        var fromUser = memory.GetConnectionById(d.FromUserId);
        var toUser = memory.GetConnectionById(d.ToUserId);
        await Clients.Client(fromUser.ConnectionId).AcceptVideoCallHandle();
        await Clients.Client(toUser.ConnectionId).AcceptVideoCallHandle();
    }


    public async Task VideoRtcSignal(string guid,  dynamic data)
    {
        var callerId = Context.User?.Claims.First(mm => mm.Type == UserClaimType.Id).Value;

        var d = memory.GetVideoCall(guid);
        if (callerId == d.FromUserId)
        {
            var toUser = memory.GetConnectionById(d.ToUserId);
            await Clients.Client(toUser.ConnectionId).VideoRtcSignalHandle( data);
        }
        else
        {
            var fromUser = memory.GetConnectionById(d.FromUserId);
            await Clients.Client(fromUser.ConnectionId).VideoRtcSignalHandle( data);
        }
    }


    #region RtcChat

    public async Task StartOfferRtcChat(string userId, string guid)
    {
        var callerEmail = Context.User?.Claims.First(mm => mm.Type == UserClaimType.Email).Value;
        if (callerEmail == null)
            return;

        var claims = await repository.GetClaims(Convert.ToInt32(userId), "");
        var name = claims.FirstOrDefault(mm => mm.Type == UserClaimType.Name)?.Value ?? callerEmail;
        var photo = claims.FirstOrDefault(mm => mm.Type == UserClaimType.ProfilPictur)?.Value ?? "";
        var callerId = Context.User?.Claims.First(mm => mm.Type == UserClaimType.Id).Value;


        var receiver = memory.GetConnectionById(userId);
        if (receiver == null)
        {
            await Clients.Caller.EndOfferRtcChatHandle("NotActive");
            return;
        }


        memory.AddRtcChat(new()
        {
            Guid = guid,
            FromUserId = callerId,
            ToUserId = userId,
            Status = "Pending"
        });

        await Clients.Client(receiver.ConnectionId).StartOfferRtcChatHandle(name, photo, callerId, guid);
    }

    public async Task EndOfferRtcChat(string guid, string result)
    {
        var d = memory.GetRtcChat(guid);

        var fromUser = memory.GetConnectionById(d.FromUserId);
        var toUser = memory.GetConnectionById(d.ToUserId);
        if (fromUser != null)
            await Clients.Client(fromUser.ConnectionId).EndOfferRtcChatHandle(result);
        if (toUser != null)
            await Clients.Client(toUser.ConnectionId).EndOfferRtcChatHandle(result);
    }

    public async Task AcceptRtcChat(string guid)
    {
        var d = memory.GetRtcChat(guid);

        var fromUser = memory.GetConnectionById(d.FromUserId);
        var toUser = memory.GetConnectionById(d.ToUserId);
        await Clients.Client(fromUser.ConnectionId).AcceptRtcChatHandle();
        await Clients.Client(toUser.ConnectionId).AcceptRtcChatHandle();
    }

    public async Task ChatRtcSignal(string guid, dynamic data)
    {
        var callerId = Context.User?.Claims.First(mm => mm.Type == UserClaimType.Id).Value;

        var d = memory.GetRtcChat(guid);
        if (callerId == d.FromUserId)
        {
            var toUser = memory.GetConnectionById(d.ToUserId);
            await Clients.Client(toUser.ConnectionId).ChatRtcSignalHandle(data);
        }
        else
        {
            var fromUser = memory.GetConnectionById(d.FromUserId);
            await Clients.Client(fromUser.ConnectionId).ChatRtcSignalHandle(data);
        }
    }

    #endregion

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Claims.First(mm => mm.Type == UserClaimType.Id).Value;
        if (userId == null)
            return;


        var claims = await repository.GetClaims(Convert.ToInt32(userId), "");
        var name = claims.FirstOrDefault(mm => mm.Type == UserClaimType.Name)?.Value;
        var photo = claims.FirstOrDefault(mm => mm.Type == UserClaimType.ProfilPictur)?.Value;

        var email = Context.User?.Claims.First(mm => mm.Type == UserClaimType.Email).Value;

        memory.AddUser(new ActiveUserModel()
        {
            UserId = userId,
            Name = name ?? "",
            Picture = photo ?? "",
            Email = email ?? "",
            ConnectionId = Context.ConnectionId
        });
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        if (connectionId != null)
            memory.RemoveUser(connectionId);
        return base.OnDisconnectedAsync(exception);
    }
}