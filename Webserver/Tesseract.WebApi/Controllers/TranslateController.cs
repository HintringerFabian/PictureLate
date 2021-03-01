using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Tesseract.WebApi.Models;

namespace Tesseract.WebApi.Controllers
{
    public class TranslateController : ApiController
    {
        private const string key_var = "Place your Azure Key here";
        private const string endpoint_var = "https://api.cognitive.microsofttranslator.com/";

        [HttpPost]
        public async Task<HttpResponseMessage> PostText([FromBody] Translate data)
        {
            var text = data.Text;

            //Console.WriteLine("Gib eine Sprache ein");
            string a = data.DestinationLanguage;
            string route = "/translate?api-version=3.0&to=" + a;


            string textToTranslate = text;
            text = await TranslateTextRequest(key_var, endpoint_var, route, textToTranslate);

            return Request.CreateResponse(HttpStatusCode.OK, text);
        }

        public async Task<string> TranslateTextRequest(string subscriptionKey, string endpoint, string route, string inputText)
        {
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                // Set the method to Post.
                request.Method = HttpMethod.Post;
                // Construct the URI and add headers.
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                // Deserialize the response using the classes created earlier.
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                // Iterate over the deserialized results.
                List<string> results = new List<string>();
                foreach (TranslationResult o in deserializedOutput)
                {
                    // Print the detected input language and confidence score.
                    // Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", o.DetectedLanguage.Language, o.DetectedLanguage.Score);
                    // Iterate over the results and print each translation.

                    foreach (Translation t in o.Translations)
                    {
                        results.Add(t.Text);
                    }
                }

                string ergebnis = "";
                foreach (string s in results)
                {
                    ergebnis += " " + s;
                }
                return ergebnis;
            }
        }

    }


    public class TranslationResult
    {
        public DetectedLanguage DetectedLanguage { get; set; }
        public TextResult SourceText { get; set; }
        public Translation[] Translations { get; set; }
    }

    public class DetectedLanguage
    {
        public string Language { get; set; }
        public float Score { get; set; }
    }

    public class TextResult
    {
        public string Text { get; set; }
        public string Script { get; set; }
    }

    public class Translation
    {
        public string Text { get; set; }
        public TextResult Transliteration { get; set; }
        public string To { get; set; }
        public Alignment Alignment { get; set; }
        public SentenceLength SentLen { get; set; }
    }

    public class Alignment
    {
        public string Proj { get; set; }
    }

    public class SentenceLength
    {
        public int[] SrcSentLen { get; set; }
        public int[] TransSentLen { get; set; }
    }
}

