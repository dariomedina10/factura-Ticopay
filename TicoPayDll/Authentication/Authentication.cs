using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Response;

namespace TicoPayDll.Authentication
{
    public class Authentication
    {
        #region Authenticate

        /// <summary>
        /// Authenticates the specified user in the Web Api.
        /// </summary>
        /// <param name="bodyInformation">Contains the Tenant, user and password information.</param>
        /// <returns>Ok if login, Bad Request if incorrect Tenant , user or password information</returns>
        static async public Task<Response.Response> Authenticate(UserCredentials bodyInformation)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                return await AuthenticateAsync(httpClient, bodyInformation);
            }
            catch(Exception ex)
            {
                // En caso de error inesperado
                Response.Response methodResponse = new Response.Response();
                methodResponse.status = ResponseType.BadRequest;
                methodResponse.message = "Error : " + ex.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }
        
        /*
         *  Metodo para realizar el Request de la Autentificacion y procesar la respuesta 
        */
        static async Task<Response.Response> AuthenticateAsync(HttpClient httpClient, UserCredentials loginInformation)
        {
            string path = Config.webServiceUrl + "Account/Authenticate";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(loginInformation);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region RefreshToken

        /// <summary>
        /// Refreshes the sesion token.
        /// </summary>
        /// <param name="bodyInformation">Contains the token and specified time lapse extension.</param>
        /// <returns>The same token if valid , or a new one with the new time lapse duration if the previous one was expired</returns>
        static async public Task<Response.Response> RefreshTokenAuthenticate(RefreshCredentials bodyInformation)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                return await RefreshAsync(httpClient, bodyInformation);
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
         *  Metodo para realizar el Request de la Renovacion de token y procesar la respuesta 
        */
        static async Task<Response.Response> RefreshAsync(HttpClient httpClient, RefreshCredentials loginInformation)
        {
            string path = Config.webServiceUrl + "Account/RefreshToken";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(loginInformation);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region VerifyDomain


        /// <summary>
        /// Verifies if the domain or Tenant exist in Ticopay.
        /// </summary>
        /// <param name="tenantName">Name of the tenant you want to verify.</param>
        /// <returns>Ok if the tenant exist or Not Found if not</returns>
        static async public Task<Response.Response> VerifyDomain(string tenantName)
        {
            // Prepara la dirección y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                return await GetVerifyDomainAsync(httpClient, tenantName);
            }
            catch (Exception error)
            {
                // En caso de error inesperado
                Response.Response methodResponse = new Response.Response();
                methodResponse.status = ResponseType.UnexpectedError;
                methodResponse.message = error.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }

        /*
         *  Método para realizar el Request de la verificacion del tenant y procesar la respuesta 
        */
        static async Task<Response.Response> GetVerifyDomainAsync(HttpClient httpClient, string tenantName)
        {
            string path = Config.webServiceUrl + "Account/VerifyDomain?tenantName=" + tenantName;
            Response.Response methodResponse = new Response.Response();                        
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region VerifyConnector


        /// <summary>
        /// Verifies if the domain or Tenant is allowed to use the Universal Connector.
        /// </summary>
        /// <param name="tenantName">Name of the tenant you want to verify.</param>
        /// /// <param name="connectorType">Name of the Connector to Verify.</param>
        /// <returns>Ok if the tenant is allowed to use the connector or Not Found if not</returns>
        static async public Task<Response.Response> VerifyConnector(string tenantName, string connectorType)
        {
            // Prepara la dirección y cabeceras para hacer el request
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Config.webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                return await GetVerifyConnectorAsync(httpClient, tenantName, connectorType);
            }
            catch (Exception error)
            {
                // En caso de error inesperado
                Response.Response methodResponse = new Response.Response();
                methodResponse.status = ResponseType.UnexpectedError;
                methodResponse.message = error.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }

        /*
         *  Método para realizar el Request de la verificacion del permiso de conector y procesar la respuesta 
        */
        static async Task<Response.Response> GetVerifyConnectorAsync(HttpClient httpClient, string tenantName, string connectorType)
        {
            string path = Config.webServiceUrl + "Account/VerifyAllowedConnector?tenantName=" + tenantName + "&ConnectorType=" + connectorType;
            Response.Response methodResponse = new Response.Response();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion
    }

    public class UserCredentials
    {
        public string tenancyName;
        public string usernameOrEmailAddress;
        public string password;
    }

    public class RefreshCredentials
    {
        public string Token;
        public TimeLapsType AdditionalTimeType;
        public decimal AdditionalTimeAmount;
    }

    public class JsonAuthentication
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public TokenResponse objectResponse;
    }

    public class JsonDomain
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public bool objectResponse;
    }

    public class JsonConnector
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public bool objectResponse;
    }

    public class JsonRefreshToken
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public TokenResponse objectResponse;
    }

    public class TokenResponse
    {
        public string tokenAuthenticate;
    }

    public enum TimeLapsType
    {
        Minutes = 0,
        Hours = 1,
        Days = 2,
    }
}
