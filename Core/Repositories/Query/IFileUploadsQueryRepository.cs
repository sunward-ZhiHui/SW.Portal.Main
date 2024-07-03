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
    public interface IReportFileUploadsQueryRepository : IQueryRepository<ReportDocuments>
    {
        Task<ReportDocuments> InsertCreateReportDocumentBySession(ReportDocuments value); 
        Task<IReadOnlyList<ReportDocuments>> GetAllAsync();
        Task<Guid?> InsertNew(ReportDocuments reportDocuments);
        Task<long> Insert(ReportDocuments reportDocuments);
        Task<long> Update(ReportDocuments reportDocuments);
        Task<long> Delete(long ReportDocumentID);
        Task<DynamicFormReport> UpdateDynamicFormReportBySession(DynamicFormReport value);
    }
}
