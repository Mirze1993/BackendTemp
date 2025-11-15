using System.Collections.Concurrent;
using AuthApi.Hubs.Models;

namespace AuthApi.Hubs;

public class CallInMemory : ICallMemory
{
    private static readonly ConcurrentDictionary<string, CallUserModel> ActiveUser = new();
    private static readonly ConcurrentDictionary<string, VideoCallDetail> VideoCall = new();


    public IEnumerable<CallUserModel> GetUsers()
    {
        return ActiveUser.Values;
    }

    public CallUserModel? GetConnectionById(string userId)
    {
        var b = ActiveUser.TryGetValue(userId, out var d);
        return b ? d : null;
    }

    public void AddUser(CallUserModel user)
    {
        if (ActiveUser.ContainsKey(user.UserId))
            ActiveUser.Remove(user.UserId, out var t);

        ActiveUser.TryAdd(user.UserId, user);
    }


    public void RemoveUser(string connectionId)
    {
        var item = ActiveUser.FirstOrDefault(mm => mm.Value.ConnectionId == connectionId);
        if (item.Key != null)
            ActiveUser.Remove(item.Key, out var t);
    }

    public VideoCallDetail GetVideoCall(string guid) => VideoCall[guid];

    public void AddVideoCall(VideoCallDetail videoCallDetail)
    {
        if (VideoCall.ContainsKey(videoCallDetail.Guid))
            VideoCall.Remove(videoCallDetail.Guid, out var t);
        VideoCall.TryAdd(videoCallDetail.Guid, videoCallDetail);
    }
}