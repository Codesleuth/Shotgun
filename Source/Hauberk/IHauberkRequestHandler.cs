using System.Net.Sockets;

namespace Hauberk
{
    public interface IHauberkRequestHandler
    {
        void Handle(TcpClient client);
    }
}