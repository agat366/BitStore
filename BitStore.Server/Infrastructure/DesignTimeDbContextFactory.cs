using BitStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BitStore.Server;

/// <summary>
/// Design-time factory for EF Core migrations.
/// This class is only used during development when running EF Core commands.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BitStoreDbContext>
{
    public BitStoreDbContext CreateDbContext(string[] args)
    {
        // Get configuration from Server project
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BitStoreDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
        optionsBuilder.UseSqlServer(connectionString);

        return new BitStoreDbContext(optionsBuilder.Options);
    }
}