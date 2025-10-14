using AutoMapper;
using ec_project_api.Dtos.response;
using ec_project_api.Interfaces.location;

namespace ec_project_api.Services.location
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;
        private readonly IMapper _mapper;

        public ProvinceService(IProvinceRepository provinceRepository, IMapper mapper)
        {
            _provinceRepository = provinceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProvinceResponseDto>> GetAllProvincesAsync()
        {
            var provinces = await _provinceRepository.GetAllProvincesAsync();
            return _mapper.Map<IEnumerable<ProvinceResponseDto>>(provinces);
        }
    }
}

