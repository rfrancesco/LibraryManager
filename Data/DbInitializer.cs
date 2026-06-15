/* DbInitializer
   Initializes the database with mock data.
   Todo: make this optional via configuration (this should only be used for development/testing).
*/

namespace LibraryManager
{
    public class DbInitializer
    {
        private static void InitializeBookDb(AppDbContext dbContext)
        {
            var books = new[]
            {
                new Book { Title = "1984", Author = "George Orwell", Genre = "Dystopian" },
                new Book { Title = "Animal Farm", Author = "George Orwell", Genre = "Dystopian" },
                new Book { Title = "Brave New World", Author = "Aldous Huxley", Genre = "Dystopian" },
                new Book { Title = "The Handmaid's Tale", Author = "Margaret Atwood", Genre = "Dystopian" },
                new Book { Title = "Fahrenheit 451", Author = "Ray Bradbury", Genre = "Dystopian" },
                new Book { Title = "We", Author = "Yevgeny Zamyatin", Genre = "Dystopian" },

                new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy" },
                new Book { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", Genre = "Fantasy" },
                new Book { Title = "The Silmarillion", Author = "J.R.R. Tolkien", Genre = "Fantasy" },
                new Book { Title = "A Game of Thrones", Author = "George R.R. Martin", Genre = "Fantasy" },
                new Book { Title = "A Clash of Kings", Author = "George R.R. Martin", Genre = "Fantasy" },
                new Book { Title = "A Storm of Swords", Author = "George R.R. Martin", Genre = "Fantasy" },
                new Book { Title = "The Name of the Wind", Author = "Patrick Rothfuss", Genre = "Fantasy" },
                new Book { Title = "The Wise Man's Fear", Author = "Patrick Rothfuss", Genre = "Fantasy" },

                new Book { Title = "Crime and Punishment", Author = "Fyodor Dostoevsky", Genre = "Classic" },
                new Book { Title = "The Brothers Karamazov", Author = "Fyodor Dostoevsky", Genre = "Classic" },
                new Book { Title = "The Idiot", Author = "Fyodor Dostoevsky", Genre = "Classic" },
                new Book { Title = "War and Peace", Author = "Leo Tolstoy", Genre = "Classic" },
                new Book { Title = "Anna Karenina", Author = "Leo Tolstoy", Genre = "Classic" },
                new Book { Title = "The Count of Monte Cristo", Author = "Alexandre Dumas", Genre = "Classic" },

                new Book { Title = "Dune", Author = "Frank Herbert", Genre = "Science Fiction" },
                new Book { Title = "Dune Messiah", Author = "Frank Herbert", Genre = "Science Fiction" },
                new Book { Title = "Children of Dune", Author = "Frank Herbert", Genre = "Science Fiction" },
                new Book { Title = "Foundation", Author = "Isaac Asimov", Genre = "Science Fiction" },
                new Book { Title = "Foundation and Empire", Author = "Isaac Asimov", Genre = "Science Fiction" },
                new Book { Title = "Second Foundation", Author = "Isaac Asimov", Genre = "Science Fiction" },
                new Book { Title = "I, Robot", Author = "Isaac Asimov", Genre = "Science Fiction" },
                new Book { Title = "Neuromancer", Author = "William Gibson", Genre = "Science Fiction" },

                new Book { Title = "The Catcher in the Rye", Author = "J.D. Salinger", Genre = "Literary Fiction" },
                new Book { Title = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Literary Fiction" },
                new Book { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Genre = "Literary Fiction" },
                new Book { Title = "Moby-Dick", Author = "Herman Melville", Genre = "Literary Fiction" },
                new Book { Title = "Jane Eyre", Author = "Charlotte Brontë", Genre = "Literary Fiction"  },

                new Book { Title = "The Da Vinci Code", Author = "Dan Brown", Genre = "Thriller" },
                new Book { Title = "Angels & Demons", Author = "Dan Brown", Genre = "Thriller" },
                new Book { Title = "The Girl with the Dragon Tattoo", Author = "Stieg Larsson", Genre = "Thriller" },
                new Book { Title = "Gone Girl", Author = "Gillian Flynn", Genre = "Thriller" },
                new Book { Title = "The Silence of the Lambs", Author = "Thomas Harris", Genre = "Thriller" },

                new Book { Title = "The Shining", Author = "Stephen King", Genre = "Horror" },
                new Book { Title = "It", Author = "Stephen King", Genre = "Horror" },
                new Book { Title = "Pet Sematary", Author = "Stephen King", Genre = "Horror" },
                new Book { Title = "Dracula", Author = "Bram Stoker", Genre = "Horror" },
                new Book { Title = "Frankenstein", Author = "Mary Shelley", Genre = "Horror" },

                new Book { Title = "The Alchemist", Author = "Paulo Coelho", Genre = "Adventure" },
                new Book { Title = "Life of Pi", Author = "Yann Martel", Genre = "Adventure" },
                new Book { Title = "Treasure Island", Author = "Robert Louis Stevenson", Genre = "Adventure" },
                new Book { Title = "The Three Musketeers", Author = "Alexandre Dumas", Genre = "Adventure" },
                new Book { Title = "Journey to the Center of the Earth", Author = "Jules Verne", Genre = "Adventure" },

                new Book { Title = "The Martian", Author = "Andy Weir", Genre = "Science Fiction" },
                new Book { Title = "Project Hail Mary", Author = "Andy Weir", Genre = "Science Fiction" },
                new Book { Title = "Ready Player One", Author = "Ernest Cline", Genre = "Science Fiction" }
            };

            dbContext.Books.AddRange(books);
            dbContext.SaveChanges();
        }

        private static void InitializeUserDb(AppDbContext dbContext)
        {
            var users = new[]
            {
                new User { Name = "Alice" },
                new User { Name = "Bob" },
                new User { Name = "Charlie" },
                new User { Name = "David" },
                new User { Name = "Eve" }
            };

            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
        }

        private static void InitializeLoanDb(AppDbContext dbContext)
        {
            var loans = new[]
            {
                new Loan { UserId = 1, BookId = 1, LoanDate = DateTime.Now.AddDays(-10), ExpiryDate = DateTime.Now.AddDays(20), ReturnDate = DateTime.Now.AddDays(-1) },
                new Loan { UserId = 1, BookId = 5, LoanDate = DateTime.Now.AddDays(-10), ExpiryDate = DateTime.Now.AddDays(20) },
                new Loan { UserId = 1, BookId = 7, LoanDate = DateTime.Now.AddDays(-10), ExpiryDate = DateTime.Now.AddDays(20) },
                new Loan { UserId = 2, BookId = 2, LoanDate = DateTime.Now.AddDays(-5), ExpiryDate = DateTime.Now.AddDays(25) },
                new Loan { UserId = 3, BookId = 3, LoanDate = DateTime.Now.AddDays(-15), ExpiryDate = DateTime.Now.AddDays(15) }
            };

            dbContext.Loans.AddRange(loans);
            dbContext.SaveChanges();
        }
        public static void InitializeIfEmpty(AppDbContext dbContext)
        {
            if (!dbContext.Users.Any())
                InitializeUserDb(dbContext);

            if (!dbContext.Books.Any())
                InitializeBookDb(dbContext);

            if (!dbContext.Loans.Any())
                InitializeLoanDb(dbContext);
        }
    }
}