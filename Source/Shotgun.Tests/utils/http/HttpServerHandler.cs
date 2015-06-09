using System.Net.Sockets;

namespace Shotgun.AcceptanceTests.utils.http
{
    public abstract class HttpServerHandler
    {
        public abstract void Handle(TcpClient client);
    }
}