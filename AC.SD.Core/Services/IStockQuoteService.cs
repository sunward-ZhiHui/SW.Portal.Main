using System;
using AC.ShippingDocument.Data.StockQuotes;

namespace AC.ShippingDocument.Services {
    public interface IStockQuoteService {
        event EventHandler<StockQuoteChangedEventArgs> StockQuoteChanged;
    }
}
