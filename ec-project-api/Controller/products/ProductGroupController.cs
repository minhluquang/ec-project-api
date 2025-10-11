using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.productGroups;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.productGroups;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ec_project_api.Controller.productGroups
{
    [Route(PathVariables.ProductGroupRoot)]
    [ApiController]
    public class ProductGroupController : ControllerBase
    {
        private readonly ProductGroupFacade _productGroupFacade;

        public ProductGroupController(ProductGroupFacade productGroupFacade)
        {
            _productGroupFacade = productGroupFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ProductGroupDto>>>> GetAll()
        {
            var result = await _productGroupFacade.GetAllAsync();
            return Ok(ResponseData<IEnumerable<ProductGroupDto>>.Success(StatusCodes.Status200OK, result));
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] ProductGroupCreateRequest request)
        {
            try
            {
                var result = await _productGroupFacade.CreateAsync(request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, result, ProductGroupMessages.SuccessfullyCreatedProductGroup));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
        }

        [HttpPatch(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(int id, [FromBody] ProductGroupUpdateRequest request)
        {
            try
            {
                var result = await _productGroupFacade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, ProductGroupMessages.SuccessfullyUpdatedProductGroup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
        }

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(int id)
        {
            try
            {
                var result = await _productGroupFacade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, ProductGroupMessages.SuccessfullyDeletedProductGroup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
        }
    }
}