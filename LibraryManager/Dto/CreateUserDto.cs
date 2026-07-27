using System.ComponentModel.DataAnnotations;

namespace LibraryManager
{
    public record CreateUserDto
    (
        [Required, MaxLength(100)] string Name
    );
}