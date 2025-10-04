using AutoMapper;
using ec_project_api.Dtos.request.users;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Dtos.response.system;
using ec_project_api.Dtos.response.products;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.users;
using ec_project_api.Dtos.Statuses;
using ec_project_api.Dtos.Users;
using ec_project_api.Models;

namespace ec_project_api.Helper {
    public class MappingProfiles : Profile {
        public MappingProfiles() {
            CreateMap<Order, OrderDto>();
            CreateMap<Resource, ResourceDto>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ResourceDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));
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
            CreateMap<ProductVariant, ProductVariantDto>();
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
            // Product Create Request
            CreateMap<ProductCreateRequest, Product>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            // Product Update Request
            CreateMap<ProductUpdateRequest, Product>();

            // Size
            CreateMap<Size, SizeDto>();
            CreateMap<Size, SizeDetailDto>()
                .IncludeBase<Size, SizeDto>();

            // Color
            CreateMap<Color, ColorDto>();
            CreateMap<Color, ColorDetailDto>()
                .IncludeBase<Color, ColorDto>();
            //         CreateMap<User, UserDto>()
            // .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
            //     src.UserRoleDetails != null
            //         ? src.UserRoleDetails.Where(urd => urd.Role != null)
            //                               .Select(urd => new RoleDto { RoleId = urd.Role.RoleId, Name = urd.Role.Name })
            //         : new List<RoleDto>()))
            // .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src =>
            //     src.Addresses != null
            //         ? src.Addresses.Select(a => new AddressDto
            //         {
            //             AddressId = a.AddressId,
            //             RecipientName = a.RecipientName,
            //             Phone = a.Phone,
            //             StreetAddress = a.StreetAddress,
            //             Ward = a.Ward,
            //             District = a.District,
            //             City = a.City,
            //             IsDefault = a.IsDefault
            //         })
            //         : new List<AddressDto>()));

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoleDetails != null
                        ? src.UserRoleDetails.Where(urd => urd.Role != null)
                                            .Select(urd => new RoleDto { RoleId = urd.Role.RoleId, Name = urd.Role.Name, Description = urd.Role.Description })
                        : new List<RoleDto>()))
                .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src =>
                    src.Addresses));

            CreateMap<Address, AddressDto>();


            CreateMap<UserRequest, User>()
    .ForMember(dest => dest.UserId, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
    .ForMember(dest => dest.Status, opt => opt.Ignore())
    .ForMember(dest => dest.UserRoleDetails, opt => opt.Ignore())
    .ForMember(dest => dest.Carts, opt => opt.Ignore())
    .ForMember(dest => dest.Orders, opt => opt.Ignore())
    .ForMember(dest => dest.PasswordHash, opt =>
        opt.MapFrom(src => Helpers.PasswordHasher.HashPassword(src.PasswordHash)));

        }
    }
}