﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace Qooba.Bot.Builder.Abstractions
{
    public interface IDialogResume
    {
        Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument);
    }
}