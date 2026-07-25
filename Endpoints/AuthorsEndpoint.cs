using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManager
{
    public class AuthorsEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/authors", async Task<Ok<List<string>>> (IBookService bookService, [AsParameters] BookQueryDto query) =>
                        {
                            var result = await bookService.SearchAuthorsMatchingBookFiltersAsync(query);
                            return TypedResults.Ok(result);
                        });

        }
    }
}