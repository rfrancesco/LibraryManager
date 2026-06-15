namespace LibraryManager
{
    public class Book
    {
        public int Id { get; set; }
        public string Author { get; set; } = string.Empty;  // Could be split 
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public ICollection<Loan> Loans { get; set; } = [];
    }
}