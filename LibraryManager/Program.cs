using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.Sqlite;

namespace LibraryManager
{
    public class Program
    {
        public static async Task Main(string[] args)
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

            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Demo"))
            {
                bool seedDemoData = builder.Configuration.GetValue<bool>("SEED_DEMO_DATA");
                if (seedDemoData)
                {
                    using (var scope = app.Services.CreateScope())
                    {
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
                        logger.LogInformation($"{app.Environment.EnvironmentName} mode: SEED_DEMO_DATA=true");

                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        await DbInitializer.InitializeIfEmpty(dbContext, logger);
                    }
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
