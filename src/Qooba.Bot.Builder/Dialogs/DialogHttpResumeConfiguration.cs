using System;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace Qooba.Bot.Builder.Dialogs
{
    [Serializable]
    public class DialogHttpResumeConfiguration : IDialogHttpResumeConfiguration
    {
        private readonly Uri uri;
        
        private readonly string cancelMessage;

        public DialogHttpResumeConfiguration(Uri uri, string cancelMessage)
        {
            SetField.NotNull(out this.uri, "uri", uri);
            SetField.NotNull(out this.cancelMessage, "cancelMessage", cancelMessage);
        }

        public Uri Uri => this.uri;
        
        public string CancelMessage => this.cancelMessage;
    }
}