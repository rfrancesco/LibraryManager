namespace LibraryManager
{
    public class AuthorsEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/authors", (AppDbContext dbContext, [AsParameters] BookQuery query) =>
                        {
                            var page = query.Page == null ? 1 : query.Page.Value;
                            var pageSize = query.PageSize == null ? BookQuery.DefaultPageSize : query.PageSize.Value;

                            var bookQuery = dbContext.Books.AsQueryable();
                            if (query.Title != null)
                                bookQuery = bookQuery.Where(b => b.Title.ToLower().Contains(query.Title.ToLower()));
                            if (query.Author != null)
                                bookQuery = bookQuery.Where(b => b.Author.ToLower().Contains(query.Author.ToLower()));
                            if (query.Genre != null)
                                bookQuery = bookQuery.Where(b => b.Genre.ToLower().Contains(query.Genre.ToLower()));
                            if (query.Available != null)
                                bookQuery = bookQuery.Where(b => (b.BorrowedByUserId == null) == query.Available);

                            return bookQuery
                                    .Select(b => new
                                    {
                                        b.Author,
                                    })
                                    .Distinct()
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
                        });

        }
    }
}