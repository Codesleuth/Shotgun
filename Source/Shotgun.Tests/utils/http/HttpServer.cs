using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Shotgun.AcceptanceTests.utils.http
{
    public class HttpServer
    {
        private readonly HttpServerHandler _handler;
        private readonly TcpListener _listener;
        private readonly object _lockState = new object();
        private IAsyncResult _asyncResult;
        private bool _running;

        public HttpServer(HttpServerHandler handler, int port, IPAddress ipAddress = null)
        {
            _handler = handler;
            _listener = new TcpListener(ipAddress ?? IPAddress.Loopback, port);
        }

        private void ListenAsync()
        {
            if (!_running) return;

            try
            {
                lock (_lockState)
                {
                    _asyncResult = _listener.BeginAcceptTcpClient(HandleTcpClient, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled exception in WebServer.ListenAsync: " + ex.Message);
            }
        }

        public void Start()
        {
            _running = true;
            _listener.Start();
            ListenAsync();
        }

        private void HandleTcpClient(IAsyncResult asyncResult)
        {
            if (!_running) return;

            try
            {
                lock (_lockState)
                {
                    var tcpClient = _listener.EndAcceptTcpClient(asyncResult);
                    _handler.Handle(tcpClient);
                    _asyncResult = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled exception in WebServer.HandleTcpClient: " + ex.Message);
            }
            finally
            {
                ListenAsync();
            }
        }

        public void Stop()
        {
            _running = false;
            _listener.Stop();
            lock (_lockState)
            {
                _asyncResult?.AsyncWaitHandle.WaitOne(1000);
            }
        }
    }
}