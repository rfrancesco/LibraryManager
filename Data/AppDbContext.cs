using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class AppDbContext : DbContext 
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     //base.OnConfiguring(optionsBuilder);
        //     optionsBuilder.UseSqlite("Data Source=library.db");
        // }
    }
}