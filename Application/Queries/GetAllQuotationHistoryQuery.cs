using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllQuotationHistoryQuery : PagedRequest, IRequest<List<QuotationHistory>>
    {
        public string? SearchString { get; set; }
    }
    public class GetAllGenericCodesQuery : PagedRequest, IRequest<List<GenericCodes>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdateQuotationHistory : QuotationHistory, IRequest<QuotationHistory>
    {
    }
    public class GetQuotationHistoryBySession : PagedRequest, IRequest<QuotationHistory>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetQuotationHistoryBySession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }



    public class GetAllQuotationHistoryLineQuery : PagedRequest, IRequest<List<QuotationHistoryLine>>
    {
        public string? SearchString { get; set; }
        public long? QutationHistoryId { get; set; }
        public GetAllQuotationHistoryLineQuery(long? qutationHistoryId)
        {
            this.QutationHistoryId = qutationHistoryId;
        }
    }
    public class InsertOrUpdateQuotationHistoryLine : QuotationHistoryLine, IRequest<QuotationHistoryLine>
    {
    }
    public class GetQuotationHistoryByLineSession : PagedRequest, IRequest<QuotationHistoryLine>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetQuotationHistoryByLineSession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }
    public class DeleteQuotationHistory : PagedRequest, IRequest<QuotationHistory>
    {
        public QuotationHistory QuotationHistory { get; set; }
        public DeleteQuotationHistory(QuotationHistory quotationHistory)
        {
            this.QuotationHistory = quotationHistory;
        }
    }
    public class DeleteQuotationHistoryLine : PagedRequest, IRequest<QuotationHistoryLine>
    {
        public QuotationHistoryLine QuotationHistoryLine { get; set; }
        public DeleteQuotationHistoryLine(QuotationHistoryLine quotationHistoryLine)
        {
            this.QuotationHistoryLine = quotationHistoryLine;
        }
    }


    public class GetAllQuotationAwardQuery : PagedRequest, IRequest<List<QuotationAward>>
    {
        public string? SearchString { get; set; }
        public long? QuotationHistoryId { get; set; }
        public GetAllQuotationAwardQuery(long? quotationHistoryId)
        {
            this.QuotationHistoryId = quotationHistoryId;
        }
    }
    public class InsertOrUpdateQuotationAward : QuotationAward, IRequest<QuotationAward>
    {
    }
    public class DeleteQuotationAward : PagedRequest, IRequest<QuotationAward>
    {
        public QuotationAward QuotationAward { get; set; }
        public DeleteQuotationAward(QuotationAward quotationHistory)
        {
            this.QuotationAward = quotationHistory;
        }
    }

    public class GetAllQuotationAwardLineQuery : PagedRequest, IRequest<List<QuotationAwardLine>>
    {
        public string? SearchString { get; set; }
        public long? QuotatonAwardId { get; set; }
        public GetAllQuotationAwardLineQuery(long? quotatonAwardId)
        {
            this.QuotatonAwardId = quotatonAwardId;
        }
    }
    public class InsertOrUpdateQuotationAwardLine : QuotationAwardLine, IRequest<QuotationAwardLine>
    {
    }
    public class DeleteQuotationAwardLine : PagedRequest, IRequest<QuotationAwardLine>
    {
        public QuotationAwardLine QuotationAwardLine { get; set; }
        public DeleteQuotationAwardLine(QuotationAwardLine quotationAwardLine)
        {
            this.QuotationAwardLine = quotationAwardLine;
        }
    }
    public class GetQuotationAwardSession : PagedRequest, IRequest<QuotationAward>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetQuotationAwardSession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }
    public class GetQuotationAwardLineSession : PagedRequest, IRequest<QuotationAwardLine>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetQuotationAwardLineSession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }
    public class GetAllQuotationHistoryLineProducts : PagedRequest, IRequest<List<GenericCodes>>
    {
        public string? SearchString { get; set; }
        public long? QuotationHistoryId { get; set; }
        public GetAllQuotationHistoryLineProducts(long? quotationHistoryId)
        {
            this.QuotationHistoryId = quotationHistoryId;
        }
    }
}
