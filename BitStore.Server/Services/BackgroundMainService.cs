using BitStore.Core.Services;
using Microsoft.Extensions.Options;

namespace BitStore.Server.Services;

public class PollingSettings
{
    public const string SectionName = "PollingSettings";
    
    public string SecondaryCurrency { get; set; } = "EUR";
    public int UpdateIntervalSeconds { get; set; } = 2;
}

public class BackgroundMainService : BackgroundService
{
    private readonly ILogger<BackgroundMainService> _logger;
    private readonly ICoreService _coreService;
    private readonly PollingSettings _settings;

    public BackgroundMainService(
        ILogger<BackgroundMainService> logger,
        IOptions<PollingSettings> options,
        ICoreService coreService)
    {
        _logger = logger;
        _coreService = coreService;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _coreService.PollDataAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling data.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_settings.UpdateIntervalSeconds), stoppingToken);
        }
    }
}