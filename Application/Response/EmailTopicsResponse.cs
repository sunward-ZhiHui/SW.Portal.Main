using Core.Entities;
using Core.Entities.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class EmailTopicsResponse : BaseEntity
    {
        public long ID { get; set; }
        public string? TicketNo { get; set; }       
        public string? TopicName { get; set; }
        public long TypeId { get; set; }
        public long CategoryId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public long TopicFrom { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Participants { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int SeqNo { get; set; }
        public string? SubjectName { get; set; }
        public string? label { get; set; }
        public byte[] FileData { get; set; }
        public string? Follow { get; set; }
        public string? OnBehalf { get; set; }
        public bool? Urgent { get; set; }
        public bool? OverDue { get; set; }
        public List<EmailTopicsResponse>? TopicList { get; set; }        

    }
}
