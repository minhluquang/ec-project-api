using ec_project_api.Models;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext {
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
    public DbSet<ReviewReport> ReviewReports { get; set; }
    public DbSet<ProductReturn> ProductReturns { get; set; }
    public DbSet<ProductGroup> ProductGroups { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasIndex(a => new { a.UserId, a.IsDefault })
                  .HasDatabaseName("IX_Address_User_Default");

            entity.Property(a => a.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(a => a.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasOne(c => c.User)
                  .WithMany(u => u.Carts)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasIndex(ci => new { ci.CartId, ci.ProductVariantId })
                  .IsUnique()
                  .HasDatabaseName("UK_CartItem_Cart_ProductVariant");

            entity.HasOne(ci => ci.Cart)
                  .WithMany(c => c.CartItems)
                  .HasForeignKey(ci => ci.CartId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ci => ci.ProductVariant)
                  .WithMany()
                  .HasForeignKey(ci => ci.ProductVariantId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Name)
                  .IsUnique()
                  .HasDatabaseName("UK_Category_Name");

            entity.HasIndex(c => c.Slug)
                  .IsUnique()
                  .HasDatabaseName("UK_Category_Slug");

            entity.Property(c => c.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(c => c.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(c => c.Status)
                  .WithMany()
                  .HasForeignKey(c => c.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();
            entity.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(c => c.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasIndex(d => d.Code).IsUnique();

            entity.Property(d => d.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(d => d.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasMany(d => d.Orders)
                  .WithOne(o => o.Discount)
                  .HasForeignKey(o => o.DiscountId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasIndex(d => d.Name).IsUnique();

            entity.Property(m => m.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(m => m.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(o => o.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(o => o.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(o => o.Status)
                  .WithMany()
                  .HasForeignKey(o => o.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Payment)
                  .WithMany()
                  .HasForeignKey(o => o.PaymentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(oi => oi.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(oi => oi.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(oi => oi.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(oi => oi.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oi => oi.ProductVariant)
                  .WithMany(pv => pv.OrderItems)
                  .HasForeignKey(oi => oi.ProductVariantId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(p => p.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(p => p.PaymentDestination)
                  .WithMany(d => d.Payments)
                  .HasForeignKey(p => p.DestinationId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.Status)
                  .WithMany()
                  .HasForeignKey(p => p.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PaymentDestination>(entity =>
        {
            entity.Property(d => d.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(d => d.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(d => d.PaymentMethod)
                  .WithMany(m => m.PaymentDestinations)
                  .HasForeignKey(d => d.PaymentMethodId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Status)
                  .WithMany()
                  .HasForeignKey(d => d.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasIndex(m => m.MethodName).IsUnique();

            entity.Property(m => m.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(m => m.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(m => m.Status)
                  .WithMany()
                  .HasForeignKey(m => m.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(p => p.PermissionName).IsUnique();

            entity.HasOne(p => p.Resource)
                  .WithMany(r => r.Permissions)
                  .HasForeignKey(p => p.ResourceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Name);
            entity.HasIndex(p => p.Slug).IsUnique();

            entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(p => p.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

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
                  .WithMany()
                  .HasForeignKey(p => p.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.ProductGroup)
                  .WithMany(pg => pg.Products)
                  .HasForeignKey(p => p.ProductGroupId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductGroup>(entity =>
        {
            entity.HasIndex(pg => pg.Name)
                  .IsUnique()
                  .HasDatabaseName("IX_ProductGroup_Name");

            entity.HasOne(pg => pg.Status)
                  .WithMany()
                  .HasForeignKey(pg => pg.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);


            entity.Property(pg => pg.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(pg => pg.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.Property(i => i.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(i => i.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(i => i.Product)
                  .WithMany(p => p.ProductImages)
                  .HasForeignKey(i => i.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductReturn>(entity =>
        {
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(r => r.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(r => r.OrderItem)
                  .WithMany(oi => oi.ProductReturns)
                  .HasForeignKey(r => r.OrderItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.ReturnProductVariant)
                  .WithMany(pv => pv.ProductReturns)
                  .HasForeignKey(r => r.ReturnProductVariantId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(r => r.Status)
                  .WithMany()
                  .HasForeignKey(r => r.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasIndex(v => v.Sku).IsUnique();

            entity.Property(v => v.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(v => v.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

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

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.Property(o => o.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(o => o.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(o => o.Supplier)
                  .WithMany(s => s.PurchaseOrders)
                  .HasForeignKey(o => o.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Status)
                  .WithMany()
                  .HasForeignKey(o => o.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.Property(i => i.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(i => i.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(i => i.PurchaseOrder)
                  .WithMany(o => o.PurchaseOrderItems)
                  .HasForeignKey(i => i.PurchaseOrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.ProductVariant)
                  .WithMany(v => v.PurchaseOrderItems)
                  .HasForeignKey(i => i.ProductVariantId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(r => r.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(r => r.OrderItem)
                  .WithMany(oi => oi.Reviews)
                  .HasForeignKey(r => r.OrderItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Status)
                  .WithMany()
                  .HasForeignKey(r => r.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.Property(i => i.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(i => i.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(i => i.Review)
                  .WithMany(r => r.ReviewImages)
                  .HasForeignKey(i => i.ReviewId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ReviewReport configuration
        modelBuilder.Entity<ReviewReport>(entity =>
        {
            entity.Property(rr => rr.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(rr => rr.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();

            entity.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(r => r.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(r => r.Status)
                  .WithMany(s => s.Roles)
                  .HasForeignKey(r => r.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            entity.Property(rp => rp.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(rp => rp.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(rp => rp.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(rp => rp.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ship>(entity =>
        {
            entity.HasIndex(s => s.CorpName).IsUnique();

            entity.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(s => s.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(s => s.Status)
                  .WithMany(st => st.Ships)
                  .HasForeignKey(s => s.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasIndex(s => s.Name).IsUnique();

            entity.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(s => s.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(s => s.Status)
                  .WithMany(st => st.Sizes)
                  .HasForeignKey(s => s.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasIndex(s => new { s.Name, s.EntityType }).IsUnique();
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasIndex(s => s.Name).IsUnique();
            entity.HasIndex(s => s.Email).IsUnique();
            entity.HasIndex(s => s.Phone).IsUnique();

            entity.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(s => s.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(s => s.Status)
                  .WithMany(st => st.Suppliers)
                  .HasForeignKey(s => s.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Phone).IsUnique();

            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(u => u.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(u => u.Status)
                  .WithMany(s => s.Users)
                  .HasForeignKey(u => u.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserRoleDetail>(entity =>
        {
            entity.HasKey(urd => new { urd.UserId, urd.RoleId });

            entity.Property(urd => urd.AssignedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

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

        modelBuilder.Entity<CartItem>()
        .Property(c => c.Price)
        .HasPrecision(18, 2);

        modelBuilder.Entity<CartItem>()
            .HasOne(c => c.ProductVariant)
            .WithMany(pv => pv.CartItems)
            .HasForeignKey(c => c.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CartItem>()
            .HasOne(c => c.Cart)
            .WithMany(ca => ca.CartItems)
            .HasForeignKey(c => c.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductVariant>()
            .HasOne(pv => pv.Product)
            .WithMany(p => p.ProductVariants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductVariant>()
            .HasOne(pv => pv.Size)
            .WithMany(s => s.ProductVariants)
            .HasForeignKey(pv => pv.SizeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.Status)
            .WithMany(s => s.Categories)
            .HasForeignKey(c => c.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Status>()
            .HasMany(s => s.Products)
            .WithOne(p => p.Status)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Status>()
           .HasMany(s => s.ProductGroups)
           .WithOne(p => p.Status)
           .HasForeignKey(p => p.StatusId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Status>()
            .HasMany(s => s.ProductVariants)
            .WithOne(p => p.Status)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Status>()
            .HasMany(s => s.Orders)
            .WithOne(o => o.Status)
            .HasForeignKey(o => o.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Status>()
            .HasMany(s => s.Payments)
            .WithOne(p => p.Status)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Status)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Payment)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Discount)
            .WithMany(d => d.Orders)
            .HasForeignKey(o => o.DiscountId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Ship)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.ShipId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Permission>()
            .HasOne(p => p.Resource)
            .WithMany(r => r.Permissions)
            .HasForeignKey(p => p.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Status)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.PaymentDestination)
            .WithMany(d => d.Payments)
            .HasForeignKey(p => p.DestinationId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PaymentDestination>()
            .HasOne(d => d.Status)
            .WithMany(s => s.PaymentDestinations)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PaymentDestination>()
            .HasOne(d => d.PaymentMethod)
            .WithMany(m => m.PaymentDestinations)
            .HasForeignKey(d => d.PaymentMethodId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PaymentMethod>()
            .HasOne(m => m.Status)
            .WithMany(s => s.PaymentMethods)
            .HasForeignKey(m => m.StatusId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Review>()
            .HasOne(r => r.OrderItem)
            .WithMany(oi => oi.Reviews)
            .HasForeignKey(r => r.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Status)
            .WithMany(s => s.Reviews)
            .HasForeignKey(r => r.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(po => po.Supplier)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(po => po.Status)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(po => po.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductReturn>()
            .HasOne(pr => pr.OrderItem)
            .WithMany(oi => oi.ProductReturns)
            .HasForeignKey(pr => pr.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductReturn>()
            .HasOne(pr => pr.ReturnProductVariant)
            .WithMany(pv => pv.ProductReturns)
            .HasForeignKey(pr => pr.ReturnProductVariantId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ProductReturn>()
            .HasOne(pr => pr.Status)
            .WithMany(s => s.ProductReturns)
            .HasForeignKey(pr => pr.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Material)
            .WithMany(m => m.Products)
            .HasForeignKey(p => p.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Color)
            .WithMany(m => m.Products)
            .HasForeignKey(p => p.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Status)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductGroup)
            .WithMany(pg => pg.Products)
            .HasForeignKey(p => p.ProductGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
