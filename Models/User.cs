namespace LibraryManager
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = [];
    }
}