using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using Qooba.Bot.Builder.Abstractions;
using System.Linq;
using System.Collections.Generic;
using Qooba.Bot.Builder.Models;
using Newtonsoft.Json.Linq;

namespace Qooba.Bot.Builder.Dialogs
{
    [Serializable]
    public class DialogFormFactory<T> : IDialog<T>
    where T : class, new()
    {
        private readonly IDialogFormResume<T> resume;

        private readonly IDialogFormBuilder<T> dialogFormBuilder;

        public DialogFormFactory(IDialogFormResume<T> resume, IDialogFormBuilder<T> dialogFormBuilder)
        {
            this.resume = resume;
            this.dialogFormBuilder = dialogFormBuilder;
        }

        public Task StartAsync(IDialogContext context)
        {
            try
            {
                context.Wait(MessageReceivedAsync);
            }
            catch (OperationCanceledException error)
            {
                Task.FromCanceled(error.CancellationToken);
            }
            catch (Exception error)
            {
                return Task.FromException(error);
            }

            return Task.CompletedTask;
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var botContext = context as IBotContext;
            var activity = botContext.Activity as Activity;
            this.dialogFormBuilder.ModelObject = (activity.Entities != null ? activity.Entities : new List<Entity>()).Where(x => x.Type == Constants.DialogFactoryResponseEntities).Select(x => x.Properties).FirstOrDefault();
            context.Call(BuildFormDialog(), (c, r) => FormComplete(c, r));
        }

        protected virtual async Task FormComplete(IDialogContext context, IAwaitable<T> result)
        {
            await this.resume.FormComplete(context, result);
        }

        private IFormDialog<T> BuildFormDialog(FormOptions options = FormOptions.PromptInStart) => FormDialog.FromForm(this.dialogFormBuilder.BuildForm, options);
    }
}