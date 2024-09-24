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
        public long UserID { get; set; }
       
        public GetAllToDoNotes(long UserID)
        {
            this.UserID = UserID;
            
        }
    }
    public class GetByToDoNotes : PagedRequest, IRequest<List<ToDoNotes>>
    {
        public long UserID { get; set; }
        public long TopicId { get; set; }
        public GetByToDoNotes(long UserID, long topicId)
        {
            this.UserID = UserID;
            TopicId = topicId;
        }
    }
    public class GetTopicTodoList : PagedRequest, IRequest<List<ToDoNotes>>
    {
        public long UserID { get; set; }
        public string notes { get; set; }
        public GetTopicTodoList(long UserID, string notes)
        {
            this.UserID = UserID;
           this.notes = notes;
        }
    }
    public class CreateToDoNotesQuery : ToDoNotes, IRequest<long>
    {
    }
    public class EditToDoNotesQuery : ToDoNotes, IRequest<long>
    {
    }
    public class EditNotesQuery : PagedRequest, IRequest<string>
    {
        public string selectNotes { get; set; }
        public string Notes { get; set; }
        public long UserID { get; set; }
        public EditNotesQuery(string selectNotes, string Notes,long UserID)
        {
            this.selectNotes = selectNotes;
            this.Notes = Notes;
            this.UserID = UserID;
        }
    }

    public class DeleteToDoNotesQuery : PagedRequest, IRequest<long>
    {
        public long ID { get; set; }
        public DeleteToDoNotesQuery(long ID)
        {
            this.ID = ID;
        }
    }
    public class GetAllDelete: PagedRequest, IRequest<long>
    {
        public long ID { get; set; }

        public GetAllDelete(long ID)
        {
            this.ID =  ID;

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
