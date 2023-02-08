using Newtonsoft.Json;

namespace Lacuna_Dev_Admission.Entity
{
    internal class ResponseJobsDna
    {
        public string code { get; set; }
        public string? message { get; set; }
        [JsonProperty("job")]
        public Jobs? job { get; set; }
    }
    public class Jobs
    {
        public string id { get; set; }
        public string type { get; set; }
        [JsonProperty("strand")]
        public string strand { get; set; }
        public string strandEncoded { get; set; }
        public string geneEncoded { get; set; }
    }
}
