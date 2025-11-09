namespace ec_project_api.Constants.variables {
    public static class PathVariables {
        public const string ServicePath = "/ec-project";
        public const string ApiV1 = "/api/v1";
        public const string SwaggerPath = "/swagger";

        public const string SwaggerIndex = SwaggerPath + "/index.html";
        public const string SwaggerJson = SwaggerPath + "/v1/swagger.json";

        public const string Health = "/health";

        public const string BasePath = ServicePath + ApiV1;

        public const string AuthRoot = BasePath + "/auth";
        public const string UserRoot = BasePath + "/users";
        public const string RoleRoot = BasePath + "/roles";
        public const string PermissionRoot = BasePath + "/permissions";
        public const string ProductReturnRoot = BasePath +  "/product-returns";
        

        public const string ProductRoot = BasePath + "/products";
        public const string ProductVariantRoot = ProductRoot + "/{productId}/variants";
        public const string ProductImageRoot = ProductRoot + "/{productId}/images";
        public const string ProductGroupRoot = BasePath + "/productgroup";
        public const string CategoryRoot = BasePath + "/categories";
        public const string MaterialRoot = BasePath + "/materials";
        public const string ColorRoot = BasePath + "/colors";
        public const string SizeRoot = BasePath + "/sizes";                           

        public const string OrderRoot = BasePath + "/orders";
        public const string PaymentRoot = BasePath + "/payments";
        public const string PaymentMethodRoot = BasePath + "/payment-methods";
        public const string PaymentDestinationRoot = BasePath + "/payment-destinations";

        public const string SupplierRoot = BasePath + "/suppliers";
        public const string ShipRoot = BasePath + "/ships";

        public const string DiscountRoot = BasePath + "/discounts";
        public const string ReviewRoot = BasePath + "/reviews";
        public const string ReviewReportRoot = ReviewRoot + "/{reviewId}/reports";
        public const string ReturnRoot = BasePath + "/returns";

        public const string StatusRoot = BasePath + "/statuses";
        public const string ResourceRoot = BasePath + "/resources";
        public const string DashboardRoot = BasePath + "/dashboard";

        // Inventory
        public const string InventoryRoot = BasePath + "/inventory";
        public const string Homepage =  BasePath + "/homepage";

        public const string PurchaseOrder = BasePath + "/purchase-orders";

        public const string OrderUserId = OrderRoot + "/user/{userId}";
        public const string ApproveOrder = OrderRoot + "/approve/{orderId}";
        public const string CancelOrder = OrderRoot + "/cancel/{orderId}";
        public const string CompleteOrder = OrderRoot + "/complete/{orderId}";

        public const string ApproveReturn = ReturnRoot + "/approve/{returnId}";
        public const string RejectedReturn = ReturnRoot + "/reject/{returnId}";
        public const string CompleteReturnForRefund = ReturnRoot + "/complete-refund/{returnId}";
        public const string CompleteReturnForExchange = ReturnRoot + "/complete-exchange/{returnId}";


        public const string ProvinceRoot = BasePath + "/provinces";    
        public const string WardRoot = BasePath + "/wards";
        public const string AddressRoot = BasePath + "/addresses";
        //cart
        public const string CartRoot = BasePath + "/carts";
        public const string GetCartByUserId = CartRoot + "/{userId}";
        public const string UpdateCartItem = CartRoot + "/update";
        public const string DeleteCartItem = CartRoot + "/{userId}/{variantId}";
        public const string ClearCart = CartRoot + "/{userId}/clear";
        // ===============================
        // COMMON ACTION PATHS
        // ===============================
        public const string GetAll = "";
        public const string GetById = "{id}";
        public const string Create = "create";
        public const string Update = "update/{id}";
        public const string Delete = "delete/{id}";
        

        // ===============================
        // AUTH ACTION PATHS
        // ===============================
        public const string Login = "login";
        public const string Register = "register";
        public const string RefreshToken = "refresh-token";
        public const string Verify = "verify";
        public const string Logout = "logout";
        public const string ForgotPassword = "forgot-password";
        public const string ResetPassword = "reset-password";


        // ===============================
        // ROLE ACTION PATHS
        // ===============================
        public const string AssignPermissions = "{id}/permissions";

        // ===============================
        // USER ACTION PATHS
        // ===============================
        public const string AssignRoles = "assign-roles";
        public const string ChangePassword = "change-password";
        public const string UserProfile = "me";
        public const string VerifyCheck = "verify-successful";
        public const string UploadAvatarImage = "upload-avatar-image";
        public const string DeleteAvatarImage = "delete-avatar-image";
    }
}