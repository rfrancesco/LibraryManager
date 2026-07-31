using System.ComponentModel.DataAnnotations;

namespace LibraryManager
{
    public class Book
    {
        public int BookId { get; set; }
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;
        public ICollection<Loan> Loans { get; set; } = [];
    }
}