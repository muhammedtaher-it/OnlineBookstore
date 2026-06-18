using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Models;

namespace OnlineBookstore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Book)
                .WithMany(b => b.CartItems)
                .HasForeignKey(ci => ci.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Book)
                .WithMany(b => b.OrderDetails)
                .HasForeignKey(od => od.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Fiction", Description = "Novels, stories, and imaginative literature" },
                new Category { CategoryId = 2, Name = "Science Fiction", Description = "Futuristic and speculative fiction" },
                new Category { CategoryId = 3, Name = "Mystery & Thriller", Description = "Suspenseful and mysterious stories" },
                new Category { CategoryId = 4, Name = "Romance", Description = "Love stories and romantic fiction" },
                new Category { CategoryId = 5, Name = "Technology", Description = "Programming, computing, and tech books" },
                new Category { CategoryId = 6, Name = "Business", Description = "Business, management, and entrepreneurship" },
                new Category { CategoryId = 7, Name = "Science", Description = "Scientific discoveries and research" },
                new Category { CategoryId = 8, Name = "History", Description = "Historical events and biographies" },
                new Category { CategoryId = 9, Name = "Self-Help", Description = "Personal development and self-improvement" },
                new Category { CategoryId = 10, Name = "Children's Books", Description = "Books for young readers" }
            );

            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "9780743273565", Price = 10.99m, Description = "A story of the mysteriously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan.", ImageUrl = "/images/books/gatsby.jpg", StockQuantity = 50, CategoryId = 1 },
                new Book { BookId = 2, Title = "1984", Author = "George Orwell", ISBN = "9780451524935", Price = 9.99m, Description = "A dystopian social science fiction novel and cautionary tale about the future.", ImageUrl = "/images/books/1984.jpg", StockQuantity = 75, CategoryId = 1 },
                new Book { BookId = 3, Title = "Dune", Author = "Frank Herbert", ISBN = "9780441013593", Price = 12.99m, Description = "Set on the desert planet Arrakis, a mix of politics, religion, and power.", ImageUrl = "/images/books/dune.jpg", StockQuantity = 60, CategoryId = 2 },
                new Book { BookId = 4, Title = "The Hitchhiker's Guide to the Galaxy", Author = "Douglas Adams", ISBN = "9780345391803", Price = 8.99m, Description = "A comedy science fiction series about the adventures of Arthur Dent.", ImageUrl = "/images/books/hitchhiker.jpg", StockQuantity = 40, CategoryId = 2 },
                new Book { BookId = 5, Title = "The Girl with the Dragon Tattoo", Author = "Stieg Larsson", ISBN = "9780307949486", Price = 11.99m, Description = "A psychological thriller about a journalist and a hacker investigating a disappearance.", ImageUrl = "/images/books/dragon-tattoo.jpg", StockQuantity = 45, CategoryId = 3 },
                new Book { BookId = 6, Title = "Gone Girl", Author = "Gillian Flynn", ISBN = "9780307588371", Price = 10.99m, Description = "A thriller about a marriage gone terribly wrong.", ImageUrl = "/images/books/gone-girl.jpg", StockQuantity = 55, CategoryId = 3 },
                new Book { BookId = 7, Title = "Pride and Prejudice", Author = "Jane Austen", ISBN = "9780141439518", Price = 7.99m, Description = "A romantic novel following the character development of Elizabeth Bennet.", ImageUrl = "/images/books/pride-prejudice.jpg", StockQuantity = 80, CategoryId = 4 },
                new Book { BookId = 8, Title = "The Notebook", Author = "Nicholas Sparks", ISBN = "9780553816716", Price = 9.99m, Description = "A touching love story that spans decades.", ImageUrl = "/images/books/notebook.jpg", StockQuantity = 65, CategoryId = 4 },
                new Book { BookId = 9, Title = "Clean Code", Author = "Robert C. Martin", ISBN = "9780132350884", Price = 42.99m, Description = "A Handbook of Agile Software Craftsmanship.", ImageUrl = "/images/books/clean-code.jpg", StockQuantity = 30, CategoryId = 5 },
                new Book { BookId = 10, Title = "Design Patterns", Author = "Gang of Four", ISBN = "9780201633610", Price = 54.99m, Description = "Elements of Reusable Object-Oriented Software.", ImageUrl = "/images/books/design-patterns.jpg", StockQuantity = 25, CategoryId = 5 },
                new Book { BookId = 11, Title = "The Lean Startup", Author = "Eric Ries", ISBN = "9780307887894", Price = 16.99m, Description = "How Today's Entrepreneurs Use Continuous Innovation to Create Radically Successful Businesses.", ImageUrl = "/images/books/lean-startup.jpg", StockQuantity = 40, CategoryId = 6 },
                new Book { BookId = 12, Title = "Zero to One", Author = "Peter Thiel", ISBN = "9780804139298", Price = 18.99m, Description = "Notes on Startups, or How to Build the Future.", ImageUrl = "/images/books/zero-to-one.jpg", StockQuantity = 35, CategoryId = 6 },
                new Book { BookId = 13, Title = "A Brief History of Time", Author = "Stephen Hawking", ISBN = "9780553380163", Price = 13.99m, Description = "From the Big Bang to Black Holes.", ImageUrl = "/images/books/brief-history.jpg", StockQuantity = 50, CategoryId = 7 },
                new Book { BookId = 14, Title = "The Selfish Gene", Author = "Richard Dawkins", ISBN = "9780199291151", Price = 14.99m, Description = "A book on evolution and genetics from the gene-centered view.", ImageUrl = "/images/books/selfish-gene.jpg", StockQuantity = 45, CategoryId = 7 },
                new Book { BookId = 15, Title = "Sapiens", Author = "Yuval Noah Harari", ISBN = "9780062316097", Price = 15.99m, Description = "A Brief History of Humankind.", ImageUrl = "/images/books/sapiens.jpg", StockQuantity = 70, CategoryId = 8 },
                new Book { BookId = 16, Title = "The Art of War", Author = "Sun Tzu", ISBN = "9781599869773", Price = 6.99m, Description = "An ancient Chinese military treatise.", ImageUrl = "/images/books/art-of-war.jpg", StockQuantity = 90, CategoryId = 8 },
                new Book { BookId = 17, Title = "Atomic Habits", Author = "James Clear", ISBN = "9780735211292", Price = 17.99m, Description = "An Easy & Proven Way to Build Good Habits & Break Bad Ones.", ImageUrl = "/images/books/atomic-habits.jpg", StockQuantity = 85, CategoryId = 9 },
                new Book { BookId = 18, Title = "The 7 Habits of Highly Effective People", Author = "Stephen Covey", ISBN = "9781982137274", Price = 14.99m, Description = "Powerful Lessons in Personal Change.", ImageUrl = "/images/books/7-habits.jpg", StockQuantity = 60, CategoryId = 9 },
                new Book { BookId = 19, Title = "Harry Potter and the Sorcerer's Stone", Author = "J.K. Rowling", ISBN = "9780590353427", Price = 11.99m, Description = "The first book in the Harry Potter series.", ImageUrl = "/images/books/harry-potter.jpg", StockQuantity = 100, CategoryId = 10 },
                new Book { BookId = 20, Title = "Charlotte's Web", Author = "E.B. White", ISBN = "9780064400558", Price = 8.99m, Description = "A children's novel about a pig named Wilbur and his friendship with a spider.", ImageUrl = "/images/books/charlottes-web.jpg", StockQuantity = 70, CategoryId = 10 }
            );
        }
    }
}
