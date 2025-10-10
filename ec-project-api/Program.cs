using ec_project_api;
using ec_project_api.Facades;
using ec_project_api.Facades.auth;
using ec_project_api.Facades.PaymentMethods;
using ec_project_api.Facades.payments;
using ec_project_api.Facades.products;
using ec_project_api.Facades.purchaseorders;
using ec_project_api.Facades.reviews;
using ec_project_api.Facades.Suppliers;
using ec_project_api.Interfaces.Payments;
using ec_project_api.Interfaces.Products;
using ec_project_api.Interfaces.PurchaseOrders;
using ec_project_api.Interfaces.Reviews;
using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Interfaces.System;
using ec_project_api.Interfaces.Users;
using ec_project_api.Security;
using ec_project_api.Services;
using ec_project_api.Services.categories;
using ec_project_api.Services.colors;
using ec_project_api.Services.Interfaces;
using ec_project_api.Services.payment;
using ec_project_api.Services.product_groups;
using ec_project_api.Services.product_images;
using ec_project_api.Services.Reviews;
using ec_project_api.Services.sizes;
using ec_project_api.Services.suppliers;
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
// Category
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
// Color
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<IColorRepository, ColorRepository>();
// Size
builder.Services.AddScoped<ColorFacade>();
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
// Product Group
builder.Services.AddScoped<IProductGroupRepository, ProductGroupRepository>();
builder.Services.AddScoped<IProductGroupService, ProductGroupService>();

builder.Services.AddScoped<CustomEmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthFacade>();
// Purchase Order
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<PurchaseOrderFacade>();
// PaymentMethod
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPaymentMethodService, PaymentService>();
builder.Services.AddScoped<PaymentMethodFacade>();
// PaymentDestination
builder.Services.AddScoped<IPaymentDestinationRepository, PaymentDestinationRepository>();
builder.Services.AddScoped<IPaymentDestinationService, PaymentDestinationService>();
builder.Services.AddScoped<PaymentDestinationFacade>();
// ============================
// Swagger + API version
// ============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
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

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.Events = new JwtBearerEvents
//        {
//            OnChallenge = async context =>
//            {
//                context.HandleResponse();

//                var entryPoint = context.HttpContext.RequestServices
//                    .GetRequiredService<CustomAuthenticationEntryPoint>();

//                await entryPoint.HandleAsync(context.HttpContext);
//            },
//            OnForbidden = async context =>
//            {
//                var deniedHandler = context.HttpContext.RequestServices
//                    .GetRequiredService<CustomAccessDeniedHandler>();

//                await deniedHandler.HandleAsync(context.HttpContext);
//            }
//        };
//    });

//builder.Services.AddAuthorization();

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

