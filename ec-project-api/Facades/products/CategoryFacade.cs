using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.categories;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Services.products;
using System.Linq.Expressions;

namespace ec_project_api.Facades.Products
{
    public class CategoryFacade
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public CategoryFacade(ICategoryService categoryService, IProductService productService, IStatusService statusService, IMapper mapper)
        {
            _categoryService = categoryService;
            _productService = productService;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetHierarchyForSelectionAsync()
        {
            var rootCategoryIds = new List<short> { 1, 2 };
            var categories = await _categoryService.GetByParentIdsAsync(rootCategoryIds);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryService.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDetailDto> GetByIdAsync(short id)
        {
            var category = await _categoryService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(CategoryMessages.CategoryNotFound);

            return _mapper.Map<CategoryDetailDto>(category);
        }

        public async Task<bool> CreateAsync(CategoryCreateRequest request)
        {
            // ✅ Kiểm tra trùng tên và slug
            var existingName = await _categoryService.FirstOrDefaultAsync(c => c.Name == request.Name.Trim());
            if (existingName != null)
                throw new InvalidOperationException(CategoryMessages.CategoryAlreadyExists);

            var existingSlug = await _categoryService.FirstOrDefaultAsync(c => c.Slug == request.Slug.Trim());
            if (existingSlug != null)
                throw new InvalidOperationException(CategoryMessages.CategorySlugAlreadyExists);

            // ✅ Lấy trạng thái mặc định Inactive
            var inActiveStatus = await _statusService.FirstOrDefaultAsync(
                s => s.Name == "Inactive" && s.EntityType == "Category"
            ) ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            // ✅ Map request -> entity
            var category = _mapper.Map<Category>(request);
            category.StatusId = inActiveStatus.StatusId;
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            // ✅ Gọi CreateAsync của service (service tự xử lý upload nếu có)
            bool created = await _categoryService.CreateAsync(category, request.FileImage);

            if (!created)
                throw new InvalidOperationException("Tạo danh mục thất bại.");

            return true;
        }

        public async Task<bool> UpdateAsync(short id, CategoryUpdateRequest request)
        {
            var existing = await _categoryService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(CategoryMessages.CategoryNotFound);

            if (id is 1 or 2)
                throw new InvalidOperationException(CategoryMessages.CategoryOriginUpdateFailed);

            var duplicateName = await _categoryService.FirstOrDefaultAsync(c =>
                c.CategoryId != id && c.Name == request.Name.Trim());
            if (duplicateName != null)
                throw new InvalidOperationException(CategoryMessages.CategoryAlreadyExists);

            var duplicateSlug = await _categoryService.FirstOrDefaultAsync(c =>
                c.CategoryId != id && c.Slug == request.Slug.Trim());
            if (duplicateSlug != null)
                throw new InvalidOperationException(CategoryMessages.CategorySlugAlreadyExists);

            var status = await _statusService.GetByIdAsync(request.StatusId);
            if (status == null || status.EntityType != EntityVariables.Category)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            // Gọi service update → service xử lý ghi đè ảnh nếu fileImage != null
            return await _categoryService.UpdateAsync(existing, request.RemoveImage, request.FileImage);
        }


        public async Task<bool> DeleteAsync(short id)
        {
            if (id is 1 or 2)
                throw new InvalidOperationException(CategoryMessages.CategoryOriginDeleteFailed);

            var category = await _categoryService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(CategoryMessages.CategoryNotFound);

            if (category.Status.Name != StatusVariables.Inactive)
                throw new InvalidOperationException(CategoryMessages.CategoryDeleteFailedNotInactive);

            var childCategories = await _categoryService.GetByParentIdAsync(category.CategoryId);
            if (childCategories.Any())
                throw new InvalidOperationException(CategoryMessages.CategoryHasChild);

            if (category.Products != null && category.Products.Any())
                throw new InvalidOperationException(CategoryMessages.CategoryInUse);

            // Xóa Category → service tự xóa ảnh
            return await _categoryService.DeleteAsync(category);
        }


        private static Expression<Func<Category, bool>> BuildCategoryFilter(CategoryFilter filter)
        {
            return c =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (c.Status != null && c.Status.Name == filter.StatusName && c.Status.EntityType == EntityVariables.Category)) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    c.Name.Contains(filter.Search) ||
                    c.Slug.Contains(filter.Search) ||
                    c.CategoryId.ToString().Contains(filter.Search) ||
                    (c.Description != null && c.Description.Contains(filter.Search))) &&
                (filter.ParentId == null || c.ParentId == filter.ParentId);
        }

        public async Task<PagedResult<CategoryDetailDto>> GetAllPagedAsync(CategoryFilter filter)
        {
            var options = new QueryOptions<Category>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Filter = BuildCategoryFilter(filter)
            };

            var pagedResult = await _categoryService.GetAllPagedAsync(options);
            var dtoList = _mapper.Map<IEnumerable<CategoryDetailDto>>(pagedResult.Items);

            var parentIds = dtoList
                .Where(c => c.ParentId.HasValue)
                .Select(c => c.ParentId!.Value)
                .Distinct()
                .ToList();

            if (parentIds.Any())
            {
                var parents = await _categoryService.FindAsync(c => parentIds.Contains(c.CategoryId));
                var parentMap = parents.ToDictionary(p => p.CategoryId, p => p.Name);

                foreach (var item in dtoList)
                {
                    if (item.ParentId.HasValue && parentMap.TryGetValue(item.ParentId.Value, out var parentName))
                        item.ParentName = parentName;
                }
            }

            return new PagedResult<CategoryDetailDto>
            {
                Items = dtoList,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
