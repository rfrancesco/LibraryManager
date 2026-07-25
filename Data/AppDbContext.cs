using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add Unique index to ensure that no two open loans (ReturnDate = NULL)
            // can be created for the same book (BookId)
            modelBuilder.Entity<Loan>()
                .HasIndex(l => l.BookId)
                .IsUnique()
                // Next line has Sqlite syntax, needs to be changed for SQL Server or Postgres
                .HasFilter("ReturnDate IS NULL");
        }
    }
}