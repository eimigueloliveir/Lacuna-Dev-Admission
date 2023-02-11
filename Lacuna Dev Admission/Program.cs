using Lacuna_Dev_Admission.Service;

namespace Lacuna_Dev_Admission
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Lacuna Dev Admission");
            UserService service = new();
            while (true)
            {
                Console.WriteLine("1. Create User");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Operations");
                Console.WriteLine("4. Exit");
                string option = Console.ReadLine();
                if (option != "1" && option != "2" && option != "3")
                {
                    Console.Clear();
                    Console.WriteLine("Invalid option");
                    continue;
                }

                switch (option)
                {
                    case "1":
                        service.CreateUser().Wait();
                        break;
                    case "2":
                        service.LoginUser().Wait();
                        break;
                    case "3":
                        OperationsService operations = new();
                        operations.OperationJobs().Wait();
                        break;
                    case "4":
                        service.Exit().Wait();
                        Console.WriteLine("Exit");
                        break;
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
        }
    }

}


