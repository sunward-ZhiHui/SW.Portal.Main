using System;
using System.Threading.Tasks;

namespace AC.SD.Core.DataProviders {
    public interface IDataProvider {
        Task<IObservable<int>> GetLoadingStateAsync();
    }
}
