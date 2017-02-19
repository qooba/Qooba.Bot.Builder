using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Qooba.Bot.Builder.BotDataStores
{
    public class InMemoryDataStore : IBotDataStore<BotData>
    {
        private static ConcurrentDictionary<string, string> store = new ConcurrentDictionary<string, string>();
        private readonly Dictionary<BotStoreType, object> locks = new Dictionary<BotStoreType, object>()
        {
            { BotStoreType.BotConversationData, new object() },
            { BotStoreType.BotPrivateConversationData, new object() },
            { BotStoreType.BotUserData, new object() }
        };

        async Task<BotData> IBotDataStore<BotData>.LoadAsync(IAddress key, BotStoreType botStoreType, CancellationToken cancellationToken)
        {
            string serializedData;
            if (store.TryGetValue(GetKey(key, botStoreType), out serializedData))
                return Deserialize(serializedData);
            return new BotData(eTag: String.Empty);
        }

        async Task IBotDataStore<BotData>.SaveAsync(IAddress key, BotStoreType botStoreType, BotData botData, CancellationToken cancellationToken)
        {
            lock (locks[botStoreType])
            {
                if (botData.Data != null)
                {
                    store.AddOrUpdate(GetKey(key, botStoreType), (dictionaryKey) =>
                    {
                        botData.ETag = Guid.NewGuid().ToString("n");
                        return Serialize(botData);
                    }, (dictionaryKey, value) =>
                    {
                        ValidateETag(botData, value);
                        botData.ETag = Guid.NewGuid().ToString("n");
                        return Serialize(botData);
                    });
                }
                else
                {
                    // remove record on null
                    string value;
                    if (store.TryGetValue(GetKey(key, botStoreType), out value))
                    {
                        ValidateETag(botData, value);
                        store.TryRemove(GetKey(key, botStoreType), out value);
                        return;
                    }
                }
            }
        }

        private static void ValidateETag(BotData botData, string value)
        {
            if (botData.ETag != "*" && Deserialize(value).ETag != botData.ETag)
            {
                throw new Exception("Inconsistent SaveAsync based on ETag!");
                //throw new HttpException((int)HttpStatusCode.PreconditionFailed, "Inconsistent SaveAsync based on ETag!");
            }
        }

        Task<bool> IBotDataStore<BotData>.FlushAsync(IAddress key, CancellationToken cancellationToken)
        {
            // Everything is saved. Flush is no-op
            return Task.FromResult(true);
        }

        private static string GetKey(IAddress key, BotStoreType botStoreType)
        {
            switch (botStoreType)
            {
                case BotStoreType.BotConversationData:
                    return $"conversation:{key.BotId}:{key.ChannelId}:{key.ConversationId}";
                case BotStoreType.BotUserData:
                    return $"user:{key.BotId}:{key.ChannelId}:{key.UserId}";
                case BotStoreType.BotPrivateConversationData:
                    return $"privateConversation:{key.BotId}:{key.ChannelId}:{key.UserId}:{key.ConversationId}";
                default:
                    throw new ArgumentException("Unsupported bot store type!");
            }
        }

        private static string Serialize(BotData data)
        {
            using (var cmpStream = new MemoryStream())
            using (var stream = new GZipStream(cmpStream, CompressionMode.Compress))
            using (var streamWriter = new StreamWriter(stream))
            {
                var serializedJSon = JsonConvert.SerializeObject(data);
                streamWriter.Write(serializedJSon);
                streamWriter.Close();
                stream.Close();
                return Convert.ToBase64String(cmpStream.ToArray());
            }
        }

        private static BotData Deserialize(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            using (var stream = new MemoryStream(bytes))
            using (var gz = new GZipStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gz))
            {
                return JsonConvert.DeserializeObject<BotData>(streamReader.ReadToEnd());
            }
        }
    }
}
