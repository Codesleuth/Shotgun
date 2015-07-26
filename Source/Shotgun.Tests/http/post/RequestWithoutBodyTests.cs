using System;
using System.Net;
using NUnit.Framework;
using Shotgun.AcceptanceTests.utils;
using Shotgun.AcceptanceTests.utils.http;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun.AcceptanceTests.http.post
{
    [TestFixture(Method.GET)]
    [TestFixture(Method.DELETE)]
    public class RequestWithoutBodyTests
    {
        private IShotgunResponse _response;
        private EasyTimer _easyTimer;
        private WebServer _webServer;
        private string _receivedBody;
        private int _requestCount;
        private string _expectedResponseContent;
        private string _receivedContentType;
        private readonly Method _method;

        public RequestWithoutBodyTests(Method method)
        {
            _method = method;
        }

        [TestFixtureSetUp]
        public void GivenARequestWhenGettingResponse()
        {
            const string url = "http://localhost:60001/";

            _requestCount = 0;

            _expectedResponseContent = "Hello World!";

            var server = new HttpServer(new StandardResponseHandler(HttpStatusCode.OK), 60001);
            server.Start();

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
                Method = _method
            };

            var client = new ShotgunClient
            {
                Timeout = 2000,
                ReadWriteTimeout = 2000
            };

            using (_easyTimer = EasyTimer.StartNew())
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
        public void ThenNoRequestContentSent()
        {
            Assert.That(_receivedBody, Is.Empty);
        }

        [Test]
        public void ThenNoRequestContentTypeWasSent()
        {
            Assert.That(_receivedContentType, Is.Null);
        }
    }
}