namespace ec_project_api.Constants.messages {
    public static class ProductMessages {
        // Product related messages
        public const string ProductNotFound = "Không tìm thấy sản phẩm.";
        public const string ProductAlreadyExists = "Sản phẩm đã tồn tại.";
        public const string ProductNameAlreadyExists = "Tên sản phẩm đã tồn tại.";
        public const string ProductSlugAlreadyExists = "Slug sản phẩm đã tồn tại.";
        public const string ProductCategorySlugNotFound = "Không tìm thấy slug thể loại sản phẩm.";
        public const string ProductAlreadyExistsWithNameCategoryMaterial = "Sản phẩm đã tồn tại với tên, thể loại và chất liệu này.";
        public const string ProductCreateFailed = "Tạo sản phẩm thất bại.";
        public const string ProductUpdateFailed = "Cập nhật sản phẩm thất bại.";
        public const string ProductDeleteFailed = "Xóa sản phẩm thất bại.";
        public const string InvalidProductData = "Dữ liệu sản phẩm không hợp lệ.";
        public const string SuccessfullyCreatedProduct = "Tạo sản phẩm thành công.";
        public const string SuccessfullyUpdatedProduct = "Cập nhật sản phẩm thành công.";
        public const string SuccessfullyDeletedProduct = "Xóa sản phẩm thành công.";

        // Product image related messages
        public const string ProductImageNotFound = "Không tìm thấy hình ảnh sản phẩm.";
        public const string ProductImageUploadFailed = "Tải lên hình ảnh sản phẩm thất bại.";
        public const string ProductImageUploadSuccessully = "Cập nhật hình ảnh sản phẩm thành công.";
        public const string ProductImageUpdateFailed = "Cập nhật hình ảnh sản phẩm thất bại.";
        public const string ProductImageDeleteFailed = "Xóa hình ảnh sản phẩm thất bại.";
        public const string InvalidProductImageData = "Dữ liệu hình ảnh sản phẩm không hợp lệ.";
        public const string ProductImageDisplayOrderUpdateFailed = "Cập nhật thứ tự hiển thị hình ảnh sản phẩm thất bại.";
        public const string ProductImageDisplayOrderUpdateSuccessully = "Cập nhật thư tự hình ảnh sản phẩm thành công.";
        public const string ProductImageDisplayOrderConflict = "Xung đột thứ tự hiển thị hình ảnh sản phẩm.";
        public const string ProductImageDisplayOrderNotEqual = "Thứ tự ảnh không thể cập nhật do dữ liệu không đồng nhất với hệ thống.";

        // Product variant related messages
        public const string ProductVariantNotFound = "Không tìm thấy biến thể sản phẩm.";
        public const string ProductVariantAlreadyExists = "Biến thể sản phẩm đã tồn tại.";
        public const string ProductVariantCreateFailed = "Tạo biến thể sản phẩm thất bại.";
        public const string ProductVariantUpdateFailed = "Cập nhật biến thể sản phẩm thất bại.";
        public const string ProductVariantDeleteFailed = "Xóa biến thể sản phẩm thất bại.";
        public const string InvalidProductVariantData = "Dữ liệu biến thể sản phẩm không hợp lệ.";
        public const string SuccessfullyCreatedProductVariant = "Tạo biến thể sản phẩm thành công.";
        public const string SuccessfullyUpdatedProductVariant = "Cập nhật biến thể sản phẩm thành công.";
        public const string SuccessfullyDeletedProductVariant = "Xóa biến thể sản phẩm thành công.";
    }
}