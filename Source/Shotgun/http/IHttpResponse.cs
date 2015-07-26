using System;
using System.Net;
using Shotgun.models;

namespace Shotgun.http
{
    internal interface IHttpResponse
    {
        Exception ErrorException { get; set; }
        ResponseStatus ResponseStatus { get; set; }
        string ErrorMessage { get; set; }
        HttpStatusCode StatusCode { get; set; }
        string StatusDescription { get; set; }
        byte[] RawBytes { get; set; }
    }
}