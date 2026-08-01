using LibraryManager;
using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;

[CollectionDefinition("SqlServer collection")]
public class SqlServerCollection : ICollectionFixture<SqlServerFixture>
{
}

public class SqlServerFixture : IAsyncLifetime
{
    public readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest").Build();
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .Options;

        var context = new AppDbContext(options);
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public AppDbContext CreateAppDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .Options;

        return new AppDbContext(options);
    }
}