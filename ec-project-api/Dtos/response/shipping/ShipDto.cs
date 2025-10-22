using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.shipping
{
    public class ShipDto
    {
        public byte ShipId { get; set; }
        public string CorpName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal BaseCost { get; set; }
        public byte EstimatedDays { get; set; }
        public short StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        public StatusDto? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
