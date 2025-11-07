namespace AuthApi.Hubs.Models;

public class VideoCallDetail
{
    public string Guid { get; set; }=string.Empty;
    public string FromUserId { get; set; } = string.Empty;
    public string ToUserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}