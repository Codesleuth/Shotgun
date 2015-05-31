using System;
using System.Net;
using NUnit.Framework;
using Shotgun.AcceptanceTests.utils;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun.AcceptanceTests.http.get
{
    [TestFixture]
    public class ShotgunHttpGetRequestTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;
        private WebServer _webServer;
        private string _expectedResponseContent;

        [TestFixtureSetUp]
        public void GivenAGetMethodRequestWhenGettingResponse()
        {
            const string url = "http://localhost:7896/";

            _expectedResponseContent = "hey yo!";

            _webServer = new WebServer(webServerRequest => WebServerResponse.Ok(_expectedResponseContent), url);
            _webServer.Start();

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

        [TestFixtureTearDown]
        public void TearDown()
        {
            _webServer.Stop();
        }

        [Test]
        public void ThenTheResponseStatusCodeIsOk()
        {
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void ThenTheExpectedResponseContentWasReceived()
        {
            Assert.That(_response.Content, Is.EqualTo(_expectedResponseContent));
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
