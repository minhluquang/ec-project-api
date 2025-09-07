using ec_project_api.Data;
using ec_project_api.Models;

namespace ec_project_api
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;
        }

        public void SeedDataContext()
        {
            if (!_context.Products.Any())
            {
                var products = new List<Product>()
                {
                    new Product
                    {
                        Name = "Áo Thun Cổ Điển",
                        Slug = "classic-tshirt",
                        BasePrice = 15,
                        SalePrice = 12,
                        DiscountPercentage = 20,
                        Category = new Category { Name = "Nam", Slug = "men", Description = "Quần áo nam" },
                        Material = new Material { Name = "Cotton", Description = "Chất liệu cotton mềm mại" },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        ProductImages = new List<ProductImage>
                        {
                            new ProductImage
                            {
                                ImageUrl = "https://picsum.photos/seed/1/300/400",
                                AltText = "Áo Thun Cổ Điển",
                                IsPrimary = true,
                                DisplayOrder = 1,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            },
                            new ProductImage
                            {
                                ImageUrl = "https://picsum.photos/seed/2/300/400",
                                AltText = "Áo Thun Cổ Điển 2",
                                IsPrimary = false,
                                DisplayOrder = 2,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            }
                        },
                        Reviews = new List<Review>
                        {
                            new Review
                            {
                                Content = "Chất lượng tốt và rất thoải mái!",
                                Rating = 4.5m,
                                HelpfulCount = 5,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            },
                            new Review
                            {
                                Content = "Vừa vặn, chất liệu đẹp.",
                                Rating = 4.0m,
                                HelpfulCount = 3,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            }
                        }
                    },
                    new Product
                    {
                        Name = "Quần Jeans Denim",
                        Slug = "denim-jeans",
                        BasePrice = 40,
                        SalePrice = 35,
                        DiscountPercentage = 12,
                        Category = new Category { Name = "Nam", Slug = "men", Description = "Quần áo nam" },
                        Material = new Material { Name = "Cotton", Description = "Chất liệu cotton mềm mại" },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        ProductImages = new List<ProductImage>
                        {
                            new ProductImage
                            {
                                ImageUrl = "https://picsum.photos/seed/3/300/400",
                                AltText = "Quần Jeans Denim",
                                IsPrimary = true,
                                DisplayOrder = 1,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            },
                            new ProductImage
                            {
                                ImageUrl = "https://picsum.photos/seed/4/300/400",
                                AltText = "Quần Jeans Denim 2",
                                IsPrimary = false,
                                DisplayOrder = 2,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            }
                        },
                        Reviews = new List<Review>
                        {
                            new Review
                            {
                                Content = "Chất liệu bền, mặc rất thoải mái!",
                                Rating = 4.8m,
                                HelpfulCount = 6,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            },
                            new Review
                            {
                                Content = "Kiểu dáng đẹp, vừa vặn.",
                                Rating = 4.2m,
                                HelpfulCount = 4,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            }
                        }
                    }
                };

                _context.Products.AddRange(products);
                _context.SaveChanges();
            }
        }
    }
}
