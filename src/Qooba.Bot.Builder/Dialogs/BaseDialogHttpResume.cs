using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using System.Globalization;
using System.Linq;

namespace Qooba.Bot.Builder.Dialogs
{
    [Serializable]
    public abstract class BaseDialogHttpResume
    {
        protected static Dictionary<string, string> PrepareHeaders(IDialogContext context)
        {
            var botContext = context as IBotContext;
            var activity = botContext?.Activity;

            return new Dictionary<string, string>
                {
                    {"Id", activity?.Id },
                    {"FromId", activity?.From?.Id },
                    {"FromName", activity?.From?.Name },
                    {"RecipientId", activity?.Recipient?.Id },
                    {"RecipientName", activity?.Recipient?.Name },
                    {"ChannelId", activity?.ChannelId },
                    {"ConversationId", activity?.Conversation?.Id },
                    {"IsGroup", activity?.Conversation?.IsGroup?.ToString() },
                    {"Locale", (activity as IMessageActivity)?.Locale},
                    {"Timestamp", activity.Timestamp.Value.ToString(CultureInfo.InvariantCulture)},
                    {"ServiceUrl", activity.ServiceUrl},
                    {"ReplyToId", activity.ReplyToId},
                    {"Type", activity.Type}
                };
        }

        protected string Intent(IDialogContext context)
        {
            var botContext = context as IBotContext;
            var activity = botContext?.Activity;
            return (activity as Activity).Entities.Where(x => x.Type == Constants.DialogKey).Select(x => x.Properties[Constants.DialogKey].ToString()).FirstOrDefault();
        }
    }
}