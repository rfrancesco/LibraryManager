using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LibraryManager.Data;

namespace LibraryManager.Migrations.Sqlite;

public class SqliteDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(
                "Data Source=design.db",
                b => b.MigrationsAssembly("LibraryManager.Migrations.Sqlite"))
            .Options;

        return new AppDbContext(options);
    }
}