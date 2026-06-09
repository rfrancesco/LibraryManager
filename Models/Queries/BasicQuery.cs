namespace LibraryManager
{
    public class BaseQuery
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public static int DefaultPageSize = 20;
    }
}