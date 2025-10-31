using System;

namespace ec_project_api.Dtos.request.shipping
{
    public class ShipFilter
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public int? StatusId { get; set; }
        public string? CorpName { get; set; }
        public string? OrderBy { get; set; }
    }
}
