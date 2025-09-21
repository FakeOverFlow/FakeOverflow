using System.Collections;
using System.Text;
using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Constants;
using FakeoverFlow.Backend.Http.Api.Context;
using FakeoverFlow.Backend.Http.Api.Options;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;

namespace FakeoverFlow.Backend.Http.Api.Extensions;

public static class BootstrapExtensions
{
    public static void ConfigureApiServices(this WebApplicationBuilder builder, ILoggerFactory loggerFactory)
    {
        builder.ConfigureCors();
        builder.ConfigureContext();
        builder.ConfigureDatabase();
        builder.ConfigureAuthentication();
        builder.ConfigureFastEndpoint();
    }

    public static ILoggerFactory SetupLoggers(this WebApplicationBuilder builder)
    {
        // Enable debugging if env is present 
        var logLevel = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("API_ENABLE_DEBUG")) 
            ? LogEventLevel.Information 
            : LogEventLevel.Debug;

        var loggerConfiguration = new LoggerConfiguration();

        loggerConfiguration.WriteTo
            .Console(restrictedToMinimumLevel: logLevel, theme: AnsiConsoleTheme.Code);
        
        var logPath = Environment.GetEnvironmentVariable("API_LOG_PATH");
        if (!string.IsNullOrWhiteSpace(logPath))
        {
            loggerConfiguration.WriteTo
                .File(
                    path: Path.Join(logPath, "rp-.log"),
                    restrictedToMinimumLevel: logLevel,
                    rollingInterval: RollingInterval.Day,
                    encoding: Encoding.UTF8,
                    formatter: new JsonFormatter()
                );
        }

        var logger = loggerConfiguration.CreateLogger();
        Log.Logger = logger;
        builder.Services.AddSerilog(logger);

        if (logLevel != LogEventLevel.Debug) return new SerilogLoggerFactory(logger);
        
        var envVariables = Environment.GetEnvironmentVariables()
            .Cast<DictionaryEntry>()
            .Select(entry => $"{entry.Key}={entry.Value}")
            .ToArray();

        logger.Debug("ENV: [{environment}]", string.Join(", ", envVariables));

        return new SerilogLoggerFactory(logger);;
    }

    private static void ConfigureContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IContextFactory, ContextFactory>();
        Log.Logger.Information("Wired HttpContextAccessor and ContextFactory");       
    }    
    private static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var modelEnums = typeof(BootstrapExtensions).Assembly
            .GetTypes()
            .Where(x => x.IsEnum && !string.IsNullOrWhiteSpace(x.FullName) &&
                        x.FullName.StartsWith("FakeoverFlow.Backend.Http.Api.Models.Enums."))
            .ToList();
        
        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSQL"), o =>
            {
                o.SetPostgresVersion(17, 0);
                o.UseNodaTime();
                Log.Logger.Debug("Mapping {Enums} to Postgres", modelEnums.Select(x => x.Name));
                foreach (var modelEnum in modelEnums)
                {
                    o.MapEnum(modelEnum);
                }
            });
        });
        Log.Logger.Information("Initializing database");
    }

    private static void ConfigureCors(this WebApplicationBuilder builder)
    {
        var frontendUrl = builder.Configuration.GetValue<string>("Frontend:BaseDomain");
        if (string.IsNullOrWhiteSpace(frontendUrl))
        {
            Log.Logger.Warning("Frontend:BaseDomain not configured, Cors endpoints will not be configured.");
            return;
        }

        builder.Services
            .AddCors(x =>
            {
                x.AddPolicy(AppConstants.CorsPolicy, builder  =>
                {
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                    builder.WithOrigins([frontendUrl]);
                });
            });
        
        Log.Logger.Information("Cors endpoints configured to {Endpoint}.", [frontendUrl]);
    }

    private static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration.GetSection("JwtSettings").Get<JwtOptions>();
        if (jwtOptions is null)
        {
            Log.Logger.Warning("JwtSettings not configured, Authentication will not be configured.");
            return;
        }
        
        Log.Logger.Information("JwtSettings has been identified. {Audience}", jwtOptions.Audience);
        builder.Services.AddAuthenticationJwtBearer(sOptions =>
        {
            sOptions.SigningKey = jwtOptions.SigningKey;
            sOptions.SigningStyle = TokenSigningStyle.Symmetric;
        }, bearerOptions =>
        {
            bearerOptions.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudience = jwtOptions.Audience,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });
        builder.Services.AddAuthorization();
        Log.Logger.Information("Authentication configured");
    }

    private static void ConfigureFastEndpoint(this WebApplicationBuilder builder)
    {
        builder.Services.AddFastEndpoints();
        builder.Services.SwaggerDocument();
        Log.Logger.Information("FastEndpoint configured");       
    }
}