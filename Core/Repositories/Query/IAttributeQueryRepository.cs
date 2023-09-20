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
        Task<IReadOnlyList<AttributeHeader>> GetComboBoxLst();
        Task<IReadOnlyList<AttributeHeader>> GetAllAsync(long ID);
        Task<AttributeHeader> GetByIdAsync(Int64 id);
        Task<long> Insert(AttributeHeader attributeHeader);
        Task<long> UpdateAsync(AttributeHeader attributeHeader);
        Task<long> DeleteAsync(long id);
    }
}