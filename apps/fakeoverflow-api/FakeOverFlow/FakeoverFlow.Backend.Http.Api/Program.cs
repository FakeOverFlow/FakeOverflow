using System.Text.Json.Serialization;
using FakeoverFlow.Backend.Http.Api;
using FakeoverFlow.Backend.Http.Api.Constants;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var loggerFactory = builder.SetupLoggers();
builder.ConfigureApiServices(loggerFactory);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.ConfigureProblemDetails();
app.UseCors(AppConstants.CorsPolicy);
app.UseHttpsRedirection();
app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Errors.UseProblemDetails();
        c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
    })
    .UseSwaggerUi()
    .UseSwaggerGen();

using (var scope = app.Services.CreateScope())
{
    Log.Logger.Information("Initializing database seeding");   
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
    await context.SeedAsync(CancellationToken.None);
}

await app.RunAsync();
