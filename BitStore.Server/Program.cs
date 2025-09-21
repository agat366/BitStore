using BitStore.Bitstamp.Services;
using BitStore.Core.Services;
using BitStore.Server.Handlers;
using BitStore.Server.Services;
using Microsoft.Extensions.Logging.Console;
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
                return new Configuration(config.PrimaryCurrency);
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddSingleton<ICoreService, CoreService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
