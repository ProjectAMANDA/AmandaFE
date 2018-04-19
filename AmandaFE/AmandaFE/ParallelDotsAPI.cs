using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using ParallelDots;

namespace AmandaFE
{
    public class ParallelDotsAPI
    {
        public static IEnumerable<string> GetTags(string text)
        {
            paralleldots pd = new paralleldots("kB9ZXk2RErNglHtNOCLMqCb0S7SDUBDtZJ5B5dJbWMU");
            JToken keywords = JObject.Parse(pd.keywords(text));
            return keywords["keywords"].OrderByDescending(k => k["confidence_score"])
                                       .Select(k => (string)k["keyword"])
                                       .Take(3);
        }
    }
}
