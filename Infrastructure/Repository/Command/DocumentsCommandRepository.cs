using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class DocumentsCommandRepository : CommandRepository<Documents>, IDocumentsCommandRepository
    {
        public DocumentsCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
