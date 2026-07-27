using System.ComponentModel.DataAnnotations;

namespace LibraryManager
{
    public record CreateBookDto
    (
        [Required, MaxLength(100)] string Title,
        [Required, MaxLength(100)] string Author,
        [Required, MaxLength(50)] string Genre
    );
}