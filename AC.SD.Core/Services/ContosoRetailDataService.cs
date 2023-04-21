using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
 
using AC.ShippingDocument.DataProviders;

namespace AC.ShippingDocument.Services {
    public partial class ContosoRetailDataService {
        protected readonly IContosoRetailDataProvider _dataProvider;

        public ContosoRetailDataService(IContosoRetailDataProvider dataProvider) {
            _dataProvider = dataProvider;
        }

    }
}
