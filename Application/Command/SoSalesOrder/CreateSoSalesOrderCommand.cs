using Application.Response;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.SoSalesOrder
{
    public class CreateSoSalesOrderCommand : IRequest<SoSalesOrderResponse>
    {
        public long SoSalesOrderId { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string SignOrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public long? SoCustomerId { get; set; }
        public DateTime? PrioityDeliveryDate { get; set; }
        public string Remark { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public Guid? SessionId { get; set; }

        public CreateSoSalesOrderCommand()
        {
        
        }
    }
}
