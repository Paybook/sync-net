using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace PaybookSDK
{
    /// <summary>
    /// This class is used to retrieve json messages
    /// </summary>
    public class JsonContent : HttpContent
    {
        private readonly JToken _value;

        public JsonContent(JToken value)
        {
            _value = value;
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream,
            TransportContext? context)
        {
            var jw = new JsonTextWriter(new StreamWriter(stream))
            {
                Formatting = Formatting.Indented
            };
            _value.WriteTo(jw);
            jw.Flush();
            return Task.FromResult<object?>(null);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }

    public static class Converter
    {
        public static string JsonToQueryString(JObject json)
        {
            if (json == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json.ToString()) ?? new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in values)
            {
                sb.AppendLine("&" + entry.Key + "=" + entry.Value);
            }
            return sb.ToString().Trim().Replace("\n", "").Replace("\r", "");
        }
    }
}
