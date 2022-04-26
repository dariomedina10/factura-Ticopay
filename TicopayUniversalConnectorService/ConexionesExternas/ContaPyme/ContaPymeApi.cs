using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Opetations;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Security;
using static TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Opetations.Operation;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme
{
    public class ContaPymeApi
    {
       #region Authenticate
       static async public Task<MethodResponse> Authenticate(string webServiceUrl, LoginJson credenciales)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(webServiceUrl);            
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                string path = webServiceUrl + "/datasnap/rest/TBasicoGeneral/\"GetAuth\"/";
                MethodResponse methodResponse = new MethodResponse();
                Parameters data = new Parameters(credenciales);
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");                
                HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
                methodResponse = await methodResponse.ProcessHttpResponse(response);
                return methodResponse;
            }
            catch (Exception ex)
            {
                // En caso de error inesperado
                MethodResponse methodResponse = new MethodResponse();
                methodResponse.status = ContapymeResponseType.InternalServerError;
                methodResponse.message = "Error : " + ex.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }
        #endregion

        #region LogOut
        static async public Task<MethodResponse> Logout(string webServiceUrl, LoginJson credenciales)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                string path = webServiceUrl + "/datasnap/rest/TBasicoGeneral/\"Logout\"/";
                MethodResponse methodResponse = new MethodResponse();
                Parameters data = new Parameters(credenciales);
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
                methodResponse = await methodResponse.ProcessHttpResponse(response);
                return methodResponse;
            }
            catch (Exception ex)
            {
                // En caso de error inesperado
                MethodResponse methodResponse = new MethodResponse();
                methodResponse.status = ContapymeResponseType.InternalServerError;
                methodResponse.message = "Error : " + ex.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }
        #endregion

        #region GetClients

        static async public Task<MethodResponse> GetClients(string webServiceUrl, OperationJson credenciales)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                string path = webServiceUrl + "/datasnap/rest/TCatTerceros/\"GetListaTerceros\"/";
                MethodResponse methodResponse = new MethodResponse();
                Operation data = new Operation(credenciales);
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
                methodResponse = await methodResponse.ProcessHttpResponse(response);
                return methodResponse;
            }
            catch (Exception ex)
            {
                // En caso de error inesperado
                MethodResponse methodResponse = new MethodResponse();
                methodResponse.status = ContapymeResponseType.InternalServerError;
                methodResponse.message = "Error : " + ex.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }

        #endregion

        #region AddClient

        static async public Task<MethodResponse> AddClient(string webServiceUrl, OperationJson credenciales)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                string path = webServiceUrl + "/datasnap/rest/TCatTerceros/\"DoCrearTercero\"/";
                MethodResponse methodResponse = new MethodResponse();
                Operation data = new Operation(credenciales);
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
                methodResponse = await methodResponse.ProcessHttpResponse(response);
                return methodResponse;
            }
            catch (Exception ex)
            {
                // En caso de error inesperado
                MethodResponse methodResponse = new MethodResponse();
                methodResponse.status = ContapymeResponseType.InternalServerError;
                methodResponse.message = "Error : " + ex.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }

        #endregion

        #region GetItems

        static async public Task<MethodResponse> GetItems(string webServiceUrl, OperationJson credenciales)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                string path = webServiceUrl + "/datasnap/rest/TCatElemInv/\"GetListaElemInv\"/";
                MethodResponse methodResponse = new MethodResponse();
                Operation data = new Operation(credenciales);
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
                methodResponse = await methodResponse.ProcessHttpResponse(response);
                return methodResponse;
            }
            catch (Exception ex)
            {
                // En caso de error inesperado
                MethodResponse methodResponse = new MethodResponse();
                methodResponse.status = ContapymeResponseType.InternalServerError;
                methodResponse.message = "Error : " + ex.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }

        #endregion

        #region AddItem

        static async public Task<MethodResponse> AddItem(string webServiceUrl, OperationJson credenciales)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                string path = webServiceUrl + "/datasnap/rest/TCatElemInv/\"DoCrearElemInv\"/";
                MethodResponse methodResponse = new MethodResponse();
                Operation data = new Operation(credenciales);
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
                methodResponse = await methodResponse.ProcessHttpResponse(response);
                return methodResponse;
            }
            catch (Exception ex)
            {
                // En caso de error inesperado
                MethodResponse methodResponse = new MethodResponse();
                methodResponse.status = ContapymeResponseType.InternalServerError;
                methodResponse.message = "Error : " + ex.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }

        #endregion

        #region AddInvoice

        static async public Task<MethodResponse> AddInvoice(string webServiceUrl, OperationJson credenciales)
        {
            HttpClient httpClient = new HttpClient();
            // Prepara la direccion y cabeceras para hacer el request
            httpClient.BaseAddress = new Uri(webServiceUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                // Ejecutar el Http Request
                string path = webServiceUrl + "/datasnap/rest/TCatOperaciones/\"DoExecuteOprAction\"/";
                MethodResponse methodResponse = new MethodResponse();
                Operation data = new Operation(credenciales);
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
                methodResponse = await methodResponse.ProcessHttpResponse(response);
                return methodResponse;
            }
            catch (Exception ex)
            {
                // En caso de error inesperado
                MethodResponse methodResponse = new MethodResponse();
                methodResponse.status = ContapymeResponseType.InternalServerError;
                methodResponse.message = "Error : " + ex.Message;
                methodResponse.result = null;
                return methodResponse;
            }
        }

        #endregion
    }
}
