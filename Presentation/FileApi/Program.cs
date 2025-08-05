using Appilcation.ExtensionMethods; 
using Microsoft.Extensions.FileProviders;  

var builder = WebApplication.CreateBuilder(args);

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
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    options.AddPolicy(name: "AllowOnlySomeOrigins",
        configurePolicy: policy =>
        {
            policy.WithOrigins("https://example1.com",
                "https://example2.com");
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
