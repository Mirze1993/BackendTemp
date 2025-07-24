using PersistenceMongo.Entities;

namespace AiAgentApi.Hubs;

public interface IChatClient
{
    Task ReceiveMessage( string message,bool isEnd );
}