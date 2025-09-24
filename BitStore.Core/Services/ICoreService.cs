using BitStore.Bitstamp.Models;

namespace BitStore.Core.Services;

/// <summary>
/// Defines core functionality for managing order book and data storage operations.
/// </summary>
public interface ICoreService
{
    Task PollDataAsync(CancellationToken cancellationToken);
    Task<OrderBook?> GetLatestOrderBookAsync(string userId);
}