# Online Bookstore - CS401 Advanced Web Development

A complete, production-ready Online Bookstore web application built with ASP.NET Core MVC (.NET 8).

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Database Design](#database-design)
- [Getting Started](#getting-started)
- [Default Accounts](#default-accounts)
- [API Documentation](#api-documentation)
- [Screenshots](#screenshots)

## Features

### Authentication & Authorization
- User Registration with validation
- User Login with remember me functionality
- Logout functionality
- Role-based access control (Admin/User)
- Admin dashboard with statistics

### Book Management
- Browse all books with pagination
- Search books by title
- Filter books by author
- Filter books by category
- Sort books by price, title, date
- Book details with full information

### Shopping Cart
- Add books to cart
- Update item quantities
- Remove items from cart
- Clear entire cart
- Cart total calculation

### Checkout & Orders
- Secure checkout process
- Shipping information collection
- Order creation and confirmation
- Order history for users
- Order status tracking
- Admin order management

### Reviews & Ratings (Bonus Feature)
- Rate books 1-5 stars
- Write review comments
- View average rating
- View rating distribution
- View all reviews
- Prevent duplicate reviews

### Categories
- Browse books by category
- Category management (Admin)
- Book count per category

### Admin Dashboard
- Total users, books, orders statistics
- Total revenue calculation
- Pending and completed orders count
- Recent orders table
- Recent users table
- Quick action buttons

### RESTful API
- GET /api/books - List all books
- GET /api/books/{id} - Get book by ID
- POST /api/books - Create book
- PUT /api/books/{id} - Update book
- DELETE /api/books/{id} - Delete book
- GET /api/books/search/author/{author} - Search by author
- GET /api/books/search/category/{categoryId} - Search by category

## Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 8)
- **Authentication**: ASP.NET Core Identity
- **ORM**: Entity Framework Core (Code First)
- **Database**: SQL Server
- **Frontend**: Razor Views, Bootstrap 5, Bootstrap Icons
- **Architecture**: Repository Pattern, Service Layer, Dependency Injection
- **API**: RESTful API with DTOs and JSON responses

## Project Structure

```
OnlineBookstore/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── BooksController.cs
│   ├── CategoriesController.cs
│   ├── CartController.cs
│   ├── OrdersController.cs
│   └── AdminController.cs
├── ApiControllers/        # API Controllers
│   └── BooksApiController.cs
├── Models/               # Domain Models
│   ├── ApplicationUser.cs
│   ├── Category.cs
│   ├── Book.cs
│   ├── CartItem.cs
│   ├── Order.cs
│   ├── OrderDetail.cs
│   └── Review.cs
├── ViewModels/           # View Models
│   ├── RegisterViewModel.cs
│   ├── LoginViewModel.cs
│   ├── BookViewModel.cs
│   ├── CategoryViewModel.cs
│   ├── CartViewModel.cs
│   ├── CheckoutViewModel.cs
│   ├── OrderViewModel.cs
│   ├── ReviewViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── BookListViewModel.cs
│   └── HomeViewModel.cs
├── DTOs/                 # Data Transfer Objects
│   ├── BookDto.cs
│   ├── CategoryDto.cs
│   ├── ReviewDto.cs
│   └── ApiResponseDto.cs
├── Data/                 # Database Context
│   └── ApplicationDbContext.cs
├── Interfaces/           # Repository Interfaces
│   ├── IBookRepository.cs
│   ├── ICategoryRepository.cs
│   ├── ICartRepository.cs
│   ├── IOrderRepository.cs
│   ├── IReviewRepository.cs
│   └── IUserRepository.cs
├── Repositories/         # Repository Implementations
│   ├── BookRepository.cs
│   ├── CategoryRepository.cs
│   ├── CartRepository.cs
│   ├── OrderRepository.cs
│   ├── ReviewRepository.cs
│   └── UserRepository.cs
├── Services/             # Business Logic Layer
│   ├── CartService.cs
│   ├── OrderService.cs
│   ├── ReviewService.cs
│   ├── DashboardService.cs
│   └── BookService.cs
├── Views/                # Razor Views
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   ├── Home/
│   │   ├── Index.cshtml
│   │   ├── About.cshtml
│   │   └── Privacy.cshtml
│   ├── Account/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   ├── Profile.cshtml
│   │   └── AccessDenied.cshtml
│   ├── Books/
│   │   ├── Index.cshtml
│   │   ├── Details.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Delete.cshtml
│   ├── Categories/
│   │   ├── Index.cshtml
│   │   ├── Details.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Delete.cshtml
│   ├── Cart/
│   │   └── Index.cshtml
│   ├── Orders/
│   │   ├── Index.cshtml
│   │   ├── Details.cshtml
│   │   ├── Checkout.cshtml
│   │   ├── Confirmation.cshtml
│   │   └── Manage.cshtml
│   └── Admin/
│       ├── Dashboard.cshtml
│       ├── Users.cshtml
│       ├── Books.cshtml
│       └── Orders.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── site.js
│   └── images/
├── Program.cs
├── appsettings.json
└── OnlineBookstore.csproj
```

## Database Design

### Entity Relationships

- **Category** (1) -> Many **Books**
- **Book** (1) -> Many **Reviews**
- **ApplicationUser** (1) -> Many **Orders**
- **ApplicationUser** (1) -> Many **Reviews**
- **ApplicationUser** (1) -> Many **CartItems**
- **Order** (1) -> Many **OrderDetails**
- **Book** (1) -> Many **OrderDetails**
- **Book** (1) -> Many **CartItems**

### Tables

| Table | Description |
|-------|-------------|
| AspNetUsers | Identity users (ApplicationUser) |
| AspNetRoles | Identity roles |
| Categories | Book categories |
| Books | Book catalog |
| CartItems | Shopping cart items |
| Orders | Customer orders |
| OrderDetails | Order line items |
| Reviews | Book reviews and ratings |

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone or download the project**
   ```bash
   cd OnlineBookstore
   ```

2. **Update connection string** (if needed)
   Open `appsettings.json` and update the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OnlineBookstoreDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

3. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open in browser**
   Navigate to `https://localhost:7001` or `http://localhost:5001`

### Database Migrations

The application uses automatic migrations. The database will be created and seeded automatically on first run.

If you need to manually run migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Default Accounts

The application automatically seeds the following accounts on first run:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@bookstore.com | Admin@123 |
| User | user@bookstore.com | User@123 |

## API Documentation

### Base URL
```
https://localhost:7001/api/books
```

### Endpoints

#### Get All Books
```http
GET /api/books
GET /api/books?search=title
GET /api/books?author=name
GET /api/books?category=1
GET /api/books?sort=price_asc
```

#### Get Book by ID
```http
GET /api/books/{id}
```

#### Create Book
```http
POST /api/books
Content-Type: application/json

{
  "title": "Book Title",
  "author": "Author Name",
  "isbn": "9781234567890",
  "price": 19.99,
  "description": "Book description",
  "imageUrl": "/images/book.jpg",
  "stockQuantity": 50,
  "categoryId": 1
}
```

#### Update Book
```http
PUT /api/books/{id}
Content-Type: application/json

{
  "title": "Updated Title",
  "author": "Author Name",
  "isbn": "9781234567890",
  "price": 24.99,
  "description": "Updated description",
  "imageUrl": "/images/book.jpg",
  "stockQuantity": 75,
  "categoryId": 1
}
```

#### Delete Book
```http
DELETE /api/books/{id}
```

#### Search by Author
```http
GET /api/books/search/author/{author}
```

#### Search by Category
```http
GET /api/books/search/category/{categoryId}
```

### Response Format

All API responses follow this structure:
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... },
  "errors": null
}
```

## Seeded Data

The application comes with:
- 10 categories (Fiction, Science Fiction, Mystery & Thriller, Romance, Technology, Business, Science, History, Self-Help, Children's Books)
- 20 sample books across all categories
- 2 default user accounts (Admin and User)

## Architecture Patterns

### Repository Pattern
- Abstracts data access logic
- Provides clean separation between business logic and data access
- Enables unit testing through interfaces

### Service Layer
- Contains business logic
- Orchestrates repository calls
- Maps entities to view models

### Dependency Injection
- All services and repositories are registered in Program.cs
- Constructor injection used throughout
- Promotes loose coupling and testability

## Professor Requirements Checklist

- [x] Register
- [x] Login
- [x] Logout
- [x] Authentication
- [x] Authorization
- [x] Roles (Admin/User)
- [x] MVC CRUD (Books, Categories)
- [x] Entity Framework Core (Code First)
- [x] RESTful API
- [x] SQL Server
- [x] Bonus Feature (Book Rating & Review System)

## License

This project is created for educational purposes as part of CS401 Advanced Web Development course.
