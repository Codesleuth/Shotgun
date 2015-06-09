using System;
using System.Net;

namespace Shotgun.models
{
    public interface IShotgunResponse
    {
        IShotgunRequest Request { get; set; }
        ResponseStatus ResponseStatus { get; set; }
        string ErrorMessage { get; set; }
        Exception ErrorException { get; set; }
        HttpStatusCode StatusCode { get; set; }
        string StatusDescription { get; set; }
        string Content { get; set; }
        WebExceptionStatus WebExceptionStatus { get; set; }
    }
}