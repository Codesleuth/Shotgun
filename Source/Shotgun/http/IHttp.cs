using System;
using System.Net;
using Shotgun.models;

namespace Shotgun.http
{
    internal interface IHttp
    {
        IWebProxy Proxy { get; set; }
        Uri Url { get; set; }
        string UserAgent { get; set; }
        int Timeout { get; set; }
        int ReadWriteTimeout { get; set; }
        IRequestBody Body { get; set; }

        IHttpResponse ExecuteGetRequest(string method);
        IHttpResponse ExecutePostRequest(string method);
    }
}