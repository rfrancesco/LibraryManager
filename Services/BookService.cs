using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _db;
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
        public BookService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<BookDetailsDto?> GetBookByIdAsync(int bookId)
        {
            return await _db.Books
                            .Where(b => b.Id == bookId)
                            .Select(b => new
                            BookDetailsDto(
                                b.Id,
                                b.Title,
                                b.Author,
                                b.Genre,
                                !(b.Loans.Any(l => l.ReturnDate == null))
                            ))
                            .FirstOrDefaultAsync();
        }

        public async Task<List<BookDetailsDto>> SearchBooksAsync(BookQueryDto query)
        {
            var page = query.Page == null ? 1 : query.Page.Value;
            var pageSize = ValidatePageSize(query.PageSize);
            Console.WriteLine($"{page}, {pageSize}, {query.Title}, {query.Author}, {query.Genre}, {query.Available}");
            var bookQuery = _db.Books.AsQueryable();
            if (query.Title != null)
                bookQuery = bookQuery.Where(b => b.Title.ToLower().Contains(query.Title.ToLower()));
            if (query.Author != null)
                bookQuery = bookQuery.Where(b => b.Author.ToLower().Contains(query.Author.ToLower()));
            if (query.Genre != null)
                bookQuery = bookQuery.Where(b => b.Genre.ToLower().Contains(query.Genre.ToLower()));
            if (query.Available != null)
                bookQuery = bookQuery.Where(b => (!b.Loans.Any(l => l.ReturnDate == null)) == query.Available);

            return await bookQuery
                    .Select(b => new BookDetailsDto(
                        b.Id,
                        b.Title,
                        b.Author,
                        b.Genre,
                        !(b.Loans.Any(l => l.ReturnDate == null))
                    ))
                    .OrderBy(b => b.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        }
    }
}