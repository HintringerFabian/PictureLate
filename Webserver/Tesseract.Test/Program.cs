using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Translate data = new Translate()
            {
                SourceLanguage = "de",
                DestinationLanguage = "en",
                Text = "Hallo du da!"
            };

            var tranlsatedText = PostTextAsync(data).Result;

            Console.WriteLine(tranlsatedText);
        }

        private static async Task<string> PostTextAsync(Translate data)
        {
            var result = "";

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:51008/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/api/translate", content);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }
    }
}
