using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data.Worldcities;
using AC.ShippingDocument.DataProviders;

namespace AC.ShippingDocument.Services {
    public partial class WorldcitiesDataService {
        readonly IWorldcitiesDataProvider _dataProvider;

        public WorldcitiesDataService(IWorldcitiesDataProvider dataProvider) {
            _dataProvider = dataProvider;
        }
    }
}
