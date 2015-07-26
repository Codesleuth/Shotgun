using System;
using System.Net;

namespace Lupara
{
    public interface ILuparaResponse
    {
        HttpStatusCode StatusCode { get; set; }
        ResponseStatus ResponseStatus { get; set; }
        string ErrorMessage { get; set; }
        Exception ErrorException { get; set; }
        string StatusDescription { get; set; }
        Uri ResponseUri { get; set; }
    }

    public class LuparaResponse: ILuparaResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
        public string StatusDescription { get; set; }
        public Uri ResponseUri { get; set; }
    }
}