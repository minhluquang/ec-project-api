namespace ec_project_api.Constants.messages 
{
    public static class InventoryMessages 
    {
        public const string InventoryItemNotFound = "Không tìm thấy sản phẩm trong kho.";
        public const string ProductVariantNotFound = "Không tìm thấy biến thể sản phẩm.";
        public const string PurchaseOrderNotFound = "Không tìm thấy đơn hàng nhập.";
        public const string InvalidStockValue = "Giá trị tồn kho không hợp lệ.";
        public const string InvalidAdjustmentMode = "Chế độ điều chỉnh không hợp lệ.";
        public const string InsufficientStock = "Không đủ hàng trong kho.";
        public const string StockValueMustBeNonNegative = "Giá trị tồn kho phải không âm.";
        public const string SuccessfullyAdjustedStock = "Điều chỉnh tồn kho thành công.";
        public const string SuccessfullyReceivedPurchaseOrder = "Nhập kho từ đơn hàng thành công.";
        public const string NoPendingItemsToReceive = "Không có sản phẩm nào cần nhập kho.";
        public const string InventoryUpdateFailed = "Cập nhật kho thất bại.";
        public const string InvalidProductVariantId = "ID biến thể sản phẩm không hợp lệ.";
        public const string InvalidPurchaseOrderId = "ID đơn hàng nhập không hợp lệ.";
    }
}

