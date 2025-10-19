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
using System.Linq.Expressions;

namespace ec_project_api.Facades.products
{
    public class CategoryFacade
    {
        private readonly ICategoryService _categoryService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public CategoryFacade(ICategoryService categoryService, IStatusService statusService, IMapper mapper)
        {
            _categoryService = categoryService;
            _statusService = statusService;
            _mapper = mapper;
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
            var existing = await _categoryService.FirstOrDefaultAsync(c => c.Slug == request.Slug.Trim());
            if (existing != null)
                throw new InvalidOperationException(CategoryMessages.CategorySlugAlreadyExists);

            var status = await _statusService.GetByIdAsync(request.StatusId)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var category = _mapper.Map<Category>(request);
            category.StatusId = status.StatusId;
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            return await _categoryService.CreateAsync(category);
        }

        public async Task<bool> UpdateAsync(short id, CategoryUpdateRequest request)
        {
            var existing = await _categoryService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(CategoryMessages.CategoryNotFound);

            var duplicate = await _categoryService.FirstOrDefaultAsync(c => c.CategoryId != id && c.Slug == request.Slug.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(CategoryMessages.CategorySlugAlreadyExists);

            var status = await _statusService.GetByIdAsync(request.StatusId);
            if (status == null || status.EntityType != EntityVariables.Category)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _categoryService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException(CategoryMessages.CategoryNotFound);

            if (category.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(CategoryMessages.CategoryDeleteFailedNotDraft);

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
                    c.CategoryId.ToString().Contains(filter.Search));
        }

        public async Task<PagedResult<CategoryDetailDto>> GetAllPagedAsync(CategoryFilter filter)
        {
            var options = new QueryOptions<Category>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
            };

            options.Filter = BuildCategoryFilter(filter);

            var pagedResult = await _categoryService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<CategoryDetailDto>>(pagedResult.Items);
            var pagedResultDto = new PagedResult<CategoryDetailDto>
            {
                Items = dtoList,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
            return pagedResultDto;
        }
    }
}