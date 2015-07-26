using System;
using System.Net;
using System.Text;

namespace Lupara
{
    public interface ILuparaClient
    {
        ILuparaResponse Execute(ILuparaRequest request);
    }

    public class LuparaClient : ILuparaClient
    {
        public ILuparaResponse Execute(ILuparaRequest request)
        {
            var webRequest = PostWebRequest(request);
            return GetResponse(webRequest);
        }

        private static HttpWebRequest PostWebRequest(ILuparaRequest request)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(request.Uri);

            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Method = request.Method;
            webRequest.ContentType = request.ContentType;

            if (request.Timeout > 0)
                webRequest.Timeout = request.Timeout;

            if (request.ReadWriteTimeout > 0)
                webRequest.ReadWriteTimeout = request.ReadWriteTimeout;

            WriteBody(request.Content, webRequest);

            return webRequest;
        }

        private static void WriteBody(string body, WebRequest webRequest)
        {
            using (var requestStream = webRequest.GetRequestStream())
            {
                var bytes = Encoding.UTF8.GetBytes(body);
                requestStream.Write(bytes, 0, bytes.Length);
            }
        }

        private static void ExtractErrorResponse(ILuparaResponse httpResponse, Exception ex)
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

        private static ILuparaResponse GetResponse(WebRequest request)
        {
            var response = new LuparaResponse { ResponseStatus = ResponseStatus.None };

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

        private static HttpWebResponse GetRawResponse(WebRequest request)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                var httpWebResponse = ex.Response as HttpWebResponse;
                if (httpWebResponse != null)
                    return httpWebResponse;

                throw;
            }
        }

        private static void ExtractResponseData(ILuparaResponse response, HttpWebResponse webResponse)
        {
            using (webResponse)
            {
                var webResponseStream = webResponse.GetResponseStream();

                response.StatusCode = webResponse.StatusCode;
                response.StatusDescription = webResponse.StatusDescription;
                response.ResponseUri = webResponse.ResponseUri;
                response.ResponseStatus = ResponseStatus.Completed;

                webResponse.Close();
            }
        }
    }
}