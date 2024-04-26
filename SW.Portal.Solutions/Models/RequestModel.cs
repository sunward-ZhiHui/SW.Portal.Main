using System.Text.Json;

namespace SW.Portal.Solutions.Models
{
    public class RequestModel
    {
        public IDictionary<string, JsonElement> DynamicsData { get; set; }
        public long Id { get; set; }
    }
}
