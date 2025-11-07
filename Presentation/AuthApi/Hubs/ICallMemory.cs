using AuthApi.Hubs.Models;

namespace AuthApi.Hubs;

public interface ICallMemory
{
    IEnumerable<CallUserModel> GetUsers();
    CallUserModel GetConnectionById(string userId);
    void AddUser(CallUserModel user);
    void RemoveUser(string connectionId);
    
    VideoCallDetail GetVideoCall(string guid);

    void AddVideoCall(VideoCallDetail videoCallDetail);
}