using System;

namespace Qooba.Bot.Builder.Dialogs
{
    [Serializable]
    public class DialogFormHttpResumeConfiguration<TForm> : DialogHttpResumeConfiguration, IDialogFormHttpResumeConfiguration<TForm>
    {
        public DialogFormHttpResumeConfiguration(Uri uri, string cancelMessage) : base(uri, cancelMessage)
        {
        }
    }
}