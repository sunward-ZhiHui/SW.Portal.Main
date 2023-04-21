using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data;

namespace AC.ShippingDocument.Services {
    public partial class ContosoRetailDataService {
        public IQueryable<Sale> GetSales() {
            // Return your data here
            /*BeginHide*/
            return _dataProvider.GetSales();
            /*EndHide*/
        }
    }
}
