namespace Domain.DTO.User;

public class GetUserDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public int Id { get; set; }
    public int FailedCount { get; set; }
    public DateTime LastFailedLogin { get; set; }
}