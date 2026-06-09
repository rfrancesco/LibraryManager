using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class Program
    {
        private static void InitializeBookDb(AppDbContext dbContext)
        {
            dbContext.Books.Add(new Book { Title = "1984",                        Author = "George Orwell",    Genre = "Dystopian" , BorrowedByUserId = 1});
            dbContext.Books.Add(new Book { Title = "Animal Farm",                 Author = "George Orwell",    Genre = "Dystopian" });
            dbContext.Books.Add(new Book { Title = "Brave New World",             Author = "Aldous Huxley",    Genre = "Dystopian" });
            dbContext.Books.Add(new Book { Title = "The Handmaid's Tale",         Author = "Margaret Atwood",  Genre = "Dystopian" });
            dbContext.Books.Add(new Book { Title = "The Hobbit",                  Author = "J.R.R. Tolkien",   Genre = "Fantasy" });
            dbContext.Books.Add(new Book { Title = "The Lord of the Rings",       Author = "J.R.R. Tolkien",   Genre = "Fantasy", BorrowedByUserId = 2});
            dbContext.Books.Add(new Book { Title = "A Game of Thrones",           Author = "George R.R. Martin", Genre = "Fantasy" });
            dbContext.Books.Add(new Book { Title = "Crime and Punishment",        Author = "Fyodor Dostoevsky", Genre = "Classic" });
            dbContext.Books.Add(new Book { Title = "The Brothers Karamazov",      Author = "Fyodor Dostoevsky", Genre = "Classic" });

            dbContext.SaveChanges();
        }

        private static void InitializeUserDb(AppDbContext dbContext)
        {
            dbContext.Users.Add(new User { Name = "Alice" });
            dbContext.Users.Add(new User { Name = "Bob" });

            dbContext.SaveChanges();
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=library.db"));
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (!dbContext.Users.Any())
                    InitializeUserDb(dbContext);
                    
                if (!dbContext.Books.Any())
                    InitializeBookDb(dbContext);

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
