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
    
    [HttpGet("me")]
    public async Task<ActionResult<ResponseData<IEnumerable<AddressDto>>>> GetMyAddresses()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _addressFacade.GetMyAddressesAsync(User);
            return ResponseData<IEnumerable<AddressDto>>.Success(StatusCodes.Status200OK, result,
                AddressMessages.GetByUserSuccess);
        });
    }

    [HttpPost("me")]
    public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] AddressCreateRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var currentUser = User; 
            var result = await _addressFacade.CreateAsync(currentUser, request);
            return ResponseData<bool>.Success(StatusCodes.Status201Created, result,
                AddressMessages.CreateSuccess);
        });
    }
    
    [HttpPatch("me/address/{addressId}")]
    public async Task<ActionResult<ResponseData<bool>>> Update(int addressId, [FromBody] AddressUpdateRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            var currentUser = User; 
            var result = await _addressFacade.UpdateAsync(currentUser, addressId, request);
            return ResponseData<bool>.Success(StatusCodes.Status200OK, result,
                AddressMessages.UpdateSuccess);
        });
    }

    [HttpDelete("me/address/{addressId}")]
    public async Task<ActionResult<ResponseData<bool>>> Delete(int addressId)
    {
        return await ExecuteAsync(async () =>
        {
            var currentUser = User; 
            var result = await _addressFacade.DeleteAsync(currentUser, addressId);
            return ResponseData<bool>.Success(StatusCodes.Status200OK, result,
                AddressMessages.DeleteSuccess);
        });
    }

    [HttpPatch("me/address/{addressId}/set-default")]
    public async Task<ActionResult<ResponseData<bool>>> SetDefault(int addressId)
    {
        return await ExecuteAsync(async () =>
        {
            var currentUser = User; 
            var result = await _addressFacade.SetDefaultAsync(currentUser, addressId);
            return ResponseData<bool>.Success(StatusCodes.Status200OK, result,
                AddressMessages.SetDefaultSuccess);
        });
    }
}
