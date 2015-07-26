using System.Net;

namespace Shotgun.AcceptanceTests.utils
{
    public class WebServerResponse
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string Body { get; private set; }
        public string ContentType { get; private set; }

        public static WebServerResponse Ok(string body = "", string contentType = "text/plain")
        {
            return new WebServerResponse
            {
                Body = body,
                ContentType = contentType,
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}