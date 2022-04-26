using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TicoPayDll.Response
{
    public class Response
    {
        public string message;
        public ResponseType status;
        public string result;

        public async Task<Response> ProcessHttpResponse(HttpResponseMessage httpResponse)
        {
            Response response = new Response();
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                response.status = ResponseType.Ok;
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
            {
                response.status = ResponseType.TimeOut;
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {

                response.status = ResponseType.Unauthorized;
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {

                response.status = ResponseType.NotFound;
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {

                response.status = ResponseType.InternalServerError;
            }
            else
            {
                response.status = ResponseType.BadRequest;
            }
            if (response.status == ResponseType.Ok)
            {
                response.message = "Success";
            }
            else
            {
                response.message = "Error";
            }
            response.result = await httpResponse.Content.ReadAsStringAsync();
            return response;
        }

        
    }

    public enum ResponseType
    {
        Ok = 200,
        TimeOut = 408,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        TooManyRequest = 429,
        InternalServerError = 500,
        UnexpectedError = 999,
    }
}
