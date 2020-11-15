using IdentityModel.Client;
using Microsoft.Identity.Client;
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

            var app = PublicClientApplicationBuilder.Create("e5f5d88f-f4b2-4e61-84f5-8a46aed17127")
                .WithAuthority("https://login.microsoftonline.com/a18c5d1e-7762-495b-96de-e36703dab8bc/v2.0/")
                .WithDefaultRedirectUri()
                .Build();

            var result = await app.AcquireTokenInteractive(new[] { "api://e5f5d88f-f4b2-4e61-84f5-8a46aed17127/.default" }).ExecuteAsync();

            var token = result.AccessToken;

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
                        var request = new HttpRequestMessage()
                        {
                            RequestUri = new Uri("http://localhost:54231/api/patients"),
                            Method = HttpMethod.Post,
                            Content = new StringContent(
                                JsonConvert.SerializeObject(patient),
                                Encoding.UTF8, "application/json"
                            )
                        };

                        request.Headers.Add("Authorization", $"Bearer {token}");

                        resp = await _client.SendAsync(request);

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

                        foreach (var item in deserialized)
                        {
                            Console.WriteLine($"Imię: {item.FirstName}, Nazwisko: {item.LastName}, Email: {item.Email}, Zakażony: {(item.TestPositive ? "TAK" : "NIE")}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"PatientService responded with: {resp.StatusCode}");
                    }
                    return;
            }
        }

        private static async Task<string> GetToken()
        {
            using var client = new HttpClient();

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = "https://login.microsoftonline.com/a18c5d1e-7762-495b-96de-e36703dab8bc/oauth2/v2.0/token", // Tenant ID
                ClientId = "e5f5d88f-f4b2-4e61-84f5-8a46aed17127", // Client ID
                ClientSecret = System.Environment.GetEnvironmentVariable("AZURE_AD_SECRET"), // Client Secret
                Scope = "api://e5f5d88f-f4b2-4e61-84f5-8a46aed17127/.default" // Scope + /.default
            };

            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

            if (token.IsError)
                throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

            return token.AccessToken;
        }
    }
}
