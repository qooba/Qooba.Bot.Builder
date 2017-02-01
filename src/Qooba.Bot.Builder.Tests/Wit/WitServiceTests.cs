using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Xunit;
using Qooba.Bot.Builder.Wit;

namespace Qooba.Bot.Builder.Tests.Wit
{
    public class WitServiceTest
    {
        [Fact]
        public void QueryMessageAsyncTest()
        {
            //var witService = new WitService();
            //var result = witService.QueryMessageAsync("Chcę kupić sukienkę", CancellationToken.None).Result;
            //Assert.True(result.);
        }

        [Fact]
        public void QuerySpeechAsyncTest()
        {
            Assert.True(true);
        }
    }
}
