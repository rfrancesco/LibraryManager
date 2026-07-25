using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class GenresEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/genres", async Task<Ok<List<string>>> (IBookService bookService, [AsParameters] BookQueryDto query) =>
            {
                var result = await bookService.SearchGenresMatchingBookFiltersAsync(query);
                return TypedResults.Ok(result);
            });
        }
    }
}