using EntityFramework.Exceptions.Common;
using LibraryManager.Data;

namespace LibraryManager.Tests;


public abstract class LoanServiceTests<TFixture>
where TFixture : class, IDatabaseFixture
{
    private readonly TFixture _fixture;

    public LoanServiceTests(TFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task LoanDbConstraint_NoTwoOpenLoansPerBook()
    {
        var db = _fixture.CreateAppDbContext();
        var bookService = new BookService(db);
        var userService = new UserService(db);

        var user1 = await userService.CreateUserAsync("A");
        var user2 = await userService.CreateUserAsync("B");

        var book1 = await bookService.CreateBookAsync(
                new CreateBookDto("AAAAA", "AAAAA", "AAAAA"));

        var loan1 = new Loan
        {
            BookId = book1.BookId,
            UserId = user1.UserId,
            LoanDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };
        db.Loans.Add(loan1);
        var loan2 = new Loan
        {
            BookId = book1.BookId,
            UserId = user2.UserId,
            LoanDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };
        db.Loans.Add(loan2);

        await Assert.ThrowsAsync<UniqueConstraintException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task LoanDbConstraint_BookCanBeLoanedAgainAfterReturn()
    {
        var db = _fixture.CreateAppDbContext();
        var bookService = new BookService(db);
        var userService = new UserService(db);

        var user1 = await userService.CreateUserAsync("A");
        var user2 = await userService.CreateUserAsync("B");

        var book1 = await bookService.CreateBookAsync(
                new CreateBookDto("AAAAA", "AAAAA", "AAAAA"));

        var loan1 = new Loan
        {
            BookId = book1.BookId,
            UserId = user1.UserId,
            LoanDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };
        db.Loans.Add(loan1);

        await db.SaveChangesAsync();    // loan book 

        loan1.ReturnDate = DateTime.UtcNow;
        await db.SaveChangesAsync();        // return loan 

        var loan2 = new Loan
        {
            BookId = book1.BookId,
            UserId = user2.UserId,
            LoanDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };
        db.Loans.Add(loan2);

        await db.SaveChangesAsync();        // now must not throw

        var found1 = await db.Loans.FindAsync(loan1.LoanId);
        var found2 = await db.Loans.FindAsync(loan2.LoanId);

        // Both loans exist and are saved
        Assert.NotNull(found1);
        Assert.NotNull(found2);

        // Loan 1 is returned, loan 2 is open
        Assert.NotNull(found1.ReturnDate);
        Assert.Null(found2.ReturnDate);

    }

}
