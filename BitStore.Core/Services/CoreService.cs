using BitStore.Bitstamp.Models;
using BitStore.Bitstamp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BitStore.Core.Services;

/// <inheritdoc cref="ICoreService" />
public class CoreService : ICoreService
{
    private readonly IBitstampService _bitstamp;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CoreService> _logger;

    public const string BtcSymbol = "BTC";

    private readonly string _currency;
    private OrderBook? _latestOrderBook;

    public CoreService(
        IBitstampService bitstamp,
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<CoreService> logger)
    {
        _bitstamp = bitstamp;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _currency = _configuration.SecondaryCurrency;
    }

    public async Task PollDataAsync(CancellationToken cancellationToken)
    {
        var orderBook = await _bitstamp.GetOrderBookAsync(BtcSymbol, _currency);
        _latestOrderBook = orderBook;
    }

    public async Task<OrderBook?> GetLatestOrderBookAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new Exception("UserId is empty.");

        var result = _latestOrderBook;
        // it's theoretically possible if to quick call this method after startup that the data is not yet polled
        if (result == null)
        {
            await PollDataAsync(CancellationToken.None);
            result = _latestOrderBook;
        }

        if (result != null)
        {
            // Filter out bids with price <= 1000 for the demo presentation purposes.
            // The reason is that there is a huge amount of bids with tiny price to "catch" the BTC momentary fall.
            // That brings a big disproportion to other bars.
            // In a real-world scenario, we would not do this and do improve the bars presentation.
            result = new OrderBook
            {
                PrimaryCurrency = result.PrimaryCurrency,
                SecondaryCurrency = result.SecondaryCurrency,
                Timestamp = result.Timestamp,
                Bids = result.Bids.Where(x => x.Price > 1000).ToArray(),
                Asks = result.Asks.ToList()
            };

            var requestedAt = DateTimeOffset.UtcNow;
            // Create a new scope for data operations as IDataService is scoped in general
            using var scope = _scopeFactory.CreateScope();

#pragma warning disable CS4014 // We must not wait until the snapshot is stored, doing that in the background
            // In real world, we would use a proper background job processing library or some conveyor pattern.
            // But here is demonstrating that we should not spend time for storing the snapshot while the user waits for the response.
            Task.Run(async () =>
            {
                try
                {
                    using var storageScope = _scopeFactory.CreateScope();
                    var storageService = storageScope.ServiceProvider.GetRequiredService<IDataService>();
                    await storageService.StoreSnapshotForUserAsync(result, Guid.Parse(userId), requestedAt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to store order book snapshot for user {UserId}", userId);
                }
            });
#pragma warning restore CS4014
        }

        return result;
    }
}