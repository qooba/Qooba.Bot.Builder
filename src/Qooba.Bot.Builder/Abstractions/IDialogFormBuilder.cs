using Microsoft.Bot.Builder.FormFlow;
using Newtonsoft.Json.Linq;

namespace Qooba.Bot.Builder.Abstractions
{
    public interface IDialogFormBuilder<TForm>
    where TForm : class, new()
    {
        IForm<TForm> BuildForm();

        JObject ModelObject { get; set; }
    }
}
