using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Tozawa.Bff.Portal.HttpClients
{
    public class HttpClientHelperBase
    {
        internal static HttpRequestMessage GetRequest(string endpoint) => new(HttpMethod.Get, endpoint);
        internal static HttpRequestMessage PostRequest(string endpoint, object value, bool multiPartContent = false) => new(HttpMethod.Post, endpoint)
        {
            Content = multiPartContent ? (MultipartFormDataContent)value : CreateHttpContent(value)
        };
        internal static HttpRequestMessage PutRequest(string endpoint, object value) => new(HttpMethod.Put, endpoint)
        {
            Content = CreateHttpContent(value)
        };

        internal static HttpRequestMessage PatchRequest(string endpoint, object value) => new(HttpMethod.Patch, endpoint)
        {
            Content = CreateHttpContent(value)
        };

        internal static HttpRequestMessage DeleteRequest(string endpoint) => new(HttpMethod.Delete, endpoint);

        public static HttpContent CreateHttpContent(object content, string mediaTypeHeader = "application/json")
        {
            if (content == null) return new StringContent(string.Empty);
            var ms = new MemoryStream();
            SerializeJsonIntoStream(content, ms);
            ms.Seek(0, SeekOrigin.Begin);
            HttpContent httpContent = new StreamContent(ms);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(mediaTypeHeader);
            return httpContent;
        }

        protected static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);
            using var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None };
            var js = new JsonSerializer();
            js.Serialize(jtw, value);
            jtw.Flush();
        }

        public static HttpResponseMessage CreateHttpResponseMessage(HttpRequestMessage request, HttpResponseMessage response)
        {
            var result = response ?? new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = CreateHttpContent("Unable to connect. Please try again or contact support."),
                RequestMessage = request
            };

            if (response == null || response.IsSuccessStatusCode)
                return result;

            foreach (var (key, value) in request.Headers)
            {
                result.Headers.Add(key, value);
            }

            return result;
        }

        public static HttpContent CreateMultiPartContent(IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentException("File cannot be null when creating multipart content", nameof(file));
            }
            return new StreamContent(file.OpenReadStream())
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue(file.ContentType)
                }
            };
        }

        internal static BadHttpRequestException HandleError(HttpResponseMessage response)
        {
            var message = (response.ReasonPhrase ?? "").Replace(@"\n", " ");
            return new BadHttpRequestException(JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {"source", GetHttpResponseRequestSoruce(response) },
                {"error", message},
                {"statusCode", ((int)response.StatusCode).ToString()  }
            }));
        }

        internal static string GetHttpResponseRequestSoruce(HttpResponseMessage response)
        {
            if (response.RequestMessage == null) return "Unknown source";
            if (response.RequestMessage.RequestUri == null) return "Unknown source";
            return response.RequestMessage.RequestUri.ToString();
        }
    }
}