using ec_project_api.Interfaces.Users;
using ec_project_api.Models;

public class AddressRepository : Repository<Address, int>, IAddressRepository
{
    public AddressRepository(DataContext context) : base(context) { }
}
