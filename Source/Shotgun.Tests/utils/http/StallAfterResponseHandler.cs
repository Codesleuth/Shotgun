using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Shotgun.AcceptanceTests.utils.http
{
    public class StallAfterResponseHandler : HttpServerHandler
    {
        public int StallMilliseconds { get; private set; }

        public StallAfterResponseHandler(int stallMilliseconds)
        {
            StallMilliseconds = stallMilliseconds;
        }

        public override void Handle(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    // Skip request and headers
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                            throw new Exception("Unexpected stream end");

                        if (line.Length == 0)
                            break;
                    }

                    // Read all content
                    reader.ReadToEnd();
                }

                using (var writer = new StreamWriter(stream))
                {
                    writer.Write("HTTP/1.1 200 OK\r\n");

                    // Stall
                    Thread.Sleep(StallMilliseconds);
                }
            }
        }
    }
}