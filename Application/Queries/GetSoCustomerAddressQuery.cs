using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetSoCustomerAddressQuery : Address, IRequest<List<Address>>
    {
        public string SearchString { get; set; }
    }
    public class GetSoCustomerAddressByTypeQuery : Address, IRequest<List<Address>>
    {
        public string AddressType { get; set; }
        public long? CustomerId { get; set; }
        public GetSoCustomerAddressByTypeQuery(string AddressType, long CustomerId)
        {
            this.AddressType = AddressType;
            this.CustomerId = CustomerId;
        }
    }
    public class CreateCustomerAddress : Address, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class EditCustomerAddress : Address, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class DeleteCustomerAddress : IRequest<String>
    {
        public long AddressID { get; private set; }
        public long SoCustomerAddressId { get; private set; }

        public DeleteCustomerAddress(long AddressID,long SoCustomerAddressId)
        {
            this.AddressID = AddressID;
            this.SoCustomerAddressId = SoCustomerAddressId;
        }
    }
}
