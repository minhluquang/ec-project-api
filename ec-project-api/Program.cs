using ec_project_api;
using ec_project_api.Security;
using ec_project_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================
// Cấu hình dịch vụ cơ bản
// ============================
builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ============================
// Đăng ký Repository, Service, Facade tự động
// ============================
RegisterServicesFromAssembly(builder.Services);

// ============================
// Swagger + API version
// ============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "EC Project API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Nhập JWT token vào đây. Ví dụ: Bearer {token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ============================
// Database (EF Core)
// ============================
builder.Services.AddDbContext<DataContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlServer(
               builder.Configuration.GetConnectionString("DefaultConnection"),
               sqlOptions => sqlOptions.EnableRetryOnFailure(
                   maxRetryCount: 5,
                   maxRetryDelay: TimeSpan.FromSeconds(30),
                   errorNumbersToAdd: null))
);

// ============================
// JWT Authentication & Authorization
// ============================
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<CustomUserService>();
builder.Services.AddScoped<CustomAuthenticationEntryPoint>();
builder.Services.AddScoped<CustomAccessDeniedHandler>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.Events = new JwtBearerEvents
       {
           OnChallenge = async context =>
           {
               context.HandleResponse();
               var entryPoint = context.HttpContext.RequestServices
                   .GetRequiredService<CustomAuthenticationEntryPoint>();
               await entryPoint.HandleAsync(context.HttpContext);
           },
           OnForbidden = async context =>
           {
               var deniedHandler = context.HttpContext.RequestServices
                   .GetRequiredService<CustomAccessDeniedHandler>();
               await deniedHandler.HandleAsync(context.HttpContext);
           }
       };
   });

builder.Services.AddAuthorization(options =>
{
    // User Management
    options.AddPolicy("User.GetAll", p => p.RequireClaim("permission", "User.GetAll"));
    options.AddPolicy("User.GetById", p => p.RequireClaim("permission", "User.GetById"));
    options.AddPolicy("User.Create", p => p.RequireClaim("permission", "User.Create"));
    options.AddPolicy("User.Update", p => p.RequireClaim("permission", "User.Update"));
    options.AddPolicy("User.AssignRole", p => p.RequireClaim("permission", "User.AssignRole"));

    // Role Management
    options.AddPolicy("Role.GetAll", p => p.RequireClaim("permission", "Role.GetAll"));
    options.AddPolicy("Role.GetById", p => p.RequireClaim("permission", "Role.GetById"));
    options.AddPolicy("Role.Update", p => p.RequireClaim("permission", "Role.Update"));
    options.AddPolicy("Role.Delete", p => p.RequireClaim("permission", "Role.Delete"));
    options.AddPolicy("Role.AddPermission", p => p.RequireClaim("permission", "Role.AddPermission"));

    // Shipping Management
    options.AddPolicy("Shipping.GetById", p => p.RequireClaim("permission", "Shipping.GetById"));
    options.AddPolicy("Shipping.Create", p => p.RequireClaim("permission", "Shipping.Create"));
    options.AddPolicy("Shipping.Update", p => p.RequireClaim("permission", "Shipping.Update"));
    options.AddPolicy("Shipping.Delete", p => p.RequireClaim("permission", "Shipping.Delete"));
    options.AddPolicy("Shipping.Activate", p => p.RequireClaim("permission", "Shipping.Activate"));

    // Supplier Management
    options.AddPolicy("Supplier.GetAll", p => p.RequireClaim("permission", "Supplier.GetAll"));
    options.AddPolicy("Supplier.GetById", p => p.RequireClaim("permission", "Supplier.GetById"));
    options.AddPolicy("Supplier.Create", p => p.RequireClaim("permission", "Supplier.Create"));
    options.AddPolicy("Supplier.Update", p => p.RequireClaim("permission", "Supplier.Update"));
    options.AddPolicy("Supplier.Delete", p => p.RequireClaim("permission", "Supplier.Delete"));

    // Purchase Order Management
    options.AddPolicy("PurchaseOrder.GetAll", p => p.RequireClaim("permission", "PurchaseOrder.GetAll"));
    options.AddPolicy("PurchaseOrder.GetById", p => p.RequireClaim("permission", "PurchaseOrder.GetById"));
    options.AddPolicy("PurchaseOrder.Create", p => p.RequireClaim("permission", "PurchaseOrder.Create"));
    options.AddPolicy("PurchaseOrder.Update", p => p.RequireClaim("permission", "PurchaseOrder.Update"));
    options.AddPolicy("PurchaseOrder.Delete", p => p.RequireClaim("permission", "PurchaseOrder.Delete"));
    options.AddPolicy("PurchaseOrder.SetStatus", p => p.RequireClaim("permission", "PurchaseOrder.SetStatus"));
    options.AddPolicy("PurchaseOrder.Cancel", p => p.RequireClaim("permission", "PurchaseOrder.Cancel"));

    // Category Management
    options.AddPolicy("Category.Create", p => p.RequireClaim("permission", "Category.Create"));
    options.AddPolicy("Category.Update", p => p.RequireClaim("permission", "Category.Update"));
    options.AddPolicy("Category.Delete", p => p.RequireClaim("permission", "Category.Delete"));
    options.AddPolicy("Category.GetAll", p => p.RequireClaim("permission", "Category.GetAll"));
    options.AddPolicy("Category.GetById", p => p.RequireClaim("permission", "Category.GetById"));
    options.AddPolicy("Category.GetHierarchy", p => p.RequireClaim("permission", "Category.GetHierarchy"));

    // Product Attributes (Color, Material, Size)
    var attributes = new[] { "Color", "Material", "Size" };
    foreach (var attr in attributes)
    {
        options.AddPolicy($"{attr}.Create", p => p.RequireClaim("permission", $"{attr}.Create"));
        options.AddPolicy($"{attr}.Update", p => p.RequireClaim("permission", $"{attr}.Update"));
        options.AddPolicy($"{attr}.Delete", p => p.RequireClaim("permission", $"{attr}.Delete"));
        options.AddPolicy($"{attr}.GetAll", p => p.RequireClaim("permission", $"{attr}.GetAll"));
        options.AddPolicy($"{attr}.GetById", p => p.RequireClaim("permission", $"{attr}.GetById"));
    }
    options.AddPolicy("Size.GetOptions", p => p.RequireClaim("permission", "Size.GetOptions"));

    // Product Group
    options.AddPolicy("ProductGroup.Create", p => p.RequireClaim("permission", "ProductGroup.Create"));
    options.AddPolicy("ProductGroup.Update", p => p.RequireClaim("permission", "ProductGroup.Update"));
    options.AddPolicy("ProductGroup.Delete", p => p.RequireClaim("permission", "ProductGroup.Delete"));
    options.AddPolicy("ProductGroup.GetAll", p => p.RequireClaim("permission", "ProductGroup.GetAll"));
    options.AddPolicy("ProductGroup.GetById", p => p.RequireClaim("permission", "ProductGroup.GetById"));

    // Discount
    options.AddPolicy("Discount.Create", p => p.RequireClaim("permission", "Discount.Create"));
    options.AddPolicy("Discount.Update", p => p.RequireClaim("permission", "Discount.Update"));
    options.AddPolicy("Discount.Delete", p => p.RequireClaim("permission", "Discount.Delete"));
    options.AddPolicy("Discount.GetAll", p => p.RequireClaim("permission", "Discount.GetAll"));
    options.AddPolicy("Discount.GetById", p => p.RequireClaim("permission", "Discount.GetById"));
    options.AddPolicy("Discount.UpdateInactive", p => p.RequireClaim("permission", "Discount.UpdateInactive"));

    // Order
    options.AddPolicy("Order.GetAll", p => p.RequireClaim("permission", "Order.GetAll"));
    options.AddPolicy("Order.Approve", p => p.RequireClaim("permission", "Order.Approve"));
    options.AddPolicy("Order.UpdateStatus", p => p.RequireClaim("permission", "Order.UpdateStatus"));

    // Payment Destination
    var payments = new[]
    {
        "PaymentDestination.Create", "PaymentDestination.Update", "PaymentDestination.Delete",
        "PaymentDestination.GetAll", "PaymentDestination.GetById", "PaymentDestination.ToggleStatus"
    };
    foreach (var perm in payments)
        options.AddPolicy(perm, p => p.RequireClaim("permission", perm));

    // Product Return
    var returns = new[]
    {
        "ProductReturn.GetAll", "ProductReturn.Create", "ProductReturn.Delete",
        "ProductReturn.Approve", "ProductReturn.Reject", "ProductReturn.CompleteRefund", "ProductReturn.CompleteExchange"
    };
    foreach (var perm in returns)
        options.AddPolicy(perm, p => p.RequireClaim("permission", perm));

    // Product Management
    var products = new[]
    {
        "Product.Create", "Product.Update", "Product.Delete", "Product.GetFormMeta"
    };
    foreach (var perm in products)
        options.AddPolicy(perm, p => p.RequireClaim("permission", perm));

    // Product Image
    var images = new[]
    {
        "ProductImage.GetAll", "ProductImage.Upload", "ProductImage.UpdateOrder", "ProductImage.Delete"
    };
    foreach (var perm in images)
        options.AddPolicy(perm, p => p.RequireClaim("permission", perm));

    // Product Variant
    var variants = new[]
    {
        "ProductVariant.GetAll", "ProductVariant.Create", "ProductVariant.Update", "ProductVariant.Delete"
    };
    foreach (var perm in variants)
        options.AddPolicy(perm, p => p.RequireClaim("permission", perm));

    // Review
    options.AddPolicy("Review.ToggleVisibility", p => p.RequireClaim("permission", "Review.ToggleVisibility"));

    // Dashboard & Analytics
    var dashboard = new[]
    {
        "Dashboard.GetOverview", "Dashboard.GetMonthlyRevenue", "Dashboard.GetCategorySalesPercentage",
        "Dashboard.GetMonthlyRevenueStats", "Dashboard.GetTopSellingProducts",
        "Dashboard.GetWeeklySales", "Dashboard.GetMonthlyProfit"
    };
    foreach (var perm in dashboard)
        options.AddPolicy(perm, p => p.RequireClaim("permission", perm));
});

// ============================
// CORS cho Frontend
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
    );
});

// ============================
// Build app
// ============================
var app = builder.Build();

// ============================
// Middleware pipeline
// ============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.Run();

// ============================
// Helper Method: Tự động đăng ký services
// ============================
static void RegisterServicesFromAssembly(IServiceCollection services)
{
    var assembly = typeof(Program).Assembly;
    
    // Lấy tất cả các interface và implementation
    var interfaceTypes = assembly.GetTypes()
        .Where(t => t.IsInterface && t.Name.StartsWith("I"))
        .ToList();

    foreach (var interfaceType in interfaceTypes)
    {
        // Tìm implementation tương ứng
        // VD: IUserRepository -> UserRepository
        var implementationType = assembly.GetTypes()
            .FirstOrDefault(t => 
                !t.IsInterface && 
                !t.IsAbstract && 
                interfaceType.IsAssignableFrom(t));

        if (implementationType != null)
        {
            services.AddScoped(interfaceType, implementationType);
        }
    }

    // Đăng ký Facades (không có interface)
    var facadeTypes = assembly.GetTypes()
        .Where(t => !t.IsInterface && 
                    !t.IsAbstract && 
                    t.Name.EndsWith("Facade"))
        .ToList();

    foreach (var facadeType in facadeTypes)
    {
        services.AddScoped(facadeType);
    }

    // Đăng ký các service đặc biệt không theo pattern
    services.AddScoped<CustomEmailService>();
}
