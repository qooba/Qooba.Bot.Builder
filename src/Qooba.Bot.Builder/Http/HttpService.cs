using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Qooba.Bot.Builder.Abstractions;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Qooba.Bot.Builder.Http
{
    [Serializable]
    public class HttpService : IHttpService
    {
        public async Task<TResult> GetAsync<TResult>(CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null)
        {
            var str = await this.GetAsync(token, baseUri, path, query, bearerToken, headers);
            return JsonConvert.DeserializeObject<TResult>(str);
        }

        public async Task<JObject> GetAsyncObject(CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null)
        {
            var str = await this.GetAsync(token, baseUri, path, query, bearerToken, headers);
            return JObject.Parse(str);
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(TRequest input, CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                var builder = new UriBuilder(baseUri);
                if (!string.IsNullOrEmpty(path))
                {
                    builder.Path = string.Concat(builder.Path, path);
                }

                if (!string.IsNullOrEmpty(query))
                {
                    builder.Query = query;
                }

                var request = new HttpRequestMessage()
                {
                    RequestUri = builder.Uri,
                    Method = HttpMethod.Post,
                    Content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json")
                };

                if (headers != null)
                {
                    foreach (var key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                if (bearerToken != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                }

                var response = await client.SendAsync(request, token);
                if (!response.IsSuccessStatusCode)
                {
                    return default(TResult);
                }

                var str = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(str);
            }
        }

        public async Task<HttpStatusCode> PostAsync<TRequest>(TRequest input, CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                var builder = new UriBuilder(baseUri);
                if (!string.IsNullOrEmpty(path))
                {
                    builder.Path = string.Concat(builder.Path, path);
                }

                if (!string.IsNullOrEmpty(query))
                {
                    builder.Query = query;
                }

                var request = new HttpRequestMessage()
                {
                    RequestUri = builder.Uri,
                    Method = HttpMethod.Post,
                    Content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json")
                };

                if (headers != null)
                {
                    foreach (var key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                if (bearerToken != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                }

                var response = await client.SendAsync(request, token);
                return response.StatusCode;
            }
        }

        public async Task<TResult> PostAsync<TResult>(byte[] data, CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null, string mediaType = null)
        {
            using (var client = new HttpClient())
            {
                var builder = new UriBuilder(baseUri);
                if (!string.IsNullOrEmpty(path))
                {
                    builder.Path = string.Concat(builder.Path, path);
                }

                if (!string.IsNullOrEmpty(query))
                {
                    builder.Query = query;
                }

                var content = new ByteArrayContent(data);
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                var request = new HttpRequestMessage()
                {
                    RequestUri = builder.Uri,
                    Method = HttpMethod.Post,
                    Content = content
                };


                if (headers != null)
                {
                    foreach (var key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                if (bearerToken != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                }

                var response = await client.SendAsync(request, token);
                if (!response.IsSuccessStatusCode)
                {
                    return default(TResult);
                }

                var str = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(str);

            }
        }

        public async Task<string> GetAsync(CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                var builder = new UriBuilder(baseUri);
                if (!string.IsNullOrEmpty(path))
                {
                    builder.Path = string.Concat(builder.Path, path);
                }

                if (!string.IsNullOrEmpty(query))
                {
                    builder.Query = query;
                }

                var request = new HttpRequestMessage()
                {
                    RequestUri = builder.Uri,
                    Method = HttpMethod.Get
                };

                if (headers != null)
                {
                    foreach (var key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                if (bearerToken != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                }

                var response = await client.SendAsync(request, token);
                if (!response.IsSuccessStatusCode)
                {
                    return string.Empty;
                }

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
