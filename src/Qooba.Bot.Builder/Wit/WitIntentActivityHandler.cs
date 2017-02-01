using System.Threading.Tasks;
using Qooba.Bot.Builder.Wit.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Qooba.Bot.Builder.Wit
{
    public delegate Task WitIntentActivityHandler(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult);
}
