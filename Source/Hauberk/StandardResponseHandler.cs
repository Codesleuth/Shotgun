using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hauberk
{
    public class StandardResponseHandler : IHauberkRequestHandler
    {
        private readonly HttpStatusCode _responseStatusCode;

        public StandardResponseHandler(HttpStatusCode responseStatusCode)
        {
            _responseStatusCode = responseStatusCode;
        }

        public string Method { get; private set; }
        public string ContentType { get; set; }
        public long? ContentLength { get; set; }

        public void Handle(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                var reader = new StreamReader(stream, Encoding.ASCII);

                // Read control line
                var line = ReadLine(reader);

                var methodLength = line.IndexOf(" ", StringComparison.Ordinal);
                Method = line.Substring(0, methodLength);

                // Read headers until body
                while (true)
                {
                    line = reader.ReadLine();
                    if (line == null)
                        throw new Exception("Unexpected stream end");

                    // Blank line = end of headers
                    if (line.Length == 0)
                        break;

                    var colonIndex = line.IndexOf(':');
                    var fieldName = line.Substring(0, colonIndex);

                    if (fieldName.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        ContentType = line.Substring(colonIndex);
                    }

                    if (fieldName.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    {
                        long contentLength;
                        long.TryParse(line.Substring(colonIndex), out contentLength);
                        ContentLength = contentLength;
                    }
                }

                // Read body
                if (ContentLength.HasValue && Method != "GET" && Method != "DELETE" && Method != "HEAD")
                {
                    ReadUpTo(stream, ContentLength.Value);
                }

                var writer = new StreamWriter(stream, Encoding.ASCII);
                writer.Write($"HTTP/1.1 {(int) _responseStatusCode} {_responseStatusCode}\r\n");
                writer.Write($"Date: {DateTime.Now.ToString("R")}\r\n");
                writer.Write("Content-Type: text/plain\r\n");
                writer.Write("Content-Length: 0\r\n");
                writer.Write("\r\n");
                writer.Write("This is a response!");
                writer.Flush();

                while (client.Connected)
                {
                    Thread.Sleep(20);
                }
            }
        }

        private static string ReadLine(TextReader reader)
        {
            var line = reader.ReadLine();
            if (line != null) return line;
            throw new Exception("Unexpected stream end");
        }

        private static void ReadUpTo(Stream stream, long count)
        {
            var buffer = new byte[1024];
            var totalRead = 0L;
            while (totalRead < count)
            {
                var read = stream.Read(buffer, 0, 1024);
                if (read == 0)
                    return;
                totalRead += read;
            }
        }
    }
}