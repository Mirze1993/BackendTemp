namespace Domain.HubModels;

public class ChatMessageReq
{
    public string SessionId { get; set; }=string.Empty;
    public string Content { get; set; }=string.Empty;
}