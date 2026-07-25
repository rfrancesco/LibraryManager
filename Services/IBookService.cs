namespace LibraryManager
{
    public interface IBookService
    {
        Task<BookDetailsDto?> GetBookByIdAsync(int bookId);
        Task<List<BookDetailsDto>> SearchBooksAsync(BookQueryDto query);
        Task<List<string>> SearchAuthorsMatchingBookFiltersAsync(BookQueryDto query);
    }
}