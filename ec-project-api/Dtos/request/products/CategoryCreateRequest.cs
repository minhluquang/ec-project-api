using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ec_project_api.Dtos.request.categories
{
    public class CategoryCreateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập slug")]
        [StringLength(100, ErrorMessage = "Slug không được vượt quá 100 ký tự")]
        public string Slug { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục cha")]
        public short? ParentId { get; set; }

        // ✅ FE gửi formData.append("FileImage", file)
        public IFormFile? FileImage { get; set; }
    }
}
