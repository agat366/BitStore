using BitStore.Bitstamp.Models;

namespace BitStore.Core.Services;

public class StorageService : IStorageService
{
    public async Task StoreSnapshotForUserAsync(OrderBook orderBook, string userId, DateTimeOffset requestedAt)
    {
        
    }
}