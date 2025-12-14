using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Bolt.Persistence.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Get the current directory
        var basePath = Directory.GetCurrentDirectory();

        // If we're in the Persistence project, go up one level
        if (basePath.EndsWith("Bolt.Persistence"))
        {
            basePath = Directory.GetParent(basePath)!.FullName;
        }

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("Bolt.API/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Bolt.API/appsettings.Development.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseSqlServer(connectionString);

        return new ApplicationDbContext(builder.Options);
    }
}
