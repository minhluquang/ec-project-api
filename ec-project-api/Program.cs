using ec_project_api;
using ec_project_api.Facades;
using ec_project_api.Facades.auth;
using ec_project_api.Facades.discounts;
using ec_project_api.Facades.Homepage;
using ec_project_api.Facades.inventory;
using ec_project_api.Facades.materials;
using ec_project_api.Facades.orders;
using ec_project_api.Facades.PaymentMethods;
using ec_project_api.Facades.payments;
using ec_project_api.Facades.products;
using ec_project_api.Facades.purchaseorders;
using ec_project_api.Facades.ReviewReports;
using ec_project_api.Facades.reviews;
using ec_project_api.Facades.Ships;
using ec_project_api.Facades.Suppliers;
using ec_project_api.Facades.system;
using ec_project_api.Interfaces;
using ec_project_api.Interfaces.Discounts;
using ec_project_api.Interfaces.inventory;
using ec_project_api.Interfaces.location;
using ec_project_api.Interfaces.Orders;
using ec_project_api.Interfaces.Payments;
using ec_project_api.Interfaces.Products;
using ec_project_api.Interfaces.PurchaseOrders;
using ec_project_api.Interfaces.Reviews;
using ec_project_api.Interfaces.Shipping;
using ec_project_api.Interfaces.Ships;
using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Interfaces.System;
using ec_project_api.Interfaces.Users;
using ec_project_api.Repository;
using ec_project_api.Repository.location;
using ec_project_api.Repository.ReviewReports;
using ec_project_api.Security;
using ec_project_api.Services;
using ec_project_api.Services.categories;
using ec_project_api.Services.colors;
using ec_project_api.Services.Interfaces;
using ec_project_api.Services.custom;
using ec_project_api.Services.discounts;
using ec_project_api.Services.homepage;
using ec_project_api.Services.inventory;
using ec_project_api.Services.location;
using ec_project_api.Services.order_items;
using ec_project_api.Services.orders;
using ec_project_api.Services.payment;
using ec_project_api.Services.products;
using ec_project_api.Services.product_groups;
using ec_project_api.Services.product_images;
using ec_project_api.Services.product_return;
using ec_project_api.Services.productReturn;
using ec_project_api.Services.review_images;
using ec_project_api.Services.ReviewReports;
using ec_project_api.Services.reviews;
using ec_project_api.Services.Ships;
using ec_project_api.Services.sizes;
using ec_project_api.Services.suppliers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ec_project_api.Facades.Payments;

var builder = WebApplication.CreateBuilder(args);

// ============================
// Cấu hình dịch vụ cơ bản
// ============================
builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ============================
// Đăng ký Repository, Service, Facade
// ============================
// Permission
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<PermissionFacade>();
// Role
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<RoleFacade>();
// Status
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<StatusFacade>();
// Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductFacade>();
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<ProductVariantFacade>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<ProductImageFacade>();
// User
builder.Services.AddScoped<IUserRoleDetailRepository, UserRoleDetailRepository>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserFacade>();
// Material
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<MaterialFacade>();
// Category
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryFacade>();
// Color
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<IColorRepository, ColorRepository>();
builder.Services.AddScoped<ColorFacade>();
// Discount
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<DiscountFacade>();
// Size
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<ISizeRepository, SizeRepository>();
builder.Services.AddScoped<SizeFacade>();
// Supplier
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<SupplierFacade>();
// Review
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ReviewFacade>();
// Review Image
builder.Services.AddScoped<IReviewImageRepository, ReviewImageRepository>();
builder.Services.AddScoped<IReviewImageService, ReviewImageService>();
builder.Services.AddScoped<ReviewFacade>();
// Review Report
builder.Services.AddScoped<IReviewReportRepository, ReviewReportRepository>();
builder.Services.AddScoped<IReviewReportService, ReviewReportService>();
builder.Services.AddScoped<ReviewReportFacade>();
// Order
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
// Order Item
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
// Product Group
builder.Services.AddScoped<IProductGroupRepository, ProductGroupRepository>();
builder.Services.AddScoped<IProductGroupService, ProductGroupService>();
builder.Services.AddScoped<ProductGroupFacade>();

builder.Services.AddScoped<CustomEmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthFacade>();
// Purchase Order
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<PurchaseOrderFacade>();
// PaymentMethod
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<PaymentMethodFacade>();
// PaymentDestination
builder.Services.AddScoped<IPaymentDestinationRepository, PaymentDestinationRepository>();
builder.Services.AddScoped<IPaymentDestinationService, PaymentDestinationService>();
builder.Services.AddScoped<PaymentDestinationFacade>();
// Payment
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<PaymentFacade>();
// Order
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<OrderFacade>();
// Product Return
builder.Services.AddScoped<IProductReturnRepository, ProductReturnRepository>();
builder.Services.AddScoped<IProductReturnService, ProductReturnService>();
builder.Services.AddScoped<ProductReturnFacade>();
builder.Services.AddScoped<IPurchaseOrderItemRepository, PurchaseOrderItemRepository>();
// Inventory
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<InventoryFacade>();
// Dashboard
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<DashboardFacade>();
// Province
builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
// Ward
builder.Services.AddScoped<IWardRepository, WardRepository>();
builder.Services.AddScoped<IWardService, WardService>();
// Ship
builder.Services.AddScoped<IShipRepository, ShipRepository>();
builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddScoped<ShipFacade>();
// Home Page
builder.Services.AddScoped<IHomepageService, HomepageService>();
builder.Services.AddScoped<HomepageFacade>();

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
    options
        .UseLazyLoadingProxies()
        .UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            })
);

// ============================
// JWT Authentication & Authorization
// ============================
var jwtConfig = builder.Configuration.GetSection("Jwt");

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

builder.Services.AddAuthorization();

// ============================
// CORS cho Frontend
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

// ============================
// Build app
// ============================
var app = builder.Build();

// ============================
// Middleware pipeline
// ============================
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

//app.UseAuthentication();
//app.UseMiddleware<JwtMiddleware>();
//app.UseAuthorization();

app.MapControllers();

app.Run();
