using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Shotgun.AcceptanceTests.utils.http
{
    public class StallConnectionCloseHandler : HttpServerHandler
    {
        public int StallMilliseconds { get; private set; }

        public StallConnectionCloseHandler(int stallMilliseconds)
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

                    // Stall
                    Thread.Sleep(StallMilliseconds);
                }
            }
        }
    }
}