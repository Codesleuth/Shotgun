using System;
using System.Net;
using Shotgun.models;

namespace Shotgun.http
{
    internal class HttpResponse : IHttpResponse
    {
        public Exception ErrorException { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public Uri ResponseUri { get; set; }
        public byte[] RawBytes { get; set; }
    }
}