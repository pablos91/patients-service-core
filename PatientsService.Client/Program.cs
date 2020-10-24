using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PatientsService.Client
{
    class Program
    {
        private static readonly HttpClient _client = new HttpClient();
        static async Task Main(string[] args)
        {
            switch (args[0])
            {
                case "add":
                    var patient = new Patient()
                    {
                        FirstName = args[1],
                        LastName = args[2],
                        BirthDate = DateTime.ParseExact(args[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        TestDate = DateTime.ParseExact(args[4], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        TestPositive = bool.Parse(args[5]),
                    };

                    try
                    {
                        var resp = await _client.PostAsync("http://localhost:54231/api/patients", new StringContent(
                            JsonConvert.SerializeObject(patient), 
                            Encoding.UTF8, "application/json"
                            ));

                        if(resp.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Patient added.");
                        }
                        else
                        {
                            Console.WriteLine($"PatientService responded with: {resp.StatusCode}");
                        }

                    } 
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                        throw;
                    }

                    return;
                default:
                    // TODO: listowanie pacjentów
                    return;
            }
        }
    }
}
