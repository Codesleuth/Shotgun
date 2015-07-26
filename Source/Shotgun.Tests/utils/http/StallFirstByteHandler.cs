using System.Net.Sockets;
using System.Threading;

namespace Shotgun.AcceptanceTests.utils.http
{
    public class StallFirstByteHandler : HttpServerHandler
    {
        public int StallMilliseconds { get; }
        public int HandleCount { get; private set; }

        public StallFirstByteHandler(int stallMilliseconds)
        {
            StallMilliseconds = stallMilliseconds;
        }

        public override void Handle(TcpClient client)
        {
            HandleCount++;
            using (client.GetStream())
            {
                Thread.Sleep(StallMilliseconds);
            }
        }
    }
}