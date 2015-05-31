using System.IO;

namespace Shotgun.http
{
    public interface IRequestBody
    {
        string ContentType { get; }

        void WriteToStream(Stream stream);
    }
}