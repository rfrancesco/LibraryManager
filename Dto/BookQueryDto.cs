namespace LibraryManager
{
    public class BookQueryDto : BaseQueryDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public bool? Available { get; set; }
    }
}