using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Response;

namespace TicoPayDll.Taxes
{
    public class TaxesController
    {
        /*
         *  Metodo para obtener los impuestos
        */
        static async public Task<Response.Response> Gettaxes(string token)
        {
            // Prepara la direccion y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            try
            {
                // Ejecutar el Http Request
                return await GetTaxesAsync(httpClient);
            }
            catch
            {
                // En caso de error inesperado
                Response.Response methodResponse = new Response.Response();
                methodResponse.status = ResponseType.BadRequest;
                methodResponse.message = "Error";
                methodResponse.result = null;
                return methodResponse;
            }
        }

        /*
         *  Metodo para realizar el Request de Busqueda de impuestos y procesar la respuesta 
        */
        static async Task<Response.Response> GetTaxesAsync(HttpClient httpClient)
        {
            string path = Config.webServiceUrl +  "/Tax/GetAll?detallado=false";
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        public class JsonTaxes
        {
            public Boolean success;
            public int total_elements;
            public int statusCode;
            public Tax[] listObjectResponse;
        }
    }
}
