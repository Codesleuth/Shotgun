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

            var client = LuparaFactory.CreateClient();
            var request = LuparaFactory.CreateRequest();

            request.Method = "POST";
            request.Uri = new Uri("http://localhost:65432");
            request.Content = "hello";
            request.ContentType = "text/plain";
            request.Timeout = 2000;
            request.ReadWriteTimeout = 2000;

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