using System.Text.Json.Serialization;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);
var loggerFactory = builder.SetupLoggers();
builder.ConfigureApiServices(loggerFactory);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
    })
    .UseSwaggerUi()
    .UseSwaggerGen();
await app.RunAsync();
