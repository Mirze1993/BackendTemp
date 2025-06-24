namespace Domain.Request.User;

public class NotifResp
{
    public int Id { get; init; } 
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public bool IsRead { get; init; } 
    public DateTime CreateDate { get; init; } 
    public string Icon { get; init; } = string.Empty;
    public int Type { get; init; } 
    public int UserId { get; init; } 


}
  