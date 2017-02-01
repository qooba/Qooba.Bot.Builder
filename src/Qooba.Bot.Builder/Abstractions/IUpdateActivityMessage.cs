using Microsoft.Bot.Connector;
using System.Threading.Tasks;
namespace Qooba.Bot.Builder.Abstractions
{
    public interface IUpdateActivityMessage
    {
        Task<string> CreateMessage(ChannelAccount account);
    }
}