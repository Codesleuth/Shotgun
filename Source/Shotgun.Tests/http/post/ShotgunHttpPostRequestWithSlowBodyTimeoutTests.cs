using System;
using System.Net;
using NUnit.Framework;
using Shotgun.AcceptanceTests.utils;
using Shotgun.AcceptanceTests.utils.http;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun.AcceptanceTests.http.post
{
    [TestFixture]
    public class ShotgunHttpPostRequestWithSlowBodyTimeoutTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;
        private HttpServer _httpServer;

        [TestFixtureSetUp]
        public void GivenARequestWhenGettingResponse()
        {
            const string url = "http://localhost:60001/";

            _httpServer = new HttpServer(new StallBodyReadHandler(100), 60001);
            _httpServer.Start();

            var request = new ShotgunRequest
            {
                Uri = new Uri(url),
                Method = Method.POST,
                Body = new StringRequestBody
                {
                    Content = "01234567890123456789",
                    ContentType = "text/plain"
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
        public void ThenTheResponseStatusCodeIsNotSet()
        {
            Assert.That(_response.StatusCode, Is.EqualTo((HttpStatusCode)0));
        }

        [Test]
        public void ThenTheResponseStatusIsTimedOut()
        {
            Assert.That(_response.ResponseStatus, Is.EqualTo(ResponseStatus.TimedOut));
        }

        [Test]
        public void ThenTheResponseFinishedWithinTheTimeoutPeriodPlusGracePeriodForThrowingStupidExceptions()
        {
            Assert.That(_easyTimer.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(2000).And.LessThanOrEqualTo(3000));
        }
    }
}