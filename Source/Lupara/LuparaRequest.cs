using System;

namespace Lupara
{
    public interface ILuparaRequest
    {
        Uri Uri { get; set; }
        string Method { get; set; }
        string Content { get; set; }
        string ContentType { get; set; }
        int Timeout { get; set; }
        int ReadWriteTimeout { get; set; }
    }

    public class LuparaRequest : ILuparaRequest
    {
        public Uri Uri { get; set; }
        public string Method { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public int Timeout { get; set; }
        public int ReadWriteTimeout { get; set; }
    }
}