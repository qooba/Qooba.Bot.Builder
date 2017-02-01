using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Qooba.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using Autofac;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Qooba.Bot.Builder.Models;

namespace Qooba.Bot.Builder.ActivityHandlers
{
    [Serializable]
    public class MessageActivityHandlers : IActivityHandler
    {
        private readonly IDialogFactory dialogFactory;

        public MessageActivityHandlers(IDialogFactory dialogFactory)
        {
            this.dialogFactory = dialogFactory;
        }

        public async Task Handle(Activity activity)
        {
            await Conversation.SendAsync(activity, () => new RootDialog(), CancellationToken.None);
        }
    }

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var activity = (Activity)await argument;
            //TODO: If possible remove service locator
            var dialogFactoryResponse = await (Conversation.Container.Resolve<IDialogFactory>().CreateAsync(activity));

            if (dialogFactoryResponse?.Entities != null)
            {
                var o = new JObject();
                dialogFactoryResponse.Entities.ToList().ForEach(e => o[e.Key] = e.Value);
                var entity = new Entity
                {
                    Type = Constants.DialogFactoryResponseEntities,
                    Properties = o
                };

                if (activity.Entities == null)
                {
                    activity.Entities = new List<Entity>();
                }

                activity.Entities.Add(entity);
            }

            if (dialogFactoryResponse?.DialogKey != null)
            {
                var o = new JObject();
                o[Constants.DialogKey] = JToken.FromObject(dialogFactoryResponse.DialogKey);
                var entity = new Entity
                {
                    Type = Constants.DialogKey,
                    Properties = o
                };

                if (activity.Entities == null)
                {
                    activity.Entities = new List<Entity>();
                }

                activity.Entities.Add(entity);
            }

            await context.Forward(dialogFactoryResponse.Dialog, FormComplete, activity, CancellationToken.None);
        }

        private async Task FormComplete(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                context.Wait(MessageReceivedAsync);
            }
            catch (Exception ex)
            {
                Conversation.Container.Resolve<Action<LogMessage>>()((LogMessage)ex.ToString());
            }
        }
    }
}