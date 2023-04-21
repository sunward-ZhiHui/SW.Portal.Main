using System.Collections.Generic;
using System.Threading.Tasks;
using AC.ShippingDocument.Data;

namespace AC.ShippingDocument.DataProviders {
    public interface IScatterRandomDataProvider {
        public Task<IEnumerable<DataPoint>> GenerateCluster(int xPlus, int xMinus, int yPlus, int yMinus, int count, int randomSeek);
    }
}

