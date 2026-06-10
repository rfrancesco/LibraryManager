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

            BooksEndpoint.Map(app);
            AuthorsEndpoint.Map(app);
            GenresEndpoint.Map(app);

            UsersEndpoint.Map(app);

            app.Run();
        }
    }
}
