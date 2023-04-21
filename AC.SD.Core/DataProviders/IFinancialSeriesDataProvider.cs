using System.Collections.Generic;
using System.Threading.Tasks;
using AC.ShippingDocument.Data;

namespace AC.ShippingDocument.DataProviders {
    public interface IFinancialSeriesDataProvider {
        public Task<IEnumerable<StockDataPoint>> Generate();
    }
}
