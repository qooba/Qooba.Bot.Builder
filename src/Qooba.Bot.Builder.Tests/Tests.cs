using System;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void Test1() 
        {
            var t = System.Configuration.ConfigurationManager.AppSettings["WitApiKey"];
            Assert.True(true);
        }
    }
}
