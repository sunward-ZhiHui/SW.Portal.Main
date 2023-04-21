using System.Collections.Generic;
using System.Threading.Tasks;
using AC.ShippingDocument.Data;

namespace AC.ShippingDocument.DataProviders {
    public interface ICurrencyExchangeDataProvider {
        public Task<IEnumerable<DatePricePoint>> GetDataAsync();
    }
}
