using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManager
{
    public class GenresEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/genres").WithTags("Books");
            group.MapGet("/", async Task<Ok<List<string>>> (IBookService bookService, [AsParameters] BookQueryDto query) =>
            {
                var result = await bookService.SearchGenresMatchingBookFiltersAsync(query);
                return TypedResults.Ok(result);
            })
            .WithSummary("Search genres matching book filters")
            .WithDescription("Returns list of distinct genres whose books match the given filters. Supports pagination");
        }
    }
}