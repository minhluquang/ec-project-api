using AutoMapper;
using ec_project_api.Dtos.response.locations;
using ec_project_api.Services.wards;

namespace ec_project_api.Facades.wards;

public class WardFacade
{
    private readonly IWardService _wardService;
    private readonly IMapper _mapper;
    
    
    public WardFacade(IWardService wardService, IMapper mapper)
    {
        _wardService = wardService;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<WardDto>> GetWardsByProvinceIdAsync(int provinceId)
    {
        var wards =  await _wardService.GetWardsByProvinceIdAsync(provinceId);
        return _mapper.Map<IEnumerable<WardDto>>(wards);
    }
}