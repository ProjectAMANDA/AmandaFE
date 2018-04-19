using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AmandaFE
{
    public class ParallelDotsAPI
    {
        public async static Task<IEnumerable<string>> GenerateTagsAsync(string text)
        {
            List<string> words = new List<string>();
            string uri = $"https://apis.paralleldots.com/v2/keywords?text={text}&api_key=kB9ZXk2RErNglHtNOCLMqCb0S7SDUBDtZJ5B5dJbWMU";
            using (var client = new HttpClient())
            {
                //HttpContent thing 
                var response = await client.PostAsync(uri, new StringContent(""));
                string responseString = await response.Content.ReadAsStringAsync();
                JToken keywords = JObject.Parse(responseString);
                words = keywords["keywords"].OrderByDescending(k => k["confidence_score"])
                                       .Select(k => (string)k["keyword"])
                                       .Take(3)
                                       .ToList();
            }
            return words;
        }

    }
}
