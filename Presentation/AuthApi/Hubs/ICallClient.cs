namespace AuthApi.Hubs;

public interface ICallClient
{
    Task VideoCallOfferCome( string userName,string profilePictures );
}