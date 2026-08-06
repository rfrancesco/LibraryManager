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
            })
            .WithSummary("Search books matching filters")
            .WithDescription("Returns list of books matching the given filters. Supports pagination");

            group.MapGet("/{id}", async Task<Results<Ok<BookDetailsDto>, NotFound>> (int id, IBookService bookService) =>
            {
                var book = await bookService.GetBookByIdAsync(id);
                return book is not null ? TypedResults.Ok(book) : TypedResults.NotFound();
            })
            .WithSummary("Get book details by id");

            group.MapPost("/", async Task<Created<BookDetailsDto>> (IBookService bookService, CreateBookDto dto) =>
            {
                var result = await bookService.CreateBookAsync(dto);
                return TypedResults.Created($"/books/{result.BookId}", result);
            })
            .WithSummary("Add a book to the library")
            .WithDescription("Creates a new book entry in the catalogue and returns its details, including the generated BookId");
        }
    }
}