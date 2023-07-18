using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ISoCustomerAddressQueryRepository : IQueryRepository<view_SoCustomerAddress>
    {
        Task<IReadOnlyList<view_SoCustomerAddress>> GetAllAsync();
        Task<IReadOnlyList<view_SoCustomerAddress>> GetAllByAddressTypeAsync(string AddressType, long? CustomerId);
        Task<Address> GetByIdAsync(Int64 AddressID);
        Task<IReadOnlyList<view_SoCustomerAddress>> GetByCustomerIdAsync(long? CustomerId);
        Task<SoCustomerAddress> GetByCustomerAddressIdAsync(Int64 SoCustomerAddressId);
        Task<long> Insert(view_SoCustomerAddress address);
        Task<long> EditAddress(view_SoCustomerAddress address);
        Task<Address> DeleteAddressAsync(Address address);
        Task<SoCustomerAddress> DeleteSoCustomerAddressAsync(SoCustomerAddress soCustomerAddress);
        Task<long> InsertSoCustomerAddress(SoCustomerAddress soCustomerAddress);

    }
    
}
