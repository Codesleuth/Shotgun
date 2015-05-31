using System;
using System.Net;
using Shotgun.models;

namespace Shotgun.http
{
    public class ShotgunResponse : IShotgunResponse
    {
        public IShotgunRequest Request { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string Content { get; set; }
    }
}