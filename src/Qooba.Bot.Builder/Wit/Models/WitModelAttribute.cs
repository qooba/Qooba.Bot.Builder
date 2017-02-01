using Microsoft.Bot.Builder.Internals.Fibers;
using Newtonsoft.Json;
using System;

namespace Qooba.Bot.Builder.Wit.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    [Serializable]
    public class WitModelAttribute : Attribute, IWitModel, IEquatable<IWitModel>
    {
        private readonly string apiKey;

        public string ApiKey
        {
            get
            {
                return this.apiKey;
            }
        }

        public WitModelAttribute(string apiKey)
        {
            SetField.NotNull<string>(out this.apiKey, "apiKey", apiKey);
        }

        public bool Equals(IWitModel other) => other != null && object.Equals((object)this.ApiKey, (object)other.ApiKey);
        
        public override bool Equals(object other) => Equals(other as IWitModel);
        
        public override int GetHashCode() => ApiKey.GetHashCode();
    }
}