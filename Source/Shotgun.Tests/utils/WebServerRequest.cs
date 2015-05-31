using System.IO;
using System.Net;
using System.Text;

namespace Shotgun.AcceptanceTests.utils
{
    public class WebServerRequest
    {
        public HttpListenerRequest HttpListenerRequest { get; private set; }

        public WebServerRequest(HttpListenerRequest httpListenerRequest)
        {
            HttpListenerRequest = httpListenerRequest;
        }

        public string GetBodyString()
        {
            using (var memoryStream = new MemoryStream())
            {
                HttpListenerRequest.InputStream.CopyTo(memoryStream);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}