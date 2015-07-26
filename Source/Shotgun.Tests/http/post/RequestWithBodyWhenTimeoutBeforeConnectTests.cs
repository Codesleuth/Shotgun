using System;
using System.Net;
using NUnit.Framework;
using Shotgun.AcceptanceTests.utils;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun.AcceptanceTests.http.post
{
    [TestFixture(Method.POST)]
    [TestFixture(Method.PATCH)]
    [TestFixture(Method.PUT)]
    [TestFixture(Method.MERGE)]
    [TestFixture(Method.OPTIONS)]
    public class RequestWithBodyWhenTimeoutBeforeConnectTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;
        private readonly Method _method;

        public RequestWithBodyWhenTimeoutBeforeConnectTests(Method method)
        {
            _method = method;
        }

        [TestFixtureSetUp]
        public void GivenAGetMethodRequestWhenGettingResponse()
        {
            const string url = "http://localhost:7896/";

            var request = new ShotgunRequest
            {
                Uri = new Uri(url),
                Method = _method,
                Body = new StringRequestBody
                {
                    Content = "Some content"
                }
            };

            var client = new ShotgunClient {Timeout = 500};

            using (_easyTimer = EasyTimer.StartNew())
            {
                _response = client.Execute(request);
            }
        }

        [Test]
        public void ThenTheResponseStatusCodeIsNotSet()
        {
            Assert.That(_response.StatusCode, Is.EqualTo((HttpStatusCode)0));
        }

        [Test]
        public void ThenTheResponseStatusIsConnectFailure()
        {
            Assert.That(_response.ResponseStatus, Is.EqualTo(ResponseStatus.TimedOut));
        }

        [Test]
        public void ThenTheResponseErrorMessageIsTimedOut()
        {
            Assert.That(_response.ErrorMessage, Is.EqualTo("The operation has timed out"));
        }

        [Test]
        public void ThenTheResponseFinishedWithinTheTimePeriod()
        {
            Assert.That(_easyTimer.ElapsedMilliseconds, Is.InRange(500, 700));
        }
    }
}