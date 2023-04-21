using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data;

namespace AC.ShippingDocument.Services {
    public partial class RentInfoDataService {
        public IQueryable<AreaRentInfo> GetAreaRentInfo() {
            // Return your data here
            /*BeginHide*/
            return _dataProvider.GetAreaRentInfo();
            /*EndHide*/
        }
    }
}
