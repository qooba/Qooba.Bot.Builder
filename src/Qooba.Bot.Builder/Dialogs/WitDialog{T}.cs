using Microsoft.Bot.Builder.Dialogs;
using Qooba.Bot.Builder.Wit;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using Qooba.Bot.Builder.Wit.Models;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System.Threading;
using Qooba.Bot.Builder.Http;

namespace Qooba.Bot.Builder.Dialogs
{
    [Serializable]
    public class WitDialog<T> : IDialog<T>
    {
        private readonly IReadOnlyList<IWitService> services;

        public WitDialog(params IWitService[] services)
        {
            if (services.Length == 0)
            {
                services = this.MakeServicesFromAttributes();
            }

            SetField.NotNull(out this.services, "services", services);
        }

        public IWitService[] MakeServicesFromAttributes() => Enumerable.ToArray(Enumerable.Cast<IWitService>(Enumerable.Select(CustomAttributeExtensions.GetCustomAttributes<WitModelAttribute>(GetType(), true), m => new WitService(m, new HttpService()))));

        public virtual async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);
        }

        protected virtual KeyValuePair<string, WitResult> BestResultFrom(IEnumerable<WitResult> results)
        {
            return results.Where(x => x.Entities.Any(e => e.Key == "intent")).Select(x =>
                new
                {
                    Result = x,
                    Confidence = x.Entities.SelectMany(e => e.Value).Max(v => v.Confidence),
                    Intent = x.Entities.OrderByDescending(e => e.Value.Max(v => v.Confidence)).Select(i => i.Key).FirstOrDefault()
                }).OrderByDescending(x => x.Confidence).Select(x => new KeyValuePair<string, WitResult>(x.Intent, x.Result)).FirstOrDefault();
        }

        protected virtual async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            IMessageActivity message = await item;

            var witResults = await Task.WhenAll(this.services.Select(s => s.QueryMessageAsync(message.Text, CancellationToken.None)));
            var bestResult = BestResultFrom(null); //TODO: use JObject witResults
            var handlers = GetHandlersByIntent();
            WitIntentActivityHandler handler;
            if (handlers.TryGetValue(bestResult.Key, out handler))
            {
                await handler(context, item, bestResult.Value);
            }
            else
            {
                //TODO: Add default handler
                await handlers.FirstOrDefault().Value(context, item, bestResult.Value);
            }
        }

        protected virtual Task<string> GetWitQueryTextAsync(IDialogContext context, IMessageActivity message) => Task.FromResult(message.Text);

        protected virtual IDictionary<string, WitIntentActivityHandler> GetHandlersByIntent()
        {
            return WitDialog.EnumerateHandlers(this).ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
