using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Qooba.Bot.Builder.Abstractions;

namespace Qooba.Bot.Builder.ActivityHandlers
{
    [Serializable]
    public class UpdateActivityMessage : IUpdateActivityMessage
    {
        public async Task<string> CreateMessage(ChannelAccount account) => $"Witaj {account?.Name} !";
    }
}
