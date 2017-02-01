using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using System;

namespace Qooba.Bot.Builder.Wit.Models
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [Serializable]
    public class WitIntentAttribute : AttributeString
    {
        public readonly string IntentName;

        protected override string Text
        {
            get
            {
                return this.IntentName;
            }
        }

        public WitIntentAttribute(string intentName)
        {
            SetField.NotNull<string>(out this.IntentName, "intentName", intentName);
        }
    }
}