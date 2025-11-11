namespace AuthApi.Hubs.Models;

public class RtcSessionDescriptionInit
{
    public string Sdp { get; set; }=string.Empty;
    public string Type { get; set; }=string.Empty;
}