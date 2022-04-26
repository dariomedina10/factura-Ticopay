using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme
{
    public class MethodResponse
    {
        public string message;
        public ContapymeResponseType status;
        public string result;

        public async Task<MethodResponse> ProcessHttpResponse(HttpResponseMessage httpResponse)
        {
            MethodResponse response = new MethodResponse();
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                response.status = ContapymeResponseType.Ok;
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
            {
                response.status = ContapymeResponseType.TimeOut;
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {

                response.status = ContapymeResponseType.Unauthorized;
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {

                response.status = ContapymeResponseType.NotFound;
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {

                response.status = ContapymeResponseType.InternalServerError;
            }
            else
            {
                response.status = ContapymeResponseType.BadRequest;
            }
            if (response.status == ContapymeResponseType.Ok)
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

    public enum ContapymeResponseType
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
