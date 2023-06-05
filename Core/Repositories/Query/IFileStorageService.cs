using Core.Entities;
using Core.Repositories.Query.Base;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IFileStorageService : IQueryRepository<Documents>
    {

        Task<bool> SaveFileAsync(UploadFileInfo file,Guid? SessionId);
    }
}
