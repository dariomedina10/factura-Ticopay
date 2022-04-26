using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Invoices;
using TicoPayDll.Response;

namespace TicoPayDll.Vouchers
{
    public class VoucherController
    {
        #region GetVouchers

        /*
         *  Método para obtener los comprobantes
        */
        static async public Task<Response.Response> GetVouchers(SearchVoucher searchParameters, string token)
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
                return await GetVoucherAsync(httpClient, searchParameters);
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
         *  Método para realizar el Request de Búsqueda de comprobantes y procesar la respuesta 
        */
        static async Task<Response.Response> GetVoucherAsync(HttpClient httpClient, SearchVoucher parameters)
        {
            string path = Config.webServiceUrl + "Voucher/GetVouchers";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetInvoiceXml

        /*
         *  Método para Obtener el Xml de un comprobante
        */
        static async public Task<Response.Response> GetXml(string voucherId, string token)
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
                return await GetXmlAsync(httpClient, voucherId);
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
         *  Método para realizar el Request de la obtención del Xml de un comprobante y procesar la respuesta 
        */
        static async Task<Response.Response> GetXmlAsync(HttpClient httpClient, string voucherId)
        {
            string path = Config.webServiceUrl + "Voucher/GetVoucherXML/" + voucherId;
            Response.Response methodResponse = new Response.Response();
            UriBuilder builder = new UriBuilder(path);
            builder.Query = voucherId;            
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region UpdateInvoiceXml

        /*
         *  Método para actualizar el Xml Firmado de un comprobante
        */
        static async public Task<Response.Response> UpdateVoucherXml(string voucherId, string voucherXml, string token)
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
                return await UpdateVoucherXmlAsync(httpClient, voucherId, voucherXml);
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
         *  Método para realizar el Request de la actualización del Xml de un comprobante y procesar la respuesta 
        */
        static async Task<Response.Response> UpdateVoucherXmlAsync(HttpClient httpClient, string voucherId, string voucherXml)
        {
            string path = Config.webServiceUrl + "/Voucher/UpdateVoucherXML/" + voucherId;
            UriBuilder builder = new UriBuilder(path);
            builder.Query = voucherId;
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(voucherXml);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion
    }

    #region Json Responses

    public class JsonVouchers
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public Voucher[] listObjectResponse;
    }
    #endregion

    #region Parameters and Additional Clases

    public class SearchVoucher
    {       
        public string Identification { get; set; }

        public string ConsecutiveNumberInvoice { get; set; }

        public string ConsecutiveNumber { get; set; }

        public DateTime? StartDueDate { get; set; }

        public DateTime? EndDueDate { get; set; }

        public string Name { get; set; }
                
        public StatusTaxAdministration? StatusTribunet { get; set; }

        public FirmType? TipoFirma { get; set; }

        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

    }

    public enum StatusVoucher
    {
        // No Enviados
        NoEnviado = 0,
        // Enviados
        Enviados = 1,
    }

    #endregion


}
