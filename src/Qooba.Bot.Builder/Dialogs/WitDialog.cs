using Microsoft.Bot.Builder.Dialogs;
using Qooba.Bot.Builder.Wit;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Qooba.Bot.Builder.Wit.Models;

namespace Qooba.Bot.Builder.Dialogs
{
    internal static class WitDialog
    {
        public static IEnumerable<KeyValuePair<string, WitIntentActivityHandler>> EnumerateHandlers(object dialog)
        {
            MethodInfo[] methodInfoArray = dialog.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int index = 0; index < methodInfoArray.Length; ++index)
            {
                MethodInfo method = methodInfoArray[index];
                WitIntentAttribute[] intents = Enumerable.ToArray(CustomAttributeExtensions.GetCustomAttributes<WitIntentAttribute>(method, true));
                WitIntentActivityHandler intentHandler = null;
                try
                {
                    intentHandler = (WitIntentActivityHandler)Delegate.CreateDelegate(typeof(WitIntentActivityHandler), dialog, method, false);
                }
                catch (ArgumentException ex)
                {
                }
                if (intentHandler == null)
                {
                    try
                    {
                        WitIntentHandler handler = (WitIntentHandler)Delegate.CreateDelegate(typeof(WitIntentHandler), dialog, method, false);
                        if (handler != null)
                            intentHandler = (context, message, result) => handler(context, result);
                    }
                    catch (ArgumentException ex)
                    {
                    }
                }
                if (intentHandler != null)
                {
                    foreach (string key in Enumerable.DefaultIfEmpty(Enumerable.Select(intents, i => i.IntentName), method.Name))
                    {
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            string str = string.Empty;
                        }
                        yield return new KeyValuePair<string, WitIntentActivityHandler>(key, intentHandler);
                    }
                }
                else if (intents.Length != 0)
                    throw new InvalidIntentHandlerException(string.Join(";", Enumerable.Select(intents, i => i.IntentName)), method);
                intents = null;
                intentHandler = null;
                method = null;
            }
            methodInfoArray = null;
        }
    }
}
