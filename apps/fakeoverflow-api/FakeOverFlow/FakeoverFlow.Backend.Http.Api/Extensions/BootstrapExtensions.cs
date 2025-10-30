using System.Collections;
using System.Text;
using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Abstracts.Clients;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Clients;
using FakeoverFlow.Backend.Http.Api.Constants;
using FakeoverFlow.Backend.Http.Api.Context;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using FakeoverFlow.Backend.Http.Api.Options;
using FakeoverFlow.Backend.Http.Api.Services;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace FakeoverFlow.Backend.Http.Api.Extensions;

public static class BootstrapExtensions
{
    public static void ConfigureApiServices(this WebApplicationBuilder builder, ILoggerFactory loggerFactory)
    {
        builder.ConfigureCors();
        builder.Services.ConfigureContext();
        builder.Services.ConfigureDatabase(builder.Configuration);
        builder.ConfigureS3();
        builder.ConfigureAuthentication();
        builder.ConfigureFastEndpoint();
        
        builder.SetupServices();
        builder.SetupClients();
    }

    private static void SetupServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPostService, PostService>();
    }

    private static void SetupClients(this WebApplicationBuilder builder)
    {
        var smtpConnectionString = builder.Configuration.GetConnectionString("SmtpConnectionString");
        if (string.IsNullOrWhiteSpace(smtpConnectionString))
        {
            builder.Services.AddSingleton<IEmailClient, NullEmailClient>();
            Log.Logger.Information("Disabled SMTP client configuration.");
        }
        else
        {
            builder.Services.AddSingleton<IEmailClient, EmailClient>();
            Log.Logger.Information("SMTP client configured.");       
        }
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

    public static void ConfigureContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddScoped<IContextFactory, ContextFactory>();
        Log.Logger.Information("Wired HttpContextAccessor and ContextFactory");       
    }    
    public static void ConfigureDatabase(this IServiceCollection service, IConfiguration configuration)
    {
        var modelEnums = typeof(BootstrapExtensions).Assembly
            .GetTypes()
            .Where(x => x.IsEnum && !string.IsNullOrWhiteSpace(x.FullName) &&
                        x.FullName.StartsWith("FakeoverFlow.Backend.Http.Api.Models.Enums."))
            .ToList();
        
        service.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("PostgresSQL"), o =>
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

        var domains = frontendUrl
            .Split(",")
            .Select(domain => domain.Trim())
            .ToArray();

        builder.Services
            .AddCors(x =>
            {
                x.AddPolicy(AppConstants.CorsPolicy, builder  =>
                {
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                    builder.WithOrigins(
                        domains
                    );
                });
            });
        
        Log.Logger.Information("Cors endpoints configured to {Endpoint}.", [domains]);
    }

    private static void ConfigureS3(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection(StorageOptions.SectionName));
        builder.Services.AddSingleton<IStorageClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<StorageOptions>>().Value;
            Log.Logger.Information("Storage configured with {option}", options.Provider);       

            return options.Provider.ToLowerInvariant() switch
            {
                "s3" => new S3StorageClient(serviceProvider.GetRequiredService<IOptions<StorageOptions>>()),
                _ => throw new InvalidOperationException($"Unknown storage provider: {options.Provider}")
            };
        });
        
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
            
        }, bearerOptions =>
        {
            bearerOptions.TokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
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

    public static void ConfigureProblemDetails(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

                var problem = new ProblemDetails()
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = exception?.Message,
                    Instance = context.TraceIdentifier
                };

                context.Response.StatusCode = problem.Status.Value;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            });
        });
        
    }
}