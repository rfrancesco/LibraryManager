using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class BooksEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/books", async Task<Ok<List<BookDetailsDto>>> (IBookService bookService, [AsParameters] BookQueryDto query) =>
            {
                var result = await bookService.SearchBooksAsync(query);
                return TypedResults.Ok(result);
            });

            app.MapGet("/books/{id}", async Task<Results<Ok<BookDetailsDto>, NotFound>> (int id, IBookService bookService) =>
            {
                var book = await bookService.GetBookByIdAsync(id);
                return book is not null ? TypedResults.Ok(book) : TypedResults.NotFound();
            }
                );
        }
    }
}