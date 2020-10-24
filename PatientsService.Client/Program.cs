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
            HttpResponseMessage resp = null;
            var action = args.Length > 0 ? args[0] : "list";

            switch (action)
            {
                case "add":
                    var patient = new Patient()
                    {
                        FirstName = args[1],
                        LastName = args[2],
                        BirthDate = DateTime.ParseExact(args[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        TestDate = DateTime.ParseExact(args[4], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        TestPositive = bool.Parse(args[5]),
                        Email = args[6]
                    };

                    try
                    {
                        resp = await _client.PostAsync("http://localhost:54231/api/patients", new StringContent(
                            JsonConvert.SerializeObject(patient),
                            Encoding.UTF8, "application/json"
                            ));

                        if (resp.IsSuccessStatusCode)
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
                    resp = await _client.GetAsync("http://localhost:54231/api/patients");

                    if (resp.IsSuccessStatusCode)
                    {
                        var deserialized = JsonConvert.DeserializeObject<Patient[]>(await resp.Content.ReadAsStringAsync());

                        foreach(var item in deserialized)
                        {
                            Console.WriteLine($"Imię: {item.FirstName}, Nazwisko: {item.LastName}, Email: {item.Email}, Zakażony: {(item.TestPositive ? "TAK": "NIE")}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"PatientService responded with: {resp.StatusCode}");
                    }
                    return;
            }
        }
    }
}
