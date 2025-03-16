namespace Domain.ConfigOptions;

public class OpenApiConfig
{
    public const string Name = "OpenApiConfig";
    
    public string Title { get; set; }=String.Empty;
    public string Description { get; set;}=String.Empty;
    public string Version { get;set; }=String.Empty;
}
