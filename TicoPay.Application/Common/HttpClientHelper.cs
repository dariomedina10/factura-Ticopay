using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TicoPay.Common
{
    /// <summary>
    /// Helper que expone los metodos necesarios para ejecutar requests a un web api
    /// </summary>
    public static class HttpClientHelper
    {
        /// <summary>
        /// Permite ejecutar un POST al recurso <paramref name="requestUri"/> del web api <paramref name="baseAddress"/>
        /// </summary>
        /// <typeparam name="TResult">Tipo de dato que representa la respuesta optenida</typeparam>
        /// <param name="baseAddress">Url base del web api</param>
        /// <param name="requestUri">Recurso que se desea acceder</param>
        /// <param name="token">Token para autenticacion Bearer</param>
        /// <returns></returns>
        public static async Task<ApiResponse<TResult>> PostAsync<TResult>(string baseAddress, string requestUri, string token)
        {
            ApiResponse<TResult> result = new ApiResponse<TResult>();
            try
            {
                using (var client = CreateClient(baseAddress, token))
                {
                    var response = await client.PostAsJsonAsync(requestUri, new { });
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        TResult content = JsonConvert.DeserializeObject<TResult>(json);
                        result = new ApiResponse<TResult>(true, response.StatusCode, content);
                    }
                    else
                    {
                        result = new ApiResponse<TResult>(false, response.StatusCode, default(TResult)) { Message = response.ReasonPhrase };
                    }
                }
            }
            catch (Exception ex)
            {
                result = ApiResponse<TResult>.FromException(ex);
            }
            return result;
        }

        /// <summary>
        /// Permite ejecutar un POST al recurso <paramref name="requestUri"/> del web api <paramref name="baseAddress"/>
        /// Enviando la <paramref name="data"/> en el body en formato json 
        /// </summary>
        /// <typeparam name="TData">Tipo de dato que se desea enviar serializado como un json</typeparam>
        /// <typeparam name="TResult">Tipo de dato que representa la respuesta optenida</typeparam>
        /// <param name="baseAddress">Url base del web api</param>
        /// <param name="requestUri">Recurso que se desea acceder</param>
        /// <param name="token">Token para autenticacion Bearer</param>
        /// <param name="data">Instancia del objeto a enviar</param>
        /// <returns></returns>
        public static async Task<ApiResponse<TResult>> PostAsJsonAsync<TData, TResult>(string baseAddress, string requestUri, string token, TData data)
        {
            ApiResponse<TResult> result = new ApiResponse<TResult>();
            try
            {
                using (var client = CreateClient(baseAddress, token))
                {
                    var response = await client.PostAsJsonAsync(requestUri, data).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        TResult content = JsonConvert.DeserializeObject<TResult>(json);
                        result = new ApiResponse<TResult>(true, response.StatusCode, content);
                    }
                    else
                    {
                        result = new ApiResponse<TResult>(false, response.StatusCode, default(TResult)) { Message = response.ReasonPhrase };
                    }
                }
            }
            catch (Exception ex)
            {
                result = ApiResponse<TResult>.FromException(ex);
            }
            return result;
        }

        /// <summary>
        /// Permite ejecutar un GET al recurso <paramref name="requestUri"/>del web api <paramref name="baseAddress"/>
        /// </summary>
        /// <typeparam name="TResult">Tipo de dato que representa la respuesta optenida</typeparam>
        /// <param name="baseAddress">Url base del web api</param>
        /// <param name="requestUri">Recurso que se desea acceder</param>
        /// <param name="token">Token para autenticacion Bearer</param>
        /// <returns></returns>
        public static async Task<ApiResponse<TResult>> GetAsync<TResult>(string baseAddress, string requestUri, string token)
        {
            ApiResponse<TResult> result = new ApiResponse<TResult>();
            try
            {
                using (var client = CreateClient(baseAddress, token))
                {
                    client.Timeout = TimeSpan.FromSeconds(Double.Parse(ConfigurationManager.AppSettings["TribunetTimeOut"]));
                    var response = await client.GetAsync(requestUri).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        TResult content = JsonConvert.DeserializeObject<TResult>(json);
                        result = new ApiResponse<TResult>(true, response.StatusCode, content);
                    }
                    else
                    {
                        result = new ApiResponse<TResult>(false, response.StatusCode, default(TResult));
                    }
                }
            }
            catch (Exception ex)
            {
                result = ApiResponse<TResult>.FromException(ex);
            }
            return result;
        }

        /// <summary>
        /// Crea una instancia de <seealso cref="HttpClient"/>
        /// </summary>
        /// <param name="baseAddress">Url base para el cliente</param>
        /// <param name="token">Token para la autenticacion Bearer</param>
        /// <returns></returns>
        public static HttpClient CreateClient(string baseAddress, string token = "", Dictionary<string, string> extraData = null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            if (extraData != null)
            {
                foreach (var item in extraData)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            return client;
        }

        /// <summary>
        /// Ejecuta un POST a un enpoint de authenticación pasando los parámetros comunes.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="baseAddress"></param>
        /// <param name="authenticateUri"></param>
        /// <param name="userNameOrEmailAddress"></param>
        /// <param name="password"></param>
        /// <param name="extraHeaderData"></param>
        /// <returns></returns>
        public static ApiResponse<TResult> Authenticate<TResult>(string baseAddress, string authenticateUri, string userNameOrEmailAddress, string password, Dictionary<string, string> extraHeaderData = null)
        {
            ApiResponse<TResult> result = new ApiResponse<TResult>();
            try
            {
                using (var client = CreateClient(baseAddress, extraData: extraHeaderData))
                {
                    var response = client.PostAsJsonAsync(authenticateUri, new { userNameOrEmailAddress = userNameOrEmailAddress, password = password }).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<ApiResponse<TResult>>(json);
                    }
                    else
                    {
                        result = new ApiResponse<TResult>(false, response.StatusCode, default(TResult)) { Message = response.ReasonPhrase };
                    }
                }
            }
            catch (Exception ex)
            {
                result = ApiResponse<TResult>.FromException(ex);
            }
            return result;
        }


        /// <summary>
        /// Permite ejecutar un POST al recurso <paramref name="requestUri"/> del web api <paramref name="baseAddress"/>
        /// Enviando la <paramref name="data"/> en el body en formato json 
        /// </summary>
        /// <typeparam name="TData">Tipo de dato que se desea enviar serializado como un json</typeparam>
        /// <typeparam name="TResult">Tipo de dato que representa la respuesta optenida</typeparam>
        /// <param name="baseAddress">Url base del web api</param>
        /// <param name="requestUri">Recurso que se desea acceder</param>
        /// <param name="token">Token para autenticacion Bearer</param>
        /// <param name="data">Instancia del objeto a enviar</param>
        /// <returns></returns>
        public static async Task<TResult> GenericPostAsJsonAsync<TData, TResult>(string baseAddress, string requestUri, string token, TData data)
        {
            TResult result = default(TResult);
            try
            {
                using (var client = CreateClient(baseAddress, token))
                {
                    var response = await client.PostAsJsonAsync(requestUri, data);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<TResult>(json);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        /// <summary>
        /// Permite ejecutar un GET al recurso <paramref name="requestUri"/>del web api <paramref name="baseAddress"/>
        /// </summary>
        /// <typeparam name="TResult">Tipo de dato que representa la respuesta optenida</typeparam>
        /// <param name="baseAddress">Url base del web api</param>
        /// <param name="requestUri">Recurso que se desea acceder</param>
        /// <param name="token">Token para autenticacion Bearer</param>
        /// <returns></returns>
        public static async Task<TResult> GenericGetAsync<TResult>(string baseAddress, string requestUri, string token)
        {
            TResult result = default(TResult);
            try
            {
                using (var client = CreateClient(baseAddress, token))
                {
                    var response = await client.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<TResult>(json);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }

    /// <summary>
    /// Permite el manejo de los datos optenidos como respuesta de un request a una web api
    /// </summary>
    /// <typeparam name="TContent"></typeparam>
    public class ApiResponse<TContent>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public TContent Result { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(bool success, HttpStatusCode statusCode, TContent content)
        {
            Success = success;
            StatusCode = (int)statusCode;
            Result = content;
        }

        internal static ApiResponse<TContent> FromException(Exception ex)
        {
            return new ApiResponse<TContent>
            {
                Success = false,
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = ex.GetBaseException().Message
            };
        }
    }
}
