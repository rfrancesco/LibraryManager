using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class LoanService : ILoanService
    {
        private readonly AppDbContext _db;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly int _defaultPageSize = 20;
        private readonly int _maxPageSize = 100;
        private int ValidatePageSize(int? pageSize)
        {
            if (pageSize is null || pageSize <= 0)
                return _defaultPageSize;
            if (pageSize > _maxPageSize)
                return _maxPageSize;
            return pageSize.Value;
        }
        public LoanService(AppDbContext db, IBookService bookService, IUserService userService)
        {
            _db = db;
            _bookService = bookService;
            _userService = userService;
        }
        public async Task<bool> HasActiveLoanAsync(int bookId)
        {
            return await _db.Loans.AnyAsync(l => l.BookId == bookId && l.ReturnDate == null);
        }

        public async Task<int?> GetActiveLoanFromBookIdAsync(int bookId)
        {
            return await _db.Loans.Where(l => l.BookId == bookId && l.ReturnDate == null)
                                  .Select(l => (int?)l.LoanId).FirstOrDefaultAsync();
        }

        public async Task<CreateLoanResult> CreateLoanAsync(int bookId, int userId)
        {
            if (!await _bookService.BookExistsAsync(bookId))
                return new CreateLoanResult(CreateLoanStatusResult.BookNotFound, null);
            if (!await _userService.UserExistsAsync(userId))
                return new CreateLoanResult(CreateLoanStatusResult.UserNotFound, null);
            if (await HasActiveLoanAsync(bookId))
                return new CreateLoanResult(CreateLoanStatusResult.BookAlreadyLoaned, null);

            var loan = new Loan { BookId = bookId, UserId = userId, LoanDate = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow.AddDays(30) };
            _db.Loans.Add(loan);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (UniqueConstraintException)
            {
                // Despite the HasActiveLoanAsync check, a race condition could apply
                // One loan per book rule is enforced at the DB level, so it could reject the update
                // UniqueConstraintException catches only the active loan case (enforced by DB)
                // other exceptions will be caught by the middleware
                return new CreateLoanResult(CreateLoanStatusResult.BookAlreadyLoaned, null);
            }

            return new CreateLoanResult(CreateLoanStatusResult.Success, new LoanDetailsDto(loan.LoanId, loan.BookId, loan.UserId, loan.LoanDate, loan.ExpiryDate, loan.ReturnDate));
        }

        public async Task<LoanDetailsDto?> GetLoanFromIdAsync(int loanId)
        {
            return await _db.Loans.Where(l => l.LoanId == loanId)
                .Select(l => new LoanDetailsDto(l.LoanId, l.BookId, l.UserId, l.LoanDate, l.ExpiryDate, l.ReturnDate))
                .FirstOrDefaultAsync();
        }
    }
}