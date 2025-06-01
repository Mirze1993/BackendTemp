namespace Domain.Entities.User;

public class UserClaims : ObjectEntity
{
    public int AppUserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
}