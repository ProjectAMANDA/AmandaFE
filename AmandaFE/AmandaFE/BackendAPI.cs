using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AmandaFE
{
    public static class BackendAPI
    {
        /// <summary>
        /// Requests the backend ProjectAMANDA API for post content analytics via
        /// the /api/analytics endpoint
        /// </summary>
        /// <param name="text">The blog post content to analyze and retrieve images for</param>
        /// <returns>JObject representing the JSON response from the ProjectAMANDA API analytics endpoint</returns>
        public static async Task<JObject> GetAnalyticsAsync(string text)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://amandapi20180416113018.azurewebsites.net/api/");
                // Blog post content is sent via the "text" header in the HTTP GET request.
                // HTTP specifies that all newline characters are followed by whitespace
                // which is accomplished through the System.String.Replace method below
                client.DefaultRequestHeaders.Add("text", text.Replace("\n", "\n "));

                HttpResponseMessage response = await client.GetAsync("analytics/true/2");

                if (response.IsSuccessStatusCode)
                {
                    return JObject.Parse(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return null;
                }
            }
        }

        public static async Task<JObject> GetBingAsync(string significantPhrase)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://amandapi20180416113018.azurewebsites.net/api/");
                HttpResponseMessage response = await client.GetAsync($"image/{significantPhrase}/2");

                if (response.IsSuccessStatusCode)
                {
                    return JObject.Parse(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
