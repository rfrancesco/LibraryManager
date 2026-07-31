namespace LibraryManager
{
    public record LoanDetailsDto
    (
        int LoanId,
        BookSummaryDto Book,
        UserDetailsDto User,
        DateTime LoanDate,
        DateTime ExpiryDate,
        DateTime? ReturnDate
    );
}