using System;
using System.Net;
using NUnit.Framework;
using Shotgun.AcceptanceTests.utils;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun.AcceptanceTests.http.get
{
    [TestFixture]
    public class ShotgunHttpGetRequestWithConnectFailureTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;

        [TestFixtureSetUp]
        public void GivenAGetMethodRequestWhenGettingResponse()
        {
            const string url = "http://localhost:7896/";

            var request = new ShotgunRequest
            {
                Uri = new Uri(url),
                Method = Method.GET
            };

            var client = new ShotgunClient();

            using (_easyTimer = new EasyTimer())
            {
                _response = client.Execute(request);
            }
        }

        [Test]
        public void ThenTheResponseStatusCodeIsNotSet()
        {
            Assert.That(_response.StatusCode, Is.EqualTo((HttpStatusCode) 0));
        }

        [Test]
        public void ThenTheResponseStatusIsConnectFailure()
        {
            Assert.That(_response.ResponseStatus, Is.EqualTo(ResponseStatus.ConnectFailure));
        }

        [Test]
        public void ThenTheResponseFinishedWithoutDelay()
        {
            Assert.That(_easyTimer.ElapsedMilliseconds, Is.LessThanOrEqualTo(5000));
        }
    }
}