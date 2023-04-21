using System.Collections.Generic;
using System.Threading.Tasks;
using AC.ShippingDocument.Data;

namespace AC.ShippingDocument.DataProviders {
    public interface IHistogramDataProvider {
        public Task<IEnumerable<DataPoint>> GetNormalDistribution();
        public Task<IEnumerable<DataPoint>> GenerateData();
    }
}
