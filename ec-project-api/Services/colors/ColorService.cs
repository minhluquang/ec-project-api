using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using System.Threading.Tasks;

namespace ec_project_api.Services.colors {
    public class ColorService : BaseService<Color, short>, IColorService {
        private readonly IColorRepository _colorRepository;

        public ColorService(IColorRepository repository) : base(repository) {
            _colorRepository = repository;
        }
        public override async Task<Color?> GetByIdAsync(short id, QueryOptions<Color>? options = null)
        {
            options ??= new QueryOptions<Color>();
            options.Includes.Add(c => c.Status);
            return await base.GetByIdAsync(id, options);
        }

        // Ví dụ: kiểm tra tên màu đã tồn tại (bổ sung nếu cần)
        public async Task<bool> IsColorNameExistsAsync(string name, short? excludeId = null) {
            var color = await _colorRepository.FirstOrDefaultAsync(
                c => c.Name == name && (!excludeId.HasValue || c.ColorId != excludeId.Value)
            );
            return color != null;
        }

        // Có thể bổ sung các logic nghiệp vụ khác tại đây nếu cần
    }
}