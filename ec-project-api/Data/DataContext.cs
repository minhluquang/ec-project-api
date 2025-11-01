using ec_project_api.Models;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    // DbSets
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
    public DbSet<ReviewReport> ReviewReports { get; set; }
    public DbSet<ProductReturn> ProductReturns { get; set; }
    public DbSet<ProductGroup> ProductGroups { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<Ward> Wards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply common configurations
        ConfigureTimestamps(modelBuilder);
        
        // Configure entities
        ConfigureAddress(modelBuilder);
        ConfigureCart(modelBuilder);
        ConfigureCartItem(modelBuilder);
        ConfigureCategory(modelBuilder);
        ConfigureColor(modelBuilder);
        ConfigureDiscount(modelBuilder);
        ConfigureMaterial(modelBuilder);
        ConfigureOrder(modelBuilder);
        ConfigureOrderItem(modelBuilder);
        ConfigurePayment(modelBuilder);
        ConfigurePaymentDestination(modelBuilder);
        ConfigurePaymentMethod(modelBuilder);
        ConfigurePermission(modelBuilder);
        ConfigureProduct(modelBuilder);
        ConfigureProductGroup(modelBuilder);
        ConfigureProductImage(modelBuilder);
        ConfigureProductReturn(modelBuilder);
        ConfigureProductVariant(modelBuilder);
        ConfigurePurchaseOrder(modelBuilder);
        ConfigurePurchaseOrderItem(modelBuilder);
        ConfigureResource(modelBuilder);
        ConfigureReview(modelBuilder);
        ConfigureReviewImage(modelBuilder);
        ConfigureReviewReport(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureRolePermission(modelBuilder);
        ConfigureShip(modelBuilder);
        ConfigureSize(modelBuilder);
        ConfigureStatus(modelBuilder);
        ConfigureSupplier(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureUserRoleDetail(modelBuilder);
        ConfigureProvince(modelBuilder);
        ConfigureWard(modelBuilder);
    }

    // Helper method for common timestamp configuration
    private void ConfigureTimestamps(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var createdAtProperty = entityType.FindProperty("CreatedAt");
            if (createdAtProperty != null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property("CreatedAt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            }

            var updatedAtProperty = entityType.FindProperty("UpdatedAt");
            if (updatedAtProperty != null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property("UpdatedAt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();
            }
        }
    }

    private void ConfigureAddress(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasIndex(a => new { a.UserId, a.IsDefault })
                .HasDatabaseName("IX_Address_User_Default");

            entity.HasOne(a => a.Province)
                .WithMany(p => p.Addresses)
                .HasForeignKey(a => a.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Ward)
                .WithMany(w => w.Addresses)
                .HasForeignKey(a => a.WardId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureCart(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureCartItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasIndex(ci => new { ci.CartId, ci.ProductVariantId })
                .IsUnique()
                .HasDatabaseName("UK_CartItem_Cart_ProductVariant");

            entity.Property(ci => ci.Price)
                .HasPrecision(18, 2);

            entity.HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ci => ci.ProductVariant)
                .WithMany(pv => pv.CartItems)
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Name)
                .IsUnique()
                .HasDatabaseName("UK_Category_Name");

            entity.HasIndex(c => c.Slug)
                .IsUnique()
                .HasDatabaseName("UK_Category_Slug");

            entity.HasOne(c => c.Status)
                .WithMany(s => s.Categories)
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureColor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();
        });
    }

    private void ConfigureDiscount(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasIndex(d => d.Code).IsUnique();

            entity.HasMany(d => d.Orders)
                .WithOne(o => o.Discount)
                .HasForeignKey(o => o.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureMaterial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasIndex(m => m.Name).IsUnique();
        });
    }

    private void ConfigureOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(o => o.Status)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Order>(o => o.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Discount)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(o => o.Ship)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.ShipId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureOrderItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oi => oi.ProductVariant)
                .WithMany(pv => pv.OrderItems)
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigurePayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasOne(p => p.PaymentDestination)
                .WithMany(d => d.Payments)
                .HasForeignKey(p => p.DestinationId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.Status)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigurePaymentDestination(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentDestination>(entity =>
        {
            entity.HasOne(d => d.PaymentMethod)
                .WithMany(m => m.PaymentDestinations)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Status)
                .WithMany(s => s.PaymentDestinations)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigurePaymentMethod(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasIndex(m => m.MethodName).IsUnique();
            
            entity.Property(m => m.MethodType)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.HasOne(m => m.Status)
                .WithMany(s => s.PaymentMethods)
                .HasForeignKey(m => m.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigurePermission(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(p => p.PermissionName).IsUnique();

            entity.HasOne(p => p.Resource)
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureProduct(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Name);
            entity.HasIndex(p => p.Slug).IsUnique();

            entity.HasOne(p => p.Material)
                .WithMany(m => m.Products)
                .HasForeignKey(p => p.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Color)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.ColorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Status)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.ProductGroup)
                .WithMany(pg => pg.Products)
                .HasForeignKey(p => p.ProductGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureProductGroup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductGroup>(entity =>
        {
            entity.HasIndex(pg => pg.Name)
                .IsUnique()
                .HasDatabaseName("IX_ProductGroup_Name");

            entity.HasOne(pg => pg.Status)
                .WithMany(s => s.ProductGroups)
                .HasForeignKey(pg => pg.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureProductImage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasOne(i => i.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureProductReturn(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductReturn>(entity =>
        {
            entity.HasOne(r => r.OrderItem)
                .WithMany(oi => oi.ProductReturns)
                .HasForeignKey(r => r.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.ReturnProductVariant)
                .WithMany(pv => pv.ProductReturns)
                .HasForeignKey(r => r.ReturnProductVariantId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(r => r.Status)
                .WithMany(s => s.ProductReturns)
                .HasForeignKey(r => r.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureProductVariant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasIndex(v => v.Sku).IsUnique();

            entity.HasOne(v => v.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(v => v.Size)
                .WithMany(s => s.ProductVariants)
                .HasForeignKey(v => v.SizeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Status)
                .WithMany(s => s.ProductVariants)
                .HasForeignKey(v => v.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigurePurchaseOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasOne(o => o.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(o => o.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Status)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(o => o.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigurePurchaseOrderItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.Property(i => i.ProfitPercentage)
                .HasPrecision(18, 4);

            entity.HasOne(i => i.PurchaseOrder)
                .WithMany(o => o.PurchaseOrderItems)
                .HasForeignKey(i => i.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.ProductVariant)
                .WithMany(v => v.PurchaseOrderItems)
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureResource(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();
        });
    }

    private void ConfigureReview(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasOne(r => r.OrderItem)
                .WithMany(oi => oi.Reviews)
                .HasForeignKey(r => r.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Status)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureReviewImage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.HasOne(i => i.Review)
                .WithMany(r => r.ReviewImages)
                .HasForeignKey(i => i.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureReviewReport(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReviewReport>(entity =>
        {
            entity.HasOne(rr => rr.Review)
                .WithMany()
                .HasForeignKey(rr => rr.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rr => rr.User)
                .WithMany()
                .HasForeignKey(rr => rr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(rr => rr.Status)
                .WithMany()
                .HasForeignKey(rr => rr.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();

            entity.HasOne(r => r.Status)
                .WithMany(s => s.Roles)
                .HasForeignKey(r => r.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureRolePermission(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            entity.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureShip(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ship>(entity =>
        {
            entity.HasIndex(s => s.CorpName).IsUnique();

            entity.HasOne(s => s.Status)
                .WithMany(st => st.Ships)
                .HasForeignKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureSize(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasIndex(s => s.Name).IsUnique();

            entity.HasOne(s => s.Status)
                .WithMany(st => st.Sizes)
                .HasForeignKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureStatus(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasIndex(s => new { s.Name, s.EntityType }).IsUnique();
        });
    }

    private void ConfigureSupplier(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasIndex(s => s.Name).IsUnique();
            entity.HasIndex(s => s.Email).IsUnique();
            entity.HasIndex(s => s.Phone).IsUnique();

            entity.HasOne(s => s.Status)
                .WithMany(st => st.Suppliers)
                .HasForeignKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Phone).IsUnique();

            entity.HasOne(u => u.Status)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureUserRoleDetail(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRoleDetail>(entity =>
        {
            entity.HasKey(urd => new { urd.UserId, urd.RoleId });

            entity.HasOne(urd => urd.User)
                .WithMany(u => u.UserRoleDetails)
                .HasForeignKey(urd => urd.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(urd => urd.Role)
                .WithMany(r => r.UserRoleDetails)
                .HasForeignKey(urd => urd.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(urd => urd.AssignedByUser)
                .WithMany()
                .HasForeignKey(urd => urd.AssignedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureProvince(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasIndex(p => p.Code)
                .IsUnique()
                .HasDatabaseName("UK_Province_Code");

            entity.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Province_Name");
        });
    }

    private void ConfigureWard(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasIndex(w => w.Code)
                .IsUnique()
                .HasDatabaseName("UK_Ward_Code");

            entity.HasIndex(w => new { w.ProvinceId, w.Name })
                .HasDatabaseName("IX_Ward_Province_Name");

            entity.HasOne(w => w.Province)
                .WithMany(p => p.Wards)
                .HasForeignKey(w => w.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}