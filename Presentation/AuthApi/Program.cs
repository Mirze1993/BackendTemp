using Appilcation.ExtensionMethods;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiCustomer(builder.Configuration);


var app = builder.Build();

app.MapGet("/hello", () => "Hello, World!").WithName("HelloWorldEndpoint");


app.MapOpenApiCust();
app.UseHttpsRedirection();


app.Run();

