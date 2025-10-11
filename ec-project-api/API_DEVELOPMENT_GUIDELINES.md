# API Development Guidelines

## Tổng quan
Document này mô tả quy trình và pattern chuẩn để phát triển API trong dự án EC Project. Tất cả API mới phải tuân theo các nguyên tắc và cấu trúc được định nghĩa dưới đây.

## Architecture Pattern
Dự án sử dụng **Layered Architecture** với pattern **Repository-Service-Facade-Controller**:

```
Controller → Facade → Service → Repository → Database
```

## Quy trình phát triển API mới

### 1. **Định nghĩa Model/Entity** (nếu cần)
- **Location**: `Models/{domain}/`
- **Naming**: PascalCase, số ít (VD: `Product`, `Category`)

```csharp
namespace ec_project_api.Models.{domain}
{
    public class EntityName
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Navigation properties
    }
}
```

### 2. **Tạo DTOs (Data Transfer Objects)**
- **Location**: 
  - Request DTOs: `Dtos/request/{domain}/`
  - Response DTOs: `Dtos/response/{domain}/`

#### Request DTOs
```csharp
// EntityCreateRequest.cs
public class EntityCreateRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    // Validation attributes
}

// EntityUpdateRequest.cs  
public class EntityUpdateRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
```

#### Response DTOs
```csharp
// EntityDto.cs - Cho danh sách
public class EntityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// EntityDetailDto.cs - Cho chi tiết
public class EntityDetailDto : EntityDto
{
    public DateTime UpdatedAt { get; set; }
    // Thêm các field detail khác
}
```

### 3. **Tạo Constants Messages**
- **Location**: `Constants/messages/{entity}.cs`

```csharp
namespace ec_project_api.Constants.messages 
{
    public static class EntityMessages 
    {
        public const string EntityNotFound = "Không tìm thấy {entity}.";
        public const string EntityAlreadyExists = "{Entity} đã tồn tại.";
        public const string EntityNameAlreadyExists = "Tên {entity} đã tồn tại.";
        public const string SuccessfullyCreatedEntity = "Tạo {entity} thành công.";
        public const string SuccessfullyUpdatedEntity = "Cập nhật {entity} thành công.";
        public const string SuccessfullyDeletedEntity = "Xóa {entity} thành công.";
        public const string EntityInUse = "{Entity} đang được sử dụng, không thể xóa.";
        public const string InvalidEntityData = "Dữ liệu {entity} không hợp lệ.";
    }
}
```

### 4. **Tạo Repository Interface & Implementation**
- **Location**: 
  - Interface: `Interfaces/{domain}/I{Entity}Repository.cs`
  - Implementation: `Repository/{domain}/{Entity}Repository.cs`

#### Interface
```csharp
namespace ec_project_api.Interfaces.{domain}
{
    public interface IEntityRepository : IRepository<Entity, int>
    {
        // Thêm methods specific cho entity này
        Task<Entity?> GetByNameAsync(string name);
    }
}
```

#### Implementation
```csharp
namespace ec_project_api.Repository.{domain}
{
    public class EntityRepository : Repository<Entity, int>, IEntityRepository
    {
        public EntityRepository(DataContext context) : base(context) { }

        public async Task<Entity?> GetByNameAsync(string name)
        {
            return await _context.Entities
                .FirstOrDefaultAsync(e => e.Name == name);
        }
    }
}
```

### 5. **Tạo Service Interface & Implementation**
- **Location**: 
  - Interface: `Interfaces/{domain}/I{Entity}Service.cs`
  - Implementation: `Services/{domain}/{Entity}Service.cs`

#### Interface
```csharp
namespace ec_project_api.Interfaces.{domain}
{
    public interface IEntityService : IService<Entity, int>
    {
        // Thêm business methods
    }
}
```

#### Implementation
```csharp
namespace ec_project_api.Services.{domain}
{
    public class EntityService : Service<Entity, int>, IEntityService
    {
        public EntityService(IEntityRepository repository) : base(repository) { }

        // Implement business logic methods
    }
}
```

### 6. **Tạo Facade**
- **Location**: `Facades/{domain}/{Entity}Facade.cs`

```csharp
namespace ec_project_api.Facades.{domain}
{
    public class EntityFacade
    {
        private readonly IEntityService _entityService;
        private readonly IMapper _mapper;

        public EntityFacade(IEntityService entityService, IMapper mapper)
        {
            _entityService = entityService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EntityDto>> GetAllAsync()
        {
            var entities = await _entityService.GetAllAsync();
            return _mapper.Map<IEnumerable<EntityDto>>(entities);
        }

        public async Task<EntityDetailDto> GetByIdAsync(int id)
        {
            var entity = await _entityService.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException(EntityMessages.EntityNotFound);

            return _mapper.Map<EntityDetailDto>(entity);
        }

        public async Task<bool> CreateAsync(EntityCreateRequest request)
        {
            // Validation
            var existing = await _entityService.FirstOrDefaultAsync(e => e.Name == request.Name.Trim());
            if (existing != null)
                throw new InvalidOperationException(EntityMessages.EntityNameAlreadyExists);

            // Mapping & Create
            var entity = _mapper.Map<Entity>(request);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            return await _entityService.CreateAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, EntityUpdateRequest request)
        {
            var existing = await _entityService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(EntityMessages.EntityNotFound);

            // Check duplicate name
            var duplicate = await _entityService.FirstOrDefaultAsync(e => e.Id != id && e.Name == request.Name.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(EntityMessages.EntityNameAlreadyExists);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _entityService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _entityService.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException(EntityMessages.EntityNotFound);

            return await _entityService.DeleteAsync(entity);
        }
    }
}
```

### 7. **Tạo Controller**
- **Location**: `Controller/{domain}/{Entity}Controller.cs`

```csharp
namespace ec_project_api.Controller.{domain}
{
    [Route(PathVariables.EntityRoot)]
    [ApiController]
    public class EntityController : ControllerBase
    {
        private readonly EntityFacade _entityFacade;

        public EntityController(EntityFacade entityFacade)
        {
            _entityFacade = entityFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<EntityDto>>>> GetAll()
        {
            try
            {
                var result = await _entityFacade.GetAllAsync();
                return Ok(ResponseData<IEnumerable<EntityDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<EntityDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<EntityDetailDto>>> GetById(int id)
        {
            try
            {
                var result = await _entityFacade.GetByIdAsync(id);
                return Ok(ResponseData<EntityDetailDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<EntityDetailDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<EntityDetailDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] EntityCreateRequest request)
        {
            try
            {
                var result = await _entityFacade.CreateAsync(request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, result, EntityMessages.SuccessfullyCreatedEntity));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPatch(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(int id, [FromBody] EntityUpdateRequest request)
        {
            try
            {
                var result = await _entityFacade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, EntityMessages.SuccessfullyUpdatedEntity));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(int id)
        {
            try
            {
                var result = await _entityFacade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, EntityMessages.SuccessfullyDeletedEntity));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}
```

### 8. **Cập nhật AutoMapper Profile**
- **Location**: `Helpers/MappingProfiles.cs`

```csharp
// Thêm vào MappingProfiles.cs
CreateMap<EntityCreateRequest, Entity>();
CreateMap<EntityUpdateRequest, Entity>();
CreateMap<Entity, EntityDto>();
CreateMap<Entity, EntityDetailDto>();
```

### 9. **Đăng ký Dependency Injection**
- **Location**: `Program.cs`

```csharp
// Thêm vào Program.cs trong section đăng ký services
// Entity
builder.Services.AddScoped<IEntityRepository, EntityRepository>();
builder.Services.AddScoped<IEntityService, EntityService>();
builder.Services.AddScoped<EntityFacade>();
```

### 10. **Thêm Path Variables** (nếu cần)
- **Location**: `Constants/variables/Path.cs`

```csharp
public const string EntityRoot = "api/entities";
```

## Nguyên tắc và Best Practices

### Error Handling
- **KeyNotFoundException**: 404 Not Found
- **InvalidOperationException**: 409 Conflict (duplicate, business rule violation)
- **ArgumentException**: 400 Bad Request (validation)
- **Exception**: 400 Bad Request (general error)

### Response Format
Tất cả API phải sử dụng `ResponseData<T>` wrapper:
```csharp
return Ok(ResponseData<T>.Success(StatusCodes.Status200OK, data, message));
return BadRequest(ResponseData<T>.Error(StatusCodes.Status400BadRequest, errorMessage));
```

### Validation
- Sử dụng Data Annotations trong Request DTOs
- Validation logic phức tạp trong Facade layer
- Kiểm tra `ModelState.IsValid` trong Controller

### Naming Conventions
- **Files**: PascalCase (`ProductController.cs`)
- **Classes**: PascalCase (`ProductService`)
- **Methods**: PascalCase (`GetAllAsync`)
- **Variables**: camelCase (`productService`)
- **Constants**: PascalCase (`ProductNotFound`)

### Async/Await
- Tất cả database operations phải async
- Suffix `Async` cho async methods
- Sử dụng `Task<T>` cho return types

## Checklist khi tạo API mới

- [ ] Model/Entity được định nghĩa (nếu cần)
- [ ] Request/Response DTOs đã tạo
- [ ] Constants Messages đã định nghĩa
- [ ] Repository Interface & Implementation
- [ ] Service Interface & Implementation  
- [ ] Facade với business logic và validation
- [ ] Controller với proper error handling
- [ ] AutoMapper profiles đã cập nhật
- [ ] Dependency Injection đã đăng ký
- [ ] Path variables đã thêm (nếu cần)
- [ ] Database migration (nếu có thay đổi schema)
- [ ] Unit tests (khuyến khích)

## Ví dụ hoàn chỉnh
Tham khảo implementation của `Category`, `Color`, hoặc `Inventory` để hiểu rõ hơn về pattern này.

---
*Document này sẽ được cập nhật khi có thay đổi trong architecture hoặc best practices.*
