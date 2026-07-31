namespace LibraryManager
{
    // Used for returning minimal information on a book (e.g. in Loan queries)
    // For detailed info, use BookDetailsDto
    public record BookSummaryDto
    (
        int BookId,
        string Title,
        string Author
    );
}