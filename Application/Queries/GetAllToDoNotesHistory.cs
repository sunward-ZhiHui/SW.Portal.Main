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
}
