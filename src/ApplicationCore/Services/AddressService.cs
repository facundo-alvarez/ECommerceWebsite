using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class AddressService : IAddressService
    {
        private readonly IGenericRepository<Address> _genericRepository;

        public AddressService(IGenericRepository<Address> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public void AddAddress(Address address)
        {
            _genericRepository.Insert(address);
            _genericRepository.Save();
        }

        public Address GetAddress(int id)
        {
            return _genericRepository.Get(id);
        }

        public IReadOnlyList<Address> GetAddressesWithUserId(string id)
        {
            return _genericRepository.GetAll().Where(a => a.User.Id == id).ToList();
        }

        public void RemoveAddress(int id)
        {
            _genericRepository.Delete(id);
            _genericRepository.Save();
        }

        public void UpdateAddress(Address address)
        {
            _genericRepository.Update(address);
            _genericRepository.Save();
        }
    }
}
