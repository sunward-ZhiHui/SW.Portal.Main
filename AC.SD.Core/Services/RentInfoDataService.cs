using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data.Northwind;
using AC.ShippingDocument.DataProviders;

namespace AC.ShippingDocument.Services {
    public partial class RentInfoDataService {
        protected readonly IRentInfoDataProvider _dataProvider;

        public RentInfoDataService(IRentInfoDataProvider dataProvider) {
            _dataProvider = dataProvider;
        }

    }
}
