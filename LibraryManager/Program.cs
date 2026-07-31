using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.Sqlite;

namespace LibraryManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=library.db").UseExceptionProcessor());
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddValidation();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddProblemDetails();
            var app = builder.Build();

            app.UseExceptionHandler();
            app.UseStatusCodePages();

            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    DbInitializer.InitializeIfEmpty(dbContext);
                }

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();



            BooksEndpoint.Map(app);
            AuthorsEndpoint.Map(app);
            GenresEndpoint.Map(app);

            UsersEndpoint.Map(app);

            LoansEndpoint.Map(app);

            app.Run();
        }
    }
}
