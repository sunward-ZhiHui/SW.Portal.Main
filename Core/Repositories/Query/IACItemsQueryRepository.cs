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
    public interface IACEntrysQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<ACEntryModel>> GetAllByAsync();
        Task<IReadOnlyList<NavCustomerModel>> GetNavCustomerAsync();
        Task<ACEntryModel> DeleteACEntry(ACEntryModel value);
        Task<ACEntryModel> InsertOrUpdateAcentry(ACEntryModel value);
        Task<IReadOnlyList<ACEntryLinesModel>> GetACEntryLinesByAsync(long? Id);
        Task<ACEntryLinesModel> DeleteACEntryLine(ACEntryLinesModel value);
        Task<ACEntryLinesModel> InsertOrUpdateAcentryLine(ACEntryLinesModel value);
        Task<ACEntryModel> CopyAcentry(ACEntryModel aCEntryModel);
    }
}
