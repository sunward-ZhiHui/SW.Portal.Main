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
    public class ActivityEmailTopicsResponse
    {
        public long ActivityEmailTopicID { get; set; }
        public string SubjectName { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? EmailTopicSessionId { get; set; }
        public string ActivityType { get; set; }
        public long FromId { get; set; }
        public string ToIds { get; set; }
        public string CcIds { get; set; }
        public string Tags { get; set; }

    }
}
