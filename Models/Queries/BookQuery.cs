namespace LibraryManager
{
    public class BookQuery : BaseQuery
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public bool? Available { get; set; }
    }
}