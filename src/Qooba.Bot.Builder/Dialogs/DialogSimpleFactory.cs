using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Qooba.Bot.Builder.Abstractions;

namespace Qooba.Bot.Builder.Dialogs
{
    [Serializable]
    public class DialogSimpleFactory<TDialogResume> : IDialog<object>
        where TDialogResume : IDialogResume
    {
        private readonly TDialogResume resume;
        
        public DialogSimpleFactory(TDialogResume resume)
        {
            this.resume = resume;
        }

        public Task StartAsync(IDialogContext context)
        {
            try
            {
                context.Wait(MessageReceivedAsync);
            }
            catch (OperationCanceledException error)
            {
                return Task.FromCanceled(error.CancellationToken);
            }
            catch (Exception error)
            {
                return Task.FromException(error);
            }

            return Task.CompletedTask;
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            await this.resume.MessageReceivedAsync(context, argument);
        }
    }
}