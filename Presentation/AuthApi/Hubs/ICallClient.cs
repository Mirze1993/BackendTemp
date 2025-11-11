using AuthApi.Hubs.Models;

namespace AuthApi.Hubs;

public interface ICallClient
{
    Task VideoCallOfferCome( string callerName,string callerPhoto,string callerId ,string guid);
    Task EndOfferVideoCallHandle ( );
    Task AcceptVideoCallHandle ( );
    
    Task RtcSignalHandle(dynamic data);
}