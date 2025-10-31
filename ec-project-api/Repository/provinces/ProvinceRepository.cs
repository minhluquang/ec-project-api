using ec_project_api.Interfaces.provinces;
using ec_project_api.Models;

namespace ec_project_api.Repository.provinces
{
    public class ProvinceRepository : Repository<Province,int>, IProvinceRepository
    {
        public ProvinceRepository(DataContext context) : base(context)
        {
        }
    }
}

