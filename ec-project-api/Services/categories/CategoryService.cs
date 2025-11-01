using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace ec_project_api.Services.categories {
    public class CategoryService : BaseService<Category, short>, ICategoryService {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository repository)
    : base(repository) {
            _categoryRepository = repository;
        }

        public async Task<IEnumerable<Category>> GetByParentIdsAsync(List<short> categoryIds)
        {
            var options = new Repository.Base.QueryOptions<Category>();
            var allCategories = await _categoryRepository.GetAllAsync(options);

            return allCategories
                .Where(c => categoryIds.Contains(c.CategoryId)
                         || (c.ParentId.HasValue && categoryIds.Contains(c.ParentId.Value)))
                .ToList();
        }

        // ✅ Không ném lỗi – chỉ trả false nếu không hợp lệ
        public override async Task<bool> UpdateAsync(Category entity)
        {
            if (entity == null)
                return false;

            if (entity.CategoryId == 1 || entity.CategoryId == 2)
                return false;

            return await base.UpdateAsync(entity);
        }

        public override async Task<bool> DeleteAsync(Category entity)
        {
            if (entity == null)
                return false;

            if (entity.CategoryId == 1 || entity.CategoryId == 2)
                return false;

            return await base.DeleteAsync(entity);
        }

        public override async Task<bool> DeleteByIdAsync(short id)
        {
            if (id == 1 || id == 2)
                return false;

            return await base.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<Category>> GetByParentIdAsync(short parentId)
        {
            return await FindAsync(c => c.ParentId == parentId);
        }

       


    }
}
