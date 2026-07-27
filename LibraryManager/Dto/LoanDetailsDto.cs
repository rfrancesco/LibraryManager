namespace LibraryManager
{
    public record LoanDetailsDto
    (
        int LoanId,
        int BookId,
        int UserId,
        DateTime LoanDate,
        DateTime ExpiryDate,
        DateTime? ReturnDate
    );
}