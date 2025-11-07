namespace AuthApi.Hubs.Models;

public class CallUserModel
{
    public string ConnectionId { get; set; }=string.Empty;
    public string UserId { get; set; }=string.Empty;
    public string Name { get; set; }=string.Empty;
    public string Email { get; set; }=string.Empty;
    public string Picture { get; set; }=string.Empty;
    public DateTime ConnectDate { get; set; }=DateTime.Now;
}