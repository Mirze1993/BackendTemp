using AuthApi.Hubs.Models;

namespace AuthApi.Hubs;

public interface ICallClient
{
    Task StartOfferVideoCallHandle( string callerName,string callerPhoto,string callerId ,string guid);
    Task EndOfferVideoCallHandle (string result );
    Task AcceptVideoCallHandle ( );
    
    Task RtcSignalHandle(dynamic data);
}