using System.Text.Json;
using BitStore.Bitstamp.Models;
using BitStore.Data.Entities;
using BitStore.Data.Repository;

namespace BitStore.Core.Services;

/// <summary>
/// Implementation of data service for storing order book snapshots and user data.
/// </summary>
public class DataService : IDataService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;

    public DataService(
        IAuditLogRepository auditLogRepository,
        IUserRepository userRepository)
    {
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _userRepository.GetByLoginAsync(login);
    }

    public async Task<User> CreateUserAsync(string login)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = login
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task UpdateUserTokenAsync(Guid userId, string? token)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new ArgumentException($"User not found: {userId}");

        user.Token = token;
        await _userRepository.UpdateAsync(user);
    }

    public async Task StoreSnapshotForUserAsync(OrderBook orderBook, Guid userId, DateTimeOffset requestedAt)
    {
        if (Guid.Empty.Equals(userId))
            throw new ArgumentException("Invalid user ID");

        if (!await _userRepository.ExistsAsync(userId))
            throw new ArgumentException($"User not found: {userId}", nameof(userId));

        var snapshot = new
        {
            orderBook.PrimaryCurrency,
            orderBook.SecondaryCurrency,
            orderBook.Timestamp,
            RequestedAt = requestedAt,
            orderBook.Bids,
            orderBook.Asks
        };

        var json = JsonSerializer.Serialize(snapshot);

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Timestamp = requestedAt,
            Data = json
        };

        await _auditLogRepository.CreateAsync(auditLog);
    }
}