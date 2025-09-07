using AutoMapper;
using ec_project_api.DTO;
using ec_project_api.Models;

namespace ec_project_api.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryDTO>();
        }
    }
}
