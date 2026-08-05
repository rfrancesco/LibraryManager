using LibraryManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using EntityFramework.Exceptions.Sqlite;
using LibraryManager.Data;

[CollectionDefinition("Sqlite collection")]
public class SqliteCollection : ICollectionFixture<SqliteFixture>
{
}

public class SqliteFixture : IDatabaseFixture
{
    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    public AppDbContext CreateAppDbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection,
                b => b.MigrationsAssembly("LibraryManager.Migrations.Sqlite"))
            .UseExceptionProcessor()
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}