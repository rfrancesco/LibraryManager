using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Tests;

public class BookServiceTests
{
    [Fact]
    public async Task CreateBookAsync_StoresToDatabase()
    {
        var db = TestDbContextFactory.Create();
        var bookService = new BookService(db);

        var book1 = await bookService.CreateBookAsync(new CreateBookDto("Title", "Author", "Genre"));
        var book2 = await bookService.CreateBookAsync(new CreateBookDto("Title2", "Author2", "Genre2"));

        var found1 = await db.Books.FindAsync(book1.BookId);
        var found2 = await db.Books.FindAsync(book2.BookId);

        var number = await db.Books.CountAsync();

        Assert.NotNull(found1);
        Assert.NotNull(found2);

        Assert.Equal(2, number);

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
        var db = TestDbContextFactory.Create();
        var bookService = new BookService(db);

        Assert.False(await bookService.BookExistsAsync(0));
        Assert.False(await bookService.BookExistsAsync(1));
        Assert.False(await bookService.BookExistsAsync(2));

        var book1 = await bookService.CreateBookAsync(new CreateBookDto("Title", "Author", "Genre"));
        var book2 = await bookService.CreateBookAsync(new CreateBookDto("Title2", "Author2", "Genre2"));

        Assert.True(await bookService.BookExistsAsync(book1.BookId));
        Assert.True(await bookService.BookExistsAsync(book2.BookId));
    }

    [Fact]
    public async Task GetBookByIdAsync_DtoMatchesStoredData()
    {
        var db = TestDbContextFactory.Create();
        var bookService = new BookService(db);

        var book1 = await bookService.CreateBookAsync(new CreateBookDto("Title", "Author", "Genre"));
        var book2 = await bookService.CreateBookAsync(new CreateBookDto("Title2", "Author2", "Genre2"));

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
