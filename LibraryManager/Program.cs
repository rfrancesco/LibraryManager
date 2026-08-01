using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.SqlServer;

namespace LibraryManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                .UseExceptionProcessor());
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

            // Apply migrations 
            // Is unsafe in Production if multiple instances are run - for now it's ok
            if (!app.Environment.IsProduction())
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            // Data seeding
            // Demo mode: automatic
            // Development mode: can be switched on with SEED_DEMO_DATA
            // Production mode: not allowed
            bool seedDemoData = app.Environment.IsEnvironment("Demo")
                                || (builder.Configuration.GetValue<bool>("SEED_DEMO_DATA")
                                    && app.Environment.IsDevelopment());

            bool apiIsReadOnly = app.Environment.IsEnvironment("Demo");

            bool enableSwagger = app.Environment.IsDevelopment()
                                || app.Environment.IsEnvironment("Demo");

            if (seedDemoData)
            {
                using (var scope = app.Services.CreateScope())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
                    logger.LogInformation($"{app.Environment.EnvironmentName} mode: SEED_DEMO_DATA=true");

                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await DbInitializer.InitializeIfEmptyAsync(dbContext, logger);
                }
            }

            if (enableSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            // Demo mode: reject all non-GET requests
            if (apiIsReadOnly)
            {
                app.Logger.LogInformation("{Environment}: API is read only. Only GET requests allowed.", app.Environment.EnvironmentName);
                app.Use(async (context, next) =>
                {
                    if (context.Request.Method != HttpMethods.Get)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsJsonAsync(
                            new { message = "Demo mode enabled: data is read-only, only GET requests are allowed." }
                        );

                        return;
                    }
                    await next.Invoke();
                });
            }



            BooksEndpoint.Map(app);
            AuthorsEndpoint.Map(app);
            GenresEndpoint.Map(app);

            UsersEndpoint.Map(app);

            LoansEndpoint.Map(app);

            app.Run();
        }
    }
}
