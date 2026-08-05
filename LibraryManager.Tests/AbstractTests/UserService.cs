namespace LibraryManager.Tests;

public abstract class UserServiceTests<TFixture>
where TFixture : class, IDatabaseFixture
{
    private readonly TFixture _fixture;

    public UserServiceTests(TFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task UserServiceTests_CreateUserAsync()
    {
        var db = _fixture.CreateAppDbContext();
        var userService = new UserService(db);

        var name = "User Name 1";
        var user = await userService.CreateUserAsync(name);


        Assert.Equal(name, user.Name);
    }

    [Fact]
    public async Task UserServiceTests_UserExistsAsync()
    {
        var db = _fixture.CreateAppDbContext();
        var userService = new UserService(db);

        bool notFound1 = await userService.UserExistsAsync(-1);
        bool notFound2 = await userService.UserExistsAsync(int.MaxValue);

        Assert.False(notFound1);
        Assert.False(notFound2);

        var user = await userService.CreateUserAsync("User Name 2");
        bool found = await userService.UserExistsAsync(user.UserId);

        Assert.True(found);
    }

    [Fact]
    public async Task UserServiceTests_GetUserByIdAsync()
    {
        var db = _fixture.CreateAppDbContext();
        var userService = new UserService(db);

        var name1 = "User Name 3";
        var name2 = "User Name 4";

        var user1 = await userService.CreateUserAsync(name1);
        var user2 = await userService.CreateUserAsync(name2);

        var found1 = await userService.GetUserByIdAsync(user1.UserId);
        var found2 = await userService.GetUserByIdAsync(user2.UserId);

        Assert.NotNull(found1);
        Assert.Equal(name1, found1.Name);
        Assert.Equal(name1, user1.Name);

        Assert.NotNull(found2);
        Assert.Equal(name2, found2.Name);
        Assert.Equal(name2, user2.Name);
    }
}
