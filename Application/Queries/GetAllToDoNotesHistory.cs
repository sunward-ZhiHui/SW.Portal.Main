using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllToDoNotesHistory : PagedRequest, IRequest<List<ToDoNotesHistory>>
    {
    }
    public class GetMyToDoDueDate : PagedRequest, IRequest<List<ToDoNotesHistory>>
    {
        public long UserId { get; set; }
        public GetMyToDoDueDate(long userID)
        {
            this.UserId = userID;
        }
    }

    public class GetMyToDoRemainderDate : PagedRequest, IRequest<List<ToDoNotesHistory>>
    {
   
        public long UserId { get; set; }
        public GetMyToDoRemainderDate(long userID)
        {            
            this.UserId = userID;
        }
    }

    public class GetByToDoNotesHistory : PagedRequest, IRequest<List<ToDoNotesHistory>>
    {
        public long NotesId { get; set; }
        public long UserId { get; set; }
        public GetByToDoNotesHistory(long notesId,long userID)
        {
            this.NotesId = notesId;
            this.UserId = userID;
        }
    }
    public class GetByToDoDocuments : PagedRequest, IRequest<List<Documents>>
    {       
        public Guid SessionId { get; set; }
        public GetByToDoDocuments(Guid SessionId)
        {
            this.SessionId = SessionId;            
        }
    }

    public class CreateToDoNotesHistoryQuery : ToDoNotesHistory, IRequest<long>
    {
    }
    public class EditToDoNotesHistoryQuery : ToDoNotesHistory, IRequest<long>
    {
    }

    public class DeleteToDoNotesHistoryQuery : PagedRequest, IRequest<long>
    {
        public long ID { get; set; }
        public DeleteToDoNotesHistoryQuery(long ID)
        {
            this.ID = ID;
        }
    }
    public class GetUserList : PagedRequest, IRequest<List<ViewEmployee>>
    {

        public string UserId { get; set; }
        public GetUserList(string userID)
        {
            this.UserId = userID;
        }
    }
}
