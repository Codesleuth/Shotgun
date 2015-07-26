using System;
using System.Net;
using NUnit.Framework;
using Shotgun.AcceptanceTests.utils;
using Shotgun.AcceptanceTests.utils.http;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun.AcceptanceTests.http.post
{
    [TestFixture(Method.POST)]
    [TestFixture(Method.PATCH)]
    [TestFixture(Method.PUT)]
    [TestFixture(Method.MERGE)]
    [TestFixture(Method.OPTIONS)]
    public class RequestWithoutBodyWithConnectionTimeoutAfterConnectTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;
        private HttpServer _httpServer;
        private StallFirstByteHandler _handler;
        private readonly Method _method;

        public RequestWithoutBodyWithConnectionTimeoutAfterConnectTests(Method method)
        {
            _method = method;
        }

        [TestFixtureSetUp]
        public void GivenAGetMethodRequestWhenGettingResponse()
        {
            const string url = "http://localhost:60001/";

            _handler = new StallFirstByteHandler(3000);

            _httpServer = new HttpServer(_handler, 60001);
            _httpServer.Start();

            var request = new ShotgunRequest
            {
                Uri = new Uri(url),
                Method = _method,
                Body = new StringRequestBody
                {
                    Content = "this is some body"
                }
            };

            var client = new ShotgunClient
            {
                Timeout = 2000
            };

            using (_easyTimer = EasyTimer.StartNew())
            {
                _response = client.Execute(request);
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _httpServer.Stop();
        }

        [Test]
        public void ThenTheHandlerReceivesTheRequest()
        {
            Assert.That(_handler.HandleCount, Is.EqualTo(1));
        }

        [Test]
        public void ThenTheResponseStatusCodeIsNotSet()
        {
            Assert.That(_response.StatusCode, Is.EqualTo((HttpStatusCode) 0));
        }

        [Test]
        public void ThenTheResponseStatusIsTimedOut()
        {
            Assert.That(_response.ResponseStatus, Is.EqualTo(ResponseStatus.TimedOut));
        }

        [Test]
        public void ThenTheResponseFinishedWithinTheTimeoutPeriodPlusGracePeriodForThrowingStupidExceptions()
        {
            Assert.That(_easyTimer.ElapsedMilliseconds, Is.InRange(2000, 2500));
        }
    }
}