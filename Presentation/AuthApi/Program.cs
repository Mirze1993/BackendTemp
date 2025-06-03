using Appilcation.CustomMiddleware;
using Appilcation.ExtensionMethods;
using Appilcation.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using PersistenceOracle.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiCustomer(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

#region Logging
// builder.Logging.ClearProviders();
// builder.Logging.AddFilter("Default", LogLevel.Debug);
// builder.Logging.AddFilter("System",
//     LogLevel.Warning); // for Microsoft.Extensions.Http.Logging
// builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
// builder.Host.UseNLog();
#endregion


#region RegisterService
builder.Services.AddTransient<ReqRespLogMiddleware>();
builder.Services.AddSingleton<IUserRepository,UserRepository>();

#endregion


var app = builder.Build();

app.MapOpenApiCust();

//app.UseRouting();
app.UseMiddleware<ReqRespLogMiddleware>();

app.UseHttpsRedirection();
app.UseAuth();

app.MapControllers();
app.Run();

