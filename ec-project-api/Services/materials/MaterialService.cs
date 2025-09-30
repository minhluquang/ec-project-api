using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services {
    public class MaterialService : BaseService<Material, short>, IMaterialService {
        public MaterialService(IMaterialRepository repository)
    : base(repository) {
        }
    }
}
