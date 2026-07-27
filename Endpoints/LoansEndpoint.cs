using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManager
{
    public class LoansEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/loans", async Task<Results<Created<LoanDetailsDto>, NotFound<string>, Conflict<string>>> (ILoanService loanService, CreateLoanDto dto) =>
            {
                var result = await loanService.CreateLoanAsync(dto.BookId, dto.UserId);
                return result.Status switch
                {
                    CreateLoanStatusResult.Success => TypedResults.Created($"/loans/{result.Dto!.LoanId}", result.Dto),
                    // ! is not great practice, the ideal thing is to redesign the CreateLoanResult object so that the Success+null status
                    // is impossible by design 
                    CreateLoanStatusResult.BookNotFound => TypedResults.NotFound("Book not found"),
                    CreateLoanStatusResult.UserNotFound => TypedResults.NotFound("User not found"),
                    CreateLoanStatusResult.BookAlreadyLoaned => TypedResults.Conflict("Book is currently not available")
                };
            });

            app.MapGet("/loans/{id}", async Task<Results<Ok<LoanDetailsDto>, NotFound>> (ILoanService loanService, int loanId) =>
            {
                var result = await loanService.GetLoanFromIdAsync(loanId);

                return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
            });
        }
    }
}