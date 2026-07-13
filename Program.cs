using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;
using OnlineBookstore.Repositories;
using OnlineBookstore.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<ArabicIdentityErrorDescriber>();

// Configure Application Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// Repository Pattern - Dependency Injection
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Service Layer - Dependency Injection
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<BookService>();

// Add MVC Controllers and Views
builder.Services.AddControllersWithViews();

// Add API Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ✨✨✨ تغيير لغة النظام بالكامل للعربية ✨✨✨
var cultureInfo = new CultureInfo("ar-SA");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedDataAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// MVC Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// API Routes
app.MapControllers();

app.Run();

// Seed Data Method
// Seed Data Method
static async Task SeedDataAsync(IServiceProvider serviceProvider)
{
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Apply pending migrations
    await context.Database.MigrateAsync();

    // Seed Roles
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Seed Admin User
    var adminEmail = "admin@bookstore.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "المشرف",
            Role = "Admin",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Seed Demo User
    var userEmail = "user@bookstore.com";
    var demoUser = await userManager.FindByEmailAsync(userEmail);
    if (demoUser == null)
    {
        demoUser = new ApplicationUser
        {
            UserName = userEmail,
            Email = userEmail,
            FullName = "مستخدم تجريبي",
            Role = "User",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(demoUser, "User@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(demoUser, "User");
        }
    }

    // === بداية إضافة التصنيفات والكتب العربية بصور حقيقية وعالية الدقة ===

    // مسح البيانات القديمة
    if (context.Books.Any())
    {
        context.Books.RemoveRange(context.Books);
        await context.SaveChangesAsync();
    }
    if (context.Categories.Any())
    {
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();
    }

    // إضافة التصنيفات العربية
    var catNovels = new Category { Name = "روايات عربية وعالمية", Description = "أفضل الروايات الكلاسيكية والحديثة التي أثرت في الأدب العالمي والعربي." };
    var catSelfHelp = new Category { Name = "تطوير الذات", Description = "كتب لتحسين المهارات الشخصية والمهنية والإنتاجية." };
    var catHistory = new Category { Name = "تاريخ وسياسة", Description = "إطلالة على الماضي وفهم الحاضر عبر أفضل الكتب التاريخية." };
    var catTech = new Category { Name = "علوم وتكنولوجيا", Description = "استكشافات علمية وتقنية لفهم الكون والتطور الرقمي." };
    var catPhilosophy = new Category { Name = "فكر وفلسفة", Description = "كتب تتناول الفلسفة، المنطق، والقضايا الفكرية العميقة." };

    context.Categories.AddRange(catNovels, catSelfHelp, catHistory, catTech, catPhilosophy);
    await context.SaveChangesAsync();

    // إضافة الكتب العربية مع صور حقيقية أو عالية الدقة
    var books = new List<Book>
    {
        // روايات
        new Book { Title = "ألف ليلة وليلة", Author = "مجهول المؤلف", ISBN = "9789771001", Price = 45.00m, Description = "سرد قصصي خيالي غني يعتبر من عيون الأدب العربي والعالمي.", ImageUrl = "https://placehold.co/600x900/1e2a5e/FFFFFF?text=ألف+ليلة+وليلة", StockQuantity = 50, CategoryId = catNovels.CategoryId, CreatedDate = DateTime.UtcNow },
        new Book { Title = "البؤساء", Author = "فيكتور هوغو", ISBN = "9780451419439", Price = 65.00m, Description = "رواية فرنسية كلاسيكية تتناول قضايا العدالة الاجتماعية والفقر في باريس.", ImageUrl = "https://covers.openlibrary.org/b/isbn/9780451419439-L.jpg", StockQuantity = 30, CategoryId = catNovels.CategoryId, CreatedDate = DateTime.UtcNow },
        new Book { Title = "مئة عام من العزلة", Author = "غابرييل غارسيا ماركيز", ISBN = "9780060883287", Price = 55.00m, Description = "رواية الواقعية السحرية الأشهر التي تحكي قصة عائلة بوينديا.", ImageUrl = "https://covers.openlibrary.org/b/isbn/9780060883287-L.jpg", StockQuantity = 40, CategoryId = catNovels.CategoryId, CreatedDate = DateTime.UtcNow },
        
        // تطوير الذات
        new Book { Title = "العادات السبع للناس الأكثر فعالية", Author = "ستيفن كوفي", ISBN = "9780743269513", Price = 70.00m, Description = "دليل عملي لتحقيق النجاح الشخصي والمهني عبر تبني عادات إيجابية.", ImageUrl = "https://covers.openlibrary.org/b/isbn/9780743269513-L.jpg", StockQuantity = 60, CategoryId = catSelfHelp.CategoryId, CreatedDate = DateTime.UtcNow },
        new Book { Title = "الذكاء العاطفي", Author = "دانيال غولمان", ISBN = "9780553383713", Price = 50.00m, Description = "كيف يمكن للذكاء العاطفي أن يكون أهم من الذكاء العقلي في حياتنا.", ImageUrl = "https://covers.openlibrary.org/b/isbn/9780553383713-L.jpg", StockQuantity = 45, CategoryId = catSelfHelp.CategoryId, CreatedDate = DateTime.UtcNow },
        
        // تاريخ
        new Book { Title = "تاريخ الخلفاء", Author = "جلال الدين السيوطي", ISBN = "9789773001", Price = 80.00m, Description = "كتاب تاريخي شامل يتناول سير الخلفاء وأحداث عصورهم.", ImageUrl = "https://placehold.co/600x900/1e2a5e/FFFFFF?text=تاريخ+الخلفاء", StockQuantity = 20, CategoryId = catHistory.CategoryId, CreatedDate = DateTime.UtcNow },
        new Book { Title = "من هنا نبدأ", Author = "محمد حسنين هيكل", ISBN = "9789773002", Price = 60.00m, Description = "تحليل عميق للسياسة العربية والمصرية في حقبة الخمسينيات والستينيات.", ImageUrl = "https://placehold.co/600x900/1e2a5e/FFFFFF?text=من+هنا+نبدأ", StockQuantity = 25, CategoryId = catHistory.CategoryId, CreatedDate = DateTime.UtcNow },
        
        // علوم
        new Book { Title = "تاريخ موجز للزمن", Author = "ستيفن هوكينغ", ISBN = "9780553380163", Price = 90.00m, Description = "رحلة مذهلة لفهم أسرار الكون، الثقوب السوداء، ونظرية النسبية.", ImageUrl = "https://covers.openlibrary.org/b/isbn/9780553380163-L.jpg", StockQuantity = 35, CategoryId = catTech.CategoryId, CreatedDate = DateTime.UtcNow },
        new Book { Title = "الخوارزمية التي غيرت العالم", Author = "كريس بيرنهارد", ISBN = "9789774002", Price = 55.00m, Description = "كتاب يشرح كيف شكلت الخوارزميات والرياضيات عالم التكنولوجيا الحديث.", ImageUrl = "https://placehold.co/600x900/c49b2a/FFFFFF?text=الخوارزمية", StockQuantity = 15, CategoryId = catTech.CategoryId, CreatedDate = DateTime.UtcNow },
        
        // فكر وفلسفة
        new Book { Title = "ريح الصبا", Author = "جبران خليل جبران", ISBN = "9789775001", Price = 40.00m, Description = "مجموعة من المقالات الفلسفية والروحية للشاعر والفيلسوف جبران.", ImageUrl = "https://placehold.co/600x900/2a9d8f/FFFFFF?text=ريح+الصبا", StockQuantity = 55, CategoryId = catPhilosophy.CategoryId, CreatedDate = DateTime.UtcNow },
        new Book { Title = "رسائل الصديق صديقه", Author = "أبو حيان التوحيدي", ISBN = "9789775002", Price = 75.00m, Description = "من روائع الأدب الفلسفي العربي، حوار عميق بين مثقفين في العصر العباسي.", ImageUrl = "https://placehold.co/600x900/2a9d8f/FFFFFF?text=رسائل+الصديق", StockQuantity = 10, CategoryId = catPhilosophy.CategoryId, CreatedDate = DateTime.UtcNow }
    };

    context.Books.AddRange(books);
    await context.SaveChangesAsync();

    // === نهاية إضافة التصنيفات والكتب العربية ===
}