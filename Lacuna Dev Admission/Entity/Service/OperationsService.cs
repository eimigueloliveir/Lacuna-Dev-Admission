using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
            Console.WriteLine(respose.Job.Type);
            switch (respose.Job.Type)
            {
                case "DecodeStrand":
                    Console.WriteLine("Decode Strand...");
                    Console.WriteLine(respose.Job.Id);
                    Console.WriteLine(respose.Job.StrandEncoded);
                    Response decoderesponse = await DecodeJob(respose.Job.Id, respose.Job.StrandEncoded);
                    Console.WriteLine($"Decode Response Code: {decoderesponse.Code}");
                    Console.WriteLine($"Decode Response Message: {decoderesponse.Message}");
                    break;
                /*case "EncodeStrand":
                    
                    Console.WriteLine("\nEncode...");
                    Console.WriteLine(respose.Job.Strand);
                    
                    Response encoderesponse = await EncodeJob(respose.Job.Id, respose.Job.Strand);
                    Console.WriteLine($"Encode Response Code: {encoderesponse.Code}");
                    Console.WriteLine($"Encode Response Message: {encoderesponse.Message}");
                    break
                /*case "CheckGene":
                    bool Activated = CheckGeneActive(respose.Job.Strand);
                    Console.WriteLine("\nCheck Gene...");
                    Response checkresponse = await CheckGene(respose.Job.Id, Activated);
                    Console.WriteLine($"Check Gene Response Code: {checkresponse.Code}");
                    Console.WriteLine($"Check Gene Response Message: {checkresponse.Message}");
                    break;*/
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
            string decodedStrand = encoserv.StringToBinaryBase64(StrandEncoded);
            Console.WriteLine(decodedStrand);
            Dictionary<string, string> requestBody = new()
                {
                    { "strand", StrandEncoded }
                };
            HttpResponseMessage response = await client.PostAsync($"/api/dna/jobs/{id}/decode", new FormUrlEncodedContent(requestBody));
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            return JsonConvert.DeserializeObject<Response>(responseContent);
        }
        private static async Task<Response> EncodeJob(string id, string Strand)
        {
            EncodingService encoserv = new();
            string strandbase64 = encoserv.StringToBinaryBase64(Strand);
            Console.WriteLine(strandbase64);
            Dictionary<string, string> requestBody = new()
            {
                { "strandEncoded", strandbase64 }
                };
            var response = await client.PostAsync($"/api/dna/jobs/{id}/encode", new FormUrlEncodedContent(requestBody));
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
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
