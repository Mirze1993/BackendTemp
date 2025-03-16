using Appilcation.ExtensionMethods;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApiCustomer(builder.Configuration);
builder.Services.AddApiVersioning(builder.Configuration);

var app = builder.Build();

app.MapGet("/hello", () => "Hello, World!").WithName("HelloWorldEndpoint");


app.MapOpenApiCust();
app.UseHttpsRedirection();


app.Run();

