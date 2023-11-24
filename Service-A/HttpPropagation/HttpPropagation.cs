using System.Text;

namespace Service_A.HttpPropagation
{
    public class HttpPropagation
    {
        /// <summary>
        /// Takes the <paramref name="values"/> and injects it into the http request carrier. Note that carrier will be <c>HttpRequestMessage</c> on .NET framework
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public static void InjectTraceContextIntoBasicProperties(HttpRequest httpRequest, string key, string values)
        {
            try
            {
                httpRequest.Headers.Add(key, values);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Takes the headers from the <paramref name="httprequest"/>. This will be used to extract the header values.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="key"></param>

        public static IEnumerable<string> ExtractTraceContextFromBasicProperties(HttpRequest httprequest, string key)
        {
            try
            {
                if (httprequest.Headers.TryGetValue(key, out var value))
                {
                    var bytes = value.SelectMany(s => Encoding.UTF8.GetBytes(s)).ToArray();
                    return new[] { Encoding.UTF8.GetString(bytes) };
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Enumerable.Empty<string>();
        }
    }
}
