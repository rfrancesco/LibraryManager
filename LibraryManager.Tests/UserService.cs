namespace LibraryManager.Tests;

[Collection("SqlServer collection")]
public class UserService_SqlServer : UserServiceTests<SqlServerFixture>
{
    public UserService_SqlServer(SqlServerFixture fixture) : base(fixture) { }
}

[Collection("Sqlite collection")]
public class UserService_Sqlite : UserServiceTests<SqliteFixture>
{
    public UserService_Sqlite(SqliteFixture fixture) : base(fixture) { }
}