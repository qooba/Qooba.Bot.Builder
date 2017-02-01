using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace Qooba.Bot.Builder.Abstractions
{
    public interface IDialogFormResume<TDialog>
    {
        Task FormComplete(IDialogContext context, IAwaitable<TDialog> result);
    }
}