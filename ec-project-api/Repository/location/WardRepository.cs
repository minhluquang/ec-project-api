using ec_project_api.Interfaces.location;
using ec_project_api.Models;
using ec_project_api.Models.location;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Repository.location
{
    public class WardRepository : IWardRepository
    {
        private readonly DataContext _context;

        public WardRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ward>> GetWardsByProvinceIdAsync(int provinceId)
        {
            return await _context.Wards
                .Where(w => w.ProvinceId == provinceId)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<Ward?> GetWardByIdAsync(int id)
        {
            return await _context.Wards
                .Include(w => w.Province)
                .FirstOrDefaultAsync(w => w.Id == id);
        }
    }
}
