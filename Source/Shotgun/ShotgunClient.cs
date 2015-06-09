using System;
using System.Net;
using System.Reflection;
using System.Text;
using Shotgun.http;
using Shotgun.models;

namespace Shotgun
{
    public class ShotgunClient
    {
        private static readonly Version _version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;

        private readonly Func<IHttp> _httpFactory = () => new Http();

        public IWebProxy Proxy { get; set; }
        public int Timeout { get; set; }
        public int ReadWriteTimeout { get; set; }

        public virtual IShotgunResponse Execute(IShotgunRequest request)
        {
            var method = Enum.GetName(typeof (Method), request.Method);

            switch (request.Method)
            {
                case Method.POST:
                case Method.PUT:
                case Method.PATCH:
                case Method.MERGE:
                    return Execute(request, method, ExecutePostRequest);

                default:
                    return Execute(request, method, ExecuteGetRequest);
            }
        }

        private IShotgunResponse Execute(IShotgunRequest request, string httpMethod, Func<IHttp, string, IHttpResponse> executeRequest)
        {
            IShotgunResponse response = new ShotgunResponse();

            try
            {
                var http = _httpFactory();

                http.Url = request.Uri;
                http.UserAgent = "ShotgunClient/" + _version;
                http.Proxy = Proxy;

                http.Timeout = Timeout;
                http.ReadWriteTimeout = ReadWriteTimeout;

                http.Body = request.Body;

                var httpResponse = executeRequest(http, httpMethod);
                response = ConvertHttpResponse(request, httpResponse);
                response.Request = request;
            }
            catch (WebException ex)
            {
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
                response.WebExceptionStatus = ex.Status;

                response.ResponseStatus = ex.Status == WebExceptionStatus.Timeout ? ResponseStatus.TimedOut : ResponseStatus.Error;
            }
            catch (Exception ex)
            {
                response.ResponseStatus = ResponseStatus.Error;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
            }

            return response;
        }

        private static IHttpResponse ExecuteGetRequest(IHttp http, string method)
        {
            return http.ExecuteGetRequest(method);
        }

        private static IHttpResponse ExecutePostRequest(IHttp http, string method)
        {
            return http.ExecutePostRequest(method);
        }

        private static IShotgunResponse ConvertHttpResponse(IShotgunRequest request, IHttpResponse httpResponse)
        {
            var restResponse = new ShotgunResponse
            {
                ErrorException = httpResponse.ErrorException,
                ErrorMessage = httpResponse.ErrorMessage,
                ResponseStatus = httpResponse.ResponseStatus,
                StatusCode = httpResponse.StatusCode,
                StatusDescription = httpResponse.StatusDescription,
                Request = request
            };

            if (httpResponse.RawBytes != null)
                restResponse.Content = Encoding.UTF8.GetString(httpResponse.RawBytes);

            return restResponse;
        }
    }
}
