using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class BooksEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/books", (AppDbContext dbContext, [AsParameters] BookQueryDto query) =>
            {
                var page = query.Page == null ? 1 : query.Page.Value;
                var pageSize = query.PageSize == null ? BookQueryDto.DefaultPageSize : query.PageSize.Value;
                Console.WriteLine($"{page}, {pageSize}, {query.Title}, {query.Author}, {query.Genre}, {query.Available}");
                var bookQuery = dbContext.Books.AsQueryable();
                if (query.Title != null)
                    bookQuery = bookQuery.Where(b => b.Title.ToLower().Contains(query.Title.ToLower()));
                if (query.Author != null)
                    bookQuery = bookQuery.Where(b => b.Author.ToLower().Contains(query.Author.ToLower()));
                if (query.Genre != null)
                    bookQuery = bookQuery.Where(b => b.Genre.ToLower().Contains(query.Genre.ToLower()));
                if (query.Available != null)
                    bookQuery = bookQuery.Where(b => (!b.Loans.Any(l => l.ReturnDate == null)) == query.Available);

                return Results.Ok(bookQuery
                        // Public version:
                        //.Select(b => b.Id, b.Title, b.Author, b.Genre, available = b.BorrowedByUserId == null)
                        // Admin version:
                        .Select(b => new
                        {
                            b.Id,
                            b.Title,
                            b.Author,
                            b.Genre,
                            available = !(b.Loans.Any(l => l.ReturnDate == null)),
                            // b.BorrowedByUserId,
                            // BorrowedBy = b.BorrowedBy != null ? b.BorrowedBy.Name : null
                        })
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList());
            });

            app.MapGet("/books/{id}", (int id, AppDbContext dbContext) => Results.Ok(dbContext.Books
                                        .Where(b => b.Id == id)
                                        .Select(b => new
                                        {
                                            b.Id,
                                            b.Title,
                                            b.Author,
                                            b.Genre,
                                            available = !(b.Loans.Any(l => l.ReturnDate == null)),
                                            // b.BorrowedByUserId,
                                            // BorrowedBy = b.BorrowedBy != null ? b.BorrowedBy.Name : null
                                        })
                                        .FirstOrDefault()));
        }
    }
}