using ec_project_api.Interfaces.Shipping;
using ec_project_api.Models;

public class ShipRepository : Repository<Ship>, IShipRepository
{
    public ShipRepository(DataContext context) : base(context) { }
}
