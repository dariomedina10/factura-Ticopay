using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Response;

namespace TicoPayDll.Clients
{
    public class ClientController
    {

        #region CreateNewClient
        /*
         *  Método para crear un cliente
        */
        static async public Task<Response.Response> CreateNewClient(Client newClient, string token)
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
                return await CreateClientAsync(httpClient, newClient);
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
         *  Método para realizar el Request de Creación de cliente y procesar la respuesta 
        */
        static async Task<Response.Response> CreateClientAsync(HttpClient httpClient, Client client)
        {
            string path = Config.webServiceUrl + "client/Post";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(client);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region UpdateClient
        /*
         *  Método para crear un cliente
        */
        static async public Task<Response.Response> UpdateClient(Client ClientToUpdate, string token)
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
                return await UpdateClientAsync(httpClient, ClientToUpdate);
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
         *  Método para realizar el Request de Creación de cliente y procesar la respuesta 
        */
        static async Task<Response.Response> UpdateClientAsync(HttpClient httpClient, Client ClientToUpdate)
        {
            string path = Config.webServiceUrl + "client/Put";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(ClientToUpdate);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region DeleteClient
        /*
         *  Método para eliminar un cliente
        */
        static async public Task<Response.Response> DeleteClient(Guid ClientToDelete, string token)
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
                return await DeleteClientAsync(httpClient, ClientToDelete);
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
         *  Método para realizar el Request de Eliminacion de cliente y procesar la respuesta 
        */
        static async Task<Response.Response> DeleteClientAsync(HttpClient httpClient, Guid ClientToDelete)
        {
            string path = Config.webServiceUrl + "client/Delete/" + ClientToDelete.ToString();
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(ClientToDelete);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetClients
        /*
         *  Método para obtener los clientes
        */
        static async public Task<Response.Response> GetClients(string token,bool detail)
        {
            // Prepara la dirección y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer "+ token);
            try
            {
                // Ejecutar el Http Request
                return await GetClientsAsync(httpClient, detail);
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
        static async Task<Response.Response> GetClientsAsync(HttpClient httpClient, bool detail)
        {
            string path = Config.webServiceUrl + "client/GetAll?detallado=";
            if (detail)
            {
                path = path + "true";
            }
            else
            {
                path = path + "false";
            }
            // string path = Config.webServiceUrl + "client/GetAll?detallado=false";
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region SearchClients
        /*
         *  Método para buscar clientes por Nº de Identificación
        */
        static async public Task<Response.Response> SearchClients(string token, bool detail, string identification)
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
                return await SearchClientsAsync(httpClient, detail, identification);
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
        static async Task<Response.Response> SearchClientsAsync(HttpClient httpClient, bool detail, string clientIdentification)
        {
            string path = Config.webServiceUrl + "Client/GetSearch/" + clientIdentification;
            if (detail)
            {
                path = path + "?detallado=true";
            }
            else
            {
                path = path + "?detallado=false";
            }
            // string path = Config.webServiceUrl + "client/GetAll?detallado=false";
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetGroupAll

        /*
         *  Método para buscar los grupos de clientes o Categorias de un Tenant
        */
        static async public Task<Response.Response> GetAllGroups(string token, bool detail)
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
                return await GetGroupAllAsync(httpClient, detail);
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
         *  Método para realizar el Request de buscar grupos de clientes o Categorias y procesar la respuesta 
        */
        static async Task<Response.Response> GetGroupAllAsync(HttpClient httpClient, bool detail)
        {
            string path = Config.webServiceUrl + "/Client/GetGroupAll";
            if (detail)
            {
                path = path + "?detallado=true";
            }
            else
            {
                path = path + "?detallado=false";
            }
            // string path = Config.webServiceUrl + "client/GetAll?detallado=false";
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region PostGroups

        /*
         *  Método para asigna los grupos o Categorías a un cliente de un Tenant
        */
        static async public Task<Response.Response> AddClientGroups(string token, ClientGroup gruposCliente)
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
                return await AddClientGroupsAsync(httpClient, gruposCliente);
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
         *  Método para realizar el Request de asignar los grupos o Categorías a un cliente y procesar la respuesta 
        */
        static async Task<Response.Response> AddClientGroupsAsync(HttpClient httpClient,ClientGroup gruposCliente)
        {
            string path = Config.webServiceUrl + "/Client/PostGroups";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(gruposCliente);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        public class JsonGroups
        {
            public Boolean success;
            public int total_elements;
            public int statusCode;
            public Group[] listObjectResponse;
        }


        public class JsonClients
        {
            public Boolean success;
            public int total_elements;
            public int statusCode;
            public Client[] listObjectResponse;
        }

        public class JsonError
        {
            public bool success;
            public string error_msg;
            public int error_code;
            public string innerExcepcion;
        }

        public class JsonSearchClient
        {
            public Boolean success;
            public int total_elements;
            public int statusCode;
            public Client objectResponse;
        }

        public class JsonCreateClient
        {
            public Boolean success;
            public int total_elements;
            public int statusCode;
            public Client objectResponse;
        }

        public class JsonUpdateClient
        {
            public Boolean success;
            public int total_elements;
            public int statusCode;
            public Client objectResponse;
        }
    }
}
