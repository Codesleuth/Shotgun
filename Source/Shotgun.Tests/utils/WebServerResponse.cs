using System.Net;

namespace Shotgun.AcceptanceTests.utils
{
    public class WebServerResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Body { get; set; }

        public static WebServerResponse Ok(string body = "")
        {
            return new WebServerResponse
            {
                Body = body,
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}