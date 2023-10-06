using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IDynamicFormQueryRepository : IQueryRepository<DynamicForm>
    {
        Task<DynamicForm> GetAllSelectedList(Guid? sessionId);
        Task<IReadOnlyList<DynamicForm>> GetAllAsync();
        Task<DynamicForm> GetDynamicFormBySessionIdAsync(Guid? SessionId);
        Task<long> Insert(DynamicForm dynamicForm);
        Task<long> Update(DynamicForm dynamicForm);
        Task<long> Delete(long id);
        DynamicForm GetDynamicFormScreenNameCheckValidation(string? value, long id);
        Task<DynamicFormSection> InsertOrUpdateDynamicFormSection(DynamicFormSection dynamicFormSection);
        Task<IReadOnlyList<DynamicFormSection>> GetDynamicFormSectionAsync(long? dynamicFormId);
        Task<DynamicFormSectionAttribute> InsertOrUpdateDynamicFormSectionAttribute(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<IReadOnlyList<DynamicFormSectionAttribute>> GetDynamicFormSectionAttributeAsync(long? dynamicFormSectionId);
        Task<long> DeleteDynamicFormSection(DynamicFormSection dynamicFormSection);
        Task<DynamicFormSection> UpdateDynamicFormSectionSortOrder(DynamicFormSection dynamicFormSection);
        Task<long> DeleteDynamicFormSectionAttribute(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<DynamicFormSectionAttribute> UpdateDynamicFormSectionAttributeSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute);
        Task<long> InsertDynamicFormAttributeSection(long dynamicFormSectionId, IEnumerable<AttributeHeader> attributeIds,long? userId);
        Task<DynamicFormData> InsertOrUpdateDynamicFormData(DynamicFormData dynamicFormData);
        Task<DynamicFormData> GetDynamicFormDataBySessionIdAsync(Guid? SessionId);
    }

}
