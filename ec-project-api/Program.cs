using ec_project_api;
using ec_project_api.Facades;
using ec_project_api.Facades.products;
using ec_project_api.Facades.Suppliers;
using ec_project_api.Interfaces.Products;
using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Interfaces.System;
using ec_project_api.Interfaces.Users;
using ec_project_api.Security; 
using ec_project_api.Services;
using ec_project_api.Services.categories;
using ec_project_api.Services.colors;
using ec_project_api.Services.product_images;
using ec_project_api.Services.product_variants;
using ec_project_api.Services.sizes;
using ec_project_api.Services.suppliers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================
// 1️⃣ Cấu hình dịch vụ cơ bản
// ============================
builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ============================
// 2️⃣ Đăng ký Repository, Service, Facade
// ============================
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<PermissionFacade>();

builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<RoleFacade>();

builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<StatusFacade>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductFacade>();

builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<ProductVariantFacade>();

builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<ProductImageFacade>();

builder.Services.AddScoped<IUserRoleDetailRepository, UserRoleDetailRepository>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserFacade>();

builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<IColorRepository, ColorRepository>();

builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<ISizeRepository, SizeRepository>();

builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<SupplierFacade>();

// ============================
// 3️⃣ Swagger + API version
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
// 4️⃣ Database (EF Core)
// ============================
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ============================
// 5️⃣ JWT Authentication & Authorization
// ============================
var jwtConfig = builder.Configuration.GetSection("Jwt");

builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<JwtMiddleware>();
builder.Services.AddScoped<CustomUserService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured.")))
        };
    });

builder.Services.AddAuthorization();

// ============================
// 6️⃣ CORS cho Frontend
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173") // 👈 domain FE (React / Vue)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ============================
// 7️⃣ Build app
// ============================
var app = builder.Build();

// ============================
// 8️⃣ Middleware pipeline
// ============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔥 Thêm CORS + Authentication + Middleware JWT
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
