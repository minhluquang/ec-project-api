using AutoMapper;
using ec_project_api.DTOs.Payments;
using ec_project_api.Dtos.request.addresses;
using ec_project_api.Dtos.request.categories;
using ec_project_api.Dtos.request.materials;
using ec_project_api.Dtos.request.payments;
using ec_project_api.Dtos.request.productGroups;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.request.purchaseorders;
using ec_project_api.Dtos.request.reviews;
using ec_project_api.Dtos.request.shipping;
using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.request.users;
using ec_project_api.Dtos.response.discounts;
using ec_project_api.Dtos.response.homepage;
using ec_project_api.Dtos.response.inventory;
using ec_project_api.Dtos.response.locations;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Dtos.response.payments;
using ec_project_api.Dtos.response.products;
using ec_project_api.Dtos.response.purchaseorders;
using ec_project_api.Dtos.response.reviewreports;
using ec_project_api.Dtos.response.reviews;
using ec_project_api.Dtos.response.shipping;
using ec_project_api.Dtos.response.suppliers;
using ec_project_api.Dtos.response.system;
using ec_project_api.Dtos.response.users;
using ec_project_api.Dtos.Statuses;
using ec_project_api.Dtos.Users;
using ec_project_api.Models;

namespace ec_project_api.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // ============================
            // System & Security
            // ============================
            ConfigureSystemMappings();
            ConfigureSecurityMappings();

            // ============================
            // Products & Variants
            // ============================
            ConfigureProductMappings();
            ConfigureProductVariantMappings();
            ConfigureProductImageMappings();

            // ============================
            // Catalog
            // ============================
            ConfigureCatalogMappings();
            // ============================
            // Cart
            // ============================
            ConfigureCartMapping();

            // ============================
            // Orders & Payments
            // ============================
            ConfigureOrderMappings();
            ConfigurePaymentMappings();

            // ============================
            // Reviews & Reports
            // ============================
            ConfigureReviewMappings();

            // ============================
            // Suppliers & Purchase Orders
            // ============================
            ConfigureSupplierMappings();
            ConfigurePurchaseOrderMappings();

            // ============================
            // Users & Addresses
            // ============================
            ConfigureUserMappings();

            // ============================
            // Shipping & Locations
            // ============================
            ConfigureShippingMappings();
            ConfigureLocationMappings();

            // ============================
            // Homepage
            // ============================
            ConfigureHomepageMappings();
        }

        #region System & Security
        private void ConfigureSystemMappings()
        {
            CreateMap<Permission, PermissionDto>();
            CreateMap<Resource, ResourceDto>()
                .ForMember(d => d.ResourceName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.ResourceDescription, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.Permissions, o => o.Ignore());
            CreateMap<Status, StatusDto>();
        }

        private void ConfigureSecurityMappings()
        {
            CreateMap<Role, RoleDto>()
                .ForMember(d => d.PermissionIds, o => o.MapFrom(s => s.RolePermissions.Select(rp => rp.PermissionId)));

            CreateMap<RoleRequest, Role>()
                .IgnoreAuditFields()
                .ForMember(d => d.RoleId, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.RolePermissions, o => o.Ignore())
                .ForMember(d => d.UserRoleDetails, o => o.Ignore());
        }
        #endregion

        #region Cart
            private void ConfigureCartMapping()
        {
            CreateMap<Cart, CartDetailDto>()
                .ForMember(d => d.CartItems, o => o.MapFrom(s => s.CartItems))
                .ForMember(CartDetailDto => CartDetailDto.UserId, o => o.MapFrom(s => s.UserId));
            CreateMap<CartItem, CartItemDetailDto>()
                .ForMember(d => d.Slug, o => o.MapFrom(s => s.Slug))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ProductVariant != null ? s.ProductVariant.Product.Name : string.Empty))
                .ForMember(d => d.ProductImageUrl, o => o.MapFrom(s => s.ProductVariant != null && s.ProductVariant.Product != null && s.ProductVariant.Product.ProductImages.Any()
                    ? s.ProductVariant.Product.ProductImages.First().ImageUrl
                    : string.Empty))
                .ForMember(d => d.Size, o => o.MapFrom(s => s.ProductVariant != null && s.ProductVariant.Size != null ? s.ProductVariant.Size.Name : string.Empty))
                .ForMember(d => d.Color, o => o.MapFrom(s => s.ProductVariant != null && s.ProductVariant.Product != null && s.ProductVariant.Product.Color != null
                    ? s.ProductVariant.Product.Color.Name
                    : string.Empty));
        }
        #endregion

        #region Products
        private void ConfigureProductMappings()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.PrimaryImage, o => o.MapFrom(s => GetPrimaryImage(s)))
                .AfterMap((src, dest) => dest.SellingPrice = CalculateSellingPrice(src));

            CreateMap<Product, ProductDetailDto>()
                .IncludeBase<Product, ProductDto>();

            CreateMap<ProductCreateRequest, Product>()
                .IgnoreAuditFields()
                .ForMember(d => d.ProductId, o => o.Ignore())
                .ForMember(d => d.BasePrice, o => o.Ignore())
                .ForMember(d => d.StatusId, o => o.Ignore())
                .ForMember(d => d.DiscountPercentage, o => o.Ignore());

            CreateMap<ProductUpdateRequest, Product>();

            CreateMap<Product, ProductSummaryDto>()
                .ForMember(d => d.Thumbnail, o => o.MapFrom(s => GetThumbnailUrl(s)))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.BasePrice))
                .ForMember(d => d.SalePrice, o => o.MapFrom(s => CalculateSalePrice(s)))
                .ForMember(d => d.SoldQuantity, o => o.Ignore());
        }

        private void ConfigureProductVariantMappings()
        {
            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(d => d.Color, o => o.MapFrom(s => s.Product.Color));

            CreateMap<ProductVariant, ProductVariantDetailDto>()
                .IncludeBase<ProductVariant, ProductVariantDto>()
                .ForMember(d => d.Color, o => o.MapFrom(s => s.Product.Color));

            CreateMap<ProductVariantCreateRequest, ProductVariant>()
                .IgnoreAuditFields()
                .ForMember(d => d.ProductVariantId, o => o.Ignore())
                .ForMember(d => d.ProductId, o => o.Ignore())
                .ForMember(d => d.Sku, o => o.Ignore())
                .ForMember(d => d.StockQuantity, o => o.Ignore());

            CreateMap<ProductVariantUpdateRequest, ProductVariant>()
                .IgnoreAuditFields()
                .ForMember(d => d.ProductVariantId, o => o.Ignore())
                .ForMember(d => d.ProductId, o => o.Ignore())
                .ForMember(d => d.Sku, o => o.Ignore())
                .ForMember(d => d.StockQuantity, o => o.Ignore());

            // Inventory mapping
            CreateMap<ProductVariant, InventoryItemDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Product != null && s.Product.Category != null ? s.Product.Category.Name : null))
                .ForMember(d => d.Size, o => o.MapFrom(s => s.Size != null ? s.Size.Name : null))
                .ForMember(d => d.Color, o => o.MapFrom(s => s.Product != null && s.Product.Color != null ? s.Product.Color.Name : null))
                .ForMember(d => d.Status, o => o.Ignore());
        }

        private void ConfigureProductImageMappings()
        {
            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<ProductImage, ProductImageDetailDto>()
                .IncludeBase<ProductImage, ProductImageDto>();
        }
        #endregion

        #region Catalog (Category, Material, Color, Size, ProductGroup, Discount)
        private void ConfigureCatalogMappings()
        {
            // Category
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryDetailDto>().IncludeBase<Category, CategoryDto>();
            CreateMap<CategoryCreateRequest, Category>().IgnoreAuditFields().ForMember(d => d.CategoryId, o => o.Ignore());
            CreateMap<CategoryUpdateRequest, Category>().IgnoreAuditFields().ForMember(d => d.CategoryId, o => o.Ignore());

            // Material
            CreateMap<Material, MaterialDto>();
            CreateMap<Material, MaterialDetailDto>().IncludeBase<Material, MaterialDto>();
            CreateMap<MaterialCreateRequest, Material>().IgnoreAuditFields().ForMember(d => d.MaterialId, o => o.Ignore());
            CreateMap<MaterialUpdateRequest, Material>().IgnoreAuditFields().ForMember(d => d.MaterialId, o => o.Ignore());

            // Color
            CreateMap<Color, ColorDto>();
            CreateMap<Color, ColorDetailDto>().IncludeBase<Color, ColorDto>();
            CreateMap<ColorCreateRequest, Color>().IgnoreAuditFields().ForMember(d => d.ColorId, o => o.Ignore());
            CreateMap<ColorUpdateRequest, Color>().IgnoreAuditFields().ForMember(d => d.ColorId, o => o.Ignore());

            // Size
            CreateMap<Size, SizeDto>();
            CreateMap<Size, SizeDetailDto>().IncludeBase<Size, SizeDto>();
            CreateMap<SizeCreateRequest, Size>().IgnoreAuditFields().ForMember(d => d.SizeId, o => o.Ignore());
            CreateMap<SizeUpdateRequest, Size>().IgnoreAuditFields().ForMember(d => d.SizeId, o => o.Ignore());

            // ProductGroup
            CreateMap<ProductGroup, ProductGroupDto>();
            CreateMap<ProductGroup, ProductGroupDetailDto>().IncludeBase<ProductGroup, ProductGroupDto>();
            CreateMap<ProductGroupCreateRequest, ProductGroup>().IgnoreAuditFields().ForMember(d => d.ProductGroupId, o => o.Ignore());
            CreateMap<ProductGroupUpdateRequest, ProductGroup>()
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.ProductGroupId, o => o.Ignore());

            // Discount
            CreateMap<Discount, DiscountDto>();
            CreateMap<Discount, DiscountDetailDto>().IncludeBase<Discount, DiscountDto>();
            CreateMap<DiscountCreateRequest, Discount>().IgnoreAuditFields().ForMember(d => d.DiscountId, o => o.Ignore());
            CreateMap<DiscountUpdateRequest, Discount>().IgnoreAuditFields().ForMember(d => d.DiscountId, o => o.Ignore());
        }
        #endregion

        #region Orders & Payments
        private void ConfigureOrderMappings()
        {
            CreateMap<Order, OrderDto>();

            CreateMap<Order, OrderDetailDto>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ShippingFee, opt => opt.MapFrom(src => src.ShippingFee))
                .ForMember(dest => dest.IsFreeShip, opt => opt.MapFrom(src => src.IsFreeShip))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment == null ? null : new PaymentOrderDto
                {
                    PaymentId = src.Payment.PaymentId,
                }));
            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<OrderItem, OrderItemsDto>()
                 .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
                 .ForMember(dest => dest.ProductImage,
                    opt => opt.MapFrom(src => src.ProductVariant.Product.ProductImages.FirstOrDefault().ImageUrl))
                .ForMember(dest => dest.Sku,
                    opt => opt.MapFrom(src => src.ProductVariant.Sku))
                .ForMember(dest => dest.Size,
                    opt => opt.MapFrom(src => src.ProductVariant.Size.Name))
                .ForMember(dest => dest.SubTotal,
                    opt => opt.MapFrom(src => src.SubTotal))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.ReviewOrder, opt => opt.MapFrom(src => src.Reviews));

            CreateMap<Review, ReviewOrderDto>()
                .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.ReviewId))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => src.IsEdited));

            CreateMap<User, UserOrderDto>();
            CreateMap<Status, StatusOrderDto>();
            CreateMap<Ship, ShipOrderDto>();
        }

        private void ConfigurePaymentMappings()
        {
            // Payment Method
            CreateMap<PaymentMethod, PaymentMethodDto>()
                .ForMember(d => d.MethodType, o => o.MapFrom(s => s.MethodType.ToString()));
            CreateMap<PaymentMethodCreateRequest, PaymentMethod>().IgnoreAuditFields().ForMember(d => d.PaymentMethodId, o => o.Ignore());
            CreateMap<PaymentMethodUpdateRequest, PaymentMethod>().IgnoreAuditFields();

            // Payment Destination
            CreateMap<PaymentDestination, PaymentDestinationDto>()
                .ForMember(d => d.PaymentMethodName, o => o.MapFrom(s => s.PaymentMethod != null ? s.PaymentMethod.MethodName : string.Empty));
            CreateMap<PaymentDestinationUpdateRequest, PaymentDestination>().IgnoreAuditFields();
            CreateMap<PaymentMethodCreateRequest, PaymentDestination>().IgnoreAuditFields().ForMember(d => d.DestinationId, o => o.Ignore());

            // Payment
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.PaidAt, o => o.MapFrom(s => s.PaidAt ?? DateTime.UtcNow));

            CreateMap<Payment, PaymentResponseDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status != null ? s.Status.Name : null))
                .ForMember(d => d.DestinationName, o => o.MapFrom(s => s.PaymentDestination != null ? s.PaymentDestination.BankName : null))
                .ForMember(d => d.OrderId, o => o.Ignore());
        }
        #endregion

        #region Reviews
        private void ConfigureReviewMappings()
        {
            CreateMap<Review, ReviewDto>()
                .ForMember(d => d.Username, o => o.MapFrom(s => s.OrderItem.Order.User.Username))
                .ForMember(d => d.AvatarImage, o => o.MapFrom(s => s.OrderItem.Order.User.ImageUrl));

            CreateMap<ReviewCreateRequest, Review>()
                .IgnoreAuditFields()
                .ForMember(d => d.ReviewId, o => o.Ignore())
                .ForMember(d => d.IsEdited, o => o.Ignore())
                .ForMember(d => d.StatusId, o => o.Ignore())
                .ForMember(d => d.OrderItem, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.ReviewImages, o => o.Ignore());

            CreateMap<ReviewUpdateRequest, Review>()
                .IgnoreAuditFields()
                .ForMember(d => d.ReviewId, o => o.Ignore())
                .ForMember(d => d.OrderItemId, o => o.Ignore())
                .ForMember(d => d.IsEdited, o => o.Ignore())
                .ForMember(d => d.StatusId, o => o.Ignore())
                .ForMember(d => d.OrderItem, o => o.Ignore())
                .ForMember(d => d.ReviewImages, o => o.Ignore());

            CreateMap<ReviewImage, ReviewImageDto>();

            // Review Report
            CreateMap<ReviewReport, ReviewReportDto>();
            CreateMap<ReviewReportCreateRequest, ReviewReport>()
                .IgnoreAuditFields()
                .ForMember(d => d.ReviewReportId, o => o.Ignore())
                .ForMember(d => d.ReviewId, o => o.Ignore())
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.StatusId, o => o.Ignore())
                .ForMember(d => d.Review, o => o.Ignore())
                .ForMember(d => d.User, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore());

            CreateMap<ReviewReportUpdateStatusRequest, ReviewReport>()
                .IgnoreAuditFields()
                .ForMember(d => d.ReviewReportId, o => o.Ignore())
                .ForMember(d => d.ReviewId, o => o.Ignore())
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.Reason, o => o.Ignore())
                .ForMember(d => d.Description, o => o.Ignore())
                .ForMember(d => d.Review, o => o.Ignore())
                .ForMember(d => d.User, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore());
        }
        #endregion

        #region Suppliers & Purchase Orders
        private void ConfigureSupplierMappings()
        {
            CreateMap<Supplier, SupplierDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status != null ? s.Status.DisplayName : string.Empty));
            CreateMap<SupplierCreateRequest, Supplier>().IgnoreAuditFields().ForMember(d => d.SupplierId, o => o.Ignore());
            CreateMap<SupplierUpdateRequest, Supplier>().IgnoreAuditFields();
        }

        private void ConfigurePurchaseOrderMappings()
        {
            CreateMap<PurchaseOrder, PurchaseOrderResponse>()
                .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.Supplier != null ? s.Supplier.Name : string.Empty))
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status != null ? s.Status.DisplayName : string.Empty))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.PurchaseOrderItems));

            CreateMap<PurchaseOrderItem, PurchaseOrderItemResponse>()
                .ForMember(d => d.Sku, o => o.MapFrom(s => s.ProductVariant != null ? s.ProductVariant.Sku : string.Empty));

            CreateMap<PurchaseOrderCreateRequest, PurchaseOrder>()
                .IgnoreAuditFields()
                .ForMember(d => d.PurchaseOrderId, o => o.Ignore())
                .ForMember(d => d.StatusId, o => o.Ignore())
                .ForMember(d => d.TotalAmount, o => o.Ignore())
                .ForMember(d => d.PurchaseOrderItems, o => o.MapFrom(s => s.Items));

            CreateMap<PurchaseOrderItemCreateRequest, PurchaseOrderItem>()
                .ForMember(d => d.PurchaseOrderItemId, o => o.Ignore())
                .ForMember(d => d.PurchaseOrderId, o => o.Ignore());
        }
        #endregion

        #region Users & Addresses
        private void ConfigureUserMappings()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Roles, o => o.MapFrom(s =>
                    s.UserRoleDetails != null
                        ? s.UserRoleDetails.Where(urd => urd.Role != null)
                                          .Select(urd => new RoleDto
                                          {
                                              RoleId = urd.Role.RoleId,
                                              Name = urd.Role.Name,
                                              Description = urd.Role.Description
                                          })
                        : new List<RoleDto>()));

            CreateMap<UserRequest, User>()
                .IgnoreAuditFields()
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.UserRoleDetails, o => o.Ignore())
                .ForMember(d => d.Carts, o => o.Ignore())
                .ForMember(d => d.Orders, o => o.Ignore());

            CreateMap<Address, AddressDto>();
            CreateMap<AddressCreateRequest, Address>().IgnoreAuditFields().ForMember(d => d.AddressId, o => o.Ignore());
            CreateMap<AddressUpdateRequest, Address>().IgnoreAuditFields().ForMember(d => d.AddressId, o => o.Ignore());
        }
        #endregion


         
        #region Shipping & Locations
        private void ConfigureShippingMappings()
        {
            CreateMap<Ship, ShipDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.Name));

            CreateMap<ShipCreateRequest, Ship>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ShipUpdateRequest, Ship>()
                .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.CreatedAt, o => o.Ignore());
        }

        private void ConfigureLocationMappings()
        {
            CreateMap<Province, ProvinceDto>();
            CreateMap<Ward, WardDto>();
        }
        #endregion

        #region Homepage
        private void ConfigureHomepageMappings()
        {
            CreateMap<Category, CategoryHomePageDto>()
                .ForMember(d => d.Children, o => o.Ignore());
        }
        #endregion

        #region Helper Methods
        private static ProductImage? GetPrimaryImage(Product product)
        {
            return product.ProductImages.FirstOrDefault(pi => pi.IsPrimary)
                ?? product.ProductImages.OrderBy(i => i.DisplayOrder ?? 999).FirstOrDefault();
        }

        private static decimal CalculateSellingPrice(Product product)
        {
            if (product.DiscountPercentage.HasValue)
            {
                return product.BasePrice - (product.BasePrice * product.DiscountPercentage.Value / 100);
            }
            return product.BasePrice;
        }

        private static string GetThumbnailUrl(Product product)
        {
            var primaryImage = product.ProductImages.FirstOrDefault(pi => pi.IsPrimary);
            if (primaryImage != null) return primaryImage.ImageUrl;

            var firstImage = product.ProductImages.OrderBy(pi => pi.DisplayOrder ?? 999).FirstOrDefault();
            return firstImage?.ImageUrl ?? string.Empty;
        }

        private static decimal? CalculateSalePrice(Product product)
        {
            if (!product.DiscountPercentage.HasValue) return null;
            return product.BasePrice - (product.BasePrice * product.DiscountPercentage.Value / 100);
        }
        #endregion
    }

    // Extension method để DRY hơn
    public static class MappingExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAuditFields<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> map)
        {
            return map
                .ForMember("CreatedAt", opt => opt.Ignore())
                .ForMember("UpdatedAt", opt => opt.Ignore());
        }
    }
}