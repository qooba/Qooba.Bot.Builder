using System;
using System.Threading.Tasks;
using System.Threading;
using Qooba.Bot.Builder.Wit.Models;
using Microsoft.Bot.Builder.Internals.Fibers;
using Qooba.Bot.Builder.Abstractions;

namespace Qooba.Bot.Builder.Wit
{
    [Serializable]
    public class WitService : IWitService
    {
        private readonly IWitModel model;

        private readonly IHttpService httpService;

        public static readonly Uri UriBase = new Uri("https://api.wit.ai/");

        public WitService(IWitModel model, IHttpService httpService)
        {
            SetField.NotNull(out this.httpService, "httpService", httpService);
            SetField.NotNull(out this.model, "model", model);
        }

        public async Task<WitResult> QueryMessageAsync(string query, CancellationToken token) => await this.httpService.GetAsync<WitResult>(token, UriBase, "message", $"v=20161214&q={query}", model.ApiKey, null);

        public async Task<WitResult> QuerySpeechAsync(byte[] data, CancellationToken token) => await this.httpService.PostAsync<WitResult>(data, token, UriBase, "speech", $"v=20161214", model.ApiKey, null, "audio/wav");
    }
}
