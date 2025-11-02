namespace ec_project_api.Dtos.request.orders
{
    public class CartUpdateRequest
    {
       
            public int UserId { get; set; }
            public int VariantId { get; set; }
            public short Quantity { get; set; }
            public decimal Price { get; set; }
        
    }     
    
}
