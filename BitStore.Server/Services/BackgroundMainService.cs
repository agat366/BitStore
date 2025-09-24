using BitStore.Core.Services;
using Microsoft.Extensions.Options;

namespace BitStore.Server.Services;

/// <summary>
/// Background service that periodically polls and updates order book data.
/// </summary>
public class BackgroundMainService(
    IServiceScopeFactory scopeFactory,
    ILogger<BackgroundMainService> logger,
    IOptions<PollingSettings> settings)
    : BackgroundService
{
    private readonly PollingSettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var coreService = scope.ServiceProvider.GetRequiredService<ICoreService>();
                await coreService.PollDataAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while polling data");
            }

            await Task.Delay(TimeSpan.FromSeconds(_settings.UpdateIntervalSeconds), stoppingToken);
        }
    }
}