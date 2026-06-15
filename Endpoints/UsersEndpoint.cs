using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class UsersEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/users", (AppDbContext dbContext, [AsParameters] UserQueryDto query) =>
            {
                var page = query.Page == null ? 1 : query.Page.Value;
                var pageSize = query.PageSize == null ? UserQueryDto.DefaultPageSize : query.PageSize.Value;
                var userQuery = dbContext.Users.AsQueryable();
                if (query.Name != null)
                    userQuery = userQuery.Where(u => u.Name.ToLower().Contains(query.Name.ToLower()));
                return Results.Ok(userQuery
                        .Select(u => new { u.UserId, u.Name })
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList());
            });

            app.MapGet("/users/{id}", (int id, AppDbContext dbContext) =>
            {
                var result = dbContext.Users.Select(u => new
                {
                    u.UserId,
                    u.Name
                    //Books = u.Books.Select(b => new { b.Id, b.Title, b.Author, b.Genre })
                }).FirstOrDefault(u => u.UserId == id);
                if (result == null)
                {
                    return Results.NotFound();
                }
                else
                {
                    return Results.Ok(result);
                }
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