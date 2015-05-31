using System.IO;
using Shotgun.http;

namespace Shotgun.models
{
    public class StreamRequestBody : IRequestBody
    {
        public string ContentType { get; set; }
        public Stream Content { get; set; }

        public void WriteToStream(Stream stream)
        {
            Content.CopyTo(stream);
        }
    }
}