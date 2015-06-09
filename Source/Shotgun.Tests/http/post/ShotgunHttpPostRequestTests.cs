using System;
using System.Net;
using NUnit.Framework;
using Shotgun.AcceptanceTests.utils;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun.AcceptanceTests.http.post
{
    [TestFixture]
    public class ShotgunHttpPostRequestTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;
        private WebServer _webServer;
        private string _receivedBody;
        private int _requestCount;
        private string _expectedResponseContent;
        private string _expectedRequestContentType;
        private string _receivedContentType;
        private string _expectedRequestContent;

        [TestFixtureSetUp]
        public void GivenARequestWhenGettingResponse()
        {
            const string url = "http://localhost:60001/";

            _expectedRequestContent = "hey yo!";
            _expectedRequestContentType = "text/plain";
            _requestCount = 0;

            _expectedResponseContent = "Hello World!";

            _webServer = new WebServer(webServerRequest =>
            {
                _receivedContentType = webServerRequest.HttpListenerRequest.ContentType;
                _receivedBody = webServerRequest.GetBodyString();
                _requestCount ++;
                return WebServerResponse.Ok(_expectedResponseContent);
            }, url);
            _webServer.Start();

            var request = new ShotgunRequest
            {
                Uri = new Uri(url),
                Method = Method.POST,
                Body = new StringRequestBody
                {
                    ContentType = _expectedRequestContentType,
                    Content = _expectedRequestContent
                }
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

        [Test]
        public void ThenASingleRequestIsReceivedByTheServer()
        {
            Assert.That(_requestCount, Is.EqualTo(1));
        }

        [Test]
        public void ThenTheExpectedRequestContentSent()
        {
            Assert.That(_receivedBody, Is.EqualTo(_expectedRequestContent));
        }

        [Test]
        public void ThenTheExpectedRequestContentTypeWasSent()
        {
            Assert.That(_receivedContentType, Is.EqualTo(_expectedRequestContentType));
        }
    }
}