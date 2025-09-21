using ec_project_api.Interfaces.System;
using ec_project_api.Models;

public class ResourceRepository : Repository<Resource>, IResourceRepository
{
    public ResourceRepository(DataContext context) : base(context) { }
}
