namespace LibraryManager.Tests;

[Collection("SqlServer collection")]
public class LoanService_SqlServer : LoanServiceTests<SqlServerFixture>
{
    public LoanService_SqlServer(SqlServerFixture fixture) : base(fixture) { }
}

[Collection("Sqlite collection")]
public class LoanService_Sqlite : LoanServiceTests<SqliteFixture>
{
    public LoanService_Sqlite(SqliteFixture fixture) : base(fixture) { }
}