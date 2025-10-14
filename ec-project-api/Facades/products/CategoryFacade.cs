using AutoMapper;
using ec_project_api.Constants.messages; // Bạn cần tạo file CategoryMessages
using ec_project_api.Dtos.request.categories;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services; // Bạn cần tạo ICategoryService
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ec_project_api.Facades.categories
{
    public class CategoryFacade
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryFacade(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
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
            // Kiểm tra slug đã tồn tại chưa
            var existing = await _categoryService.FirstOrDefaultAsync(c => c.Slug == request.Slug.Trim());
            if (existing != null)
                throw new InvalidOperationException(CategoryMessages.CategorySlugAlreadyExists);

            var category = _mapper.Map<Category>(request);
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            return await _categoryService.CreateAsync(category);
        }

        public async Task<bool> UpdateAsync(short id, CategoryUpdateRequest request)
        {
            var existing = await _categoryService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(CategoryMessages.CategoryNotFound);

            // Kiểm tra trùng slug với danh mục khác
            var duplicate = await _categoryService.FirstOrDefaultAsync(c => c.CategoryId != id && c.Slug == request.Slug.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(CategoryMessages.CategorySlugAlreadyExists);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _categoryService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException(CategoryMessages.CategoryNotFound);

            // (Tùy chọn) Kiểm tra xem danh mục có con hoặc có sản phẩm không trước khi xóa
            // if (category.Children.Any() || category.Products.Any())
            //    throw new InvalidOperationException(CategoryMessages.CategoryInUse);

            return await _categoryService.DeleteAsync(category);
        }
    }
}