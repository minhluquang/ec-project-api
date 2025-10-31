using ec_project_api.Interfaces.System;
using ec_project_api.Models;

public class ResourceRepository : Repository<Resource, short>, IResourceRepository
{
    public ResourceRepository(DataContext context) : base(context) { }
}
