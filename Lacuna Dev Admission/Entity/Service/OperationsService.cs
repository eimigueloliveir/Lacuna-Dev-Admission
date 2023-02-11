using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Lacuna_Dev_Admission.Entity.Service
{
    public class OperationsService
    {
        private static readonly HttpClient client = new()
        {
            BaseAddress = new Uri("https://gene.lacuna.cc/"),

        };
        public async Task OperationJobs()
        {
            Console.Clear();
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Token")))
            {
                Console.WriteLine("You need to login first");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Get Jobs...");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("Token"));
            ResponseJobsDna? respose = await GetJobs();

            // Operation types ['DecodeStrand', 'EncodeStrand', 'CheckGene']
            Console.WriteLine($"Job received: {respose.Job.Type}");
            switch (respose.Job.Type)
            {
                case "DecodeStrand":
                    Console.WriteLine("Decode Strand...");
                    Response decoderesponse = await DecodeJob(respose.Job.Id, respose.Job.StrandEncoded);
                    Console.WriteLine($"Decode Response Code: {decoderesponse.Code}");
                    if (decoderesponse.Code != "Success")
                    {
                        Console.WriteLine($"Erro: {decoderesponse.Message}");
                    }
                    break;
                case "EncodeStrand":

                    Console.WriteLine("Encode...");
                    Response encoderesponse = await EncodeJob(respose.Job.Id, respose.Job.Strand);
                    Console.WriteLine($"Encode Response Code: {encoderesponse.Code}");
                    if (encoderesponse.Code != "Success")
                    {
                        Console.WriteLine($"Erro: {encoderesponse.Message}");
                    }
                    break;
                case "CheckGene":

                    Console.WriteLine("Check Gene...");
                    Response checkresponse = await CheckGene(respose.Job.Id, respose.Job.GeneEncoded, respose.Job.StrandEncoded);
                    Console.WriteLine($"Check Gene Response Code: {checkresponse.Code}");
                    if (checkresponse.Code != "Success")
                    {
                        Console.WriteLine($"Erro: {checkresponse.Message}");
                    }
                    break;
            }
        }
        private static async Task<ResponseJobsDna> GetJobs()
        {
            string responseContent = await client.GetStringAsync("/api/dna/jobs");
            ResponseJobsDna jobs = JsonConvert.DeserializeObject<ResponseJobsDna>(responseContent);
            return jobs;
        }
        private static async Task<Response> DecodeJob(string id, string StrandEncoded)
        {
            EncodingService encoserv = new();
            string decodedStrand = encoserv.StringBase64ToBinaryString(StrandEncoded);
            var json = JsonConvert.SerializeObject(new { strand = decodedStrand });
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"/api/dna/jobs/{id}/decode", content);
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Response>(responseContent);
        }
        private static async Task<Response> EncodeJob(string id, string Strand)
        {
            EncodingService encoserv = new();
            string strandbase64 = encoserv.BinaryStringToString(Strand);
            var json = JsonConvert.SerializeObject(new { strandEncoded = strandbase64 });
            StringContent content = new(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/dna/jobs/{id}/encode", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Response>(responseContent);
        }
        private static async Task<Response> CheckGene(string id, string geneEncoded, string strandEncoded)
        {
            EncodingService encoserv = new();
            string genedecode = encoserv.StringBase64ToBinaryString(geneEncoded);
            string stranddecode = encoserv.StringBase64ToBinaryString(strandEncoded);
            bool isActivated = CheckGeneActive(genedecode, stranddecode);
            Console.WriteLine(isActivated);
            var json = JsonConvert.SerializeObject(new { isActivated });
            StringContent content = new(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/dna/jobs/{id}/gene", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            return JsonConvert.DeserializeObject<Response>(responseContent);
        }
        private static bool CheckGeneActive(string gene, string strand)
        {
            if (!strand.StartsWith("CAT"))
            {
                strand = GetComplementaryStrand(strand);
            }
            string GetComplementaryStrand(string strand)
            {
                StringBuilder complementaryStrand = new StringBuilder();
                foreach (char nucleobase in strand)
                {
                    switch (nucleobase)
                    {
                        case 'A':
                            complementaryStrand.Append('T');
                            break;
                        case 'T':
                            complementaryStrand.Append('A');
                            break;
                        case 'C':
                            complementaryStrand.Append('G');
                            break;
                        case 'G':
                            complementaryStrand.Append('C');
                            break;
                        default:
                            break;
                    }
                }
                return complementaryStrand.ToString();
            }
            bool IsGeneActivated(string gene, string dnaTemplateStrand)
            {
                int geneLength = gene.Length;
                int matchCount = 0;
                for (int i = 0; i < dnaTemplateStrand.Length - geneLength + 1; i++)
                {
                    if (dnaTemplateStrand.Substring(i, geneLength) == gene)
                    {
                        matchCount++;
                    }
                }
                return (double)matchCount / (double)dnaTemplateStrand.Length >= 0.5;
            }
            return IsGeneActivated(gene, strand);
        }
    }
}
