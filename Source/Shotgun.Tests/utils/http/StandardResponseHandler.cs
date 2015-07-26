using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Shotgun.AcceptanceTests.utils.http
{
    public class StandardResponseHandler : HttpServerHandler
    {
        private readonly HttpStatusCode _responseStatusCode;

        public string RequestMethod { get; private set; }
        public string RequestBody { get; private set; }

        public StandardResponseHandler(HttpStatusCode responseStatusCode)
        {
            _responseStatusCode = responseStatusCode;
        }

        public override void Handle(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    // Read control line
                    var line = ReadLine(reader);

                    var methodLength = line.IndexOf(" ", StringComparison.Ordinal);
                    RequestMethod = line.Substring(0, methodLength);

                    // Read headers until body
                    while (true)
                    {
                        line = reader.ReadLine();
                        if (line == null)
                            throw new Exception("Unexpected stream end");

                        // Blank line = end of headers
                        if (line.Length == 0)
                            break;
                    }

                    // Read body
                    if (RequestMethod != "GET" && RequestMethod != "DELETE" && RequestMethod != "HEAD")
                        RequestBody = reader.ReadToEnd();
                }
            }
        }

        private static string ReadLine(TextReader reader)
        {
            var line = reader.ReadLine();
            if (line != null) return line;
            throw new Exception("Unexpected stream end");
        }
    }
}