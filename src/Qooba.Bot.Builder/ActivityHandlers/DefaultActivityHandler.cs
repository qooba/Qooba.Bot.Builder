using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Qooba.Bot.Builder.Abstractions;

namespace Qooba.Bot.Builder.ActivityHandlers
{
    [Serializable]
    public class DefaultActivityHandlers : IActivityHandler
    {
        public async Task Handle(Activity activity) => await Task.CompletedTask;
    }
}