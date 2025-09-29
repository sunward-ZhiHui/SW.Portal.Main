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
    public class GetAllStockInformationMasterQuery : PagedRequest, IRequest<List<StockInformationMaster>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdateStockInformationMaster : StockInformationMaster, IRequest<StockInformationMaster>
    {

    }
    public class DeleteStockInformationMaster : StockInformationMaster, IRequest<StockInformationMaster>
    {
        public StockInformationMaster StockInformationMaster { get; private set; }
        public DeleteStockInformationMaster(StockInformationMaster stockInformationMaster)
        {
            this.StockInformationMaster = stockInformationMaster;
        }
    }
}
