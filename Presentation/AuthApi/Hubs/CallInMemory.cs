using System.Collections.Concurrent;

namespace AuthApi.Hubs;

public class CallInMemory:ICallMemory
{
    private static readonly ConcurrentDictionary<string, CallUserModel> ActiveUser  = new();


    public IEnumerable<CallUserModel> GetUsers()
    {
        return ActiveUser.Values;
    }
    
    public CallUserModel GetConnectionById(string userId)
    {
        return ActiveUser[userId];
    }

    public void AddUser(CallUserModel user)
    {
        if(ActiveUser.ContainsKey(user.UserId))
            ActiveUser.Remove(user.UserId, out var t);
        
        ActiveUser.TryAdd(user.UserId, user);
    }
    
    

    public void RemoveUser(string connectionId)
    {
       var item= ActiveUser.FirstOrDefault(mm => mm.Value.ConnectionId == connectionId);
       if(item.Key != null)
        ActiveUser.Remove(item.Key, out var t);
    }
}