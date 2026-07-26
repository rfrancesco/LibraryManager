using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class UsersEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/users", async Task<Ok<List<UserDetailsDto>>> (IUserService userService, [AsParameters] UserQueryDto query) =>
            {
                var result = await userService.SearchUsersAsync(query);
                return TypedResults.Ok(result);
            });

            app.MapGet("/users/{id}", async Task<Results<Ok<UserDetailsDto>, NotFound>> (int id, IUserService userService) =>
            {
                var result = await userService.GetUserByIdAsync(id);
                return (result is not null) ? TypedResults.Ok(result) : TypedResults.NotFound();
            });

            app.MapGet("/users/{id}/books", (int id, AppDbContext dbContext, [AsParameters] BaseQueryDto query) =>
            {
                var page = query.Page == null ? 1 : query.Page.Value;
                var pageSize = query.PageSize == null ? BaseQueryDto.DefaultPageSize : query.PageSize.Value;
                return dbContext.Books.Where(b => (b.Loans.Any(l => l.UserId == id && l.ReturnDate == null))).Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.Genre
                }).Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .ToList();
            });
        }
    }
}