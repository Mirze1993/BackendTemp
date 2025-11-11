namespace AuthApi.Hubs.Models;

public class StartOfferModel
{
    public string UserId { get; set; }=string.Empty;
    public string Guid { get; set; }=string.Empty;
    public RtcSessionDescriptionInit? SessionDescriptionInit  { get; set; }
}