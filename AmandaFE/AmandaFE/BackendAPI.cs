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
        public static async Task<JObject> GetAnalyticsAsync(string text)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://amandapi20180416113018.azurewebsites.net/api/");
                client.DefaultRequestHeaders.Add("text", text);

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
    }
}
