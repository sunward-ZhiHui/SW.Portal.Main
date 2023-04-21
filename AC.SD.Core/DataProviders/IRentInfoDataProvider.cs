using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AC.ShippingDocument.Data;

namespace AC.ShippingDocument.DataProviders {
    [Guid("EC9EA76C-2166-4839-85E2-1B061B6EFBA2")]
    public interface IRentInfoDataProvider : IDataProvider {
        IQueryable<AreaRentInfo> GetAreaRentInfo();
    }
}
