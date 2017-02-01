using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Qooba.Bot.Builder.Abstractions;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace Qooba.Bot.Builder.Dialogs
{
    public interface IDialogFormHttpResumeConfiguration<TForm> : IDialogHttpResumeConfiguration
    {
    }
}