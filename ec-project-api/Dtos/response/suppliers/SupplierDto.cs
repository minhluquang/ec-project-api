namespace ec_project_api.Dtos.response.suppliers
{
	public class SupplierDto
	{
		public int SupplierId { get; set; }
		public string Name { get; set; } = null!;
		public string ContactName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public string Address { get; set; } = null!;
		public int StatusId { get; set; }
		public string StatusName { get; set; } = null!;
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
