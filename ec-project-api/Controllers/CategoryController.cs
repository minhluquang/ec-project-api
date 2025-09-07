using Microsoft.AspNetCore.Mvc;
using ec_project_api.Interfaces;
using ec_project_api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ec_project_api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/category")]
    [ApiVersion("1.0")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]

        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            return Ok(categories);
        }
    }
}
