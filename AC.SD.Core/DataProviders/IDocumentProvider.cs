using System.Threading;
using System.Threading.Tasks;

namespace AC.ShippingDocument.DataProviders {
    public interface IDocumentProvider {
        public Task<byte[]> GetDocumentAsync(string name, CancellationToken cancellationToken = default(CancellationToken));
    }
}
