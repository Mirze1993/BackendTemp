namespace AuthApi.Hubs;

public interface ICallClient
{
    Task VideoCallOfferCome( string callerName,string callerPhoto,string callerId ,string guid);
    Task VideoCallOfferEnd ( );
    Task VideoCallOfferAccept ( );
}