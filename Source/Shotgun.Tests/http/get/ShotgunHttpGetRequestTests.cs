using System;
using System.Net;
using NUnit.Framework;
using Shotgun.AcceptanceTests.utils;
using Shotgun.AcceptanceTests.utils.http;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun.AcceptanceTests.http.get
{
    [TestFixture]
    public class ShotgunHttpGetRequestTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;
        private HttpServer _server;

        [TestFixtureSetUp]
        public void GivenAGetMethodRequestWhenGettingResponse()
        {
            const string url = "http://localhost:7896/";

            _server = new HttpServer(new StandardResponseHandler(HttpStatusCode.OK), 7896);
            _server.Start();

            var request = new ShotgunRequest
            {
                Uri = new Uri(url), 
                Method = Method.GET
            };

            var client = new ShotgunClient();

            using (_easyTimer = EasyTimer.StartNew())
            {
                _response = client.Execute(request);
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _server.Stop();
        }

        [Test]
        public void ThenTheResponseStatusCodeIsOk()
        {
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void ThenTheExpectedResponseContentWasEmpty()
        {
            Assert.That(_response.Content, Is.Null);
        }

        [Test]
        public void ThenTheResponseStatusIsCompleted()
        {
            Assert.That(_response.ResponseStatus, Is.EqualTo(ResponseStatus.Completed));
        }

        [Test]
        public void ThenTheResponseFinishedWithoutDelay()
        {
            Assert.That(_easyTimer.ElapsedMilliseconds, Is.LessThanOrEqualTo(1500));
        }
    }
}
