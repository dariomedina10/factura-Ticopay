using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Response;

namespace TicoPayDll.Addresses
{
    public class AddressController
    {
        #region GetCountries

        static async public Task<Response.Response> GetAllCountries(string token)
        {
            // Prepara la dirección y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            try
            {
                // Ejecutar el Http Request
                return await GetAllCountriesAsync(httpClient);
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
        static async Task<Response.Response> GetAllCountriesAsync(HttpClient httpClient)
        {
            string path = Config.webServiceUrl + "Address/GetAllCountries";
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetProvincias

        static async public Task<Response.Response> GetAllProvincias(string token)
        {
            // Prepara la dirección y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            try
            {
                // Ejecutar el Http Request
                return await GetAllProvinciasAsync(httpClient);
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
        static async Task<Response.Response> GetAllProvinciasAsync(HttpClient httpClient)
        {
            string path = Config.webServiceUrl + "Address/GetAllProvincias";
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetCanton

        static async public Task<Response.Response> GetCanton(string token, int idProvincia)
        {
            // Prepara la dirección y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            try
            {
                // Ejecutar el Http Request
                return await GetCantonAsync(httpClient, idProvincia);
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
        static async Task<Response.Response> GetCantonAsync(HttpClient httpClient, int idProvincia)
        {
            string path = Config.webServiceUrl + "Address/GetCanton?IdProvincia=" + idProvincia.ToString();
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetDistrito

        static async public Task<Response.Response> GetDistrito(string token, int idCanton)
        {
            // Prepara la dirección y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            try
            {
                // Ejecutar el Http Request
                return await GetDistritoAsync(httpClient, idCanton);
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
        static async Task<Response.Response> GetDistritoAsync(HttpClient httpClient, int idCanton)
        {
            string path = Config.webServiceUrl + "Address/GetDistrito?IdCanton=" + idCanton.ToString();
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetBarrio

        static async public Task<Response.Response> GetBarrio(string token, int idDistrito)
        {
            // Prepara la dirección y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            try
            {
                // Ejecutar el Http Request
                return await GetBarrioAsync(httpClient, idDistrito);
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
        static async Task<Response.Response> GetBarrioAsync(HttpClient httpClient, int idDistrito)
        {
            string path = Config.webServiceUrl + "Address/GetBarrio?IdDistrito=" + idDistrito.ToString();
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion
    }
}
