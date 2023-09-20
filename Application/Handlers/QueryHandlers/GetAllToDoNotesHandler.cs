using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllToDoNotesHandler : IRequestHandler<GetAllToDoNotes, List<ToDoNotes>>
    {
        private readonly IToDoNotesQueryRepository _toDoNotesQueryRepository;        
        public GetAllToDoNotesHandler(IToDoNotesQueryRepository toDoNotesQueryRepository)
        {
            _toDoNotesQueryRepository = toDoNotesQueryRepository;
        }
        public async Task<List<ToDoNotes>> Handle(GetAllToDoNotes request, CancellationToken cancellationToken)
        {   
            return (List<ToDoNotes>)await _toDoNotesQueryRepository.GetAllAsync(request.UserID);
        }
    }
    public class GetByToDoNotesHandler : IRequestHandler<GetByToDoNotes, List<ToDoNotes>>
    {

        private readonly IToDoNotesQueryRepository _toDoNotesQueryRepository;
        public GetByToDoNotesHandler(IToDoNotesQueryRepository toDoNotesQueryRepository)
        {
            _toDoNotesQueryRepository = toDoNotesQueryRepository;
        }
        public async Task<List<ToDoNotes>> Handle(GetByToDoNotes request, CancellationToken cancellationToken)
        {
            return (List<ToDoNotes>)await _toDoNotesQueryRepository.GetAllToDoNotesAsync(request.UserID,request.TopicId);
            
        }
    }
    public class CreateToDoNotesHandler : IRequestHandler<CreateToDoNotesQuery, long>
    {
        private readonly IToDoNotesQueryRepository _toDoNotesQueryRepository;
        public CreateToDoNotesHandler(IToDoNotesQueryRepository toDoNotesQueryRepository)
        {
            _toDoNotesQueryRepository = toDoNotesQueryRepository;
        }

        public async Task<long> Handle(CreateToDoNotesQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _toDoNotesQueryRepository.Insert(request);
            return newlist;
        }
    }

    public class EditToDoNotesHandler : IRequestHandler<EditToDoNotesQuery, long>
    {
        private readonly IToDoNotesQueryRepository _toDoNotesQueryRepository;
        public EditToDoNotesHandler(IToDoNotesQueryRepository toDoNotesQueryRepository)
        {
            _toDoNotesQueryRepository = toDoNotesQueryRepository;
        }

        public async Task<long> Handle(EditToDoNotesQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _toDoNotesQueryRepository.UpdateAsync(request);
            return newlist;
        }
    }

    public class DeleteToDoNotesHandler : IRequestHandler<DeleteToDoNotesQuery, long>
    {
        private readonly IToDoNotesQueryRepository _toDoNotesQueryRepository;
        public DeleteToDoNotesHandler(IToDoNotesQueryRepository toDoNotesQueryRepository)
        {
            _toDoNotesQueryRepository = toDoNotesQueryRepository;
        }

        public async Task<long> Handle(DeleteToDoNotesQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _toDoNotesQueryRepository.DeleteAsync(request.ID);
            return newlist;
        }
    }
    public class IncompleteToDoNotesHandler : IRequestHandler<IncompleteTodoNoteQuery, long>
    {
        private readonly IToDoNotesQueryRepository _toDoNotesQueryRepository;
        public IncompleteToDoNotesHandler(IToDoNotesQueryRepository toDoNotesQueryRepository)
        {
            _toDoNotesQueryRepository = toDoNotesQueryRepository;
        }

        public async Task<long> Handle(IncompleteTodoNoteQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _toDoNotesQueryRepository.IncompleteAsync(request.incompleteID);
            return newlist;
        }
    }
}
