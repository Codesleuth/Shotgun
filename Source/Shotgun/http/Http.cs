using System;
using System.IO;
using System.Net;
using Shotgun.models;

namespace Shotgun.http
{
    internal class Http : IHttp
    {
        public IWebProxy Proxy { get; set; }
        public Uri Url { get; set; }
        public string UserAgent { get; set; }

        /// <summary>
        ///     Timeout in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        ///     Used to control the Timeout when calling Stream.Read (AND) Stream.Write. Effects Streams returned from
        ///     GetResponse().GetResponseStream() (AND) GetRequestStream(). Default is 5 mins.
        /// </summary>
        public int ReadWriteTimeout { get; set; }

        public IRequestBody Body { get; set; }

        public IHttpResponse ExecuteGetRequest(string httpMethod)
        {
            return GetWebRequest(httpMethod.ToUpperInvariant());
        }

        public IHttpResponse ExecutePostRequest(string httpMethod)
        {
            return PostWebRequest(httpMethod.ToUpperInvariant());
        }

        private HttpResponse GetWebRequest(string method)
        {
            var webRequest = CreateWebRequest(method, Url, Timeout, ReadWriteTimeout, Proxy);

            if (method == "DELETE" || method == "OPTIONS")
                WriteBody(webRequest);

            return GetResponse(webRequest);
        }

        private HttpResponse PostWebRequest(string method)
        {
            var webRequest = CreateWebRequest(method, Url, Timeout, ReadWriteTimeout, Proxy);

            WriteBody(webRequest);

            return GetResponse(webRequest);
        }

        private void WriteBody(WebRequest webRequest)
        {
            if (Body == null)
                return;

            webRequest.ContentType = Body.ContentType;

            using (var requestStream = webRequest.GetRequestStream())
            {
                Body.WriteToStream(requestStream);
            }
        }

        private static void ExtractErrorResponse(IHttpResponse httpResponse, Exception ex)
        {
            var webException = ex as WebException;

            if (webException != null)
            {
                switch (webException.Status)
                {
                    case WebExceptionStatus.Timeout:
                        httpResponse.ResponseStatus = ResponseStatus.TimedOut;
                        break;
                    case WebExceptionStatus.ConnectFailure:
                        httpResponse.ResponseStatus = ResponseStatus.ConnectFailure;
                        break;
                }

                httpResponse.ErrorMessage = ex.Message;
                httpResponse.ErrorException = webException;
                return;
            }

            httpResponse.ErrorMessage = ex.Message;
            httpResponse.ErrorException = ex;
            httpResponse.ResponseStatus = ResponseStatus.Error;
        }

        private static HttpResponse GetResponse(HttpWebRequest request)
        {
            var response = new HttpResponse {ResponseStatus = ResponseStatus.None};

            try
            {
                var webResponse = GetRawResponse(request);
                ExtractResponseData(response, webResponse);
            }
            catch (Exception ex)
            {
                ExtractErrorResponse(response, ex);
            }

            return response;
        }

        private static void ExtractResponseData(HttpResponse response, HttpWebResponse webResponse)
        {
            using (webResponse)
            {
                var webResponseStream = webResponse.GetResponseStream();

                response.RawBytes = ReadStreamBytes(webResponseStream);

                response.StatusCode = webResponse.StatusCode;
                response.StatusDescription = webResponse.StatusDescription;
                response.ResponseUri = webResponse.ResponseUri;
                response.ResponseStatus = ResponseStatus.Completed;

                webResponse.Close();
            }
        }

        private static byte[] ReadStreamBytes(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static HttpWebResponse GetRawResponse(WebRequest request)
        {
            try
            {
                return (HttpWebResponse) request.GetResponse();
            }
            catch (WebException ex)
            {
                var httpWebResponse = ex.Response as HttpWebResponse;
                if (httpWebResponse != null)
                    return httpWebResponse;

                throw;
            }
        }

        private static HttpWebRequest CreateWebRequest(string method, Uri url, int timeout, int readWriteTimeout,
                                                       IWebProxy proxy)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(url);

            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Method = method;
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip |
                                                DecompressionMethods.None;

            if (timeout != 0)
                webRequest.Timeout = timeout;

            if (readWriteTimeout != 0)
                webRequest.ReadWriteTimeout = readWriteTimeout;

            if (proxy != null)
                webRequest.Proxy = proxy;

            return webRequest;
        }
    }
}