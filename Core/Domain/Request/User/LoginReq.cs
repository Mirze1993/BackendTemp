

using Domain.ValidAttributes;

namespace Domain.Request.User;

public class LoginReq
{
    [NoHtml]
    public string Email { get; set; } = string.Empty;
    
    [DecodeHtml]
    public string Password { get; set; } = string.Empty;
}