using Microsoft.AspNetCore.Mvc;
using ec_project_api.Interfaces;
using ec_project_api.Models;
using ec_project_api.DTO;
using AutoMapper;

namespace ec_project_api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/category")]
    [ApiVersion("1.0")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]

        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDTO>>(_categoryRepository.GetCategories());

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            return Ok(categories);
        }
    }
}
