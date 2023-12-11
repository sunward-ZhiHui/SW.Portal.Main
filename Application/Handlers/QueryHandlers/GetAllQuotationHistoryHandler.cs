using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllQuotationHistoryHandler : IRequestHandler<GetAllQuotationHistoryQuery, List<QuotationHistory>>
    {
        private readonly IQuotationHistoryQueryRepository _queryRepository;
        public GetAllQuotationHistoryHandler(IQuotationHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<QuotationHistory>> Handle(GetAllQuotationHistoryQuery request, CancellationToken cancellationToken)
        {
            return (List<QuotationHistory>)await _queryRepository.GetAllByAsync();
        }
    }
    public class InsertOrUpdateQuotationHistoryHandler : IRequestHandler<InsertOrUpdateQuotationHistory, QuotationHistory>
    {
        private readonly IQuotationHistoryQueryRepository _queryRepository;
        public InsertOrUpdateQuotationHistoryHandler(IQuotationHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<QuotationHistory> Handle(InsertOrUpdateQuotationHistory request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateQuotationHistory(request);
        }
    }
    public class GetQuotationHistoryBySessionHandler : IRequestHandler<GetQuotationHistoryBySession, QuotationHistory>
    {
        private readonly IQuotationHistoryQueryRepository _queryRepository;
        public GetQuotationHistoryBySessionHandler(IQuotationHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<QuotationHistory> Handle(GetQuotationHistoryBySession request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetQuotationHistoryBySession(request.SesionId);
        }
    }


    public class GetAllQuotationHistoryLineHandler : IRequestHandler<GetAllQuotationHistoryLineQuery, List<QuotationHistoryLine>>
    {
        private readonly IQuotationHistoryQueryRepository _queryRepository;
        public GetAllQuotationHistoryLineHandler(IQuotationHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<QuotationHistoryLine>> Handle(GetAllQuotationHistoryLineQuery request, CancellationToken cancellationToken)
        {
            return (List<QuotationHistoryLine>)await _queryRepository.GetAllByLineAsync(request.QutationHistoryId);
        }
    }
    public class InsertOrUpdateQuotationHistoryLineHandler : IRequestHandler<InsertOrUpdateQuotationHistoryLine, QuotationHistoryLine>
    {
        private readonly IQuotationHistoryQueryRepository _queryRepository;
        public InsertOrUpdateQuotationHistoryLineHandler(IQuotationHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<QuotationHistoryLine> Handle(InsertOrUpdateQuotationHistoryLine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateQuotationHistoryLine(request);
        }
    }
    public class GetQuotationHistoryByLineSessionHandler : IRequestHandler<GetQuotationHistoryByLineSession, QuotationHistoryLine>
    {
        private readonly IQuotationHistoryQueryRepository _queryRepository;
        public GetQuotationHistoryByLineSessionHandler(IQuotationHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<QuotationHistoryLine> Handle(GetQuotationHistoryByLineSession request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetQuotationHistoryByLineSession(request.SesionId);
        }
    }
    public class GetAllGenericCodesHandler : IRequestHandler<GetAllGenericCodesQuery, List<GenericCodes>>
    {
        private readonly IQuotationHistoryQueryRepository _queryRepository;
        public GetAllGenericCodesHandler(IQuotationHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<GenericCodes>> Handle(GetAllGenericCodesQuery request, CancellationToken cancellationToken)
        {
            return (List<GenericCodes>)await _queryRepository.GetAllByGenericCodesAsync();
        }
    }
    public class DeleteQuotationHistoryHandler : IRequestHandler<DeleteQuotationHistory, QuotationHistory>
    {
        private readonly IQuotationHistoryQueryRepository _queryRepository;

        public DeleteQuotationHistoryHandler(IQuotationHistoryQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<QuotationHistory> Handle(DeleteQuotationHistory request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteQuotationHistory(request.QuotationHistory);
        }
    }
    public class DeleteQuotationHistoryLineHandler : IRequestHandler<DeleteQuotationHistoryLine, QuotationHistoryLine>
    {
        private readonly IQuotationHistoryQueryRepository _queryRepository;

        public DeleteQuotationHistoryLineHandler(IQuotationHistoryQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<QuotationHistoryLine> Handle(DeleteQuotationHistoryLine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteQuotationHistoryLine(request.QuotationHistoryLine);
        }
    }
}
