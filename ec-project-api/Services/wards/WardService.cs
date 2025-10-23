using ec_project_api.Interfaces.wards;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.wards
{
    public class WardService : BaseService<Ward, int>, IWardService
    {
        private readonly IWardRepository _wardRepository;

        public WardService(IWardRepository wardRepository) : base(wardRepository) 
        {
            _wardRepository = wardRepository;
        }

        public async Task<IEnumerable<Ward>> GetWardsByProvinceIdAsync(int provinceId)
        {
            var options = new QueryOptions<Ward>
            {
                Filter = w => w.ProvinceId == provinceId
            };

            return await _wardRepository.GetAllAsync(options);
        }
    }
}
