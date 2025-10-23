using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.addresses;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.Users;
using ec_project_api.Facades.addresses;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.addresses;

[Route(PathVariables.AddressRoot)]
[ApiController]
public class AddressController : BaseController
{
    private readonly AddressFacade _addressFacade;
    
    public AddressController(AddressFacade addressFacade)
    {
        _addressFacade = addressFacade;
    }
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<ResponseData<IEnumerable<AddressDto>>>> GetByUserId(int userId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _addressFacade.GetByUserIdAsync(userId);
            return ResponseData<IEnumerable<AddressDto>>.Success(StatusCodes.Status200OK, result,
                AddressMessages.GetByUserSuccess);
        });
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult<ResponseData<bool>>> Create(int userId, [FromBody] AddressCreateRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _addressFacade.CreateAsync(userId, request);
            return ResponseData<bool>.Success(StatusCodes.Status201Created, result,
                AddressMessages.CreateSuccess);
        });
    }
    
    [HttpPatch("{userId}/address/{addressId}")]
    public async Task<ActionResult<ResponseData<bool>>> Update(int userId, int addressId, [FromBody] AddressUpdateRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _addressFacade.UpdateAsync(userId, addressId, request);
            return ResponseData<bool>.Success(StatusCodes.Status200OK, result,
                AddressMessages.UpdateSuccess);
        });
    }

    [HttpDelete("{userId}/address/{addressId}")]
    public async Task<ActionResult<ResponseData<bool>>> Delete(int userId, int addressId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _addressFacade.DeleteAsync(userId, addressId);
            return ResponseData<bool>.Success(StatusCodes.Status200OK, result,
                AddressMessages.DeleteSuccess);
        });
    }

    [HttpPatch("{userId}/address/{addressId}/set-default")]
    public async Task<ActionResult<ResponseData<bool>>> SetDefault(int userId, int addressId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _addressFacade.SetDefaultAsync(userId, addressId);
            return ResponseData<bool>.Success(StatusCodes.Status200OK, result,
                AddressMessages.SetDefaultSuccess);
        });
    }
}
