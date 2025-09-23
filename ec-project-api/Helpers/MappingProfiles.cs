using AutoMapper;
using ec_project_api.Dto.response.orders;
using ec_project_api.Dto.response.system;
using ec_project_api.Models;

namespace ec_project_api.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<Resource, ResourceDto>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ResourceDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));
        }
    }
}