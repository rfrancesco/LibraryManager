using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=library.db"));
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DbInitializer.InitializeIfEmpty(dbContext);
            }

            app.MapGet("/books", (AppDbContext dbContext, [AsParameters] BookQuery query) =>
            {
                var page = query.Page == null ? 1 : query.Page.Value;
                var pageSize = query.PageSize == null ? BookQuery.DefaultPageSize : query.PageSize.Value;
                Console.WriteLine($"{page}, {pageSize}, {query.Title}, {query.Author}, {query.Genre}, {query.Available}");
                var bookQuery = dbContext.Books.AsQueryable();
                if (query.Title != null)
                    bookQuery = bookQuery.Where(b => b.Title.ToLower().Contains(query.Title.ToLower()));
                if (query.Author != null)
                    bookQuery = bookQuery.Where(b => b.Author.ToLower().Contains(query.Author.ToLower()));
                if (query.Genre != null)
                    bookQuery = bookQuery.Where(b => b.Genre.ToLower().Contains(query.Genre.ToLower()));
                if (query.Available != null)
                    bookQuery = bookQuery.Where(b => (b.BorrowedByUserId == null) == query.Available);

                return bookQuery
                        // Public version:
                        //.Select(b => b.Id, b.Title, b.Author, b.Genre, available = b.BorrowedByUserId == null)
                        // Admin version:
                        .Select(b => new
                        {
                            b.Id,
                            b.Title,
                            b.Author,
                            b.Genre,
                            available = b.BorrowedByUserId == null,
                            b.BorrowedByUserId,
                            BorrowedBy = b.BorrowedBy != null ? b.BorrowedBy.Name : null
                        })
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            });

            app.MapGet("/books/{id}", (int id, AppDbContext dbContext) => dbContext.Books
                                        .Where(b => b.Id == id)
                                        .Select(b => new
                                        {
                                            b.Id,
                                            b.Title,
                                            b.Author,
                                            b.Genre,
                                            available = b.BorrowedByUserId == null,
                                            b.BorrowedByUserId,
                                            BorrowedBy = b.BorrowedBy != null ? b.BorrowedBy.Name : null
                                        })
                                        .FirstOrDefault());

            app.MapGet("/authors", (AppDbContext dbContext, [AsParameters] BookQuery query) =>
            {
                var page = query.Page == null ? 1 : query.Page.Value;
                var pageSize = query.PageSize == null ? BookQuery.DefaultPageSize : query.PageSize.Value;

                var bookQuery = dbContext.Books.AsQueryable();
                if (query.Title != null)
                    bookQuery = bookQuery.Where(b => b.Title.ToLower().Contains(query.Title.ToLower()));
                if (query.Author != null)
                    bookQuery = bookQuery.Where(b => b.Author.ToLower().Contains(query.Author.ToLower()));
                if (query.Genre != null)
                    bookQuery = bookQuery.Where(b => b.Genre.ToLower().Contains(query.Genre.ToLower()));
                if (query.Available != null)
                    bookQuery = bookQuery.Where(b => (b.BorrowedByUserId == null) == query.Available);

                return bookQuery
                        .Select(b => new
                        {
                            b.Author,
                        })
                        .Distinct()
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            });

            app.MapGet("/genres", (AppDbContext dbContext, [AsParameters] BookQuery query) =>
            {
                var page = query.Page == null ? 1 : query.Page.Value;
                var pageSize = query.PageSize == null ? BookQuery.DefaultPageSize : query.PageSize.Value;

                var bookQuery = dbContext.Books.AsQueryable();
                if (query.Title != null)
                    bookQuery = bookQuery.Where(b => b.Title.ToLower().Contains(query.Title.ToLower()));
                if (query.Author != null)
                    bookQuery = bookQuery.Where(b => b.Author.ToLower().Contains(query.Author.ToLower()));
                if (query.Genre != null)
                    bookQuery = bookQuery.Where(b => b.Genre.ToLower().Contains(query.Genre.ToLower()));
                if (query.Available != null)
                    bookQuery = bookQuery.Where(b => (b.BorrowedByUserId == null) == query.Available);

                return bookQuery
                        .Select(b => new
                        {
                            b.Genre,
                        })
                        .Distinct()
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            });

            app.MapGet("/users", (AppDbContext dbContext) => dbContext.Users.ToList());
            app.MapGet("/users/{id}", (int id, AppDbContext dbContext) => dbContext.Users.Include(u => u.Books).FirstOrDefault(u => u.UserId == id));


            app.Run();
        }
    }
}
