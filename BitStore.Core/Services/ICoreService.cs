
using BitStore.Bitstamp.Models;

namespace BitStore.Core.Services;

public interface ICoreService
{
    Task PollDataAsync(CancellationToken cancellationToken);
    OrderBook? GetLatestOrderBook();
}