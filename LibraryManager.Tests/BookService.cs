namespace LibraryManager.Tests;

[Collection("SqlServer collection")]
public class BookService_SqlServer : BookServiceTests<SqlServerFixture>
{
    public BookService_SqlServer(SqlServerFixture fixture) : base(fixture) { }
}

[Collection("Sqlite collection")]
public class BookService_Sqlite : BookServiceTests<SqliteFixture>
{
    public BookService_Sqlite(SqliteFixture fixture) : base(fixture) { }
}