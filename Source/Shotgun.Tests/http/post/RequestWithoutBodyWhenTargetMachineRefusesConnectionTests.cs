using System;
using System.Net;
using NUnit.Framework;
using Shotgun.AcceptanceTests.utils;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun.AcceptanceTests.http.post
{
    [TestFixture(Method.GET)]
    [TestFixture(Method.DELETE)]
    public class RequestWithoutBodyWhenTargetMachineRefusesConnectionTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;
        private readonly Method _method;

        public RequestWithoutBodyWhenTargetMachineRefusesConnectionTests(Method method)
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
                Method = _method
            };

            var client = new ShotgunClient();

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
            Assert.That(_response.ResponseStatus, Is.EqualTo(ResponseStatus.ConnectFailure));
        }

        [Test]
        public void ThenTheResponseFinishedWithTheDefaultTwoSecondsConnectTimeout()
        {
            Assert.That(_easyTimer.ElapsedMilliseconds, Is.EqualTo(2000).Within(500));
        }
    }
}