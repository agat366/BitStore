using BitStore.Bitstamp.Models;
using BitStore.Bitstamp.Services;

namespace BitStore.Core.Services;

public class CoreService : ICoreService
{
    private readonly IBitstampService _bitstamp;
    private readonly IConfiguration _configuration;
    private readonly IStorageService _storage;

    public const string BtcSymbol = "BTC";
    private readonly string _currency;

    private OrderBook? _latestOrderBook;

    public CoreService(
        IBitstampService bitstamp,
        IConfiguration configuration,
        IStorageService storage)
    {
        _bitstamp = bitstamp;
        _configuration = configuration;
        _storage = storage;

        _currency = _configuration.SecondaryCurrency;
    }

    public async Task PollDataAsync(CancellationToken cancellationToken)
    {
        var orderBook = await _bitstamp.GetOrderBookAsync(BtcSymbol, _currency);
        _latestOrderBook = orderBook;
    }

    public async Task<OrderBook?> GetLatestOrderBookAsync(string userId)
    {
        var latestOrderBook = _latestOrderBook;
        if (latestOrderBook == null)
        {
            await PollDataAsync(CancellationToken.None);
        }

        if (latestOrderBook != null)
        {
            var requestedAt = DateTimeOffset.UtcNow;
#pragma warning disable CS4014 // We must not wait until the snapshot is stored, doing that in the background
            Task.Run(() => _storage.StoreSnapshotForUserAsync(latestOrderBook, userId, requestedAt));
#pragma warning restore CS4014
        }

        return _latestOrderBook;
    }
}