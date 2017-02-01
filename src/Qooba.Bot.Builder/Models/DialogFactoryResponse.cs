using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;

namespace Qooba.Bot.Builder.Models
{
    public class DialogFactoryResponse
    {
        public DialogFactoryResponse(IDialog<object> dialog, object dialogKey, IEnumerable<DialogFactoryEntity> entities)
        {
            this.Dialog = dialog;
            this.DialogKey = dialogKey;
            this.Entities = entities;
        }

        public IDialog<object> Dialog { get; private set; }

        public object DialogKey { get; private set; }

        public IEnumerable<DialogFactoryEntity> Entities { get; private set; }
    }
}