using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IToDoNotesHistoryQueryRepository : IQueryRepository<ToDoNotesHistory>
    {
        Task<IReadOnlyList<ToDoNotesHistory>> GetAllAsync();
        Task<IReadOnlyList<ToDoNotesHistory>> GetTodoDueAsync(long UserId);
        Task<IReadOnlyList<ToDoNotesHistory>> GetTodoRemainderAsync(long UserId);        
        Task<IReadOnlyList<ToDoNotesHistory>> GetAllToDoNotesHistoryAsync(long NotesId,long UserId);
        Task<IReadOnlyList<Documents>> GetToDoDocumentsAsync(string SessionId);        
        Task<long> Insert(ToDoNotesHistory ToDoNotesHistory);
        Task<long> UpdateAsync(ToDoNotesHistory ToDoNotesHistory);
        Task<long> DeleteAsync(long id);
        Task<IReadOnlyList<ViewEmployee>> GetUserLst(string Userid);
    }
}
