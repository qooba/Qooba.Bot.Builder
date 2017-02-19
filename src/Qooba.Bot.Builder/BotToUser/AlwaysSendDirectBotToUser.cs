using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace Qooba.Bot.Builder.BotToUser
{
    public sealed class AlwaysSendDirectBotToUser : IBotToUser
    {
        private readonly IMessageActivity toBot;
        private readonly IConnectorClient client;
        public AlwaysSendDirectBotToUser(IMessageActivity toBot, IConnectorClient client)
        {
            SetField.NotNull(out this.toBot, nameof(toBot), toBot);
            SetField.NotNull(out this.client, nameof(client), client);
        }

        IMessageActivity IBotToUser.MakeMessage()
        {
            var toBotActivity = (Activity)this.toBot;
            return toBotActivity.CreateReply();
        }

        async Task IBotToUser.PostAsync(IMessageActivity message, CancellationToken cancellationToken)
        {
            await this.client.Conversations.ReplyToActivityAsync((Activity)message, cancellationToken);
        }
    }
}
