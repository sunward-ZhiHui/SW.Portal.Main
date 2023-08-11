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
    public class GetAllToDoNotesHistoryHandler : IRequestHandler<GetAllToDoNotesHistory, List<ToDoNotesHistory>>
    {
        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;        
        public GetAllToDoNotesHistoryHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }
        public async Task<List<ToDoNotesHistory>> Handle(GetAllToDoNotesHistory request, CancellationToken cancellationToken)
        {   
            return (List<ToDoNotesHistory>)await _ToDoNotesHistoryQueryRepository.GetAllAsync();
        }
    }
    public class GetByToDoNotesHistoryHandler : IRequestHandler<GetByToDoNotesHistory, List<ToDoNotesHistory>>
    {

        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public GetByToDoNotesHistoryHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }
        public async Task<List<ToDoNotesHistory>> Handle(GetByToDoNotesHistory request, CancellationToken cancellationToken)
        {
            return (List<ToDoNotesHistory>)await _ToDoNotesHistoryQueryRepository.GetAllToDoNotesHistoryAsync(request.NotesId,request.UserId);
            
        }
    }
    public class CreateToDoNotesHistoryHandler : IRequestHandler<CreateToDoNotesHistoryQuery, long>
    {
        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public CreateToDoNotesHistoryHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }

        public async Task<long> Handle(CreateToDoNotesHistoryQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _ToDoNotesHistoryQueryRepository.Insert(request);
            return newlist;
        }
    }

    public class EditToDoNotesHistoryHandler : IRequestHandler<EditToDoNotesHistoryQuery, long>
    {
        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public EditToDoNotesHistoryHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }

        public async Task<long> Handle(EditToDoNotesHistoryQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _ToDoNotesHistoryQueryRepository.UpdateAsync(request);
            return newlist;
        }
    }

    public class DeleteToDoNotesHistoryHandler : IRequestHandler<DeleteToDoNotesHistoryQuery, long>
    {
        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public DeleteToDoNotesHistoryHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }

        public async Task<long> Handle(DeleteToDoNotesHistoryQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _ToDoNotesHistoryQueryRepository.DeleteAsync(request.ID);
            return newlist;
        }
    }
}
