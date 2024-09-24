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
    public interface IToDoNotesQueryRepository : IQueryRepository<ToDoNotes>
    {
        Task<IReadOnlyList<ToDoNotes>> GetAllAsync(long userId);        
        Task<IReadOnlyList<ToDoNotes>> GetAllToDoNotesAsync(long UserId,long Topicid);
        Task<IReadOnlyList<ToDoNotes>> GetAllNotesAsync(long UserId, string notes);
        Task<long> Insert(ToDoNotes ToDoNotes);
        Task<long> UpdateAsync(ToDoNotes ToDoNotes);
        Task<string> UpdateNoteAsync(string selectNotes,string Notes,long userid);
        Task<long> DeleteAsync(long id);
        Task<long> Delete(long id);
        Task <long> IncompleteAsync( long incompleteid);
    }
}
