namespace LibraryManager
{
    public record BookDetailsDto
    (
        int BookId,
        string Title,
        string Author,
        string Genre,
        bool Available
    );
}