using System;
using Shotgun.models;

namespace Shotgun.http
{
    public class ShotgunRequest : IShotgunRequest
    {
        public Method Method { get; set; }
        public Uri Uri { get; set; }

        public IRequestBody Body { get; set; }
    }
}