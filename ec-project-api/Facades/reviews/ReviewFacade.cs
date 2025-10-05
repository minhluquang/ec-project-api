using AutoMapper;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.reviews;
using ec_project_api.Interfaces.Reviews;

namespace ec_project_api.Facades.reviews {
    public class ReviewFacade {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public ReviewFacade(IReviewService reviewService, IMapper mapper) {
            _reviewService = reviewService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllByProductIdAsync(int productId) {
            var reviews = await _reviewService.GetAllByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
    }
}