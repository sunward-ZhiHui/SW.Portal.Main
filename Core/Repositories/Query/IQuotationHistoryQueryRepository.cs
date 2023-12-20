using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IQuotationHistoryQueryRepository : IQueryRepository<QuotationHistory>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<QuotationHistory>> GetAllByAsync();
        Task<QuotationHistory> InsertOrUpdateQuotationHistory(QuotationHistory value);

        Task<QuotationHistory> GetQuotationHistoryBySession(Guid? SessionId);
        Task<IReadOnlyList<QuotationHistoryLine>> GetAllByLineAsync(long? quotationHistoryId);
        Task<QuotationHistoryLine> GetQuotationHistoryByLineSession(Guid? SessionId);
        Task<QuotationHistoryLine> InsertOrUpdateQuotationHistoryLine(QuotationHistoryLine value);
        Task<IReadOnlyList<GenericCodes>> GetAllByGenericCodesAsync();
        Task<IReadOnlyList<GenericCodes>> GetQuotationHistoryLineProducts(long? id);
        Task<QuotationHistory> DeleteQuotationHistory(QuotationHistory value);
        Task<QuotationHistoryLine> DeleteQuotationHistoryLine(QuotationHistoryLine value);
        Task<IReadOnlyList<QuotationAward>> GetAllByQuotationAwardAsync(long? id);
        Task<QuotationAward> DeleteQuotationAward(QuotationAward value);
        Task<QuotationAward> InsertOrUpdateQuotationAward(QuotationAward value);
        Task<IReadOnlyList<QuotationAwardLine>> GetAllByQuotationAwardLineAsync(long? id);
        Task<QuotationAwardLine> InsertOrUpdateQuotationAwardLine(QuotationAwardLine value);
        Task<QuotationAwardLine> DeleteQuotationAwardLine(QuotationAwardLine value);
        Task<QuotationAward> GetQuotationAwardSession(Guid? SessionId);
        Task<QuotationAwardLine> GetQuotationAwardLineSession(Guid? SessionId);
    }
}
