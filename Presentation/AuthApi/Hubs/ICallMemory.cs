namespace AuthApi.Hubs;

public interface ICallMemory
{
    IEnumerable<CallUserModel> GetUsers();
    CallUserModel GetConnectionById(string userId);
    void AddUser(CallUserModel user);
    void RemoveUser(string connectionId);
}