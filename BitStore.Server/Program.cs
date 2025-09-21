using BitStore.Bitstamp.Services;
using BitStore.Core.Services;
using BitStore.Server.Context;
using BitStore.Server.Handlers;
using BitStore.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IConfiguration = BitStore.Core.Services.IConfiguration;

namespace BitStore.Server
{
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

            builder.Services.AddHostedService<BackgroundMainService>();
            builder.Services.AddSingleton<IConfiguration>(provider =>
            {
                var config = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<PollingSettings>>().Value;
                return new Configuration(config.SecondaryCurrency);
            });

            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<ICoreService, CoreService>();
            builder.Services.AddSingleton<IStorageService, StorageService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserContext, UserContext>();

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

            // Make sure to add this BEFORE app.UseAuthorization() but after UseRouting
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

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
}
