namespace Domain.Response.User;

public class SearchUserResp
{
    public int AppUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public string Email { get; set; }= string.Empty;
}