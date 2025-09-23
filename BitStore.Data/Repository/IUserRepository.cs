using BitStore.Data.Entities;

namespace BitStore.Data.Repository;

/// <summary>
/// Defines operations for managing user data in the database.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByLoginAsync(string login);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> ExistsAsync(Guid id);
}