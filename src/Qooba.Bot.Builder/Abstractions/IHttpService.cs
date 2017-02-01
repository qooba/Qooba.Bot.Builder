using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Qooba.Bot.Builder.Abstractions
{
    public interface IHttpService
    {
        Task<TResult> GetAsync<TResult>(CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null);

        Task<JObject> GetAsyncObject(CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null);

        Task<HttpStatusCode> PostAsync<TRequest>(TRequest input, CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null);

        Task<TResult> PostAsync<TRequest, TResult>(TRequest input, CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null);

        Task<TResult> PostAsync<TResult>(byte[] data, CancellationToken token, Uri baseUri, string path = null, string query = null, string bearerToken = null, IDictionary<string, string> headers = null, string mediaType = null);
    }
}
