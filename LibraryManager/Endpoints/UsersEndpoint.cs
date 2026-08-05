using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using LibraryManager.Data;

namespace LibraryManager
{
    public class UsersEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/users").WithTags("Users");
            group.MapGet("/", async Task<Ok<List<UserDetailsDto>>> (IUserService userService, [AsParameters] UserQueryDto query) =>
            {
                var result = await userService.SearchUsersAsync(query);
                return TypedResults.Ok(result);
            })
            .WithSummary("Search users")
            .WithDescription("Returns list of users matching given filters. Supports pagination");

            group.MapGet("/{id}", async Task<Results<Ok<UserDetailsDto>, NotFound>> (int id, IUserService userService) =>
            {
                var result = await userService.GetUserByIdAsync(id);
                return (result is not null) ? TypedResults.Ok(result) : TypedResults.NotFound();
            })
            .WithSummary("Get user details by id");

            group.MapGet("/{id}/books", async Task<Results<Ok<List<BookDetailsDto>>, NotFound<string>>> (int id, AppDbContext dbContext, IUserService userService, [AsParameters] BaseQueryDto query) =>
            {
                if (!await userService.UserExistsAsync(id))
                    return TypedResults.NotFound("User not found");

                var page = query.Page == null ? 1 : query.Page.Value;
                // On the magic numbers 20, 100:
                // This needs to be refactored into LoanService, where DefaultPageSize and MaxPageSize are provided
                var pageSize = query.PageSize == null ? 20 : Math.Clamp(query.PageSize.Value, 1, 100);
                var result = await dbContext.Books
                    .Where(b => (b.Loans.Any(l => l.UserId == id && l.ReturnDate == null)))
                    .OrderBy(b => b.BookId)
                    .Select(b => new BookDetailsDto(b.BookId,
                                                    b.Title,
                                                    b.Author,
                                                    b.Genre, false))
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return TypedResults.Ok(result);
            })
            .WithSummary("Get list of active loans for the specified user");

            group.MapPost("/", async Task<Created<UserDetailsDto>> (IUserService userService, CreateUserDto dto) =>
            {
                var result = await userService.CreateUserAsync(dto.Name);
                return TypedResults.Created($"/users/{result.UserId}", result);
            })
            .WithSummary("Create a new user");
        }
    }
}