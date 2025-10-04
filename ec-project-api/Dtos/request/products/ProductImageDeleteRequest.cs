using System.ComponentModel.DataAnnotations;
namespace ec_project_api.Dtos.request.products {
    public class ProductImageDeleteRequest {
        [Required(ErrorMessage = "Vui lòng chọn id hình ảnh.")]
        public int ProductImageId { get; set; }
    }
}
