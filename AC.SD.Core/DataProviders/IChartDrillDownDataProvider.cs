using System.Collections.Generic;
using AC.ShippingDocument.Data;

namespace AC.ShippingDocument.DataProviders {
    public interface IChartDrillDownDataProvider {
        public List<SaleItem> Generate();
    }
}
