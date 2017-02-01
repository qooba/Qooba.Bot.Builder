using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Qooba.Bot.Builder.Models;
using System.Threading.Tasks;

namespace Qooba.Bot.Builder.Abstractions
{
    public interface IDialogFactory
    {
        Task<DialogFactoryResponse> CreateAsync(IMessageActivity activity);
    }
}