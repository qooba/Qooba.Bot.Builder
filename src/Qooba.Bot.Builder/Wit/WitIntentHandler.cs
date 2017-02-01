using System.Threading.Tasks;
using Qooba.Bot.Builder.Wit.Models;
using Microsoft.Bot.Builder.Dialogs;

namespace Qooba.Bot.Builder.Wit
{
    public delegate Task WitIntentHandler(IDialogContext context, WitResult witResult);
}
