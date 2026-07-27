using System.ComponentModel.DataAnnotations;

namespace LibraryManager
{
    public record CreateLoanDto
    (
        [Required] int BookId,
        [Required] int UserId
    );
}