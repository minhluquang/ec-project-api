using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.productGroups
{
    public class ProductGroupUpdateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên nhóm sản phẩm")]
        [StringLength(100, ErrorMessage = "Tên nhóm sản phẩm không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;
    }
}