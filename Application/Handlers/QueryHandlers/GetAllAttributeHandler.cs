using Application.Common.Mapper;
using Application.Queries;
using Core.Entities;
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
    public class GetAllAttributeHandler : IRequestHandler<GetAllAttributeHeader, List<AttributeHeader>>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAllAttributeHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository=attrubutequeryRepository;
        }
        public async Task<List<AttributeHeader>> Handle(GetAllAttributeHeader request, CancellationToken cancellationToken)
        {
            return (List<AttributeHeader>)await _attrubutequeryRepository.GetComboBoxLst();
        }
    }
    public class GetAllAttributeNameHandler : IRequestHandler<GetAllAttributeNameHeader, List<AttributeHeader>>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAllAttributeNameHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeHeader>> Handle(GetAllAttributeNameHeader request, CancellationToken cancellationToken)
        {
            return (List<AttributeHeader>)await _attrubutequeryRepository.GetAllAttributeName();
        }
    }
    public class GetAllAttributeValueHandler : IRequestHandler<GetAllAttributeValues, List<AttributeHeader>>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAllAttributeValueHandler( IAttributeQueryRepository attrubutequeryRepository)
        {
           
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeHeader>> Handle(GetAllAttributeValues request, CancellationToken cancellationToken)
        {
            return (List<AttributeHeader>)await _attrubutequeryRepository.GetAllAsync(request.ID);
        }
    }
    public class CreateAttributeHandler : IRequestHandler<CreateAttributeHeader, long>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public CreateAttributeHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<long> Handle(CreateAttributeHeader request, CancellationToken cancellationToken)
        {
           // return (long)await _attrubutequeryRepository.Insert(request);
          var lst = await _attrubutequeryRepository.Insert(request);
            return lst;

        }
    }
    public class EditAttributeHandler : IRequestHandler<EditAttributeHeader, long>
    {
        private readonly IAttributeQueryRepository _conversationQueryRepository;

        public EditAttributeHandler(IAttributeQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(EditAttributeHeader request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.UpdateAsync(request);
            return req;
        }
    }

    public class DeleteAttributeHandler : IRequestHandler<DeleteAttributeHeader, long>
    {
        private readonly IAttributeQueryRepository _conversationQueryRepository;

        public DeleteAttributeHandler(IAttributeQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(DeleteAttributeHeader request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.DeleteAsync(request.AttributeID);
            return req;
        }
    }
}
