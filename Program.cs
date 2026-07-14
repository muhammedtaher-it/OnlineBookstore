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

    // === زراعة التصنيفات والكتب (15 كتاب) ===
    // نتخطى الزراعة إذا كانت قاعدة البيانات تحتوي على كتب بالفعل لتجنب أخطاء الحذف
    if (!context.Books.Any())
    {
        // إضافة التصنيفات
        var catNovels = new Category { Name = "روايات عربية وعالمية", Description = "أفضل الروايات الكلاسيكية والحديثة التي أثرت في الأدب." };
        var catSelfHelp = new Category { Name = "تطوير الذات", Description = "كتب لتحسين المهارات الشخصية والمهنية." };
        var catHistory = new Category { Name = "تاريخ وسياسة", Description = "إطلالة على الماضي وفهم الحاضر." };
        var catTech = new Category { Name = "علوم وتكنولوجيا", Description = "استكشافات علمية وتقنية لفهم الكون." };
        var catPhilosophy = new Category { Name = "فكر وفلسفة", Description = "كتب تتناول الفلسفة والقضايا الفكرية العميقة." };

        context.Categories.AddRange(catNovels, catSelfHelp, catHistory, catTech, catPhilosophy);
        await context.SaveChangesAsync();

        // إضافة 15 كتاب مع صور احترافية مخصصة بالألوان
        var books = new List<Book>
        {
            // روايات (3 كتب)
            new Book { Title = "ألف ليلة وليلة", Author = "مجهول المؤلف", ISBN = "9789771001", Price = 45.00m, Description = "سرد قصصي خيالي غني يعتبر من عيون الأدب العربي والعالمي.", ImageUrl = "https://placehold.co/400x600/1e2a5e/FFFFFF?text=1001+Nights", StockQuantity = 50, CategoryId = catNovels.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "البؤساء", Author = "فيكتور هوغو", ISBN = "9789771002", Price = 65.00m, Description = "رواية فرنسية كلاسيكية تتناول قضايا العدالة الاجتماعية والفقر.", ImageUrl = "https://placehold.co/400x600/1e2a5e/FFFFFF?text=Les+Miserables", StockQuantity = 30, CategoryId = catNovels.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "مئة عام من العزلة", Author = "غابرييل غارسيا ماركيز", ISBN = "9789771003", Price = 55.00m, Description = "رواية الواقعية السحرية الأشهر التي تحكي قصة عائلة بوينديا.", ImageUrl = "https://placehold.co/400x600/c49b2a/FFFFFF?text=100+Years", StockQuantity = 40, CategoryId = catNovels.CategoryId, CreatedDate = DateTime.UtcNow },
            
            // تطوير الذات (3 كتب)
            new Book { Title = "العادات السبع للناس الأكثر فعالية", Author = "ستيفن كوفي", ISBN = "9789772001", Price = 70.00m, Description = "دليل عملي لتحقيق النجاح الشخصي والمهني.", ImageUrl = "https://placehold.co/400x600/2a9d8f/FFFFFF?text=7+Habits", StockQuantity = 60, CategoryId = catSelfHelp.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "الذكاء العاطفي", Author = "دانيال غولمان", ISBN = "9789772002", Price = 50.00m, Description = "كيف يمكن للذكاء العاطفي أن يكون أهم من الذكاء العقلي.", ImageUrl = "https://placehold.co/400x600/2a9d8f/FFFFFF?text=Emotional+IQ", StockQuantity = 45, CategoryId = catSelfHelp.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "فكر تصنع ثروتك", Author = "نابليون هيل", ISBN = "9789772003", Price = 40.00m, Description = "كتاب كلاسيكي في التنمية البشرية وتطوير عقلية الثروة.", ImageUrl = "https://placehold.co/400x600/2a9d8f/FFFFFF?text=Think+Grow+Rich", StockQuantity = 55, CategoryId = catSelfHelp.CategoryId, CreatedDate = DateTime.UtcNow },
            
            // تاريخ (3 كتب)
            new Book { Title = "تاريخ الخلفاء", Author = "جلال الدين السيوطي", ISBN = "9789773001", Price = 80.00m, Description = "كتاب تاريخي شامل يتناول سير الخلفاء وأحداث عصورهم.", ImageUrl = "https://placehold.co/400x600/1e2a5e/FFFFFF?text=Al+Khulafa", StockQuantity = 20, CategoryId = catHistory.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "من هنا نبدأ", Author = "محمد حسنين هيكل", ISBN = "9789773002", Price = 60.00m, Description = "تحليل عميق للسياسة العربية في حقبة الخمسينيات والستينيات.", ImageUrl = "https://placehold.co/400x600/1e2a5e/FFFFFF?text=Min+Huna", StockQuantity = 25, CategoryId = catHistory.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "الأمير", Author = "نيكولو مكيافيلي", ISBN = "9789773003", Price = 35.00m, Description = "أطروحة سياسية كلاسيكية حول اكتساب السلطة والحفاظ عليها.", ImageUrl = "https://placehold.co/400x600/c49b2a/FFFFFF?text=The+Prince", StockQuantity = 70, CategoryId = catHistory.CategoryId, CreatedDate = DateTime.UtcNow },
            
            // علوم (3 كتب)
            new Book { Title = "تاريخ موجز للزمن", Author = "ستيفن هوكينغ", ISBN = "9789774001", Price = 90.00m, Description = "رحلة مذهلة لفهم أسرار الكون، الثقوب السوداء، ونظرية النسبية.", ImageUrl = "https://placehold.co/400x600/c49b2a/FFFFFF?text=Brief+History", StockQuantity = 35, CategoryId = catTech.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "الخوارزمية التي غيرت العالم", Author = "كريس بيرنهارد", ISBN = "9789774002", Price = 55.00m, Description = "كتاب يشرح كيف شكلت الخوارزميات عالم التكنولوجيا الحديث.", ImageUrl = "https://placehold.co/400x600/c49b2a/FFFFFF?text=Algorithms", StockQuantity = 15, CategoryId = catTech.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "ستيف جوبز", Author = "والتر إيزاكسون", ISBN = "9789774003", Price = 75.00m, Description = "السيرة الذاتية الرسمية لصاحب رؤية أبل التقنية.", ImageUrl = "https://placehold.co/400x600/2a9d8f/FFFFFF?text=Steve+Jobs", StockQuantity = 30, CategoryId = catTech.CategoryId, CreatedDate = DateTime.UtcNow },
            
            // فكر وفلسفة (3 كتب)
            new Book { Title = "ريح الصبا", Author = "جبران خليل جبران", ISBN = "9789775001", Price = 40.00m, Description = "مجموعة من المقالات الفلسفية والروحية للشاعر والفيلسوف جبران.", ImageUrl = "https://placehold.co/400x600/2a9d8f/FFFFFF?text=Rih+Al+Saba", StockQuantity = 55, CategoryId = catPhilosophy.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "رسائل الصديق صديقه", Author = "أبو حيان التوحيدي", ISBN = "9789775002", Price = 75.00m, Description = "من روائع الأدب الفلسفي العربي، حوار عميق بين مثقفين.", ImageUrl = "https://placehold.co/400x600/1e2a5e/FFFFFF?text=Rasail", StockQuantity = 10, CategoryId = catPhilosophy.CategoryId, CreatedDate = DateTime.UtcNow },
            new Book { Title = "نبيك هو نفسك", Author = "ميخائيل نعيمة", ISBN = "9789775003", Price = 50.00m, Description = "كتاب فلسفي يتناول سبر أغوار النفس الإنسانية والبحث عن الحقيقة.", ImageUrl = "https://placehold.co/400x600/c49b2a/FFFFFF?text=Nabiyuka", StockQuantity = 40, CategoryId = catPhilosophy.CategoryId, CreatedDate = DateTime.UtcNow }
        };

        context.Books.AddRange(books);
        await context.SaveChangesAsync();
    }
}