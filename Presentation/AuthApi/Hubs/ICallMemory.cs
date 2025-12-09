using AuthApi.Hubs.Models;

namespace AuthApi.Hubs;

public interface ICallMemory
{
    IEnumerable<ActiveUserModel> GetUsers();
    ActiveUserModel? GetConnectionById(string userId);
    void AddUser(ActiveUserModel user);
    void RemoveUser(string connectionId);
    
    VideoCallDetail GetVideoCall(string guid);

    void AddVideoCall(VideoCallDetail videoCallDetail);

    RtcChatDetail GetRtcChat(string guid);

    void AddRtcChat(RtcChatDetail videoCallDetail);
}