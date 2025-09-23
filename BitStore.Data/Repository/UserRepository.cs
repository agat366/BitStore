using BitStore.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitStore.Data.Repository;

/// <summary>
/// Implements user data persistence operations using Entity Framework Core.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly BitStoreDbContext _context;

    public UserRepository(BitStoreDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }
}