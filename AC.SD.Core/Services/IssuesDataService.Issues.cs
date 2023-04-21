using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data.Issues;
using AC.ShippingDocument.DataProviders;

namespace AC.ShippingDocument.Services {
    public partial class IssuesDataService {
        public Task<IEnumerable<Issue>> GetIssuesAsync(CancellationToken ct = default) {
            // Return your data here
            /*BeginHide*/
            return _dataProvider.GetIssuesAsync(ct);
            /*EndHide*/
        }
    }
}
