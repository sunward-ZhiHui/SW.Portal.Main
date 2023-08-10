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
    public interface IFileprofileQueryRepository : IQueryRepository<Fileprofiletype>
    {
        Task<IReadOnlyList<Fileprofiletype>> GetAllAsync(long FileProfileTypeID);
        Task<IReadOnlyList<view_GetFileProfileTypeDocument>> GetAllSelectedFileAsync(long selectedFileProfileTypeID);
    }
}
