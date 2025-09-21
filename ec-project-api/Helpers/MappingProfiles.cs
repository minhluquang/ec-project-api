using AutoMapper;
using ec_project_api.Dto.response.orders;
using ec_project_api.Models;

namespace ec_project_api.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Order, OrderDto>();
        }
    }
}