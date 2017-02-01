using Qooba.Bot.Builder.Wit.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Qooba.Bot.Builder.Wit
{
    public interface IWitService
    {
        Task<WitResult> QueryMessageAsync(string query, CancellationToken token);

        Task<WitResult> QuerySpeechAsync(byte[] data, CancellationToken token);
    }
}
