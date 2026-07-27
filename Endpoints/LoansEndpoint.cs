using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManager
{
    public class LoansEndpoint
    {
        public static async Task Map(WebApplication app)
        {
            var group = app.MapGroup("/loans").WithTags("Loans");

            group.MapGet("/", async Task<Ok<List<LoanDetailsDto>>> (ILoanService loanService, [AsParameters] LoanQueryDto query) =>
            {
                var result = await loanService.SearchLoansAsync(query);

                return TypedResults.Ok(result);
            })
            .WithSummary("Search loan history")
            .WithDescription("Returns list of loans matching given filters. Supports pagination");

            group.MapGet("/{id}", async Task<Results<Ok<LoanDetailsDto>, NotFound>> (ILoanService loanService, int loanId) =>
            {
                var result = await loanService.GetLoanFromIdAsync(loanId);

                return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
            })
            .WithSummary("Get loan details by id");

            group.MapPost("/", async Task<Results<Created<LoanDetailsDto>, NotFound<string>, Conflict<string>>> (ILoanService loanService, CreateLoanDto dto) =>
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
            })
            .WithSummary("Create new loan")
            .WithDescription("Returns Created and loan details on success, otherwise NotFound (user or book not found) of Conflict (book already loaned)");

            group.MapPost("/{id}/return", async Task<Results<Ok<LoanDetailsDto>, NotFound>> (ILoanService loanService, int loanId) =>
            {
                var result = await loanService.ReturnLoanAsync(loanId);

                return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
            })
            .WithSummary("Register a loan as returned (by id)");
        }
    }
}