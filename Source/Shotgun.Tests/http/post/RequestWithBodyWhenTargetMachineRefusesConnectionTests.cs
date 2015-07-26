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
    public class RequestWithBodyWhenTargetMachineRefusesConnectionTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;
        private readonly Method _method;

        public RequestWithBodyWhenTargetMachineRefusesConnectionTests(Method method)
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

            var client = new ShotgunClient();

            using (_easyTimer = EasyTimer.StartNew())
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
        public void ThenTheResponseStatusIsError()
        {
            Assert.That(_response.ResponseStatus, Is.EqualTo(ResponseStatus.Error));
        }

        [Test]
        public void ThenTheResponseErrorMessageIsAsExpected()
        {
            Assert.That(_response.ErrorMessage, Is.EqualTo("Unable to connect to the remote server"));
        }

        [Test]
        public void ThenTheResponseFinishedWithTheDefaultTwoSecondsConnectTimeout()
        {
            Assert.That(_easyTimer.ElapsedMilliseconds, Is.EqualTo(2000).Within(500));
        }
    }
}