using System;
using Microsoft.Bot.Builder.FormFlow;
using Qooba.Bot.Builder.Abstractions;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Qooba.Bot.Builder.Dialogs
{
    [Serializable]
    public class DialogFormBuilder<TForm> : IDialogFormBuilder<TForm>
    where TForm : class, new()
    {
        private static IDictionary<string, Action<TForm, TForm>> setters = new ConcurrentDictionary<string, Action<TForm, TForm>>();

        private static IDictionary<string, Func<TForm, bool>> checkers = new ConcurrentDictionary<string, Func<TForm, bool>>();

        private static IList<string> fieldNames;

        private Lazy<TForm> model;

        private JObject modelObject;

        public virtual IForm<TForm> BuildForm()
        {
            IFormBuilder<TForm> fb = new FormBuilder<TForm>().AddRemainingFields();
            if (fieldNames == null)
            {
                fieldNames = typeof(TForm).GetProperties().Select(x => x.Name).ToList();
            }

            foreach (var name in fieldNames)
            {
                fb.Field(name, active: (state) =>
                {
                    GetValueSetter(name)(state, Model);
                    return CheckValue(name)(state);
                });
            }

            return fb.Build();
        }

        public TForm Model => model.Value;

        public JObject ModelObject
        {
            get
            {
                return modelObject;
            }
            set
            {
                model = new Lazy<TForm>(() => value != null ? value.ToObject<TForm>() : new TForm());
                modelObject = value;
            }
        }

        private Action<TForm, TForm> GetValueSetter(string propertyName)
        {
            Action<TForm, TForm> action;
            if (!setters.TryGetValue(propertyName, out action))
            {
                var formType = typeof(TForm);
                var propertyType = formType.GetProperty(propertyName).PropertyType;
                var instance = Expression.Parameter(formType, "instance");
                var argument = Expression.Parameter(formType, "argument");
                var property = Expression.Property(instance, propertyName);
                var inProperty = Expression.Property(argument, propertyName);
                var assign = Expression.Assign(property, inProperty);
                action = Expression.Lambda<Action<TForm, TForm>>(assign, instance, argument).Compile();
                setters[propertyName] = action;
                return action;
            }

            return action;
        }

        public Func<TForm, bool> CheckValue(string propertyName)
        {
            Func<TForm, bool> action;
            if (!checkers.TryGetValue(propertyName, out action))
            {
                var formType = typeof(TForm);
                var propertyType = formType.GetProperty(propertyName).PropertyType;

                if (propertyType.IsClass)
                {
                    var instance = Expression.Parameter(formType, "instance");
                    var property = Expression.Equal(Expression.Property(instance, propertyName), Expression.Constant(null, typeof(object)));
                    action = Expression.Lambda<Func<TForm, bool>>(property, instance).Compile();
                }
                else
                {
                    action = f => true;
                }

                checkers[propertyName] = action;
            }

            return action;
        }
    }
}
