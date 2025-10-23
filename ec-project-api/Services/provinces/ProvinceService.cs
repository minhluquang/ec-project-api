using AutoMapper;
using ec_project_api.Interfaces.provinces;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.provinces
{
    public class ProvinceService : BaseService<Province, int>, IProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;

        public ProvinceService(IProvinceRepository provinceRepository) : base(provinceRepository) 
        {
            _provinceRepository = provinceRepository;
        }
    }
}

