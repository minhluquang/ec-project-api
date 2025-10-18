using AutoMapper;
using ec_project_api.Dtos.request.categories;
using ec_project_api.Dtos.request.materials;
using ec_project_api.Dtos.request.productGroups;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.request.purchaseorders;
using ec_project_api.Dtos.request.reviews;
using ec_project_api.Dtos.request.shipping;
using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.request.users;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.discounts;
using ec_project_api.Dtos.response.inventory;
using ec_project_api.Dtos.response.orders;
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
using ec_project_api.Models.location;

namespace ec_project_api.Helper {
    public class MappingProfiles : Profile {
        public MappingProfiles() {
            CreateMap<Permission, PermissionDto>()
                .ForMember(dest => dest.PermissionId, opt => opt.MapFrom(src => src.PermissionId))
                .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.PermissionName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
            CreateMap<Resource, ResourceDto>()
                .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.ResourceId))
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ResourceDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Permissions, opt => opt.Ignore());
            CreateMap<Status, StatusDto>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.EntityType));
            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PermissionIds,
                           opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.PermissionId)));
            CreateMap<RoleRequest, Role>()
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoleDetails, opt => opt.Ignore());


            CreateMap<Order, OrderDto>();

            // Category
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryDetailDto>()
                .IncludeBase<Category, CategoryDto>();

            // Material
            CreateMap<Material, MaterialDto>();
            CreateMap<Material, MaterialDetailDto>()
                .IncludeBase<Material, MaterialDto>();

            // Product
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.PrimaryImage, opt => opt.MapFrom(src =>
                    src.ProductImages
                        .FirstOrDefault(pi => pi.IsPrimary) ??
                    src.ProductImages
                        .OrderBy(i => i.DisplayOrder ?? 999)
                        .FirstOrDefault()
                ))
                .AfterMap((src, dest) =>
                 {
                     if (src.DiscountPercentage.HasValue) {
                         dest.SellingPrice = src.BasePrice - (src.BasePrice * src.DiscountPercentage.Value / 100);
                     }
                     else {
                         dest.SellingPrice = src.BasePrice;
                     }
                 });
            CreateMap<Product, ProductDetailDto>()
                .IncludeBase<Product, ProductDto>();
            // Product Image
            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<ProductImage, ProductImageDetailDto>()
                .IncludeBase<ProductImage, ProductImageDto>();
            // Product Variant
            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Product != null ? src.Product.Color : null));
            CreateMap<ProductVariant, ProductVariantDetailDto>()
                .IncludeBase<ProductVariant, ProductVariantDto>()
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Product != null ? src.Product.Color : null));
            CreateMap<ProductVariantCreateRequest, ProductVariant>()
                .ForMember(dest => dest.ProductVariantId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.Sku, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.StockQuantity, opt => opt.Ignore());
            CreateMap<ProductVariantUpdateRequest, ProductVariant>()
                .ForMember(dest => dest.ProductVariantId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.Sku, opt => opt.Ignore())
                .ForMember(dest => dest.StockQuantity, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            // Inventory Item
            CreateMap<ProductVariant, InventoryItemDto>()
                .ForMember(d => d.ProductVariantId, m => m.MapFrom(s => s.ProductVariantId))
                .ForMember(d => d.Sku, m => m.MapFrom(s => s.Sku))
                .ForMember(d => d.ProductName, m => m.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
                .ForMember(d => d.CategoryName, m => m.MapFrom(s => s.Product != null && s.Product.Category != null ? s.Product.Category.Name : null))
                .ForMember(d => d.Size, m => m.MapFrom(s => s.Size != null ? s.Size.Name : null))
                .ForMember(d => d.Color, m => m.MapFrom(s => s.Product != null && s.Product.Color != null ? s.Product.Color.Name : null))
                .ForMember(d => d.StockQuantity, m => m.MapFrom(s => s.StockQuantity))
                .ForMember(d => d.Status, m => m.Ignore())
                .ForMember(d => d.UpdatedAt, m => m.MapFrom(s => s.UpdatedAt));
            // ProductGroup
            CreateMap<ProductGroup, ProductGroupDto>();
            CreateMap<ProductGroup, ProductGroupDetailDto>()
                .IncludeBase<ProductGroup, ProductGroupDto>();

            CreateMap<ProductGroupCreateRequest, ProductGroup>()
                .ForMember(dest => dest.ProductGroupId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<ProductGroupUpdateRequest, ProductGroup>()
                .ForMember(dest => dest.ProductGroupId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            // Product Create Request
            CreateMap<ProductCreateRequest, Product>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.BasePrice, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.DiscountPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            // Product Update Request
            CreateMap<ProductUpdateRequest, Product>();

            // Size
            CreateMap<Size, SizeDto>();
            CreateMap<Size, SizeDetailDto>().IncludeBase<Size, SizeDto>();

            CreateMap<SizeCreateRequest, Size>()
                .ForMember(dest => dest.SizeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<SizeUpdateRequest, Size>()
                .ForMember(dest => dest.SizeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Discount
            CreateMap<Discount, DiscountDto>();
            CreateMap<Discount, DiscountDetailDto>()
                .IncludeBase<Discount, DiscountDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<DiscountCreateRequest, Discount>()
                .ForMember(dest => dest.DiscountId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<DiscountUpdateRequest, Discount>()
                .ForMember(dest => dest.DiscountId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Color
            CreateMap<Color, ColorDto>();
            CreateMap<Color, ColorDetailDto>()
                .IncludeBase<Color, ColorDto>();

            CreateMap<ColorCreateRequest, Color>()
                .ForMember(dest => dest.ColorId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<ColorUpdateRequest, Color>()
                .ForMember(dest => dest.ColorId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Material
            CreateMap<Material, MaterialDto>();
            CreateMap<Material, MaterialDetailDto>()
                .IncludeBase<Material, MaterialDto>();

            CreateMap<MaterialCreateRequest, Material>()
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<MaterialUpdateRequest, Material>()
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Category
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryDetailDto>()
                .IncludeBase<Category, CategoryDto>();

            CreateMap<CategoryCreateRequest, Category>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<CategoryUpdateRequest, Category>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Review
            CreateMap<Review, ReviewDto>();
            CreateMap<ReviewCreateRequest, Review>()
                .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
                .ForMember(dest => dest.IsEdited, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItem, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewImages, opt => opt.Ignore());
            CreateMap<ReviewUpdateRequest, Review>()
                .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItemId, opt => opt.Ignore())
                .ForMember(dest => dest.IsEdited, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItem, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewImages, opt => opt.Ignore());

            // Review Image 
            CreateMap<ReviewImage, ReviewImageDto>();

            // Review Report
            CreateMap<ReviewReport, ReviewReportDto>();
            CreateMap<ReviewReportCreateRequest, ReviewReport>()
                .ForMember(dest => dest.ReviewReportId, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Review, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());
            CreateMap<ReviewReportUpdateStatusRequest, ReviewReport>()
                .ForMember(dest => dest.ReviewReportId, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Reason, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Review, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            // Order
            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoleDetails != null
                        ? src.UserRoleDetails.Where(urd => urd.Role != null)
                                            .Select(urd => new RoleDto { RoleId = urd.Role.RoleId, Name = urd.Role.Name, Description = urd.Role.Description })
                        : new List<RoleDto>()))
                .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src =>
                    src.Addresses));

            CreateMap<Address, AddressDto>();

            // Supplier
            CreateMap<SupplierCreateRequest, Supplier>()
                .ForMember(dest => dest.SupplierId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<SupplierUpdateRequest, Supplier>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<Supplier, SupplierDto>()
               .ForMember(dest => dest.StatusName,
                   opt => opt.MapFrom(src => src.Status != null ? src.Status.DisplayName : string.Empty));
            // Purchase Order
            CreateMap<PurchaseOrder, PurchaseOrderResponse>()
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.MapFrom(src => src.PurchaseOrderId))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.DisplayName : string.Empty))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PurchaseOrderItems));
            CreateMap<PurchaseOrderItem, PurchaseOrderItemResponse>()
                .ForMember(dest => dest.PurchaseOrderItemId, opt => opt.MapFrom(src => src.PurchaseOrderItemId))
                .ForMember(dest => dest.ProductVariantId, opt => opt.MapFrom(src => src.ProductVariantId))
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Sku : string.Empty))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.ProfitPercentage, opt => opt.MapFrom(src => src.ProfitPercentage))
                .ForMember(dest => dest.IsPushed, opt => opt.MapFrom(src => src.IsPushed));
            CreateMap<PurchaseOrderCreateRequest, PurchaseOrder>()
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderItems, opt => opt.MapFrom(src => src.Items));
            CreateMap<PurchaseOrderItemCreateRequest, PurchaseOrderItem>()
                .ForMember(dest => dest.PurchaseOrderItemId, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductVariantId, opt => opt.MapFrom(src => src.ProductVariantId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.ProfitPercentage, opt => opt.MapFrom(src => src.ProfitPercentage))
                .ForMember(dest => dest.IsPushed, opt => opt.MapFrom(src => src.IsPushed));


            CreateMap<UserRequest, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoleDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Carts, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore());
            // Ship
            CreateMap<ShipCreateRequest, Ship>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<ShipUpdateRequest, Ship>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            CreateMap<Ship, ShipDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name));
            // Homepage
            CreateMap<Category, ec_project_api.Dtos.response.homepage.CategoryHomePageDto>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Children, opt => opt.Ignore());

            CreateMap<Product, ec_project_api.Dtos.response.homepage.ProductSummaryDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src =>
                    src.ProductImages.FirstOrDefault(pi => pi.IsPrimary) != null
                        ? src.ProductImages.FirstOrDefault(pi => pi.IsPrimary)!.ImageUrl
                        : src.ProductImages.OrderBy(pi => pi.DisplayOrder ?? 999).FirstOrDefault() != null
                            ? src.ProductImages.OrderBy(pi => pi.DisplayOrder ?? 999).FirstOrDefault()!.ImageUrl
                            : string.Empty))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.SalePrice, opt => opt.MapFrom(src => src.DiscountPercentage.HasValue
                    ? (decimal?)(src.BasePrice - (src.BasePrice * src.DiscountPercentage.Value / 100))
                    : null))
                .ForMember(dest => dest.SoldQuantity, opt => opt.Ignore());
            
            // Province
            CreateMap<Province, ProvinceResponseDto>();

            // Ward
            CreateMap<Ward, WardResponseDto>();
        }
    }
}