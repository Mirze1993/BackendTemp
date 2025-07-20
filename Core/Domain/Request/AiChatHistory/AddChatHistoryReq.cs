namespace Domain.Request.AiChatHistory;

public class AddChatHistoryReq
{
    public string SessionId { get; set; }=string.Empty;
    public string Role   { get; set; }=string.Empty;
    public string Content   { get; set; }=string.Empty;
    public string AuthorName  { get; set; }=string.Empty;
}