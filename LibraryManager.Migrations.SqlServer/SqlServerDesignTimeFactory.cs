using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LibraryManager.Data;

namespace LibraryManager.Migrations.SqlServer;

public class SqlServerDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(
                "Server=.;Database=Design;Trusted_Connection=True;TrustServerCertificate=True",
                b => b.MigrationsAssembly("LibraryManager.Migrations.SqlServer"))
            .Options;

        return new AppDbContext(options);
    }
}