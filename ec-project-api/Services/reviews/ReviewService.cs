using CloudinaryDotNet.Actions;
using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.review_images;
using Microsoft.EntityFrameworkCore;
using ec_project_api.Dtos.response.reviews;
using ec_project_api.Interfaces.Orders;

namespace ec_project_api.Services.reviews {
    public class ReviewService : BaseService<Review, int>, IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewImageService _reviewImageService;

        public ReviewService(IReviewRepository reviewRepository, IReviewImageService reviewImageService,
            DataContext dbContext) : base(reviewRepository)
        {
            _reviewRepository = reviewRepository;
            _reviewImageService = reviewImageService;
        }

        public async Task<IEnumerable<Review>> GetAllByProductIdAsync(int productId,
            QueryOptions<Review>? options = null)
        {
            options ??= new QueryOptions<Review>();

            options.IncludeThen.Add(q => q
                .Include(p => p.OrderItem!)
                .ThenInclude(v => v!.ProductVariant!)
                .ThenInclude(pv => pv.Size));

            options.IncludeThen.Add(q => q
                .Include(r => r.OrderItem)
                .ThenInclude(oi => oi!.ProductVariant)
                .ThenInclude(pv => pv!.Product)
                .ThenInclude(p => p!.Color));

            options.Filter = r => r.OrderItem != null && r.OrderItem.ProductVariant != null &&
                                  r.OrderItem.ProductVariant.ProductId == productId;
            options.Includes.Add(r => r.ReviewImages);

            return await _reviewRepository.GetAllAsync(options);
        }

        public async Task<bool> CreateReviewAndUploadReviewImagesAsync(Review review, List<IFormFile>? images)
        {
            // Validate images trước khi làm bất cứ điều gì
            if (images != null && images.Count > 0)
            {
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                const int maxImageCount = 5;
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var errors = new List<string>();

                // Kiểm tra số lượng
                if (images.Count > maxImageCount)
                    throw new InvalidOperationException($"Chỉ được upload tối đa {maxImageCount} ảnh");

                // Kiểm tra từng ảnh
                foreach (var image in images)
                {
                    var fileName = image.FileName;

                    // Kiểm tra kích thước
                    if (image.Length > maxFileSize)
                        errors.Add($"'{fileName}' vượt quá 5MB");

                    // Kiểm tra định dạng
                    var ext = Path.GetExtension(fileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(ext))
                        errors.Add($"'{fileName}' không đúng định dạng (chỉ chấp nhận JPG, PNG, WEBP)");
                }

                // Nếu có lỗi thì reject tất cả
                if (errors.Count > 0)
                    throw new InvalidOperationException($"Lỗi upload ảnh: {string.Join("; ", errors)}");
            }
            
            await base.CreateAsync(review);

            if (images != null)
            {
                foreach (var image in images)
                {
                    var reviewImage = new ReviewImage
                    {
                        ReviewId = review.ReviewId,
                    };
                    await _reviewImageService.UploadSingleReviewImageAsync(reviewImage, image);
                }
            }

            return true;
        }
        
        public async Task<bool> UpdateReviewAndUploadReviewImagesAsync(Review review, List<int> keepImageIds, List<IFormFile>? images)
        {
            if (images != null && images.Count > 0)
            {
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                var errors = new List<string>();

                foreach (var image in images)
                {
                    if (image.Length > maxFileSize)
                        errors.Add($"'{image.FileName}' vượt quá 5MB");
                }

                if (errors.Count > 0)
                    throw new InvalidOperationException($"Lỗi upload ảnh: {string.Join("; ", errors)}");
            }
            
            // Update review info (rating, comment)
            await base.UpdateAsync(review);
            
            var keepSet = new HashSet<int>(keepImageIds ?? Enumerable.Empty<int>());

            // Get all images
            var imageOptions = new QueryOptions<ReviewImage>
            {
                Filter = ri => ri.ReviewId == review.ReviewId
            };
            var oldImages = (await _reviewImageService.GetAllAsync(imageOptions)) ?? Enumerable.Empty<ReviewImage>();
                    
            // If you want to delete images not in keepImageIds:
            var imagesToDelete = oldImages
                .Where(img => !keepSet.Contains(img.ReviewImageId))
                .ToList();
            
            // Delete image
            foreach (var image in imagesToDelete)
            {
                await _reviewImageService.DeleteSingleReviewImageAsync(image);
            }
            
            // Upload new images
            if (images != null)
            {
                foreach (var image in images)
                {
                    var reviewImage = new ReviewImage
                    {
                        ReviewId = review.ReviewId
                    };

                    await _reviewImageService.UploadSingleReviewImageAsync(reviewImage, image);
                }
            }

            return true;
        }

        public override async Task<Review?> GetByIdAsync(int reviewId, QueryOptions<Review>? options = null)
        {
            options ??= new QueryOptions<Review>();

            options.IncludeThen.Add(q => q
                .Include(p => p.OrderItem)
                .ThenInclude(v => v!.ProductVariant!)
                .ThenInclude(pv => pv!.Size!));

            options.IncludeThen.Add(q => q
                .Include(p => p.OrderItem!)
                .ThenInclude(v => v!.ProductVariant!)
                .ThenInclude(pv => pv!.Product!)
                .ThenInclude(p => p!.Color!));

            options.IncludeThen.Add(q => q
                .Include(p => p.OrderItem)
                .ThenInclude(oi => oi!.Order));

            options.Filter = r => r.ReviewId == reviewId;
            options.Includes.Add(r => r.ReviewImages);

            return await _reviewRepository.GetByIdAsync(reviewId, options);
        }

        public async Task<ReviewSummaryDto> GetSummaryByProductIdAsync(int productId)
        {
            var options = new QueryOptions<Review>
            {
                Filter = r =>
                    r.Status.Name == StatusVariables.Approved &&
                    r.OrderItem != null &&
                    r.OrderItem.ProductVariant != null &&
                    r.OrderItem.ProductVariant.ProductId == productId &&
                    r.OrderItem.Order != null &&
                    r.OrderItem.Order.Status != null &&
                    r.OrderItem.Order.Status.Name == StatusVariables.Delivered
            };

            options.IncludeThen.Add(q => q
                .Include(r => r.OrderItem!)
                .ThenInclude(oi => oi.Order!));
            options.IncludeThen.Add(q => q
                .Include(r => r.OrderItem)
                .ThenInclude(oi => oi.ProductVariant));

            var reviews = await _reviewRepository.GetAllAsync(options);
            if (!reviews.Any())
                return new ReviewSummaryDto { AverageRating = 0, ReviewCount = 0 };

            var reviewCount = reviews.Count();
            var averageRating = reviewCount > 0 ? reviews.Average(r => r.Rating) : 0.0;

            var groups = reviews.GroupBy(r => r.Rating).ToDictionary(g => g.Key, g => g.Count());
            var details = new Dictionary<int, int>();
            
            for (byte i = 1; i <= 5; i++)
            {
                details[i] = groups.ContainsKey(i) ? groups[i] : 0;
            }
            
            var hasImageCount = reviews.Count(r => r.Status.Name == StatusVariables.Approved && r.ReviewImages != null && r.ReviewImages.Any());
                
            return new ReviewSummaryDto
            {
                AverageRating = averageRating,
                ReviewCount = reviewCount,
                ReviewDetails = details,
                HasImageCount = hasImageCount
            };
        }
    }
}