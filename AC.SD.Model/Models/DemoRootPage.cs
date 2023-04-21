using System;
using System.Linq;
using Newtonsoft.Json;

namespace AC.SD.Model.DemoData {
    public class DemoRootPage : DemoPageBase {
        public string AnalyticsId { get; set; }

        public override DemoItem[] GetChildItems() { return Pages; }
    }
}
