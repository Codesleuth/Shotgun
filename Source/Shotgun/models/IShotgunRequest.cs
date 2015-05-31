using System;
using Shotgun.http;

namespace Shotgun.models
{
    public interface IShotgunRequest
    {
        Method Method { get; set; }
        Uri Uri { get; set; }
        IRequestBody Body { get; set; }
    }
}