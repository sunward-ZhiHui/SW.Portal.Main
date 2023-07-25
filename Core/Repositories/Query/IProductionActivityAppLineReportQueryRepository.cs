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
    public interface IProductionActivityAppLineReportQueryRepository: IQueryRepository<view_ProductionActivityAppLineReport>
    {
        Task<IReadOnlyList<view_ProductionActivityAppLineReport>> GetAllAsync();
        Task<IReadOnlyList<view_ProductionActivityAppLineReport>> GetAllFilterAsync(long? CompanyId , DateTime? FromDate , DateTime? ToDate);
        Task<List<Documents>> GetDocumentListAsync(Guid sessionId);
        Task<view_ProductionActivityAppLineReport> GetDocumentListByCommentImageAsync(long? ProductionActivityAppLineID);
    }
}

