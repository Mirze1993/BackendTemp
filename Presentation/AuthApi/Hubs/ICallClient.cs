using AuthApi.Hubs.Models;

namespace AuthApi.Hubs;

public interface ICallClient
{
    Task StartOfferVideoCallHandle( string callerName,string callerPhoto,string callerId ,string guid);
    Task EndOfferVideoCallHandle (string result );
    Task AcceptVideoCallHandle ( );
    
    
    Task StartOfferRtcChatHandle( string callerName,string callerPhoto,string callerId ,string guid,string publicKey);
    Task EndOfferRtcChatHandle (string result );
    Task AcceptRtcChatHandle ( string publicKey);
    
    Task VideoRtcSignalHandle(dynamic data);
    Task ChatRtcSignalHandle(dynamic data);
}