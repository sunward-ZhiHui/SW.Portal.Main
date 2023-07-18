using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetSoCustomerAddressHandler : IRequestHandler<GetSoCustomerAddressQuery, List<view_SoCustomerAddress>>
    {
        private readonly ISoCustomerAddressQueryRepository _soCustomerAddressQueryRepository;
        public GetSoCustomerAddressHandler(ISoCustomerAddressQueryRepository soCustomerAddressQueryRepository)
        {
            _soCustomerAddressQueryRepository = soCustomerAddressQueryRepository;
        }
        public async Task<List<view_SoCustomerAddress>> Handle(GetSoCustomerAddressQuery request, CancellationToken cancellationToken)
        {           
            return (List<view_SoCustomerAddress>)await _soCustomerAddressQueryRepository.GetAllAsync();
        }
    }
    public class GetSoCustomerAddressByAddressTypeHandler : IRequestHandler<GetSoCustomerAddressByTypeQuery, List<view_SoCustomerAddress>>
    {
        private readonly ISoCustomerAddressQueryRepository _soCustomerAddressQueryRepository;
        public GetSoCustomerAddressByAddressTypeHandler(ISoCustomerAddressQueryRepository soCustomerAddressQueryRepository)
        {
            _soCustomerAddressQueryRepository = soCustomerAddressQueryRepository;
        }
        public async Task<List<view_SoCustomerAddress>> Handle(GetSoCustomerAddressByTypeQuery request, CancellationToken cancellationToken)
        {
            return (List<view_SoCustomerAddress>)await _soCustomerAddressQueryRepository.GetAllByAddressTypeAsync(request.AddressType,request.CustomerId);
        }
    }
    public class GetSoCustomerAddressByAddressByCustomerHandler : IRequestHandler<GetSoCustomerAddressByCustomerQuery, List<view_SoCustomerAddress>>
    {
        private readonly ISoCustomerAddressQueryRepository _soCustomerAddressQueryRepository;
        public GetSoCustomerAddressByAddressByCustomerHandler(ISoCustomerAddressQueryRepository soCustomerAddressQueryRepository)
        {
            _soCustomerAddressQueryRepository = soCustomerAddressQueryRepository;
        }
        public async Task<List<view_SoCustomerAddress>> Handle(GetSoCustomerAddressByCustomerQuery request, CancellationToken cancellationToken)
        {
            return (List<view_SoCustomerAddress>)await _soCustomerAddressQueryRepository.GetByCustomerIdAsync(request.CustomerId);
        }
    }

    public class CreateCustomerAddressHandler : IRequestHandler<CreateCustomerAddress, long>
    {
        private readonly ISoCustomerAddressQueryRepository _soCustomerAddressQueryRepository;

        public CreateCustomerAddressHandler(ISoCustomerAddressQueryRepository soCustomerAddressQueryRepository)
        {
            _soCustomerAddressQueryRepository = soCustomerAddressQueryRepository;
        }

        public async Task<long> Handle(CreateCustomerAddress request, CancellationToken cancellationToken)
        {
            var req = await _soCustomerAddressQueryRepository.Insert(request);


            var soCustomerAddress = new SoCustomerAddress();
            soCustomerAddress.AddressId = req;
            soCustomerAddress.CustomerId = request.CustomerId;
            soCustomerAddress.AddressType = request.AddressType;
            soCustomerAddress.isBilling = request.isBilling;
            soCustomerAddress.isShipping=request.isShipping;
            var reqq = await _soCustomerAddressQueryRepository.InsertSoCustomerAddress(soCustomerAddress);

            return req;
        }
    }

    public class EditCustomerAddressHandler : IRequestHandler<EditCustomerAddress, long>
    {
        private readonly ISoCustomerAddressQueryRepository _soCustomerAddressQueryRepository;

        public EditCustomerAddressHandler(ISoCustomerAddressQueryRepository soCustomerAddressQueryRepository)
        {
            _soCustomerAddressQueryRepository = soCustomerAddressQueryRepository;
        }

        public async Task<long> Handle(EditCustomerAddress request, CancellationToken cancellationToken)
        {
            var req = await _soCustomerAddressQueryRepository.EditAddress(request);            
            return req;
        }
    }
    public class DeleteCustomerAddressHandler : IRequestHandler<DeleteCustomerAddress, string>
    {
        private readonly ISoCustomerAddressQueryRepository _soCustomerAddressQueryRepository;
        private readonly IQueryRepository<Address> _queryRepository;

        public DeleteCustomerAddressHandler(ISoCustomerAddressQueryRepository soCustomerAddressQueryRepository, IQueryRepository<Address> queryRepository)
        {
            _soCustomerAddressQueryRepository = soCustomerAddressQueryRepository;
            _queryRepository = queryRepository;
        }

        public async Task<string> Handle(DeleteCustomerAddress request, CancellationToken cancellationToken)
        {
            //var req = await _soCustomerAddressQueryRepository.DeleteAddress(request.AddressID,request.SoCustomerAddressId);
            //return req;

            try
            {
                var addressqueryEntity = await _soCustomerAddressQueryRepository.GetByIdAsync(request.AddressID);
                var socustomerEntity = await _soCustomerAddressQueryRepository.GetByCustomerAddressIdAsync(request.SoCustomerAddressId);
                //var addressData = new Address
                //{
                //    SoSalesOrderLineId = queryEntity.SoSalesOrderLineId
                //};

                await _soCustomerAddressQueryRepository.DeleteAddressAsync(addressqueryEntity);
                await _soCustomerAddressQueryRepository.DeleteSoCustomerAddressAsync(socustomerEntity);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Sales Order Address information has been deleted!";


        }
    }
}
