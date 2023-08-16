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
    public class GetAllfileprofiletypeQuery : PagedRequest, IRequest<List<Fileprofiletype>>
    {
        public long FileProfileTypeID { get; private set; }
        public GetAllfileprofiletypeQuery(long FileProfileTypeID)
        {
            this.FileProfileTypeID = FileProfileTypeID;
        }
    }
    public class GetAllSelectedfileprofiletypeQuery : PagedRequest, IRequest<DocumentTypeModel>
    {
        public long selectedFileProfileTypeID { get; private set; }
        public GetAllSelectedfileprofiletypeQuery(long selectedFileProfileTypeID)
        {
            this.selectedFileProfileTypeID = selectedFileProfileTypeID;
        }
    }
    public class GetFileProfileTypeDocumentByHistory : PagedRequest, IRequest<DocumentTypeModel>
    {
        public SearchModel SearchModel { get; private set; }
        public GetFileProfileTypeDocumentByHistory(SearchModel searchModel)
        {
            this.SearchModel = searchModel;
        }
    }

}
