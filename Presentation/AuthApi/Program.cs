using System.Diagnostics;
using System.Text.Json.Serialization;
using Appilcation.CustomMiddleware;
using Appilcation.ExtensionMethods;
using AuthApi.Hubs;
using ExternalServices;
using OpenTelemetryLib;
using PersistenceMongo;
using PersistenceOracle;  
using Refit;    
   
var builder = WebApplication.CreateBuilder(args); 

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

builder.Services.AddApiVersioning(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiCustomer(builder.Configuration);
 
builder.Services.AddJwtAuthentication(builder.Configuration);

await builder.Services.OracleDbConfig(builder.Configuration);

builder.Services.AddOpenTelemetryServices(builder.Configuration);
builder.Logging.AddOpenTelemetryLogging(builder.Configuration);

builder.Services.AddMongoClient(builder.Configuration);

builder.Services.AddRefitClient<IAsanFinance>().ConfigureHttpClient(configureClient =>
{
    configureClient.BaseAddress = new Uri(builder.Configuration.GetValue("AsanFinance",""));
    configureClient.DefaultRequestHeaders.Add("token",builder.Configuration.GetValue("AsanFinanceToken",""));
});

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
#if DEBUG

#else
builder.WebHost.UseKestrel(). UseUrls("http://+:80");
#endif
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
                .WithOrigins("http://localhost:4200",
                    "http://localhost:4201",
                    "http://localhost:4202",
                    "http://localhost:4204",
                    "https://mc-blog.space",
                    "https://www.mc-blog.space",
                    "mc-blog.space"
                );
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
builder.Services.AddSingleton<ICallMemory,CallInMemory>();

#endregion


var app = builder.Build();

app.MapOpenApiCust();
app.UseCors("AllowAllOrigins");
//app.UseRouting();
app.UseMiddleware<ReqRespLogMiddleware>();


app.UseHttpsRedirection();
app.UseAuth();

app.MapControllers();
app.MapHub<CallHub>("/callChat");
app.Run();

