using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewEmployeeModel
    {
        public long EmployeeID { get; set; }
        public long? UserID { get; set; }
        public string? FirstName { get; set; }
      //  public string? LastName { get; set; }
       // public string? NickName { get; set; }
      //  public string? UserCode { get; set; }
        public string? LoginID { get; set; }
        public string? Name { get; set; }
    }
}
