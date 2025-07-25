using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IChartRepository
    {
        Task<List<dynamic>> GetDynamicDataAsync(string tableName, string xField, string yField);
        Task<List<string>> GetTableListAsync();
        Task<List<string>> GetColumnListAsync(string tableName);
        Task<List<string>> GetNumericColumnListAsync(string tableName);
        Task<List<dynamic>> GetDynamicRawTableDataAsync(string tableName);


    }

}
