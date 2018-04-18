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
        public static async Task<IEnumerable<string>> GetImageHrefs(string text)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:50063/api/");
                client.DefaultRequestHeaders.Add("text", text);

                HttpResponseMessage response = await client.GetAsync("image");

                if (response.IsSuccessStatusCode)
                {
                    JArray apiArray = JArray.Parse(await response.Content.ReadAsStringAsync());
                    return apiArray.ToObject<string[]>();
                }
                else
                {
                    return new string[0];
                }
            }
        }
    }
}
