using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManager
{
    public class AuthorsEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/authors").WithTags("Books");
            group.MapGet("/", async Task<Ok<List<string>>> (IBookService bookService, [AsParameters] BookQueryDto query) =>
                        {
                            var result = await bookService.SearchAuthorsMatchingBookFiltersAsync(query);
                            return TypedResults.Ok(result);
                        })
                        .WithSummary("Search authors matching book filters")
                        .WithDescription("Returns list of distinct author names whose books match the given filters. Supports pagination");

        }
    }
}