using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Response;

namespace TicoPayDll.Tenants
{
    public class TenantController
    {
        #region GetTenant
        /*
         *  Método para obtener los datos del Tenant
        */
        static async public Task<Response.Response> GetTenant(string token, bool detail)
        {
            // Prepara la dirección y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            try
            {
                // Ejecutar el Http Request
                return await GetTenantAsync(httpClient, detail);
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
         *  Método para realizar el Request de Búsqueda de clientes y procesar la respuesta 
        */
        static async Task<Response.Response> GetTenantAsync(HttpClient httpClient, bool detail)
        {
            string path = Config.webServiceUrl + "Tenant/Get?detallado=";
            if (detail)
            {
                path = path + "true";
            }
            else
            {
                path = path + "false";
            }
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion
    }
}
