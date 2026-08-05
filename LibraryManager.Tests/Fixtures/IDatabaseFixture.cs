using LibraryManager.Data;

public interface IDatabaseFixture : IAsyncLifetime
{
    AppDbContext CreateAppDbContext();
};