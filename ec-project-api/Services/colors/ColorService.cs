using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.colors {
    public class ColorService : BaseService<Color, short>, IColorService {
        public ColorService(IColorRepository repository) : base(repository) {
        }
    }
}