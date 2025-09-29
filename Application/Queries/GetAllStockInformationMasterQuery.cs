using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllNAVItemLinksQuery : PagedRequest, IRequest<List<NAVItemLinksModel>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdateNavitemLinks : NAVItemLinksModel, IRequest<NAVItemLinksModel>
    {

    }
    public class DeleteNavitemLinks : ACEntryModel, IRequest<NAVItemLinksModel>
    {
        public NAVItemLinksModel NAVItemLinksModel { get; private set; }
        public DeleteNavitemLinks(NAVItemLinksModel aCEntryModel)
        {
            this.NAVItemLinksModel = aCEntryModel;
        }
    }
}
