using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.inventory;
using ec_project_api.Facades.inventory;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.inventory
{
    [ApiController]
    [Route(PathVariables.InventoryRoot)]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryFacade _inventoryFacade;

        public InventoryController(InventoryFacade inventoryFacade)
        {
            _inventoryFacade = inventoryFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<InventoryItemDto>>>> GetList()
        {
            try
            {
                var (items, total) = await _inventoryFacade.GetListAsync();
                Response.Headers["X-Total-Count"] = total.ToString();
                return Ok(ResponseData<IEnumerable<InventoryItemDto>>.Success(StatusCodes.Status200OK, items));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<InventoryItemDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet("{productVariantId:int}")]
        public async Task<ActionResult<ResponseData<InventoryItemDto>>> GetByVariantId(int productVariantId)
        {
            try
            {
                var dto = await _inventoryFacade.GetByVariantIdAsync(productVariantId);
                return Ok(ResponseData<InventoryItemDto>.Success(StatusCodes.Status200OK, dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<InventoryItemDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResponseData<InventoryItemDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<InventoryItemDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
        
    }
}
