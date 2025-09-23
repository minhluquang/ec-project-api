using ec_project_api.Interfaces.System;
using ec_project_api.Models;

public class StatusRepository : Repository<Status, int>, IStatusRepository
{
    public StatusRepository(DataContext context) : base(context) { }
}
