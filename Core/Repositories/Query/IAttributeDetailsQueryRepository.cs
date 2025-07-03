using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IAttributeDetailsQueryRepository : IQueryRepository<AttributeDetails>
    {
        Task<IReadOnlyList<AttributeDetails>> GetAllAsync();
        Task<AttributeDetails> GetByIdAsync(Int64 id);
        Task<long> Insert(AttributeDetails attributeDetails);
        Task<long> UpdateAsync(AttributeDetails attributeDetails);
        Task<long> Delete(long id);
        Task<IReadOnlyList<AttributeDetails>> LoadAttributelst(long Id);
        AttributeDetails AttributeDetailsValueCheckValidation(string? value, long id, long? attributeId);
        Task<IReadOnlyList<AttributeGroupCheckBox>> GetAttributeGroupCheckBoxList(long? AttributeId);
        Task<AttributeGroupCheckBox> InsertOrUpdateAttributeGroupCheckBox(AttributeGroupCheckBox attributeGroupCheckBox);
        Task<AttributeGroupCheckBox> DeleteAttributeGroupCheckBox(AttributeGroupCheckBox attributeGroupCheckBox);
    }
}
