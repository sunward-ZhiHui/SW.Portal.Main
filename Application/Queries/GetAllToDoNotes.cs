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
    public class GetAllToDoNotes : PagedRequest, IRequest<List<ToDoNotes>>
    {
    }
    public class GetByToDoNotes : PagedRequest, IRequest<List<ToDoNotes>>
    {
        public long UserID { get; set; }
        public GetByToDoNotes(long UserID)
        {
            this.UserID = UserID;
        }
    }

    public class CreateToDoNotesQuery : ToDoNotes, IRequest<long>
    {
    }
    public class EditToDoNotesQuery : ToDoNotes, IRequest<long>
    {
    }

    public class DeleteToDoNotesQuery : PagedRequest, IRequest<long>
    {
        public long ID { get; set; }
        public DeleteToDoNotesQuery(long ID)
        {
            this.ID = ID;
        }
    }
    public class IncompleteTodoNoteQuery : PagedRequest, IRequest<long>
    {
        public long incompleteID { get; set; }
        public IncompleteTodoNoteQuery(long incompleteID)
        {
            this.incompleteID = incompleteID;
        }
    }
    
}
