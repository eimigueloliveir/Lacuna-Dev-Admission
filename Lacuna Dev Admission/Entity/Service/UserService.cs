using Newtonsoft.Json;
using System;
using System.Text;

namespace Lacuna_Dev_Admission.Entity.Service
{
    public class UserService
    {
        private static readonly HttpClient client = new()
        {
            BaseAddress = new Uri("https://gene.lacuna.cc/")
        };

        public async Task CreateUser()
        {
            User user = new();
            Console.WriteLine("Username: ");
            user.username = Console.ReadLine();
            Console.WriteLine("Email: ");
            user.email = Console.ReadLine();
            Console.WriteLine("Password: ");
            user.password = Console.ReadLine();
            Console.WriteLine("Criando Usuario...");
            ResponseCreateUser respose = await CreateUserAsync(user);
            Console.WriteLine(respose.Code);
            Console.WriteLine(respose.Message);
        }
        public async Task LoginUser()
        {
            LoginUser login = new();
            Console.Clear();
            Console.WriteLine("Login");
            Console.WriteLine("Username: ");
            login.username = Console.ReadLine();
            Console.WriteLine("Password: ");
            login.password = Console.ReadLine();
            Console.WriteLine("Logando...");
            Response response = await LoginUserAsync(login);
            if (response.Code == "Success")
            {
                Environment.SetEnvironmentVariable("Token", response.AccessToken);
                Console.WriteLine(response.AccessToken);
                Console.WriteLine("Logado com sucesso.");
            }
            else
            {
                Console.WriteLine("Falha ao logar.");
                Console.WriteLine("Pressione qualquer tecla para continuar.");
                Console.ReadKey();
                Console.Clear();
            }
        }
        private async Task<ResponseCreateUser> CreateUserAsync(User user)
        {
            StringContent content = new(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("/api/users/create", content);
            string responseContent = await response.Content.ReadAsStringAsync();
            ResponseCreateUser responseUser = JsonConvert.DeserializeObject<ResponseCreateUser>(responseContent);
            return responseUser;
        }
        private static async Task<Response> LoginUserAsync(LoginUser user)
        {
            StringContent content = new(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("/api/users/login", content);
            string responseContent = await response.Content.ReadAsStringAsync();
            Response responseUser = JsonConvert.DeserializeObject<Response>(responseContent);
            return responseUser;
        }

        public static Task Exit()
        {
            Environment.SetEnvironmentVariable("token", null);
            return Task.CompletedTask;
        }
    }
}
