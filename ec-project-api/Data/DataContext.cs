using ec_project_api.Models;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }  
        public DbSet<ReviewImage> ReviewImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships and constraints
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                 .HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasMany(r => r.ReviewImages)
                .WithOne(ri => ri.Review)
                .HasForeignKey(ri => ri.ReviewId)   
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Material>()
                .HasMany(m => m.Products)
                .WithOne(p => p.Material)
                .HasForeignKey(p => p.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Decimal precision ---
            modelBuilder.Entity<Product>()
                .Property(p => p.BasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.SalePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Review>()
                .Property(r => r.Rating)
                .HasPrecision(5, 2);

            // --- Snake_case conversion ---
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Table's Name
                entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                // Column's Name
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }

                // Primary Key's Name
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(ToSnakeCase(key.GetName()));
                }

                // Foreign Key's Name
                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()));
                }

                // Indexes' Name
                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
                }
            }
        }

        private static string ToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var sb = new System.Text.StringBuilder();
            sb.Append(char.ToLowerInvariant(name[0]));
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(name[i]));
                }
                else
                {
                    sb.Append(name[i]);
                }
            }
            return sb.ToString();
        }
    }
}

