using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ec_project_api.Constants.messages;
using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;
using ec_project_api.Services.Bases;
using ec_project_api.Services.review_images;

namespace ec_project_api.Services {
    public class ReviewImageService : BaseService<ReviewImage, int>, IReviewImageService {
        private readonly IReviewImageRepository _reviewImageRepository;
        private readonly Cloudinary _cloudinary;
        private readonly string _uploadPresent = "unsigned_preset";
        public ReviewImageService(IReviewImageRepository reviewImageRepository, IConfiguration configuration) : base(reviewImageRepository) {
            _reviewImageRepository = reviewImageRepository;
            var account = new Account(
                configuration["Cloudinary:MY_CLOUD_NAME"],
                configuration["Cloudinary:MY_API_KEY"],
                configuration["Cloudinary:MY_API_SECRET"]
            );
            _uploadPresent = configuration["Cloudinary:UPLOAD_PRESET"] ?? "unsigned_preset";
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        public async Task<bool> UploadSingleReviewImageAsync(ReviewImage reviewImage, IFormFile fileImage) {
            await CreateAsync(reviewImage);

            string publicId = $"review_{reviewImage.ReviewId}_{reviewImage.ReviewImageId}";

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileImage.FileName, fileImage.OpenReadStream()),
                UseFilename = true,
                UniqueFilename = false,
                QualityAnalysis = false,
                Folder = "reviews",
                UploadPreset = _uploadPresent,
                PublicId = publicId,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult?.SecureUrl == null)
                throw new Exception(ReviewMessages.ReviewImageUploadFailed);

            reviewImage.ImageUrl = uploadResult.SecureUrl.ToString();
            reviewImage.UpdatedAt = DateTime.UtcNow;
            return await UpdateAsync(reviewImage);
        }

        public async Task<bool> DeleteSingleReviewImageAsync(ReviewImage reviewImage) {
            if (reviewImage == null)
                throw new Exception(ReviewMessages.ReviewImageNotFound);

            string? imageUrl = reviewImage.ImageUrl;
            if (string.IsNullOrEmpty(imageUrl))
                throw new Exception(ReviewMessages.ReviewImageNotFound);

            int lastImage = imageUrl.LastIndexOf("reviews_");
            int lastDot = imageUrl.LastIndexOf('.');
            string publicId = imageUrl.Substring(lastImage, lastDot - lastImage);

            // Delete image from Cloudinary
            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            // Check if deleteion was successful
            if (deleteResult.Result != "ok")
                throw new Exception(ReviewMessages.ReviewImageDeleteCloudinaryFailed);

            // Delete image record from database
            return await DeleteAsync(reviewImage);
        }
    }
}
