using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Qooba.Bot.Builder.Dialogs;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Qooba.Bot.Builder.Abstractions
{
    public interface IBotBuilder
    {
        IBotBuilder RegisterActivityHandler<THandler>(string activityTypes) where THandler : IActivityHandler;

        IBotBuilder RegisterFactory<TFactory>() where TFactory : IDialogFactory;

        IBotBuilder RegisterDialog<TDialog>(object dialogKey)
            where TDialog : IDialog<object>;

        IBotBuilder RegisterDefault<TDialogResume>()
            where TDialogResume : IDialogResume;

        IBotBuilder RegisterSimple<TDialogResume>(object dialogKey)
            where TDialogResume : IDialogResume;

        IBotBuilder RegisterForm<TForm, TDialogResume>(object dialogKey)
            where TForm : class, new()
            where TDialogResume : IDialogFormResume<TForm>;

        IBotBuilder RegisterForm<TForm, TDialogResume, TDialogFormBuilder>(object dialogKey)
            where TForm : class, new()
            where TDialogResume : IDialogFormResume<TForm>
            where TDialogFormBuilder : IDialogFormBuilder<TForm>;

        IBotBuilder RegisterBootstrapper<TBootstrapper>()
            where TBootstrapper : IDialogBootstrapper;

        IBotBuilder RegisterBootstrapper(Func<Task> bootstrapp);

        IBotBuilder RegisterType<TTo, TFrom>();

        IBotBuilder RegisterType<TTo, TFrom>(object dialogKey);

        IBotBuilder RegisterLogger(Action<string> logAction);

        Task<object> SendAsync(HttpRequestMessage req);

        Task<HttpResponseMessage> SendAsync(Activity activity);
    }
}