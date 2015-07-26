using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Shotgun.AcceptanceTests.utils
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<WebServerRequest, WebServerResponse> _responderHandler;
        private readonly object _lockState = new object();
        private IAsyncResult _asyncResult;

        private WebServer(ICollection<string> prefixes, Func<WebServerRequest, WebServerResponse> handler)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

            if (prefixes == null || prefixes.Count == 0)
                throw new ArgumentException("prefixes");

            if (handler == null)
                throw new ArgumentException("handler");

            foreach (var s in prefixes)
                _listener.Prefixes.Add(s);

            _responderHandler = handler;
        }

        public WebServer(Func<WebServerRequest, WebServerResponse> method, params string[] prefixes)
            : this(prefixes, method)
        {
            
        }

        private void ListenAsync()
        {
            try
            {
                if (!_listener.IsListening)
                    return;

                lock (_lockState)
                {
                    _asyncResult = _listener.BeginGetContext(HandleRequest, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled exception in WebServer.ListenAsync: " + ex.Message);
            }
        }

        public void Start()
        {
            _listener.Start();
            ListenAsync();
        }

        private void HandleRequest(IAsyncResult asyncResult)
        {
            try
            {
                if (!_listener.IsListening)
                    return;

                lock (_lockState)
                {
                    var context = _listener.EndGetContext(asyncResult);
                    var webServerRequest = new WebServerRequest(context.Request);
                    
                    var responseContent = _responderHandler(webServerRequest);
                    context.Response.StatusCode = (int) responseContent.StatusCode;
                    context.Response.ContentType = responseContent.ContentType;

                    if (!string.IsNullOrEmpty(responseContent.Body))
                    {
                        var bytes = Encoding.UTF8.GetBytes(responseContent.Body);
                        context.Response.ContentLength64 = bytes.Length;
                        context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                    }

                    _asyncResult = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled exception in WebServer.HandleRequest: " + ex.Message);
            }
            finally
            {
                ListenAsync();
            }
        }

        public void Stop()
        {
            _listener.Stop();
            lock (_lockState)
            {
                _asyncResult?.AsyncWaitHandle.WaitOne(1000);
            }
        }
    }
}