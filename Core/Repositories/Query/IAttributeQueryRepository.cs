using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IAttributeQueryRepository : IQueryRepository<AttributeHeader>
    {
        Task<AttributeHeaderListModel> GetAllAttributeNameAsync(DynamicForm dynamicForm,long? UserId);
        Task<IReadOnlyList<AttributeHeader>> GetAllAttributeName();
        Task<IReadOnlyList<DynamicForm>> GetComboBoxList();
        Task<long> Insert(AttributeHeader attributeHeader);
        Task<long> UpdateAsync(AttributeHeader attributeHeader);
        Task<long> DeleteAsync(long id);
        IReadOnlyList<AttributeHeader> GetAllAttributeNameCheckValidation(string? value);
        Task<IReadOnlyList<AttributeHeader>> GetAllAttributeNameNotInDynamicForm(long? dynamicFormSectionId, long? attributeID);

    }
}