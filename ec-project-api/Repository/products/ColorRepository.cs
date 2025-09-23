using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class ColorRepository : Repository<Color, short>, IColorRepository
{
    public ColorRepository(DataContext context) : base(context) { }
}
