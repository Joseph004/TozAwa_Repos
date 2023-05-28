using Newtonsoft.Json;

namespace TozAwaHome.Extensions
{
public static class HttpContentExtensions
{
    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
    {
			using (Stream s = await content.ReadAsStreamAsync())
			using (StreamReader sr = new StreamReader(s))
			using (JsonReader reader = new JsonTextReader(sr))
			{
				JsonSerializer serializer = new JsonSerializer();

				// read the json from a stream
				// json size doesn't matter because only a small piece is read at a time from the HTTP request
				T t = serializer.Deserialize<T>(reader);
				return t;
			}

			/*string json = await content.ReadAsStringAsync();
        T value = JsonConvert.DeserializeObject<T>(json);
        return value;*/
    }
}
}

