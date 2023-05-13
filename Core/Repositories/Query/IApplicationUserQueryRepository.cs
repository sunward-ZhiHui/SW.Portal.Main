using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IApplicationUserQueryRepository : IQueryRepository<ApplicationUser>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<ApplicationUser>> GetAllAsync();
        Task Initialize();
        ApplicationUser User { get; }
        Task<ApplicationUser> LoginAuth(string LoginID, string Password);
        Task<ApplicationUser> Auth(string LoginID, string Password);
        Task<ApplicationUser> GetByIdAsync(Int64 id);
        Task<ApplicationUser> GetByUsers(string LoginID);
        Task<ApplicationUser> UpdatePasswordUser(long UserID, string NewPassword, string OldPassword, string LoginID);
        Task<ApplicationUser> ForGotPasswordUser(string LoginID, string NewPassword);
        Task<ApplicationUser> UnLockedPassword(string LoginID, string NewPassword);
        

    }
    public interface IApplicationUserRoleQueryRepository : IQueryRepository<ApplicationUserRole>
    {

        Task<ApplicationUserRole> GetByIdAsync(Int64 id);

    }
}
