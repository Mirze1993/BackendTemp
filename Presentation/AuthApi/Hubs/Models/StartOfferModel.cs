namespace AuthApi.Hubs.Models;

public class StartOfferModel
{
    public string UserId { get; set; }=string.Empty;
    public int Timer { get; set; }
    public bool IsEnd  { get; set; }
}