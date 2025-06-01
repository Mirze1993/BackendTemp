namespace Domain.Entities.User;

public class AppUser : ObjectEntity
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime LastLogin { get; set; }
    public int FallidCount { get; set; }
    public DateTime LastFallidLogin { get; set; }
}