using ec_project_api.Interfaces.wards;
using ec_project_api.Models;

namespace ec_project_api.Repository.wards
{
    public class WardRepository : Repository<Ward,int>, IWardRepository
    {
        public WardRepository(DataContext context) : base(context)
        {
        }
    }
}
