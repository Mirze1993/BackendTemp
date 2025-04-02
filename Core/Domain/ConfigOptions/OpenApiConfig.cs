using System;
using System.Collections.Generic;

namespace Domain.ConfigOptions;

public class OpenApiConfig
{
    public const string Name = "OpenApiConfig";
    
    public string Title { get; set; }=String.Empty;
    public string Description { get; set;}=String.Empty;
    public List<string> Version { get;set; }=new List<string>();
}
