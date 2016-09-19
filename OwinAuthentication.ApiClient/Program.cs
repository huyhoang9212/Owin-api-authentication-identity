using Newtonsoft.Json;
using OwinAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OwinAuthentication.ApiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();

            // Then Write Done...
            Console.WriteLine("");
            Console.WriteLine("Done! Press the Enter key to Exit...");
            Console.ReadLine();
            return;
        }

        static async Task Run()
        {
            // Create an http client provider:
            string hostUriString = "http://localhost:9999";
            var provider = new apiClientProvider(hostUriString);
            string _accessToken;
            Dictionary<string, string> _tokenDictionary;

            try
            {
                // Pass in the credentials and retrieve a token dictionary:
                _tokenDictionary = await provider.GetTokenDictionary(
                        "john@example.com", "password");
                _accessToken = _tokenDictionary["access_token"];

                // Write the contents of the dictionary:
                foreach (var kvp in _tokenDictionary)
                {
                    Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
                    Console.WriteLine("");
                }

                // Create a company client instance
                var baseUri = new Uri(hostUriString);
                var companyClient = new CompanyClient(baseUri, _accessToken);

                // Read inital companies:
                var companies = await companyClient.GetCompaniesAsync();
                WriteCompaniesList(companies);

                int nextId = 5;

                Console.WriteLine("Add a new company...");
                var result = await companyClient.AddCompanyAsync(
                    new Company { Name = string.Format("New Company #{0}", nextId) , Id = nextId});
                WriteStatusCodeResult(result);

                Console.WriteLine("Updated List after Add:");
                companies = await companyClient.GetCompaniesAsync();
                WriteCompaniesList(companies);

                Console.WriteLine("Update a company...");
                var updateMe = await companyClient.GetCompanyAsync(nextId);
                updateMe.Name = string.Format("Updated company #{0}", updateMe.Id);
                result = await companyClient.UpdateCompanyAsync(updateMe);
                WriteStatusCodeResult(result);

                Console.WriteLine("Updated List after Update:");
                companies = await companyClient.GetCompaniesAsync();
                WriteCompaniesList(companies);

                Console.WriteLine("Delete a company...");
                result = await companyClient.DeleteCompanyAsync(nextId);
                WriteStatusCodeResult(result);

                Console.WriteLine("Updated List after Delete:");
                companies = await companyClient.GetCompaniesAsync();
                WriteCompaniesList(companies);
            }
            catch (AggregateException ex)
            {
                // If it's an aggregate exception, an async error occurred:
                Console.WriteLine(ex.InnerExceptions[0].Message);
                Console.WriteLine("Press the Enter key to Exit...");
                Console.ReadLine();
                return;
            }
            catch (Exception ex)
            {
                // Something else happened:
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press the Enter key to Exit...");
                Console.ReadLine();
                return;
            }

           
        }

        static void WriteCompaniesList(IEnumerable<Company> companies)
        {
            foreach (var company in companies)
            {
                Console.WriteLine("Id: {0} Name: {1}", company.Id, company.Name);
            }
            Console.WriteLine("");
        }

        static void WriteStatusCodeResult(System.Net.HttpStatusCode statusCode)
        {
            if (statusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Opreation Succeeded - status code {0}", statusCode);
            }
            else
            {
                Console.WriteLine("Opreation Failed - status code {0}", statusCode);
            }
            Console.WriteLine("");
        }
    }

    public class apiClientProvider
    {
        string _hostUri;
        public string AccessToken { get; private set; }

        public apiClientProvider(string hostUri)
        {
            _hostUri = hostUri;
        }


        public async Task<Dictionary<string, string>> GetTokenDictionary(
            string userName, string password)
        {
            HttpResponseMessage response;
            var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "grant_type", "password" ),
                    new KeyValuePair<string, string>( "username", userName ),
                    new KeyValuePair<string, string> ( "password", password )
                };
            var content = new FormUrlEncodedContent(pairs);

            using (var client = new HttpClient())
            {
                var tokenEndpoint = new Uri(new Uri(_hostUri), "Token");
                response = await client.PostAsync(tokenEndpoint, content);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("Error: {0}", responseContent));
            }

            return GetTokenDictionary(responseContent);
        }


        private Dictionary<string, string> GetTokenDictionary(
            string responseContent)
        {
            Dictionary<string, string> tokenDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(
                responseContent);
            return tokenDictionary;
        }
    }
}
