using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Hauberk
{
    public class HauberkServer
    {
        private readonly IHauberkRequestHandler _handler;
        private readonly TcpListener _listener;
        private bool _running;

        public HauberkServer(IHauberkRequestHandler handler, int port, IPAddress ipAddress = null)
        {
            _handler = handler;
            _listener = new TcpListener(ipAddress ?? IPAddress.Loopback, port);
        }

        private void AcceptClient(TcpListener tcpListener)
        {
            Task.Factory
                .FromAsync<TcpClient>(tcpListener.BeginAcceptTcpClient, tcpListener.EndAcceptTcpClient, tcpListener)
                .ContinueWith(task =>
                {
                    HandleTcpClient(task.Result);
                    AcceptClient(tcpListener);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void Start()
        {
            _running = true;
            _listener.Start();
            AcceptClient(_listener);
        }

        private void HandleTcpClient(TcpClient client)
        {
            if (!_running) return;

            try
            {
                _handler.Handle(client);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled exception in HauberkServer.HandleTcpClient: " + ex.Message);
            }
        }

        public void Stop()
        {
            _running = false;
            _listener.Stop();
        }
    }
}