using System.Collections.Generic;
using System.Threading.Tasks;
using AC.ShippingDocument.Data;

namespace AC.ShippingDocument.DataProviders {
    public interface IWeatherSummaryDataProvider {
        public Task<IEnumerable<DetailedWeatherSummary>> GetDataAsync(bool aggregateByMonth = false);
    }
}
