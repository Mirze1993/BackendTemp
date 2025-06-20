using Appilcation.CustomMiddleware;
using Appilcation.ExtensionMethods;
using Appilcation.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using PersistenceOracle;
using PersistenceOracle.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiCustomer(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

await builder.Services.OracleDbConfig(builder.Configuration);

#region Logging
// builder.Logging.ClearProviders();
// builder.Logging.AddFilter("Default", LogLevel.Debug);
// builder.Logging.AddFilter("System",
//     LogLevel.Warning); // for Microsoft.Extensions.Http.Logging
// builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
// builder.Host.UseNLog();
#endregion

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

#region RegisterService
builder.Services.AddTransient<ReqRespLogMiddleware>();

#endregion


var app = builder.Build();

app.MapOpenApiCust();
app.UseCors("AllowAllOrigins");
//app.UseRouting();
app.UseMiddleware<ReqRespLogMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseAuth();

app.MapControllers();
app.Run();

