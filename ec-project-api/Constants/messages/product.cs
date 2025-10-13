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
        public const string ProductDeleteFailedNotDraft = "Chỉ có thể xóa sản phẩm ở trạng thái nháp.";
        public const string InvalidProductData = "Dữ liệu sản phẩm không hợp lệ.";
        public const string SuccessfullyCreatedProduct = "Tạo sản phẩm thành công.";
        public const string SuccessfullyUpdatedProduct = "Cập nhật sản phẩm thành công.";
        public const string SuccessfullyDeletedProduct = "Xóa sản phẩm thành công.";


        // Product image related messages
        public const string ProductImageNotFound = "Không tìm thấy hình ảnh sản phẩm.";
        public const string ProductImageUploadFailed = "Tải lên hình ảnh sản phẩm thất bại.";
        public const string ProductImageUploadSuccessully = "Thêm hình ảnh sản phẩm thành công.";
        public const string ProductImageUpdateFailed = "Cập nhật hình ảnh sản phẩm thất bại.";
        public const string ProductImageDeleteFailed = "Xóa hình ảnh sản phẩm thất bại.";
        public const string SuccessfullyDeletedProductImage = "Xóa hình ảnh sản phẩm thành công.";
        public const string ProductImageDeleteCloudinaryFailed = "Xóa hình ảnh sản phẩm trên Cloudinary thất bại.";
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
        public const string ProductVariantDeleteFailedNotDraft = "Chỉ có thể xóa biến thể sản phẩm ở trạng thái nháp.";
        public const string InvalidProductVariantData = "Dữ liệu biến thể sản phẩm không hợp lệ.";
        public const string SuccessfullyCreatedProductVariant = "Tạo biến thể sản phẩm thành công.";
        public const string SuccessfullyUpdatedProductVariant = "Cập nhật biến thể sản phẩm thành công.";
        public const string SuccessfullyDeletedProductVariant = "Xóa biến thể sản phẩm thành công.";
        public const string NoChangeDataToUpdate = "Vui lòng chọn dữ liệu mới cần cập nhật.";
        public const string ProductVariantNotBelongToProduct = "Biến thể sản phẩm không thuộc về sản phẩm này.";

        // Product group related messages
        public const string ProductGroupNotFound = "Không tìm thấy nhóm sản phẩm.";
        public const string ProductGroupAlreadyExists = "Nhóm sản phẩm đã tồn tại.";
        public const string ProductGroupNameAlreadyExists = "Tên nhóm sản phẩm đã tồn tại.";
        public const string ProductGroupCreateFailed = "Tạo nhóm sản phẩm thất bại.";
        public const string ProductGroupUpdateFailed = "Cập nhật nhóm sản phẩm thất bại.";
        public const string ProductGroupDeleteFailed = "Xóa nhóm sản phẩm thất bại.";
        public const string ProductGroupHasProducts = "Không thể xóa nhóm sản phẩm vì còn sản phẩm đang sử dụng.";
        public const string InvalidProductGroupData = "Dữ liệu nhóm sản phẩm không hợp lệ.";
        public const string SuccessfullyCreatedProductGroup = "Tạo nhóm sản phẩm thành công.";
        public const string SuccessfullyUpdatedProductGroup = "Cập nhật nhóm sản phẩm thành công.";
        public const string SuccessfullyDeletedProductGroup = "Xóa nhóm sản phẩm thành công.";
        public const string ProductGroupsRetrievedSuccessfully = "Lấy danh sách nhóm sản phẩm thành công.";
        public const string ProductGroupRetrievedSuccessfully = "Lấy thông tin nhóm sản phẩm thành công.";
    }
}