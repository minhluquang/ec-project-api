namespace ec_project_api.Dtos.request.orders
{
    public class CartUpdateRequest
    {
       
            public int UserId { get; set; }
            public int VariantId { get; set; }
            public short Quantity { get; set; }
            public decimal Price { get; set; }
            public string Slug { get; set; } = string.Empty;
        
    }     
    
}
