using Core.Repositories.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AC.SD.Core.Services
{
    public class ChartService
    {
        private readonly IChartRepository _repo;
        public ChartService(IChartRepository repo)
        {
            _repo = repo;
        }

        public Task<List<string>> GetTableListAsync() => _repo.GetTableListAsync();
        public Task<List<string>> GetColumnListAsync(string tableName) => _repo.GetColumnListAsync(tableName);
        public Task<List<string>> GetNumericColumnsAsync(string table) => _repo.GetNumericColumnListAsync(table);
        public Task<List<dynamic>> GetDynamicRawTableDataAsync(string table) => _repo.GetDynamicRawTableDataAsync(table);


        public async Task<List<(double X, double Y)>> GetChartDataAsync(string table, string xField, string yField)
        {
            var rows = await _repo.GetDynamicDataAsync(table, xField, yField);

            return rows.Select(r =>
            {
                double xVal = Convert.ToDouble(r.X);
                double yVal = Convert.ToDouble(r.Y);
                return (xVal, yVal);
            }).ToList();
        }

        //    public async Task<List<(double X, double Y, double? Z)>> GetChartDataAsync(
        //string table, string xField, string yField, string lineField = null)
        //    {
        //        var rows = await _repo.GetDynamicDataAsync(table, xField, yField, lineField);

        //        return rows.Select(r =>
        //        {
        //            double xVal = Convert.ToDouble(r.X);
        //            double yVal = Convert.ToDouble(r.Y);

        //            double? zVal = null;
        //            if (!string.IsNullOrEmpty(lineField) && r.Z != null)
        //            {
        //                if (double.TryParse(r.Z.ToString(), out double tmp)) // ✅ FIX
        //                {
        //                    zVal = tmp;
        //                }
        //            }

        //            return (xVal, yVal, zVal);
        //        }).ToList();
        //    }






    }

}
