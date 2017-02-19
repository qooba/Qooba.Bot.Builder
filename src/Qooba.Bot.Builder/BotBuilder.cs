using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Qooba.Bot.Builder.Abstractions;
using Qooba.Bot.Builder.ActivityHandlers;
using Qooba.Bot.Builder.Dialogs;
using Qooba.Bot.Builder.Http;
using Qooba.Bot.Builder.Models;
using Qooba.Bot.Builder.Wit;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace Qooba.Bot.Builder
{
    [Serializable]
    public class BotBuilder : IBotBuilder
    {
        private static Lazy<Task<bool>> initialized = new Lazy<Task<bool>>(async () => await Initialize());

        private static ConcurrentQueue<Func<Task>> bootstrappers = new ConcurrentQueue<Func<Task>>();

        private static ConcurrentQueue<Action<ContainerBuilder>> registrations = new ConcurrentQueue<Action<ContainerBuilder>>();

        public IBotBuilder RegisterActivityHandler<THandler>(string activityTypes) where THandler : IActivityHandler
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<THandler>().Keyed<IActivityHandler>(activityTypes);
                registrations.Enqueue(registration);
            }

            return this;
        }

        public IBotBuilder RegisterBootstrapper(Func<Task> bootstrapp)
        {
            if (!initialized.IsValueCreated)
            {
                bootstrappers.Enqueue(bootstrapp);
            }

            return this;
        }

        public IBotBuilder RegisterType<TTo, TFrom>()
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<TFrom>().As<TTo>();
                registrations.Enqueue(registration);
            }

            return this;
        }

        public IBotBuilder RegisterType<TTo, TFrom>(object dialogKey)
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<TFrom>().Keyed<TTo>(dialogKey);
                registrations.Enqueue(registration);
            }

            return this;
        }

        public IBotBuilder RegisterBootstrapper<TBootstrapper>() where TBootstrapper : IDialogBootstrapper
        {
            if (!initialized.IsValueCreated && bootstrappers.Count == 0)
            {
                var bootstrapper = Conversation.Container.Resolve<TBootstrapper>();
                bootstrappers.Enqueue(() => bootstrapper.BoostrappAsync());
            }

            return this;
        }

        public IBotBuilder RegisterDialog<TDialog>(object dialogKey) where TDialog : IDialog<object>
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<TDialog>().Keyed<IDialog<object>>(dialogKey);
                registrations.Enqueue(registration);
            }

            return this;
        }

        public IBotBuilder RegisterFactory<TFactory>() where TFactory : IDialogFactory
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<TFactory>().As<IDialogFactory>();
                registrations.Enqueue(registration);
            }

            return this;
        }

        public IBotBuilder RegisterDefault<TDialogResume>()
            where TDialogResume : IDialogResume
        {
            return this.RegisterSimple<TDialogResume>(Constants.DefaultIntent);
        }

        public IBotBuilder RegisterSimple<TDialogResume>(object dialogKey)
            where TDialogResume : IDialogResume
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<DialogSimpleFactory<TDialogResume>>().Keyed<IDialog<object>>(dialogKey);
                Action<ContainerBuilder> resumeRegistration = builder => builder.RegisterType<TDialogResume>();
                registrations.Enqueue(registration);
                registrations.Enqueue(resumeRegistration);
            }

            return this;
        }

        public IBotBuilder RegisterForm<TForm, TDialogResume>(object dialogKey)
            where TForm : class, new()
            where TDialogResume : IDialogFormResume<TForm>
        {
            this.RegisterForm<TForm, TDialogResume, DialogFormBuilder<TForm>>(dialogKey);
            return this;
        }

        public IBotBuilder RegisterForm<TForm, TDialogResume, TDialogFormBuilder>(object dialogKey)
            where TForm : class, new()
            where TDialogResume : IDialogFormResume<TForm>
            where TDialogFormBuilder : IDialogFormBuilder<TForm>
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<DialogFormFactory<TForm>>().Keyed<IDialog<object>>(dialogKey);
                Action<ContainerBuilder> resumeRegistration = builder => builder.RegisterType<TDialogResume>().As<IDialogFormResume<TForm>>();
                Action<ContainerBuilder> formBuilderRegistration = builder => builder.RegisterType<TDialogFormBuilder>().As<IDialogFormBuilder<TForm>>();
                registrations.Enqueue(registration);
                registrations.Enqueue(resumeRegistration);
                registrations.Enqueue(formBuilderRegistration);
            }

            return this;
        }

        public IBotBuilder RegisterFormService<TForm>(object dialogKey, Uri uri, string cancelMessage)
            where TForm : class, new()
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<DialogFormFactory<TForm>>().Keyed<IDialog<object>>(dialogKey);
                Action<ContainerBuilder> resumeRegistration = builder => builder.RegisterType<DialogFormHttpResume<TForm>>().As<IDialogFormResume<TForm>>();
                Action<ContainerBuilder> formBuilderRegistration = builder => builder.RegisterType<DialogFormBuilder<TForm>>().As<IDialogFormBuilder<TForm>>();
                Action<ContainerBuilder> dialogHttpResumeConfigurationRegistration = builder => builder.RegisterInstance(new DialogFormHttpResumeConfiguration<TForm>(uri, cancelMessage)).As<IDialogFormHttpResumeConfiguration<TForm>>();
                registrations.Enqueue(registration);
                registrations.Enqueue(resumeRegistration);
                registrations.Enqueue(formBuilderRegistration);
                registrations.Enqueue(dialogHttpResumeConfigurationRegistration);
            }

            return this;
        }

        public IBotBuilder RegisterSimpleService(object dialogKey, Uri uri, string cancelMessage)
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<DialogSimpleFactory<DialogHttpResume>>().Keyed<IDialog<object>>(dialogKey);
                Action<ContainerBuilder> resumeRegistration = builder => builder.RegisterInstance(new DialogHttpResumeConfiguration(uri, cancelMessage)).Keyed<IDialogHttpResumeConfiguration>(dialogKey);
                registrations.Enqueue(registration);
                registrations.Enqueue(resumeRegistration);
            }

            return this;
        }

        public IBotBuilder RegisterLogger(Action<string> logAction)
        {
            if (!initialized.IsValueCreated)
            {
                Action<LogMessage> a = m => logAction(m.Value);
                Action<ContainerBuilder> registration = builder => builder.Register(c => a);
                registrations.Enqueue(registration);
            }

            return this;
        }

        public IBotBuilder RegisterDataStore<TDataStore>()
            where TDataStore : IBotDataStore<BotData>
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<TDataStore>().Keyed<IBotDataStore<BotData>>(typeof(ConnectorStore)).InstancePerLifetimeScope();
                registrations.Enqueue(registration);
            }

            return this;
        }

        public IBotBuilder RegisterBotToUser<TBotToUser>()
            where TBotToUser : IBotToUser
        {
            if (!initialized.IsValueCreated)
            {
                Action<ContainerBuilder> registration = builder => builder.RegisterType<TBotToUser>().As<IBotToUser>().InstancePerLifetimeScope();
                registrations.Enqueue(registration);
            }

            return this;
        }

        //public async Task<object> SendAsync(HttpRequestMessage req)
        //{
        //    if (!initialized.IsValueCreated)
        //    {
        //        await (initialized.Value);
        //    }

        //    using (BotService.Initialize())
        //    {
        //        string jsonContent = await req.Content.ReadAsStringAsync();
        //        var activity = JsonConvert.DeserializeObject<Activity>(jsonContent);

        //        if (!await BotService.Authenticator.TryAuthenticateAsync(req, new[] { activity }, CancellationToken.None))
        //        {
        //            return BotAuthenticator.GenerateUnauthorizedResponse(req);
        //        }

        //        if (activity != null)
        //        {
        //            await Conversation.Container.ResolveKeyed<IActivityHandler>(activity.GetActivityType()).Handle(activity);
        //        }
        //    }

        //    return req.CreateResponse(HttpStatusCode.Accepted);
        //}

        public async Task<HttpResponseMessage> SendAsync(Activity activity)
        {
            if (!initialized.IsValueCreated)
            {
                await (initialized.Value);
            }

            if (activity != null)
            {
                await Conversation.Container.ResolveKeyed<IActivityHandler>(activity.GetActivityType()).Handle(activity);
            }

            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        private static async Task<bool> Initialize()
        {
            Func<Task> bootstrappTask;
            while (bootstrappers.TryDequeue(out bootstrappTask))
            {
                await bootstrappTask();
            }

            ContainerBuilder builder = new ContainerBuilder();

            //builder.RegisterModule(new ReflectionSurrogateModule());
            //Default activity handlers configuration
            builder.RegisterType<MessageActivityHandlers>().Keyed<IActivityHandler>(ActivityTypes.Message);
            builder.RegisterType<ConversationUpdateActivityHandlers>().Keyed<IActivityHandler>(ActivityTypes.ConversationUpdate);
            builder.RegisterType<DefaultActivityHandlers>().Keyed<IActivityHandler>(ActivityTypes.ContactRelationUpdate);
            builder.RegisterType<DefaultActivityHandlers>().Keyed<IActivityHandler>(ActivityTypes.DeleteUserData);
            builder.RegisterType<DefaultActivityHandlers>().Keyed<IActivityHandler>(ActivityTypes.EndOfConversation);
            builder.RegisterType<DefaultActivityHandlers>().Keyed<IActivityHandler>(ActivityTypes.Ping);
            builder.RegisterType<DefaultActivityHandlers>().Keyed<IActivityHandler>(ActivityTypes.Trigger);
            builder.RegisterType<DefaultActivityHandlers>().Keyed<IActivityHandler>(ActivityTypes.Typing);

            builder.RegisterType<UpdateActivityMessage>().As<IUpdateActivityMessage>();
            builder.RegisterType<HttpService>().As<IHttpService>();
            builder.RegisterType<WitService>().As<IWitService>();
            builder.RegisterType<RootDialog>();

            builder.Register<Func<string, IDialog<object>>>(c =>
            {
                var ctx = c.Resolve<IComponentContext>();
                return (s) => ctx.ResolveKeyed<IDialog<object>>(s);
            });

            builder.Register<Func<string, IDialogHttpResumeConfiguration>>(c =>
            {
                var ctx = c.Resolve<IComponentContext>();
                return (s) => ctx.ResolveKeyed<IDialogHttpResumeConfiguration>(s);
            });

            Action<ContainerBuilder> registration;
            while (registrations.TryDequeue(out registration))
            {
                registration(builder);
            }
            
            builder.Update(Conversation.Container);
            return true;
        }
    }
}