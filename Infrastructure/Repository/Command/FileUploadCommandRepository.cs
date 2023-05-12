using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using Core.Entities.Base;

namespace Infrastructure.Repository.Command
{
    public class FileUploadCommandRepository : CommandRepository<BaseEntity>, IFileUploadCommandRepository
    {
        public FileUploadCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
