using AutoMapper;
using ec_project_api.Dtos.response.locations;
using ec_project_api.Services.provinces;

namespace ec_project_api.Facades.provinces;

public class ProvinceFacade
{
        private readonly IProvinceService _provinceService;
        private readonly IMapper _mapper;
        
        public ProvinceFacade(IProvinceService provinceService, IMapper mapper)
        {
                _provinceService = provinceService;
                _mapper = mapper;
        }
        
        public async Task<IEnumerable<ProvinceDto>> GetAllProvincesAsync()
        {
                var provinces =  await _provinceService.GetAllAsync();
                return _mapper.Map<IEnumerable<ProvinceDto>>(provinces);
        }
}