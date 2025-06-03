namespace Domain.Request.User;

public class LoginByRefTokReq
{
    public string? RefreshToken { get; set; }
    public int Id { get; set; }
}