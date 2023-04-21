using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data.Northwind;
using AC.ShippingDocument.DataProviders;

namespace AC.ShippingDocument.Services {
    public partial class NwindDataService {
        public Task<IEnumerable<Shipper>> GetShippersAsync(CancellationToken ct = default) {
            // Return your data here
            /*BeginHide*/
            return _dataProvider.GetShippersAsync(ct);
            /*EndHide*/
        }
    }
}
