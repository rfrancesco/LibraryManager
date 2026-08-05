using System.ComponentModel.DataAnnotations;

namespace LibraryManager.Data
{
    public class User
    {
        public int UserId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public ICollection<Loan> Loans { get; set; } = [];
    }
}