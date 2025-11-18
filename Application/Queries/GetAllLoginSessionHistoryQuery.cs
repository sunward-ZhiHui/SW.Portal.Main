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
    public class GetAllLoginSessionHistoryQuery : PagedRequest, IRequest<List<LoginSessionHistory>>
    {
        public string? SearchString { get; set; }
    }


    public class InsertLoginSessionHistory : PagedRequest, IRequest<LoginSessionHistory>
    {
        public LoginSessionHistory LoginSessionHistory { get; set; }
        public InsertLoginSessionHistory(LoginSessionHistory loginSessionHistory)
        {
            LoginSessionHistory = loginSessionHistory;
        }
    }
    public class UpdateLastActivity : PagedRequest, IRequest<LoginSessionHistory>
    {
        public LoginSessionHistory LoginSessionHistory { get; set; }
        public UpdateLastActivity(LoginSessionHistory loginSessionHistory)
        {
            LoginSessionHistory = loginSessionHistory;
        }
    }
    public class UpdateLogOutActivity : PagedRequest, IRequest<LoginSessionHistory>
    {
        public LoginSessionHistory LoginSessionHistory { get; set; }
        public UpdateLogOutActivity(LoginSessionHistory loginSessionHistory)
        {
            LoginSessionHistory = loginSessionHistory;
        }
    }
    public class GetLoginSessionHistoryOne : PagedRequest, IRequest<LoginSessionHistory>
    {
        public Guid? SessionId { get; set; }
        public long? UserId { get; set; }
        public GetLoginSessionHistoryOne(Guid? sessionid, long? userId)
        {
            SessionId = sessionid;
            UserId = userId;
        }
    }

}
