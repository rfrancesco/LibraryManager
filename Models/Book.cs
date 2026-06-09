namespace LibraryManager
{
    public class Book {
        public int Id { get; set; }
        public string Author { get; set; }  // Could be split 
        public string Title { get; set; }
        public string Genre { get; set; }
        public int? BorrowedByUserId { get; set; }
        public User BorrowedBy { get; set; }


    }
}