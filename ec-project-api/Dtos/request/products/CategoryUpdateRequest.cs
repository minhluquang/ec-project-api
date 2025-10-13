using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.categories
{
    public class CategoryUpdateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập slug")]
        [StringLength(100, ErrorMessage = "Slug không được vượt quá 100 ký tự")]
        public string Slug { get; set; } = null!;

        public string? Description { get; set; }

        public string? SizeDetail { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public short StatusId { get; set; }

        public short? ParentId { get; set; }
    }
}