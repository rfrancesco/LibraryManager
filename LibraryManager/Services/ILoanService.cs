namespace LibraryManager
{
    public enum CreateLoanStatusResult
    {
        Success,
        BookAlreadyLoaned,
        BookNotFound,
        UserNotFound
    }

    public record CreateLoanResult(CreateLoanStatusResult Status, LoanDetailsDto? Dto);

    public interface ILoanService
    {
        Task<bool> LoanExistsAsync(int loanId);
        Task<bool> HasActiveLoanAsync(int bookId);
        Task<int?> GetActiveLoanFromBookIdAsync(int bookId);
        Task<CreateLoanResult> CreateLoanAsync(int bookId, int userId);
        Task<LoanDetailsDto?> GetLoanFromIdAsync(int loanId);
        Task<List<LoanDetailsDto>> SearchLoansAsync(LoanQueryDto query);
        Task<LoanDetailsDto?> ReturnLoanAsync(int loanId);
    }
}