using Newtonsoft.Json;

namespace Lacuna_Dev_Admission.Entity
{
    internal class ResponseJobsDna
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public Jobs Job { get; set; }
    }
    public class Jobs
    {
        public string Id { get; set; }
        public string Type { get; set; }
        [JsonProperty("strand")]
        public string Strand { get; set; }
        public string StrandEncoded { get; set; }
        public string GeneEncoded { get; set; }
    }
}
