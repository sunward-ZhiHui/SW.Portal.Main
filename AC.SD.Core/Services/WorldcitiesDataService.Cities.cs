using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data.Worldcities;
using AC.ShippingDocument.DataProviders;

namespace AC.ShippingDocument.Services {
    public partial class WorldcitiesDataService {
        public Task<IEnumerable<City>> GetCitiesAsync(CancellationToken ct = default) {
            // Return your data here
            /*BeginHide*/
            return _dataProvider.GetCitiesAsync(ct);
            /*EndHide*/
        }
    }
}
