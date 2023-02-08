using Newtonsoft.Json;
using System.Text;

namespace Lacuna_Dev_Admission.Entity.Service
{
    public class OperationsService
    {
        private static readonly HttpClient client = new()
        {
            BaseAddress = new Uri("https://gene.lacuna.cc/")
        };
        public async Task OperationJobs()
        {
            Console.Clear();
            Console.WriteLine("Get Jobs...");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {UserService.GetToken()}");
            ResponseJobsDna? respose = await GetJobs();
            Console.WriteLine("\nDesencode...");
            Response decoderesponse = await DecodeJob(respose.job.id, respose.job.strandEncoded);
            Console.WriteLine($"Decode Response Code: {decoderesponse.code}");
            Console.WriteLine($"Decode Response Message: {decoderesponse.message}");
            EncodingService encoserv = new();
            string decodedStrand = encoserv.DecodeBase64Strand(respose.job.strandEncoded);
            Console.WriteLine("\nEncode...");
            Response encoderesponse = await EncodeJob(respose.job.id, decodedStrand);
            Console.WriteLine($"Encode Response Code: {encoderesponse.code}");
            Console.WriteLine($"Encode Response Message: {encoderesponse.message}");
            bool Activated = CheckGeneActive(decodedStrand);
            Console.WriteLine("\nCheck Gene...");
            Response checkresponse = await CheckGene(respose.job.id, Activated);
            Console.WriteLine($"Check Gene Response Code: {checkresponse.code}");
            Console.WriteLine($"Check Gene Response Message: {checkresponse.message}");
        }
        private static async Task<ResponseJobsDna> GetJobs()
        {
            string responseContent = await client.GetStringAsync("/api/dna/jobs");
            ResponseJobsDna jobs = JsonConvert.DeserializeObject<ResponseJobsDna>(responseContent);
            return jobs;
        }
        private static async Task<Response> DecodeJob(string id, string strand)
        {

            byte[] base64EncodedBytes = Convert.FromBase64String(strand);
            string genedecode = Encoding.UTF8.GetString(base64EncodedBytes);
            Dictionary<string, string> requestBody = new()
                {
                    { "strand", genedecode }
                };
            HttpResponseMessage response = await client.PostAsync($"/api/dna/jobs/{id}/decode", new FormUrlEncodedContent(requestBody));
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Response>(responseContent);


        }
        private static async Task<Response> EncodeJob(string id, string strandEncoded)
        {
            StringContent content = new(JsonConvert.SerializeObject(new
            {
                strandEncoded
            }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/dna/jobs/{id}/encode", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Response>(responseContent);
        }
        private static async Task<Response> CheckGene(string id, bool isActivated)
        {
            Dictionary<string, string> requestBody = new()
                {
                    { "isActivated", isActivated.ToString() }
                };
            var response = await client.PostAsync($"/api/dna/jobs/{id}/gene", new FormUrlEncodedContent(requestBody));
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Response>(responseContent);
        }
        private static bool CheckGeneActive(string gene)
        {
            string dnaTemplateStrand = "CATCTCAGTCCTACTAAACTCGCGAAGCTCATACTAGCTACTAAACCGCTAGACTGCATGATCGCATAGCTAGCTACGCT";
            int count = 0;
            for (int i = 0; i < gene.Length; i++)
            {
                int index = dnaTemplateStrand.IndexOf(gene[i]);
                if (index != -1)
                {
                    count++;
                    dnaTemplateStrand = dnaTemplateStrand.Remove(index, 1);
                }
            }

            return count >= (gene.Length / 2);
        }
    }
}
