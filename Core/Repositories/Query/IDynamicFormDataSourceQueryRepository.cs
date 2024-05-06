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
    public interface IDynamicFormDataSourceQueryRepository : IQueryRepository<AttributeDetails>
    {
        Task<DataSourceAttributeDetails> GetAllDropDownDataSources();
        Task<IReadOnlyList<AttributeDetails>> GetDataSourceDropDownList(long? CompanyId, List<string?> dataSourceTableIds,string? plantCode, List<long?> applicationMasterIds, List<long?> applicationMasterParentIds);
        Task<DynamicFormFilterDataSoureList> GetAllFilterDropDownDataSources();
        Task<IReadOnlyList<DynamicFormFilterBy>> GetDynamicFormFilterByDataSource(List<string?> dataSourceTableIds);
        Task<IReadOnlyList<AttributeDetails>> GetFilterDataSourceLists(List<DynamicFormFilterBy> dynamicFormFilterBies);
        Task<IReadOnlyList<AttributeDetails>> GetFilterByDataSourceLists(List<DynamicFormFilterBy> dynamicFormFilterBies,object Data,string? DataSource);
    }
}
