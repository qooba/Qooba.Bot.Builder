using Microsoft.Bot.Connector;
using System.Threading.Tasks;
namespace Qooba.Bot.Builder.Abstractions
{
    public interface IActivityHandler
    {
        Task Handle(Activity activity);
    }
}