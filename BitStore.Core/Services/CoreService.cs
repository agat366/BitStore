using BitStore.Bitstamp.Models;
using BitStore.Bitstamp.Services;

namespace BitStore.Core.Services;

public class CoreService : ICoreService
{
    private readonly IBitstampService _bitstamp;
    private readonly IConfiguration _configuration;
    
    public const string BtcSymbol = "BTC";
    private readonly string _currency;

    private OrderBook? _latestOrderBook;

    public CoreService(
        IBitstampService bitstamp,
        IConfiguration configuration)
    {
        _bitstamp = bitstamp;
        _configuration = configuration;

        _currency = _configuration.PrimaryCurrency;
    }

    public async Task PollDataAsync(CancellationToken cancellationToken)
    {
        var orderBook = await _bitstamp.GetOrderBookAsync(BtcSymbol, _currency);
        _latestOrderBook = orderBook;
    }

    public async Task<OrderBook?> GetLatestOrderBookAsync(string userId)
    {
        // todo: store audit log

        return _latestOrderBook;
    }
}