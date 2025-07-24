using System.Text.Json.Serialization;
using AiAgentApi.Hubs;
using AiIntegration;
using Appilcation.CustomMiddleware;
using Appilcation.ExtensionMethods;
using NLog.Web;
using PersistenceMongo;
using PersistenceOracle;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiCustomer(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

await builder.Services.OracleDbConfig(builder.Configuration);

builder.Services.AddAiIntegration();
builder.Services.AddMongoClient(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

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
            policy
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200","http://localhost:4201", "http://localhost:4202", "http://localhost:4204","http://localhost:4205");
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


app.UseHttpsRedirection();
app.UseAuth();

app.MapControllers();

app.MapHub<ChatHub>("/chat");
app.Run();

