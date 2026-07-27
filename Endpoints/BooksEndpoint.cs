using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManager
{
    public class BooksEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/books").WithTags("Books");
            group.MapGet("/", async Task<Ok<List<BookDetailsDto>>> (IBookService bookService, [AsParameters] BookQueryDto query) =>
            {
                var result = await bookService.SearchBooksAsync(query);
                return TypedResults.Ok(result);
            });

            group.MapGet("/{id}", async Task<Results<Ok<BookDetailsDto>, NotFound>> (int id, IBookService bookService) =>
            {
                var book = await bookService.GetBookByIdAsync(id);
                return book is not null ? TypedResults.Ok(book) : TypedResults.NotFound();
            });

            group.MapPost("/", async Task<Ok<BookDetailsDto>> (IBookService bookService, CreateBookDto dto) =>
            {
                var result = await bookService.CreateBookAsync(dto);
                return TypedResults.Ok(result);
            });
        }
    }
}