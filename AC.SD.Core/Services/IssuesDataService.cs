using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data.Issues;
using AC.ShippingDocument.DataProviders;

namespace AC.ShippingDocument.Services {
    public partial class IssuesDataService {
        readonly IIssuesDataProvider _dataProvider;

        public IssuesDataService(IIssuesDataProvider dataProvider) {
            _dataProvider = dataProvider;
        }
    }
}
