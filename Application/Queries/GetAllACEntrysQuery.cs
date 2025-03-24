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
    public class GetAllACEntrysQuery : PagedRequest, IRequest<List<ACEntryModel>>
    {
        public string? SearchString { get; set; }
    }
    public class GetAllNavCustomerQuery : PagedRequest, IRequest<List<NavCustomerModel>>
    {
        public string? SearchString { get; set; }
    }

    public class InsertOrUpdateAcentry : ACEntryModel, IRequest<ACEntryModel>
    {

    }
    public class DeleteACEntry : ACEntryModel, IRequest<ACEntryModel>
    {
        public ACEntryModel ACEntryModel { get; private set; }
        public DeleteACEntry(ACEntryModel aCEntryModel)
        {
            this.ACEntryModel = aCEntryModel;
        }
    }
    public class CoptyACEntry : ACEntryModel, IRequest<ACEntryModel>
    {
    }
    public class GetAllACEntryLinesQuery : PagedRequest, IRequest<List<ACEntryLinesModel>>
    {
        public long? ACEntryId { get; set; }
        public GetAllACEntryLinesQuery(long? aCEntryId)
        {
            this.ACEntryId = aCEntryId;
        }
    }
    public class DeleteACEntryLine : ACEntryModel, IRequest<ACEntryLinesModel>
    {
        public ACEntryLinesModel ACEntryLinesModel { get; private set; }
        public DeleteACEntryLine(ACEntryLinesModel aCEntryLinesModel)
        {
            this.ACEntryLinesModel = aCEntryLinesModel;
        }
    }
    public class InsertOrUpdateAcentryLine : ACEntryLinesModel, IRequest<ACEntryLinesModel>
    {

    }
}
