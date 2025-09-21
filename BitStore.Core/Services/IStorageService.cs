using BitStore.Bitstamp.Models;

namespace BitStore.Core.Services;

public interface IStorageService
{
    Task StoreSnapshotForUserAsync(OrderBook orderBook, string userId, DateTimeOffset requestedAt);
}