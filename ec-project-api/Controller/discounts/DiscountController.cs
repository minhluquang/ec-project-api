using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.discounts;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Facades.discounts;
using ec_project_api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.DiscountRoot)]
    [ApiController]
    public class DiscountController : BaseController
    {
        private readonly DiscountFacade _discountFacade;

        public DiscountController(DiscountFacade discountFacade)
        {
            _discountFacade = discountFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<PagedResult<DiscountDetailDto>>>> GetAll([FromQuery] DiscountFilter filter)
        {
            return await ExecuteAsync(async () =>
            {
                var discounts = await _discountFacade.GetAllPagedAsync(filter);
                return ResponseData<PagedResult<DiscountDetailDto>>.Success(StatusCodes.Status200OK, discounts, DiscountMessages.DiscountRetrievedSuccessfully);
            });
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<DiscountDetailDto>>> GetById(int id)
        {
            try
            {
                var result = await _discountFacade.GetByIdAsync(id);
                return Ok(ResponseData<DiscountDetailDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<DiscountDetailDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<DiscountDetailDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] DiscountCreateRequest request)
        {
            try
            {
                var result = await _discountFacade.CreateAsync(request);
                if (result)
                {
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, DiscountMessages.SuccessfullyCreatedDiscount));
                }
                else
                {
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Failed to create discount."));
                }
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPatch(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(int id, [FromBody] DiscountUpdateRequest request)
        {
            try
            {
                await _discountFacade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, DiscountMessages.SuccessfullyUpdatedDiscount));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(int id)
        {
            try
            {
                await _discountFacade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, DiscountMessages.SuccessfullyDeletedDiscount));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}