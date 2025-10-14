using AutoMapper;
using ec_project_api.Dtos.response;
using ec_project_api.Interfaces.location;

namespace ec_project_api.Services.location
{
    public class WardService : IWardService
    {
        private readonly IWardRepository _wardRepository;
        private readonly IMapper _mapper;

        public WardService(IWardRepository wardRepository, IMapper mapper)
        {
            _wardRepository = wardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WardResponseDto>> GetWardsByProvinceIdAsync(int provinceId)
        {
            var wards = await _wardRepository.GetWardsByProvinceIdAsync(provinceId);
            return _mapper.Map<IEnumerable<WardResponseDto>>(wards);
        }
    }
}
