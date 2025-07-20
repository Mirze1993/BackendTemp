using PersistenceMongo.Entities;

namespace AuthApi.Hubs;

public interface IChatClient
{
    Task ReceiveMessage( string message,bool isEnd );
}