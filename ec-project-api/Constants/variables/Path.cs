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
        public const string ProductReturnRoot = "/product-returns";

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
        
        public const string ProvinceRoot = BasePath + "/provinces";    
        public const string WardRoot = BasePath + "/wards";
        public const string AddressRoot = BasePath + "/addresses";
        
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
    }
}