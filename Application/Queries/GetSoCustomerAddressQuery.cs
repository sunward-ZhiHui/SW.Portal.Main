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
    public class GetSoCustomerAddressQuery : view_SoCustomerAddress, IRequest<List<view_SoCustomerAddress>>
    {
        public string SearchString { get; set; }
    }
    public class GetSoCustomerAddressByTypeQuery : view_SoCustomerAddress, IRequest<List<view_SoCustomerAddress>>
    {
        public string AddressType { get; set; }
        public long? CustomerId { get; set; }
        public GetSoCustomerAddressByTypeQuery(string AddressType, long CustomerId)
        {
            this.AddressType = AddressType;
            this.CustomerId = CustomerId;
        }
    }
    public class GetSoCustomerAddressByCustomerQuery : view_SoCustomerAddress, IRequest<List<view_SoCustomerAddress>>
    {
        public long? CustomerId { get; set; }
        public GetSoCustomerAddressByCustomerQuery( long CustomerId)
        {
            this.CustomerId = CustomerId;
        }
    }
    public class CreateCustomerAddress : view_SoCustomerAddress, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class EditCustomerAddress : view_SoCustomerAddress, IRequest<long>
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
