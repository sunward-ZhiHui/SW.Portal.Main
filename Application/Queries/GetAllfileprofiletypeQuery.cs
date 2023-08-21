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
    public class GetAllfileprofiletypeListQuery : PagedRequest, IRequest<List<DocumentsModel>>
    {
        
    }
    public class GetAllSelectedfileprofiletypeQuery : PagedRequest, IRequest<DocumentTypeModel>
    {
        public long? selectedFileProfileTypeID { get; private set; }
        public GetAllSelectedfileprofiletypeQuery(long? selectedFileProfileTypeID)
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
    public class GetFileProfileTypeDelete : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public GetFileProfileTypeDelete(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }
    public class GetFileProfileTypeCheckOut : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public GetFileProfileTypeCheckOut(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }
    
    public class GetFileDownload : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public GetFileDownload(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }

}
