using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IAddressService
    {
        IReadOnlyList<Address> GetAddressesWithUserId(string id);
        Address GetAddress(int id);
        void AddAddress(Address address);
        void RemoveAddress(int id);
        void UpdateAddress(Address address);
    }
}
