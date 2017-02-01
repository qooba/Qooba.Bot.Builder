using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Qooba.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Linq;

namespace Qooba.Bot.Builder.ActivityHandlers
{
    [Serializable]
    public class ConversationUpdateActivityHandlers : IActivityHandler
    {
        private readonly IUpdateActivityMessage updateActivityMessage;

        public ConversationUpdateActivityHandlers(IUpdateActivityMessage updateActivityMessage)
        {
            this.updateActivityMessage = updateActivityMessage;
        }

        public async Task Handle(Activity activity)
        {
            var client = new ConnectorClient(new Uri(activity.ServiceUrl));
            IConversationUpdateActivity update = activity;
            if (update.MembersAdded.Any())
            {
                var reply = activity.CreateReply();
                var newMembers = update.MembersAdded?.Where(t => t.Id != activity.Recipient.Id);
                foreach (var newMember in newMembers)
                {
                    reply.Text = await this.updateActivityMessage.CreateMessage(newMember);
                    await client.Conversations.ReplyToActivityAsync(reply);
                }
            }
        }
    }
}