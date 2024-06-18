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
    public interface IDynamicFormItemQueryRepository : IQueryRepository<DynamicFormItem>
    {
        Task<IReadOnlyList<DynamicFormItem>> GetAllAsync();
        Task<long> Insert(DynamicFormItem dynamicformitem);
        Task<long> Update(DynamicFormItem dynamicformitem);
        Task<long> Delete(long id);

        Task<long> Update(DynamicFormItemLine DynamicFormItemLine);
        Task<long> Insert(DynamicFormItemLine DynamicFormItemLine);
        Task<long> DeleteLine(long DynamicFormItemLineID);
        Task<IReadOnlyList<DynamicFormItemLine>> GetAllDynamicLineAsync(long DynamicFormItemID);
        Task<IReadOnlyList<DynamicForm>> GetAllDynamicFormDropdownAsync();
        Task<IReadOnlyList<DynamicFormItem>> GetAllDynamicAsync(long DynamicFormItemID);

    }
    
}
