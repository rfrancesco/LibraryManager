namespace LibraryManager
{
    public record BookDetailsDto
    (
        int Id,
        string Title,
        string Author,
        string Genre,
        bool available
    );
}