using Application.Response;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.PostSalesOrder
{
    public class EditPostSalesOrderCommand : IRequest<PostSalesOrderResponse>
    {
        public long PostSalesOrderID { get; set; }
        public long? SoSalesOrderID { get; set; }
        public string ItemSerialNo { get; set; }
        public string SignOrderNo { get; set; }
        public string SalesOrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderBy { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public EditPostSalesOrderCommand()
        {
        }
    }
}
