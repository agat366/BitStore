using BitStore.Bitstamp.Models;
using BitStore.Data.Entities;

namespace BitStore.Core.Services;

/// <summary>
/// Service for handling data-storage related operations.
/// </summary>
public interface IDataService
{
    Task<User?> GetUserByLoginAsync(string login);
    Task<User> CreateUserAsync(string login);
    Task UpdateUserTokenAsync(Guid userId, string? token);

    Task StoreSnapshotForUserAsync(OrderBook orderBook, Guid userId, DateTimeOffset requestedAt);
}