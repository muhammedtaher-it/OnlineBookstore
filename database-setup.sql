-- Online Bookstore - SQL Server Database Setup Script
-- CS401 Advanced Web Development

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OnlineBookstoreDB')
BEGIN
    CREATE DATABASE OnlineBookstoreDB;
END
GO

USE OnlineBookstoreDB;
GO

-- =====================================================
-- ASP.NET Core Identity Tables (Created by EF Migrations)
-- =====================================================

-- The following tables are automatically created by Entity Framework migrations:
-- AspNetRoles, AspNetUsers, AspNetRoleClaims, AspNetUserClaims, 
-- AspNetUserLogins, AspNetUserRoles, AspNetUserTokens

-- =====================================================
-- Application Tables
-- =====================================================

-- Categories Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        CategoryId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL
    );
END
GO

-- Books Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Books')
BEGIN
    CREATE TABLE Books (
        BookId INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Author NVARCHAR(150) NOT NULL,
        ISBN NVARCHAR(13) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Description NVARCHAR(2000) NULL,
        ImageUrl NVARCHAR(500) NULL,
        StockQuantity INT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CategoryId INT NOT NULL,
        CONSTRAINT FK_Books_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
    );
END
GO

-- CartItems Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CartItems')
BEGIN
    CREATE TABLE CartItems (
        CartItemId INT IDENTITY(1,1) PRIMARY KEY,
        UserId NVARCHAR(450) NOT NULL,
        BookId INT NOT NULL,
        Quantity INT NOT NULL DEFAULT 1,
        AddedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_CartItems_Books FOREIGN KEY (BookId) REFERENCES Books(BookId) ON DELETE CASCADE,
        CONSTRAINT FK_CartItems_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
    );
END
GO

-- Orders Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        OrderId INT IDENTITY(1,1) PRIMARY KEY,
        UserId NVARCHAR(450) NOT NULL,
        OrderDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        TotalPrice DECIMAL(18,2) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        ShippingAddress NVARCHAR(200) NULL,
        ShippingCity NVARCHAR(100) NULL,
        ShippingPostalCode NVARCHAR(20) NULL,
        ShippingCountry NVARCHAR(100) NULL,
        PaymentMethod NVARCHAR(50) NULL,
        CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
    );
END
GO

-- OrderDetails Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderDetails')
BEGIN
    CREATE TABLE OrderDetails (
        OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT NOT NULL,
        BookId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
        CONSTRAINT FK_OrderDetails_Books FOREIGN KEY (BookId) REFERENCES Books(BookId)
    );
END
GO

-- Reviews Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reviews')
BEGIN
    CREATE TABLE Reviews (
        ReviewId INT IDENTITY(1,1) PRIMARY KEY,
        BookId INT NOT NULL,
        UserId NVARCHAR(450) NOT NULL,
        Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
        Comment NVARCHAR(1000) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Reviews_Books FOREIGN KEY (BookId) REFERENCES Books(BookId) ON DELETE CASCADE,
        CONSTRAINT FK_Reviews_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
    );
END
GO

-- =====================================================
-- Seed Data
-- =====================================================

-- Seed Categories
SET IDENTITY_INSERT Categories ON;

MERGE INTO Categories AS target
USING (VALUES
    (1, 'Fiction', 'Novels, stories, and imaginative literature'),
    (2, 'Science Fiction', 'Futuristic and speculative fiction'),
    (3, 'Mystery & Thriller', 'Suspenseful and mysterious stories'),
    (4, 'Romance', 'Love stories and romantic fiction'),
    (5, 'Technology', 'Programming, computing, and tech books'),
    (6, 'Business', 'Business, management, and entrepreneurship'),
    (7, 'Science', 'Scientific discoveries and research'),
    (8, 'History', 'Historical events and biographies'),
    (9, 'Self-Help', 'Personal development and self-improvement'),
    (10, 'Children''s Books', 'Books for young readers')
) AS source (CategoryId, Name, Description)
ON target.CategoryId = source.CategoryId
WHEN NOT MATCHED THEN
    INSERT (CategoryId, Name, Description)
    VALUES (source.CategoryId, source.Name, source.Description);

SET IDENTITY_INSERT Categories OFF;
GO

-- Seed Books
SET IDENTITY_INSERT Books ON;

MERGE INTO Books AS target
USING (VALUES
    (1, 'The Great Gatsby', 'F. Scott Fitzgerald', '9780743273565', 10.99, 'A story of the mysteriously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan.', '/images/books/gatsby.jpg', 50, 1),
    (2, '1984', 'George Orwell', '9780451524935', 9.99, 'A dystopian social science fiction novel and cautionary tale about the future.', '/images/books/1984.jpg', 75, 1),
    (3, 'Dune', 'Frank Herbert', '9780441013593', 12.99, 'Set on the desert planet Arrakis, a mix of politics, religion, and power.', '/images/books/dune.jpg', 60, 2),
    (4, 'The Hitchhiker''s Guide to the Galaxy', 'Douglas Adams', '9780345391803', 8.99, 'A comedy science fiction series about the adventures of Arthur Dent.', '/images/books/hitchhiker.jpg', 40, 2),
    (5, 'The Girl with the Dragon Tattoo', 'Stieg Larsson', '9780307949486', 11.99, 'A psychological thriller about a journalist and a hacker investigating a disappearance.', '/images/books/dragon-tattoo.jpg', 45, 3),
    (6, 'Gone Girl', 'Gillian Flynn', '9780307588371', 10.99, 'A thriller about a marriage gone terribly wrong.', '/images/books/gone-girl.jpg', 55, 3),
    (7, 'Pride and Prejudice', 'Jane Austen', '9780141439518', 7.99, 'A romantic novel following the character development of Elizabeth Bennet.', '/images/books/pride-prejudice.jpg', 80, 4),
    (8, 'The Notebook', 'Nicholas Sparks', '9780553816716', 9.99, 'A touching love story that spans decades.', '/images/books/notebook.jpg', 65, 4),
    (9, 'Clean Code', 'Robert C. Martin', '9780132350884', 42.99, 'A Handbook of Agile Software Craftsmanship.', '/images/books/clean-code.jpg', 30, 5),
    (10, 'Design Patterns', 'Gang of Four', '9780201633610', 54.99, 'Elements of Reusable Object-Oriented Software.', '/images/books/design-patterns.jpg', 25, 5),
    (11, 'The Lean Startup', 'Eric Ries', '9780307887894', 16.99, 'How Today''s Entrepreneurs Use Continuous Innovation to Create Radically Successful Businesses.', '/images/books/lean-startup.jpg', 40, 6),
    (12, 'Zero to One', 'Peter Thiel', '9780804139298', 18.99, 'Notes on Startups, or How to Build the Future.', '/images/books/zero-to-one.jpg', 35, 6),
    (13, 'A Brief History of Time', 'Stephen Hawking', '9780553380163', 13.99, 'From the Big Bang to Black Holes.', '/images/books/brief-history.jpg', 50, 7),
    (14, 'The Selfish Gene', 'Richard Dawkins', '9780199291151', 14.99, 'A book on evolution and genetics from the gene-centered view.', '/images/books/selfish-gene.jpg', 45, 7),
    (15, 'Sapiens', 'Yuval Noah Harari', '9780062316097', 15.99, 'A Brief History of Humankind.', '/images/books/sapiens.jpg', 70, 8),
    (16, 'The Art of War', 'Sun Tzu', '9781599869773', 6.99, 'An ancient Chinese military treatise.', '/images/books/art-of-war.jpg', 90, 8),
    (17, 'Atomic Habits', 'James Clear', '9780735211292', 17.99, 'An Easy & Proven Way to Build Good Habits & Break Bad Ones.', '/images/books/atomic-habits.jpg', 85, 9),
    (18, 'The 7 Habits of Highly Effective People', 'Stephen Covey', '9781982137274', 14.99, 'Powerful Lessons in Personal Change.', '/images/books/7-habits.jpg', 60, 9),
    (19, 'Harry Potter and the Sorcerer''s Stone', 'J.K. Rowling', '9780590353427', 11.99, 'The first book in the Harry Potter series.', '/images/books/harry-potter.jpg', 100, 10),
    (20, 'Charlotte''s Web', 'E.B. White', '9780064400558', 8.99, 'A children''s novel about a pig named Wilbur and his friendship with a spider.', '/images/books/charlottes-web.jpg', 70, 10)
) AS source (BookId, Title, Author, ISBN, Price, Description, ImageUrl, StockQuantity, CategoryId)
ON target.BookId = source.BookId
WHEN NOT MATCHED THEN
    INSERT (BookId, Title, Author, ISBN, Price, Description, ImageUrl, StockQuantity, CategoryId)
    VALUES (source.BookId, source.Title, source.Author, source.ISBN, source.Price, source.Description, source.ImageUrl, source.StockQuantity, source.CategoryId);

SET IDENTITY_INSERT Books OFF;
GO

-- =====================================================
-- Indexes for Performance
-- =====================================================

-- Book indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Books_Title')
    CREATE INDEX IX_Books_Title ON Books(Title);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Books_Author')
    CREATE INDEX IX_Books_Author ON Books(Author);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Books_CategoryId')
    CREATE INDEX IX_Books_CategoryId ON Books(CategoryId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Books_Price')
    CREATE INDEX IX_Books_Price ON Books(Price);

-- Order indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_UserId')
    CREATE INDEX IX_Orders_UserId ON Orders(UserId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_OrderDate')
    CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);

-- Review indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reviews_BookId')
    CREATE INDEX IX_Reviews_BookId ON Reviews(BookId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reviews_UserId')
    CREATE INDEX IX_Reviews_UserId ON Reviews(UserId);

-- Cart indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CartItems_UserId')
    CREATE INDEX IX_CartItems_UserId ON CartItems(UserId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CartItems_BookId')
    CREATE INDEX IX_CartItems_BookId ON CartItems(BookId);

GO

-- =====================================================
-- Useful Queries
-- =====================================================

-- Get all books with category info
SELECT b.BookId, b.Title, b.Author, b.ISBN, b.Price, b.StockQuantity, c.Name AS CategoryName
FROM Books b
INNER JOIN Categories c ON b.CategoryId = c.CategoryId;

-- Get total sales
SELECT COUNT(*) AS TotalOrders, SUM(TotalPrice) AS TotalRevenue FROM Orders;

-- Get top rated books
SELECT TOP 5 b.Title, b.Author, AVG(CAST(r.Rating AS DECIMAL(3,2))) AS AvgRating, COUNT(r.ReviewId) AS ReviewCount
FROM Books b
LEFT JOIN Reviews r ON b.BookId = r.BookId
GROUP BY b.BookId, b.Title, b.Author
ORDER BY AvgRating DESC;

-- Get low stock books
SELECT Title, Author, StockQuantity FROM Books WHERE StockQuantity < 20 ORDER BY StockQuantity;

GO

PRINT 'Database setup completed successfully!';
PRINT '';
PRINT 'Default Admin Account: admin@bookstore.com / Admin@123';
PRINT 'Default User Account: user@bookstore.com / User@123';
