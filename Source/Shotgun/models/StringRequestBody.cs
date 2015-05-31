using System.IO;
using System.Text;
using Shotgun.http;

namespace Shotgun.models
{
    public class StringRequestBody : IRequestBody
    {
        public string ContentType { get; set; }
        public string Content { get; set; }
        public Encoding Encoding { get; set; }

        public StringRequestBody()
        {
            Encoding = Encoding.UTF8;
        }

        public void WriteToStream(Stream stream)
        {
            var bytes = Encoding.GetBytes(Content);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}