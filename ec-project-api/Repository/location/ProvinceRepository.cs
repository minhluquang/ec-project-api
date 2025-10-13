using ec_project_api.Interfaces.location;
using ec_project_api.Models;
using ec_project_api.Models.location;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Repository.location
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly DataContext _context;

        public ProvinceRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Province>> GetAllProvincesAsync()
        {
            return await _context.Provinces
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Province?> GetProvinceByIdAsync(int id)
        {
            return await _context.Provinces
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}

