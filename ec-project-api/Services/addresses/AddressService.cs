using ec_project_api.Dtos.request.addresses;
using ec_project_api.Dtos.Users;
using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using AddressDto = ec_project_api.DTOs.response.addresses.AddressDto;

namespace ec_project_api.Services.addresses
{
    public class AddressService : BaseService<Address, int>, IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository) : base(addressRepository)
        {
            _addressRepository = addressRepository;
        }
        
        public async Task<IEnumerable<Address>> GetByUserIdAsync(int userId)
        {
            var options = new QueryOptions<Address>
            {
                Filter = a => a.UserId == userId,
            };
            
            options.Includes.Add(a => a.Ward);
            options.Includes.Add(a => a.Province);
            
            var addresses = await _addressRepository.GetAllAsync(options);
            return addresses;
        }

        public async Task<bool> CreateAsync(Address request)
        {   
            
            if (request.IsDefault)
            {
                var defaultOptions = new QueryOptions<Address>
                {
                    Filter = a => a.UserId == request.UserId && a.IsDefault
                };

                var existingDefaults = await _addressRepository.GetAllAsync(defaultOptions);
                if (existingDefaults != null)
                {
                    foreach (var existing in existingDefaults)
                    {
                        existing.IsDefault = false;
                        existing.UpdatedAt = DateTime.UtcNow;
                        await _addressRepository.UpdateAsync(existing);
                    }
                }
                
            }
            
            await _addressRepository.AddAsync(request);
            await _addressRepository.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> UpdateAsync(Address request)
        {   
            
            if (request.IsDefault)
            {
                var defaultOptions = new QueryOptions<Address>
                {
                    Filter = a => a.UserId == request.UserId && a.IsDefault
                };

                var existingDefaults = await _addressRepository.GetAllAsync(defaultOptions);
                if (existingDefaults != null)
                {
                    foreach (var existing in existingDefaults)
                    {
                        existing.IsDefault = false;
                        existing.UpdatedAt = DateTime.UtcNow;
                        await _addressRepository.UpdateAsync(existing);
                    }
                }
                
            }
            
            await _addressRepository.UpdateAsync(request);
            await _addressRepository.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> DeleteAsync(Address request)
        {   
            bool isDefault = request.IsDefault;
            
            await _addressRepository.DeleteAsync(request);
            await _addressRepository.SaveChangesAsync();
                
            var remainingAddressesOptions = new QueryOptions<Address>
            {
                Filter = a => a.UserId == request.UserId
            };
            var remainingAddresses = await _addressRepository.GetAllAsync(remainingAddressesOptions);

            if (!remainingAddresses.Any())
                return true;
            
            if (isDefault)
            {
                var newDefault = remainingAddresses
                    .OrderByDescending(a => a.UpdatedAt)
                    .ThenByDescending(a => a.CreatedAt)
                    .First();

                newDefault.IsDefault = true;
                newDefault.UpdatedAt = DateTime.UtcNow;

                await _addressRepository.UpdateAsync(newDefault);
                await _addressRepository.SaveChangesAsync();
            }
            
            return true;
        }
    }
}