using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Core.Repositories.Query
{
    public interface IAttributeQueryRepository : IQueryRepository<AttributeHeader>
    {
        Task<AttributeHeaderListModel> GetAllAttributeNameAsync(DynamicForm dynamicForm, long? UserId,bool? IsSubFormLoad);
        Task<IReadOnlyList<AttributeHeader>> GetAllAttributeName(bool? IsSubForm, string? type, long? subId);
        Task<IReadOnlyList<AttributeHeaderDataSource>> GetAttributeHeaderDataSource();
        Task<IReadOnlyList<DynamicFormFilter>> GetFilterDataSource();
        Task<IReadOnlyList<DynamicForm>> GetComboBoxList();
        Task<long> Insert(AttributeHeader attributeHeader);
        Task<long> UpdateAsync(AttributeHeader attributeHeader);
        Task<long> DeleteAsync(AttributeHeader attributeHeader);
        AttributeHeader GetAllAttributeNameCheckValidation(AttributeHeader attributeHeader);
        Task<IReadOnlyList<AttributeHeader>> GetAllAttributeNameNotInDynamicForm(long? dynamicFormSectionId, long? attributeID);
        Task<AttributeHeader> GetAllBySessionAttributeName(Guid? SessionId);
        Task<AttributeHeader> UpdateAttributeHeaderSortOrder(AttributeHeader attributeHeader);
        Task<IReadOnlyList<DropDownOptionsListModel>> GetApplicationMasterParentByList(IDictionary<string, object> DynamicMasterParentIds,long? applicationMasterParentId);
        Task<IReadOnlyList<DropDownOptionsListModel>> GetApplicationMasterParentByMobileList(IDictionary<string, JsonElement> DynamicMasterParentIds, long? applicationMasterParentId);
        Task<DropDownOptionsGridListModel> GetDynamicGridNested(List<long?> DynamicFormDataId, long? userId);
        Task<List<DynamicFormData>> GetAllDynamicFormApiAsync(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId,Guid? DynamicFormDataGridSessionId,string? BaseUrl);


    }
}