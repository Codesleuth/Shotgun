using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Shotgun.AcceptanceTests.utils
{
    public class SlowBodyHandler : HttpServerHandler
    {
        public int StallMilliseconds { get; private set; }

        public SlowBodyHandler(int stallMilliseconds)
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

                    var b = 0;
                    while (b > -1)
                    {
                        b = stream.ReadByte();
                        Thread.Sleep(StallMilliseconds);
                    }
                }
            }
        }
    }
}