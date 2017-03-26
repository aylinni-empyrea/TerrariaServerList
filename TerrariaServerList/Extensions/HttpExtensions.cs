using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TerrariaServerList.Extensions
{
  internal static class UriExtensions
  {
    internal static HttpClient _client = new HttpClient
    {
      DefaultRequestHeaders = {Accept = {new MediaTypeWithQualityHeaderValue("application/json")}}
    };

    private static readonly JsonSerializer _serializer = JsonSerializer.CreateDefault();

    internal static async Task<string> GetStringAsync(this Uri uri)
      => await _client.GetStringAsync(uri).ConfigureAwait(false);

    internal static async Task<string> GetStringAsync(this string uri)
      => await _client.GetStringAsync(uri).ConfigureAwait(false);

    internal static async Task<T> GetJsonAsync<T>(this Uri uri)
    {
      var resp = await _client.GetStringAsync(uri).ConfigureAwait(false);

      return _serializer.Deserialize<T>(new JsonTextReader(new StringReader(resp)));
    }

    internal static async Task<HttpResponseMessage> PostStringAsync(this Uri uri, string content)
      => await _client.PostAsync(uri, new StringContent(content)).ConfigureAwait(false);

    internal static async Task<HttpResponseMessage> PostJsonAsync(this Uri uri, object content)
    {
      using (var payload = new StringWriter())
      {
        _serializer.Serialize(payload, content);
        return await uri.PostStringAsync(payload.ToString());
      }
    }

    internal static async Task<T> GetJsonAsync<T>(this string uri)
      => await GetJsonAsync<T>(new Uri(uri)).ConfigureAwait(false);
  }
}