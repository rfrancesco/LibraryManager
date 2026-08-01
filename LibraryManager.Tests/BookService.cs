namespace LibraryManager.Tests;

[Collection("SqlServer collection")]
public class BookServiceTests
{
    private readonly SqlServerFixture _fixture;

    public BookServiceTests(SqlServerFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CreateBookAsync_StoresToDatabase()
    {
        var db = _fixture.CreateAppDbContext();
        var bookService = new BookService(db);

        var book1 = await bookService.CreateBookAsync(new CreateBookDto("Title1", "Author1", "Genre1"));
        var book2 = await bookService.CreateBookAsync(new CreateBookDto("Title2", "Author2", "Genre2"));

        var found1 = await db.Books.FindAsync(book1.BookId);
        var found2 = await db.Books.FindAsync(book2.BookId);

        Assert.NotNull(found1);
        Assert.NotNull(found2);

        Assert.Equal(book1.Title, found1.Title);
        Assert.Equal(book1.Author, found1.Author);
        Assert.Equal(book1.Genre, found1.Genre);

        Assert.Equal(book2.Title, found2.Title);
        Assert.Equal(book2.Author, found2.Author);
        Assert.Equal(book2.Genre, found2.Genre);
    }

    [Fact]
    public async Task BookExistsAsync_TrueOnlyForStoredBooks()
    {
        var db = _fixture.CreateAppDbContext();
        var bookService = new BookService(db);

        Assert.False(await bookService.BookExistsAsync(-1));
        Assert.False(await bookService.BookExistsAsync(int.MaxValue));
        Assert.False(await bookService.BookExistsAsync(1000));

        var book1 = await bookService.CreateBookAsync(new CreateBookDto("Title3", "Author3", "Genre3"));
        var book2 = await bookService.CreateBookAsync(new CreateBookDto("Title4", "Author4", "Genre4"));

        Assert.True(await bookService.BookExistsAsync(book1.BookId));
        Assert.True(await bookService.BookExistsAsync(book2.BookId));
    }

    [Fact]
    public async Task GetBookByIdAsync_DtoMatchesStoredData()
    {
        var db = _fixture.CreateAppDbContext();
        var bookService = new BookService(db);

        var book1 = await bookService.CreateBookAsync(new CreateBookDto("Title5", "Author5", "Genre5"));
        var book2 = await bookService.CreateBookAsync(new CreateBookDto("Title6", "Author6", "Genre6"));

        var found1 = await bookService.GetBookByIdAsync(book1.BookId);
        var found2 = await bookService.GetBookByIdAsync(book2.BookId);
        var notFound = await bookService.GetBookByIdAsync(book2.BookId + 10);

        Assert.NotNull(found1);
        Assert.NotNull(found2);
        Assert.Null(notFound);

        Assert.Equal(book1.Title, found1.Title);
        Assert.Equal(book1.Author, found1.Author);
        Assert.Equal(book1.Genre, found1.Genre);

        Assert.Equal(book2.Title, found2.Title);
        Assert.Equal(book2.Author, found2.Author);
        Assert.Equal(book2.Genre, found2.Genre);
    }
}
