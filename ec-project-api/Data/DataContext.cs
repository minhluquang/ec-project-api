using ec_project_api.Models;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Status> Statuses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRoleDetail> UserRoleDetails { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentDestination> PaymentDestinations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Status
            modelBuilder.Entity<Status>()
                .ToTable("status")
                .HasKey(s => s.StatusId);

            modelBuilder.Entity<Status>()
                .HasIndex(s => new { s.Name, s.EntityType })
                .HasDatabaseName("uk_status_name_entity")
                .IsUnique();

            modelBuilder.Entity<Status>()
                .HasIndex(s => s.EntityType)
                .HasDatabaseName("idx_entity_type");

            modelBuilder.Entity<Status>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 2. Role
            modelBuilder.Entity<Role>()
                .ToTable("role")
                .HasKey(r => r.RoleId);

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .HasDatabaseName("uk_roles_name")
                .IsUnique();

            modelBuilder.Entity<Role>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Role>()
                .Property(r => r.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 3. Resource
            modelBuilder.Entity<Resource>()
                .ToTable("resource")
                .HasKey(r => r.ResourceId);

            modelBuilder.Entity<Resource>()
                .HasIndex(r => r.Name)
                .HasDatabaseName("uk_resource_name")
                .IsUnique();

            // 4. Size
            modelBuilder.Entity<Size>()
                .ToTable("size")
                .HasKey(s => s.SizeId);

            modelBuilder.Entity<Size>()
                .HasIndex(s => s.Name)
                .HasDatabaseName("uk_size_name")
                .IsUnique();

            modelBuilder.Entity<Size>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Size>()
                .Property(s => s.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 5. Supplier
            modelBuilder.Entity<Supplier>()
                .ToTable("supplier")
                .HasKey(s => s.SupplierId);

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Email)
                .HasDatabaseName("uk_supplier_email")
                .IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Name)
                .HasDatabaseName("idx_supplier_name");

            modelBuilder.Entity<Supplier>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Supplier>()
                .Property(s => s.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 6. Permission
            modelBuilder.Entity<Permission>()
                .ToTable("permission")
                .HasKey(p => p.PermissionId);

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.PermissionName)
                .HasDatabaseName("uk_permission_name")
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasOne(p => p.Status)
                .WithMany(s => s.Permissions)
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_permission_status");

            modelBuilder.Entity<Permission>()
                .HasOne(p => p.Resource)
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.ResourceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_permission_resource");

            // 7. User
            modelBuilder.Entity<User>()
                .ToTable("user")
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .HasDatabaseName("uk_user_username")
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .HasDatabaseName("uk_user_email")
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .HasDatabaseName("idx_user_email");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.CreatedAt)
                .HasDatabaseName("idx_user_created_at");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_user_role");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Status)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.StatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_user_status");

            modelBuilder.Entity<User>()
                .Property(u => u.IsVerified)
                .HasDefaultValue(false);

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 8. RolePermission
            modelBuilder.Entity<RolePermission>()
                .ToTable("role_permission")
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_role_permission_role");

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_role_permission_permission");

            modelBuilder.Entity<RolePermission>()
                .Property(rp => rp.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<RolePermission>()
                .Property(rp => rp.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 9. UserRoleDetail
            modelBuilder.Entity<UserRoleDetail>()
                .ToTable("user_role_detail")
                .HasKey(urd => new { urd.UserId, urd.RoleId });

            modelBuilder.Entity<UserRoleDetail>()
                .HasOne(urd => urd.User)
                .WithMany(u => u.UserRoleDetails)
                .HasForeignKey(urd => urd.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_user_role_detail_user");

            modelBuilder.Entity<UserRoleDetail>()
                .HasOne(urd => urd.Role)
                .WithMany(r => r.UserRoleDetails)
                .HasForeignKey(urd => urd.RoleId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_user_role_detail_role");

            modelBuilder.Entity<UserRoleDetail>()
                .HasOne(urd => urd.AssignedByUser)
                .WithMany()
                .HasForeignKey(urd => urd.AssignedBy)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_user_role_detail_assigned_by");

            modelBuilder.Entity<UserRoleDetail>()
                .Property(urd => urd.AssignedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<UserRoleDetail>()
                .Property(urd => urd.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<UserRoleDetail>()
                .Property(urd => urd.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 10. Color
            modelBuilder.Entity<Color>()
                .ToTable("color")
                .HasKey(c => c.ColorId);

            modelBuilder.Entity<Color>()
                .HasIndex(c => c.Name)
                .HasDatabaseName("uk_color_name")
                .IsUnique();

            modelBuilder.Entity<Color>()
                .HasIndex(c => c.StatusId)
                .HasDatabaseName("fk_color_status_idx");

            modelBuilder.Entity<Color>()
                .HasOne(c => c.Status)
                .WithMany(s => s.Colors)
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_color_status");

            modelBuilder.Entity<Color>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Color>()
                .Property(c => c.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 11. Category
            modelBuilder.Entity<Category>()
                .ToTable("category")
                .HasKey(c => c.CategoryId);

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .HasDatabaseName("uk_category_slug")
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .HasDatabaseName("idx_category_name");

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Status)
                .WithMany(s => s.Categories)
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_category_status");

            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Category>()
                .Property(c => c.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 12. Material
            modelBuilder.Entity<Material>()
                .ToTable("material")
                .HasKey(m => m.MaterialId);

            modelBuilder.Entity<Material>()
                .HasIndex(m => m.Name)
                .HasDatabaseName("idx_material_name");

            modelBuilder.Entity<Material>()
                .HasOne(m => m.Status)
                .WithMany(s => s.Materials)
                .HasForeignKey(m => m.StatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_material_status");

            modelBuilder.Entity<Material>()
                .Property(m => m.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Material>()
                .Property(m => m.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 13. Ship
            modelBuilder.Entity<Ship>()
                .ToTable("ship")
                .HasKey(s => s.ShipId);

            modelBuilder.Entity<Ship>()
                .HasIndex(s => s.CorpName)
                .HasDatabaseName("idx_ship_corp_name");

            modelBuilder.Entity<Ship>()
                .HasOne(s => s.Status)
                .WithMany(st => st.Ships)
                .HasForeignKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_ship_status");

            modelBuilder.Entity<Ship>()
                .Property(s => s.BaseCost)
                .HasPrecision(10, 2); 

            modelBuilder.Entity<Ship>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Ship>()
                .Property(s => s.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 14. Discount
            modelBuilder.Entity<Discount>()
                .ToTable("discount")
                .HasKey(d => d.DiscountId);

            modelBuilder.Entity<Discount>()
                .HasIndex(d => d.Code)
                .HasDatabaseName("uk_discount_code")
                .IsUnique();

            modelBuilder.Entity<Discount>()
                .HasIndex(d => new { d.StartAt, d.EndAt })
                .HasDatabaseName("idx_discount_dates");

            modelBuilder.Entity<Discount>()
                .HasIndex(d => new { d.Code, d.StatusId })
                .HasDatabaseName("idx_discount_code_status");

            modelBuilder.Entity<Discount>()
                .HasOne(d => d.Status)
                .WithMany(s => s.Discounts)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_discount_status");

            modelBuilder.Entity<Discount>()
                .Property(d => d.DiscountValue)
                .HasPrecision(10, 2); 

            modelBuilder.Entity<Discount>()
                .Property(d => d.MinOrderAmount)
                .HasPrecision(12, 2)
                .HasDefaultValue(0.00m);

            modelBuilder.Entity<Discount>()
                .Property(d => d.MaxDiscountAmount)
                .HasPrecision(10, 2); 

            modelBuilder.Entity<Discount>()
                .Property(d => d.UsedCount)
                .HasDefaultValue(0);

            modelBuilder.Entity<Discount>()
                .Property(d => d.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Discount>()
                .Property(d => d.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 15. Product
            modelBuilder.Entity<Product>()
                .ToTable("product")
                .HasKey(p => p.ProductId);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .HasDatabaseName("uk_product_slug")
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("idx_product_name");

            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.BasePrice, p.SalePrice })
                .HasDatabaseName("idx_product_price");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CreatedAt)
                .HasDatabaseName("idx_product_created_at");

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Material)
                .WithMany(m => m.Products)
                .HasForeignKey(p => p.MaterialId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_product_material");

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_product_category");

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Status)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_product_status");

            modelBuilder.Entity<Product>()
                .Property(p => p.BasePrice)
                .HasPrecision(12, 2); 

            modelBuilder.Entity<Product>()
                .Property(p => p.SalePrice)
                .HasPrecision(12, 2); 

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountPercentage)
                .HasPrecision(5, 2);    

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Product>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 16. ProductVariant
            modelBuilder.Entity<ProductVariant>()
                .ToTable("product_variant")
                .HasKey(pv => pv.ProductVariantId);

            modelBuilder.Entity<ProductVariant>()
                .HasIndex(pv => pv.Sku)
                .HasDatabaseName("uk_product_variant_sku")
                .IsUnique();

            modelBuilder.Entity<ProductVariant>()
                .HasIndex(pv => pv.StockQuantity)
                .HasDatabaseName("idx_variant_stock");

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_product_variant_product");

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Color)
                .WithMany(c => c.ProductVariants)
                .HasForeignKey(pv => pv.ColorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_product_variant_color");

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Size)
                .WithMany(s => s.ProductVariants)
                .HasForeignKey(pv => pv.SizeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_product_variant_size");

            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.StockQuantity)
                .HasDefaultValue(0);

            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 17. ProductImage
            modelBuilder.Entity<ProductImage>()
                .ToTable("product_image")
                .HasKey(pi => pi.ProductImageId);

            modelBuilder.Entity<ProductImage>()
                .HasIndex(pi => new { pi.ProductId, pi.IsPrimary })
                .HasDatabaseName("idx_product_primary_image");

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_product_image_product");

            modelBuilder.Entity<ProductImage>()
                .Property(pi => pi.IsPrimary)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductImage>()
                .Property(pi => pi.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ProductImage>()
                .Property(pi => pi.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 18. Address
            modelBuilder.Entity<Address>()
                .ToTable("address")
                .HasKey(a => a.AddressId);

            modelBuilder.Entity<Address>()
                .HasIndex(a => new { a.UserId, a.IsDefault })
                .HasDatabaseName("idx_address_default");

            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_address_user");

            modelBuilder.Entity<Address>()
                .Property(a => a.IsDefault)
                .HasDefaultValue(false);

            modelBuilder.Entity<Address>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Address>()
                .Property(a => a.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 19. Cart
            modelBuilder.Entity<Cart>()
                .ToTable("cart")
                .HasKey(c => c.CartId);

            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .HasDatabaseName("uk_cart_user")
                .IsUnique();

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_cart_user");

            modelBuilder.Entity<Cart>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Cart>()
                .Property(c => c.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 20. CartItem
            modelBuilder.Entity<CartItem>()
                .ToTable("cart_item")
                .HasKey(ci => ci.CartItemId);

            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductVariantId })
                .HasDatabaseName("uk_cart_variant")
                .IsUnique();

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_cart_item_cart");

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.ProductVariant)
                .WithMany(pv => pv.CartItems)
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_cart_item_product_variant");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Price)
                .HasPrecision(12, 2);

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 21. PurchaseOrder
            modelBuilder.Entity<PurchaseOrder>()
                .ToTable("purchase_order")
                .HasKey(po => po.PurchaseOrderId);

            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(po => po.OrderDate)
                .HasDatabaseName("idx_po_order_date");

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_purchase_order_supplier");

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Status)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.StatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_purchase_order_status");

            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.TotalAmount)
                .HasPrecision(15, 2); 

            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.OrderDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 22. PurchaseOrderItem
            modelBuilder.Entity<PurchaseOrderItem>()
                .ToTable("purchase_order_item")
                .HasKey(poi => poi.PurchaseOrderItemId);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.PurchaseOrderItems)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_purchase_order_item_purchase_order");

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.ProductVariant)
                .WithMany(pv => pv.PurchaseOrderItems)
                .HasForeignKey(poi => poi.ProductVariantId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_purchase_order_item_product_variant");

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(poi => poi.UnitPrice)
                .HasPrecision(12, 2); 

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(poi => poi.TotalPrice)
                .HasPrecision(15, 2);

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(poi => poi.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(poi => poi.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 23. PaymentMethod
            modelBuilder.Entity<PaymentMethod>()
                .ToTable("Payment_Methods")
                .HasKey(pm => pm.PaymentMethodId);

            modelBuilder.Entity<PaymentMethod>()
                .HasOne(pm => pm.Status)
                .WithMany(s => s.PaymentMethods)
                .HasForeignKey(pm => pm.StatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payment_methods_status");

            modelBuilder.Entity<PaymentMethod>()
                .Property(pm => pm.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<PaymentMethod>()
                .Property(pm => pm.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 24. PaymentDestination
            modelBuilder.Entity<PaymentDestination>()
                .ToTable("Payment_Destinations")
                .HasKey(pd => pd.DestinationId);

            modelBuilder.Entity<PaymentDestination>()
                .HasOne(pd => pd.PaymentMethod)
                .WithMany(pm => pm.PaymentDestinations)
                .HasForeignKey(pd => pd.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payment_destinations_method");

            modelBuilder.Entity<PaymentDestination>()
                .HasOne(pd => pd.Status)
                .WithMany(s => s.PaymentDestinations)
                .HasForeignKey(pd => pd.StatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payment_destinations_status");

            modelBuilder.Entity<PaymentDestination>()
                .Property(pd => pd.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<PaymentDestination>()
                .Property(pd => pd.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 25. Payment
            modelBuilder.Entity<Payment>()
                .ToTable("Payments")
                .HasKey(p => p.PaymentId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentDestination)
                .WithMany(pd => pd.Payments)
                .HasForeignKey(p => p.DestinationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payments_dest");

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Status)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payments_status");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(15, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Payment>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 26. Order
            modelBuilder.Entity<Order>()
                .ToTable("order")
                .HasKey(o => o.OrderId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CreatedAt)
                .HasDatabaseName("idx_order_created_at");

            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.UserId, o.StatusId })
                .HasDatabaseName("idx_order_user_status");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_order_address");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_order_user");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Discount)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DiscountId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_order_discount");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Status)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_order_status");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Ship)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.ShipId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_order_ship");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PaymentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_order_payment");

            modelBuilder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasDefaultValue(0.00m);

            modelBuilder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasPrecision(12, 2) 
                .HasDefaultValue(0.00m);

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingFee)
                .HasPrecision(10, 2) 
                .HasDefaultValue(0.00m);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(15, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.IsFreeShip)
                .HasDefaultValue(false);

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingFee)
                .HasDefaultValue(0.00m);

            modelBuilder.Entity<Order>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Order>()
                .Property(o => o.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 27. OrderItem
            modelBuilder.Entity<OrderItem>()
                .ToTable("order_item")
                .HasKey(oi => oi.OrderItemId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_order_item_order");

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.ProductVariant)
                .WithMany(pv => pv.OrderItems)
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_order_item_product_variant");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(12, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 28. Review
            modelBuilder.Entity<Review>()
                .ToTable("review")
                .HasKey(r => r.ReviewId);

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.ProductId, r.Rating })
                .HasDatabaseName("idx_review_product_rating");

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.ProductId })
                .HasDatabaseName("idx_review_user_product");

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_review_user");

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_review_product");

            modelBuilder.Entity<Review>()
                .HasOne(r => r.OrderItem)
                .WithMany(oi => oi.Reviews)
                .HasForeignKey(r => r.OrderItemId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_review_order_item");

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Status)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.StatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_review_status");

            modelBuilder.Entity<Review>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Review>()
                .Property(r => r.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 29. ReviewImage
            modelBuilder.Entity<ReviewImage>()
                .ToTable("review_image")
                .HasKey(ri => ri.ReviewImageId);

            modelBuilder.Entity<ReviewImage>()
                .HasOne(ri => ri.Review)
                .WithMany(r => r.ReviewImages)
                .HasForeignKey(ri => ri.ReviewId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_review_image_review");

            modelBuilder.Entity<ReviewImage>()
                .Property(ri => ri.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ReviewImage>()
                .Property(ri => ri.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }