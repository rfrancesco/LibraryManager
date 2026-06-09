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

            app.MapGet("/books", (AppDbContext dbContext, string? title, string? author, string? genre, bool? available) =>
            {
                var query = dbContext.Books.AsQueryable();
                if (title != null)
                    query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));
                if (author != null)
                    query = query.Where(b => b.Author.ToLower().Contains(author.ToLower()));
                if (genre != null)
                    query = query.Where(b => b.Genre.ToLower().Contains(genre.ToLower()));
                if (available != null)
                    query = query.Where(b => (b.BorrowedByUserId == null) == available);

                return query
                        // Public version:
                        //.Select(b => b.Id, b.Title, b.Author, b.Genre, available = b.BorrowedByUserId == null)
                        // Admin version:
                        .Select(b => new { b.Id, b.Title, b.Author, b.Genre, available = b.BorrowedByUserId == null, 
                                b.BorrowedByUserId, BorrowedBy = b.BorrowedBy != null ? b.BorrowedBy.Name : null})
                        .ToList();
            });

            app.MapGet("/books/{id}", (int id, AppDbContext dbContext) => dbContext.Books.Find(id));

            app.MapGet("/users", (AppDbContext dbContext) => dbContext.Users.ToList());
            app.MapGet("/users/{id}", (int id, AppDbContext dbContext) => dbContext.Users.Include(u => u.Books).FirstOrDefault(u => u.UserId == id));
            

            app.Run();
        }
    }
}
