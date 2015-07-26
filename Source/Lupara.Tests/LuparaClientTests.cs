using System;
using System.Net;
using Hauberk;
using NUnit.Framework;

namespace Lupara.Tests
{
    [TestFixture]
    public class LuparaClientTests
    {
        private ILuparaResponse _response;
        private HauberkServer _server;

        [TestFixtureSetUp]
        public void GivenABodyWhenRequesting()
        {
            _server = new HauberkServer(new StandardResponseHandler(HttpStatusCode.Forbidden), 65432);
            _server.Start();

            var client = new LuparaClient();

            var request = new LuparaRequest
            {
                Method = "POST",
                Uri = new Uri("http://localhost:65432"),
                Content = "hello",
                ContentType = "text/plain",
                Timeout = 2000
            };

            _response = client.Execute(request);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _server.Stop();
        }

        [Test]
        public void ThenTheResponseStatusCodeIsForbidden()
        {
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }
    }
}