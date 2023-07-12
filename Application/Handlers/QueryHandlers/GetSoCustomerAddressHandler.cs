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
    public class GetSoCustomerAddressHandler : IRequestHandler<GetSoCustomerAddressQuery, List<Address>>
    {
        private readonly ISoCustomerAddressQueryRepository _soCustomerAddressQueryRepository;
        public GetSoCustomerAddressHandler(ISoCustomerAddressQueryRepository soCustomerAddressQueryRepository)
        {
            _soCustomerAddressQueryRepository = soCustomerAddressQueryRepository;
        }
        public async Task<List<Address>> Handle(GetSoCustomerAddressQuery request, CancellationToken cancellationToken)
        {           
            return (List<Address>)await _soCustomerAddressQueryRepository.GetAllAsync();
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
