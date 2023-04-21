using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data.Northwind;
using AC.ShippingDocument.DataProviders;

namespace AC.ShippingDocument.Services {
    public partial class NwindDataService {
        protected readonly INwindDataProvider _dataProvider;

        public NwindDataService(INwindDataProvider dataProvider) {
            _dataProvider = dataProvider;
        }
    }
}
