using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Response;

namespace TicoPayDll.Services
{
    public class ServiceController
    {
        /*
         *  Metodo para crear un servicio
        */
        static async public Task<Response.Response> CreateNewService(Service newService, string token)
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
                return await CreateServiceAsync(httpClient, newService);
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
         *  Metodo para obtener los servicios
        */
        static async public Task<Response.Response> GetServices(string token)
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
                return await GetServicesAsync(httpClient);
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
         *  Metodo para realizar el Request de Creacion de servicio y procesar la respuesta 
        */
        static async Task<Response.Response> CreateServiceAsync(HttpClient httpClient,Service service)
        {
            string path = Config.webServiceUrl + "/service/Post";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(service);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(
                path, content);
            methodResponse = methodResponse.ProcessHttpResponse(response).GetAwaiter().GetResult();
            return methodResponse;
        }

        /*
         *  Metodo para realizar el Request de Busqueda de servicios y procesar la respuesta 
        */
        static async Task<Response.Response> GetServicesAsync(HttpClient httpClient)
        {
            string path = Config.webServiceUrl + "/service/GetAll?detallado=false";
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = await httpClient.GetAsync(path);
            methodResponse = methodResponse.ProcessHttpResponse(response).GetAwaiter().GetResult();
            return methodResponse;
        }

        public class JsonServices
        {
            public Boolean success;
            public int total_elements;
            public int statusCode;
            public Service[] listObjectResponse;
        }

        public class JsonCreateService
        {
            public Boolean success;
            public int total_elements;
            public int statusCode;
            public Service objectResponse;
        }
    }
}
