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
if (app.Environment.IsDevelopment())
{
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