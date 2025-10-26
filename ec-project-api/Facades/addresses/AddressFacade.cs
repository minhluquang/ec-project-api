using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.request.addresses;
using ec_project_api.Dtos.Users;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.addresses;
using ec_project_api.Services.wards;
using ec_project_api.Services.provinces;

namespace ec_project_api.Facades.addresses
{
    public class AddressFacade
    {
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IProvinceService _provinceService;
        private readonly IWardService _wardService;
        private readonly IMapper _mapper;

        public AddressFacade(IAddressService addressService, IUserService userService,  IProvinceService provinceService, IWardService wardService, IMapper mapper)
        {
            _addressService = addressService;
            _userService = userService;
            _provinceService = provinceService;
            _wardService = wardService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> GetByUserIdAsync(int userId)
        {
            var userExist = await _userService.GetByIdAsync(userId);
            if (userExist == null)
                throw new KeyNotFoundException(AddressMessages.InvalidUserId);

            var addresses = await _addressService.GetByUserIdAsync(userId);
            var addressDtos = _mapper.Map<IEnumerable<AddressDto>>(addresses);
            return addressDtos;
        }

        public async Task<bool> CreateAsync(int userId, AddressCreateRequest request)
        {
            var userExist = await _userService.GetByIdAsync(userId);
            if (userExist == null)
                throw new KeyNotFoundException(AddressMessages.InvalidUserId);
            
            var provinceExist = await _provinceService.GetByIdAsync(request.ProvinceId);
            if (provinceExist == null)
                throw new KeyNotFoundException(LocationMessages.InvalidProvinceId);
            
            var wardExist = await _wardService.GetByIdAsync(request.WardId);
            if (wardExist == null)
                throw new KeyNotFoundException(LocationMessages.InvalidProvinceId);
            
            var addressDto = _mapper.Map<Address>(request);
            addressDto.UserId = userId;
            var result = await _addressService.CreateAsync(addressDto);
            return result;
        }
        
        public async Task<bool> UpdateAsync(int userId, int addressId, AddressUpdateRequest request)
        {
            var userExist = await _userService.GetByIdAsync(userId);
            if (userExist == null)
                throw new KeyNotFoundException(AddressMessages.InvalidUserId);
            
            var existingAddress  = await _addressService.GetByIdAsync(addressId);
            if (existingAddress  == null || existingAddress .UserId != userId)
                throw new KeyNotFoundException(AddressMessages.InvalidAddressId);
            
            var provinceExist = await _provinceService.GetByIdAsync(request.ProvinceId);
            if (provinceExist == null)
                throw new KeyNotFoundException(LocationMessages.InvalidProvinceId);
            
            var wardExist = await _wardService.GetByIdAsync(request.WardId);
            if (wardExist == null)
                throw new KeyNotFoundException(LocationMessages.InvalidProvinceId);
            
            existingAddress.RecipientName = request.RecipientName;
            existingAddress.Phone = request.Phone;
            existingAddress.StreetAddress = request.StreetAddress;
            existingAddress.ProvinceId = request.ProvinceId;
            existingAddress.WardId = request.WardId;
            existingAddress.IsDefault = request.IsDefault;
            existingAddress.UpdatedAt = DateTime.UtcNow;
            
            var result = await _addressService.UpdateAsync(existingAddress);
            return result;
        }
        
        public async Task<bool> DeleteAsync(int userId, int addressId)
        {
            var userExist = await _userService.GetByIdAsync(userId);
            if (userExist == null)
                throw new KeyNotFoundException(AddressMessages.InvalidUserId);
            
            var existingAddress  = await _addressService.GetByIdAsync(addressId);
            if (existingAddress  == null || existingAddress .UserId != userId)
                throw new KeyNotFoundException(AddressMessages.InvalidAddressId);
            
            if (existingAddress.IsDefault)
                throw new InvalidOperationException(AddressMessages.CannotDeleteDefaultAddress);
            
            var result = await _addressService.DeleteAsync(existingAddress);
            return result;
        }

        public async Task<bool> SetDefaultAsync(int userId, int addressId)
        {
            var userExist = await _userService.GetByIdAsync(userId);
            if (userExist == null)
                throw new KeyNotFoundException(AddressMessages.InvalidUserId);
            
            var existingAddress  = await _addressService.GetByIdAsync(addressId);
            if (existingAddress  == null || existingAddress .UserId != userId)
                throw new KeyNotFoundException(AddressMessages.InvalidAddressId);

            existingAddress.IsDefault = true;
            existingAddress.UpdatedAt = DateTime.UtcNow;
            var result =  await _addressService.UpdateAsync(existingAddress);
            return result;
        }
    }
}