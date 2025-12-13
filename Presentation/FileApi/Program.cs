using Appilcation.ExtensionMethods; 
using Microsoft.Extensions.FileProviders;   
 
var builder = WebApplication.CreateBuilder(args); 

#if DEBUG

#else
builder.WebHost.UseKestrel(). UseUrls("http://+:80");
#endif

builder.Services.AddApiVersioning(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();  

builder.Services.AddOpenApiCustomer(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
#region cors
 
builder.Services.AddCors(options => 
{
    options.AddPolicy(name: "AllowAllOrigins",
        configurePolicy: policy =>
        {
            policy.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin();
        });
    options.AddPolicy(name: "AllowOnlySomeOrigins",
        configurePolicy: policy =>
        {policy
            .SetIsOriginAllowedToAllowWildcardSubdomains()
                .WithOrigins(
                    "http://localhost:4200",
                    "https://localhost:4200",
                    "http://localhost:4201",
                    "http://localhost:4202",
                    "http://localhost:4204",
                    "http://192.168.31.146:4200",
                    "http://192.168.31.146",
                    "http://192.168.31.146:5197",
                    "https://192.168.31.146:4200",
                    "https://mc-blog.space",
                    "https://*.mc-blog.space",
                    "https://www.mc-blog.space",
                    "https://www.*.mc-blog.space",
                    "mc-blog.space",
                    "*.mc-blog.space"
                );
        });
});

#endregion

var app = builder.Build();



app.UseHttpsRedirection();
app.MapOpenApiCust();
app.UseCors("AllowAllOrigins");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/StaticFiles"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/StaticFiles"
});
app.UseAuth();
app.MapControllers();
app.Run();
