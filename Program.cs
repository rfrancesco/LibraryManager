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

            app.MapGet("/books", (AppDbContext dbContext) => dbContext.Books.Select(b => new {b.Id, b.Author, b.Title, b.Genre, Available = b.BorrowedByUserId == null}));
            app.MapGet("/books/borrowed", (AppDbContext dbContext) => dbContext.Books.Where(b => b.BorrowedByUserId != null).Select(b => new {b.Id, b.Author, b.Title, b.Genre, Available = b.BorrowedByUserId == null}));
            app.MapGet("/books/available", (AppDbContext dbContext) => dbContext.Books.Where(b => b.BorrowedByUserId == null).Select(b => new {b.Id, b.Author, b.Title, b.Genre, Available = b.BorrowedByUserId == null}));

            app.MapGet("/books/{id}", (int id, AppDbContext dbContext) => dbContext.Books.Find(id));

            app.MapGet("/users", (AppDbContext dbContext) => dbContext.Users.ToList());
            app.MapGet("/users/{id}", (int id, AppDbContext dbContext) => dbContext.Users.Include(u => u.Books).FirstOrDefault(u => u.UserId == id));
            

            app.Run();
        }
    }
}
