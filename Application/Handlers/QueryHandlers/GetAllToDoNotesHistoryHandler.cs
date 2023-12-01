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
    public class GetToDoDueDateHandler : IRequestHandler<GetMyToDoDueDate, List<ToDoNotesHistory>>
    {
        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public GetToDoDueDateHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }
        public async Task<List<ToDoNotesHistory>> Handle(GetMyToDoDueDate request, CancellationToken cancellationToken)
        {
            return (List<ToDoNotesHistory>)await _ToDoNotesHistoryQueryRepository.GetTodoDueAsync(request.UserId);
        }
    }
    public class GetToDoRemainderDateHandler : IRequestHandler<GetMyToDoRemainderDate, List<ToDoNotesHistory>>
    {
        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public GetToDoRemainderDateHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }
        public async Task<List<ToDoNotesHistory>> Handle(GetMyToDoRemainderDate request, CancellationToken cancellationToken)
        {
            return (List<ToDoNotesHistory>)await _ToDoNotesHistoryQueryRepository.GetTodoRemainderAsync(request.UserId);
        }
    }
    public class GetMyToDoHandler : IRequestHandler<GetMyToDo, List<ToDoNotesHistory>>
    {
        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public GetMyToDoHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }
        public async Task<List<ToDoNotesHistory>> Handle(GetMyToDo request, CancellationToken cancellationToken)
        {
            return (List<ToDoNotesHistory>)await _ToDoNotesHistoryQueryRepository.GetMyToDoAsync(request.UserId);
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
    public class GetByToDoSessionIdHandler : IRequestHandler<GetByToDoSessionId, List<ToDoNotesHistory>>
    {

        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public GetByToDoSessionIdHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }
        public async Task<List<ToDoNotesHistory>> Handle(GetByToDoSessionId request, CancellationToken cancellationToken)
        {
            return (List<ToDoNotesHistory>)await _ToDoNotesHistoryQueryRepository.GetByToDoSessionIdAsync(request.SessionId);

        }
    }
    
    public class GetByToDoDocumentsHandler : IRequestHandler<GetByToDoDocuments, List<Documents>>
    {

        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public GetByToDoDocumentsHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }
        public async Task<List<Documents>> Handle(GetByToDoDocuments request, CancellationToken cancellationToken)
        {
            return (List<Documents>)await _ToDoNotesHistoryQueryRepository.GetToDoDocumentsAsync(request.SessionId);

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
            var nslineData = request.UserIds.ToList();
            if (nslineData.Count > 0)
            {
                request.UserIds.ToList().ForEach(async a =>
                {
                    var lineItem = new ToDoNotesUsers();
                    lineItem.NotesHistoryID = newlist;
                    lineItem.UserID = a;
                    lineItem.AddedByUserID = request.AddedByUserID.Value;
                    lineItem.AddedDate = request.AddedDate;
                   
                    await _ToDoNotesHistoryQueryRepository.InsertToDoNotesUsersAsync(lineItem);
                });
            }


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
            var existUser = !string.IsNullOrEmpty(request.Users);
            var userIdList = request.UserIds?.ToList() ?? new List<long>();

            List<long> existList;
            List<long> notExistList;
            List<long> removedList;

            if (existUser)
            {
                // Convert the Users string to a list of long
                var existingUserIds = request.Users.Split(',').Select(long.Parse).ToList();

                // Find the intersection (common elements) between existingUserIds and userIdList
                existList = existingUserIds.Intersect(userIdList).ToList();

                // Find the difference (elements in userIdList but not in existingUserIds)
                notExistList = userIdList.Except(existingUserIds).ToList();

                removedList = existingUserIds.Except(userIdList).ToList();
            }
            else
            {
                // If Users is null or empty, consider all userIdList as not existing
                existList = new List<long>();
                notExistList = userIdList;
                removedList = userIdList;
            }
            

            var insertlst = notExistList;
            var removelst = removedList;

            var removelstData = removelst.ToList();
            if (removelstData.Count > 0)
            {
                removelst.ToList().ForEach(async a =>
                {
                    var lineItem = new ToDoNotesUsers();
                    lineItem.NotesHistoryID = request.ID;
                    lineItem.UserID = a;                    
                    await _ToDoNotesHistoryQueryRepository.ToDoNotesUsersDeleteAsync(lineItem.NotesHistoryID,lineItem.UserID);
                });
            }


            var newlist = await _ToDoNotesHistoryQueryRepository.UpdateAsync(request);

            var inslineData = insertlst.ToList();
            if (inslineData.Count > 0)
            {
                insertlst.ToList().ForEach(async a =>
                {
                    var lineItem = new ToDoNotesUsers();
                    lineItem.NotesHistoryID = request.ID;
                    lineItem.UserID = a;
                    lineItem.AddedByUserID = request.AddedByUserID.Value;
                    lineItem.AddedDate = request.AddedDate;
                    await _ToDoNotesHistoryQueryRepository.InsertToDoNotesUsersAsync(lineItem);
                });
            }

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
    public class GetUserListHandler : IRequestHandler<GetUserList, List<ViewEmployee>>
    {

        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public GetUserListHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetUserList request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _ToDoNotesHistoryQueryRepository.GetUserLst(request.UserId);

        }
    }

    public class StatusChangedToDoNotesHistoryHandler : IRequestHandler<StatusChangedQuery, long>
    {
        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public StatusChangedToDoNotesHistoryHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }

        public async Task<long> Handle(StatusChangedQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _ToDoNotesHistoryQueryRepository.StatusUpdateAsync(request.ID);
            return newlist;
        }
    }

    public class StatusChangedByUsersHistoryHandler : IRequestHandler<StatusChangedByUsersQuery, long>
    {
        private readonly IToDoNotesHistoryQueryRepository _ToDoNotesHistoryQueryRepository;
        public StatusChangedByUsersHistoryHandler(IToDoNotesHistoryQueryRepository ToDoNotesHistoryQueryRepository)
        {
            _ToDoNotesHistoryQueryRepository = ToDoNotesHistoryQueryRepository;
        }

        public async Task<long> Handle(StatusChangedByUsersQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _ToDoNotesHistoryQueryRepository.StatusUpdateNotesUsersAsync(request.ID);
            return newlist;
        }
    }
    
}
