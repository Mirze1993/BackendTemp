namespace Domain.Tool;

public class GenTokenDto
{
    public string? Name { get; set; }
    public string? Photo { get; set; }
    public string? Email { get; set; }
    public int Id { get; set; }
    public List<string>? Roles { get; set; }
}