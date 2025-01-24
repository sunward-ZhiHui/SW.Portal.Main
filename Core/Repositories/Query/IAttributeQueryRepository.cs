using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Core.Repositories.Query
{
    public interface IAttributeQueryRepository : IQueryRepository<AttributeHeader>
    {
        Task<AttributeHeaderListModel> GetAllAttributeNameAsync(DynamicForm dynamicForm, long? UserId,bool? IsSubFormLoad,bool? isNoDelete);
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
        Task<List<DynamicFormDataResponse>> GetAllDynamicFormApiAsync(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId,Guid? DynamicFormDataGridSessionId,Guid? DynamicFormSectionGridAttributeSessionId, string? BaseUrl,bool? IsAll,int? PageNo,int? PageSize);
        Task<DropDownOptionsGridListModel> GetDynamicGridDropDownById(List<long?> DynamicFormId, long? userId);
        Task<IReadOnlyList<AttributeDetails>> GetAttributeDetailsDataSource(long? AttributeId);
        Task<IReadOnlyList<QCTestRequirement>> GetQcTestRequirementSummery();
        Task<List<DynamicFormDataResponse>> GetAllDynamicFormAttributeAllApiAsync(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId, string? BasUrl, bool? IsAll);
        Task<List<DynamicFormDataResponse>> GetAllDynamicFormDataOneApiAsync(Guid? DynamicFormDataSessionId);
        Task<IReadOnlyList<ExpandoObject>> GetAllDataObjectDynamicFormApiAsync(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId, string? BasUrl, bool? IsAll);
        Task<List<object>> GetAttributeHeaderDataSource1();
    }
}