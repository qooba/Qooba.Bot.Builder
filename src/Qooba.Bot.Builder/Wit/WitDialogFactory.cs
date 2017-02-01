using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Qooba.Bot.Builder.Abstractions;
using Qooba.Bot.Builder.Models;
using Qooba.Bot.Builder.Wit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Qooba.Bot.Builder.Wit
{
    [Serializable]
    public class WitDialogFactory : IDialogFactory
    {
        private readonly IWitService witService;

        private readonly Func<string, IDialog<object>> dialogFactory;

        public WitDialogFactory(IWitService witService, Func<string, IDialog<object>> dialogFactory)
        {
            this.witService = witService;
            this.dialogFactory = dialogFactory;
        }

        protected virtual double ConfidenceTreshold => 0.8;

        public async Task<DialogFactoryResponse> CreateAsync(IMessageActivity activity)
        {
            WitResult witResponse;
            if (activity?.Attachments != null && activity.Attachments.Any(a => a.ContentType.Contains("audio") || a.ContentType.Contains("video")))
            {
                witResponse = await GetSpeechIntent(activity);
            }
            else
            {
                witResponse = await GetTextIntent(activity);
            }

            IList<WitEntity> entity;
            if (witResponse?.Entities != null && witResponse.Entities.TryGetValue("intent", out entity))
            {
                var intent = entity.OrderByDescending(x => x.Confidence).FirstOrDefault();

                //TODO: Add possibility to configure confidence treshold
                if (intent.Confidence != null && intent.Confidence > this.ConfidenceTreshold)
                {
                    var dialog = dialogFactory(intent.Value.ToString()) as IDialog<object>;
                    var entities = witResponse.Entities.Select(x =>
                    {
                        var e = x.Value.OrderByDescending(z => z.Confidence).FirstOrDefault();
                        return new DialogFactoryEntity
                        {
                            Key = x.Key,
                            Confidence = e?.Confidence,
                            Value = e?.Value,
                            Type = e.Type
                        };
                    });

                    return new DialogFactoryResponse(dialog, intent, entities);
                }
            }

            var d = dialogFactory(Constants.DefaultIntent) as IDialog<object>;
            return new DialogFactoryResponse(d, null, new List<DialogFactoryEntity>());
        }

        protected virtual async Task<WitResult> GetTextIntent(IMessageActivity activity) => await this.witService.QueryMessageAsync(activity.Text, CancellationToken.None);

        protected virtual async Task<WitResult> GetSpeechIntent(IMessageActivity activity)
        {
            var attachment = activity.Attachments.FirstOrDefault(x => x.ContentType.Contains("audio") || x.ContentType.Contains("video"));
            if (attachment != null)
            {
                var wav = WebRequest.Create(attachment.ContentUrl);
                using (Stream wavStream = wav.GetResponse().GetResponseStream())
                using (var ms = new MemoryStream())
                {
                    await wavStream.CopyToAsync(ms);
                    return await this.witService.QuerySpeechAsync(ms.ToArray(), CancellationToken.None);
                }
            }

            return null;
        }
    }
}