using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Qooba.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Internals.Fibers;
using System.Threading;
using Microsoft.Bot.Connector;

namespace Qooba.Bot.Builder.Dialogs
{
    [Serializable]
    public class DialogHttpResume : BaseDialogHttpResume, IDialogResume
    {
        private readonly IHttpService httpService;

        private readonly Func<string, IDialogHttpResumeConfiguration> dialogHttpResumeMessage;

        public DialogHttpResume(IHttpService httpService, Func<string, IDialogHttpResumeConfiguration> dialogHttpResumeMessage)
        {
            SetField.NotNull(out this.httpService, "httpService", httpService);
            SetField.NotNull(out this.dialogHttpResumeMessage, "dialogHttpResumeMessage", dialogHttpResumeMessage);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var botContext = context as IBotContext;
            var activity = botContext?.Activity;
            var intent = Intent(context);
            var config = this.dialogHttpResumeMessage(intent);

            try
            {
                var headers = PrepareHeaders(context);
                //TODO: Response contract
                var result = await this.httpService.GetAsync<string>(CancellationToken.None, baseUri: config.Uri, headers: headers);
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync(config.CancelMessage);
            }
        }
    }
}