using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Qooba.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Internals.Fibers;
using System.Threading;

namespace Qooba.Bot.Builder.Dialogs
{
    [Serializable]
    public class DialogFormHttpResume<TForm> : BaseDialogHttpResume, IDialogFormResume<TForm>
    {
        private readonly IHttpService httpService;

        private readonly IDialogFormHttpResumeConfiguration<TForm> dialogHttpResumeMessage;

        public DialogFormHttpResume(IHttpService httpService, IDialogFormHttpResumeConfiguration<TForm> dialogHttpResumeMessage)
        {
            SetField.NotNull(out this.httpService, "httpService", httpService);
            SetField.NotNull(out this.dialogHttpResumeMessage, "dialogHttpResumeMessage", dialogHttpResumeMessage);
        }

        public async Task FormComplete(IDialogContext context, IAwaitable<TForm> result)
        {
            try
            {
                var form = await result;
                if (form != null)
                {
                    var headers = PrepareHeaders(context);
                    //TODO: Response contract
                    await this.httpService.PostAsync(form, CancellationToken.None, this.dialogHttpResumeMessage.Uri, headers: headers);
                    await context.PostAsync(this.dialogHttpResumeMessage.CancelMessage);
                }
                else
                {
                    await context.PostAsync(this.dialogHttpResumeMessage.CancelMessage);
                }
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync(this.dialogHttpResumeMessage.CancelMessage);
            }
        }
    }
}