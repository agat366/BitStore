using BitStore.Bitstamp.Services;
using BitStore.Core.Services;
using BitStore.Server.Context;
using BitStore.Server.Handlers;
using BitStore.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BitStore.Data;
using Microsoft.EntityFrameworkCore;
using IConfiguration = BitStore.Core.Services.IConfiguration;

namespace BitStore.Server;

/// <summary>
/// Entry point class for the BitStore server application.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options =>
        {
            options.FormatterName = ConsoleFormatterNames.Simple;
        });

        // Configure JWT
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key")))
                };
            });

        // Add services to the container.
        builder.Services.Configure<PollingSettings>(
            builder.Configuration.GetSection(PollingSettings.SectionName));
        
        var httpClientBuilder = builder.Services.AddHttpClient<IBitstampService, BitstampService>();
#if DEBUG
        builder.Services.AddTransient<LoggingHandler>();
        httpClientBuilder.AddHttpMessageHandler<LoggingHandler>();
#endif

        // Change the order of service registration for clarity
        builder.Services.AddBitStoreData(builder.Configuration);  // Register DbContext and repositories first
        
        // Register scoped services
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IDataService, DataService>();
        builder.Services.AddScoped<IUserContext, UserContext>();

        // Register singletons
        builder.Services.AddSingleton<IConfiguration>(provider =>
        {
            var config = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<PollingSettings>>().Value;
            return new Configuration(config.SecondaryCurrency);
        });
        builder.Services.AddSingleton<ICoreService, CoreService>();  // Keep as singleton
        
        // Register hosted services
        builder.Services.AddHostedService<BackgroundMainService>();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins("http://localhost:52134")  // Your Vue app URL
                    .AllowAnyMethod()                       // Allows OPTIONS
                    .AllowAnyHeader()
                    .WithExposedHeaders("*");
            });
        });

        // Register framework services first
        builder.Services.AddHttpContextAccessor();  // Move this up before dependent services
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // Register data and repositories
        builder.Services.AddBitStoreData(builder.Configuration);
        
        // Register scoped services
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IDataService, DataService>();
        builder.Services.AddScoped<IUserContext, UserContext>();  // This depends on IHttpContextAccessor

        // Register singletons
        builder.Services.AddSingleton<IConfiguration>(provider =>
        {
            var config = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<PollingSettings>>().Value;
            return new Configuration(config.SecondaryCurrency);
        });
        builder.Services.AddSingleton<ICoreService, CoreService>();
        
        // Register hosted services
        builder.Services.AddHostedService<BackgroundMainService>();

        var app = builder.Build();

        // Initialize Database
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BitStoreDbContext>();
            
            if (app.Environment.IsDevelopment())
            {
                // In development, automatically create/update database
                context.Database.Migrate();
            }
            else
            {
                // In production, only verify database is up to date
                if (context.Database.GetPendingMigrations().Any())
                {
                    throw new Exception("Database is out of sync - pending migrations detected");
                }
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseCors();

        app.Run();
    }
}
