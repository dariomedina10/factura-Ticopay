using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Response;

namespace TicoPayDll.Invoices
{
    public class InvoiceController
    {
        #region GetInvoices

        /*
         *  Metodo para obtener las facturas
        */
        static async public Task<Response.Response> GetInvoices(InvoiceSearchConfiguration searchParameters, string token)
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
                return await GetInvoicesAsync(httpClient, searchParameters);
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
         *  Metodo para realizar el Request de Busqueda de facturas y procesar la respuesta 
        */
        static async Task<Response.Response> GetInvoicesAsync(HttpClient httpClient, InvoiceSearchConfiguration parameters)
        {
            string path = Config.webServiceUrl + "/Invoice/GetInvoices";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region CreateNewInvoice

        /*
         *  Metodo para crear una factura
        */
        static async public Task<Response.Response> CreateNewInvoice(CreateInvoice newInvoice, string token)
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
                return await CreateInvoiceAsync(httpClient, newInvoice);
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
         *  Metodo para realizar el Request de Creacion de una factura y procesar la respuesta 
        */
        static async Task<Response.Response> CreateInvoiceAsync(HttpClient httpClient, CreateInvoice invoice)
        {
            string path = Config.webServiceUrl + "/Invoice/PostAsync";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(invoice);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region CreateNewTicket

        /*
         *  Metodo para crear una tiquete
        */
        static async public Task<Response.Response> CreateNewTicket(CreateInvoice newTicket, string token)
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
                return await CreateTicketAsync(httpClient, newTicket);
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
         *  Metodo para realizar el Request de Creacion de un tiquete y procesar la respuesta 
        */
        static async Task<Response.Response> CreateTicketAsync(HttpClient httpClient, CreateInvoice ticket)
        {
            string path = Config.webServiceUrl + "/Invoice/PostTicketAsync";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(ticket);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region PayInvoiceOrTicket

        /*
         *  Metodo para pagar una factura o tiquete
        */
        static async public Task<Response.Response> PayInvoiceOrTicket(string IdInvoiceOrTicket, List<PaymentInvoce> PaymentList, string token)
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
                return await PayInvoiceTicketAsync(httpClient, IdInvoiceOrTicket, PaymentList);
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
         *  Metodo para realizar el Request de pago de factura o tiquete y procesar la respuesta 
        */
        static async Task<Response.Response> PayInvoiceTicketAsync(HttpClient httpClient, string IdInvoiceOrTicket, List<PaymentInvoce> PaymentList)
        {
            string path = Config.webServiceUrl + "/Invoice/PayInvoiceOrTicket?IdInvoiceOrTicket=" + IdInvoiceOrTicket;
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(PaymentList);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region ReSendInvoices

        /*
         *  Metodo para reenviar las facturas
        */
        static async public Task<Response.Response> ReSendInvoices(string[] invoices, string token)
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
                return await ReSendInvoicesAsync(httpClient, invoices);
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
         *  Metodo para realizar el Request del reenvio de facturas y procesar la respuesta 
        */
        static async Task<Response.Response> ReSendInvoicesAsync(HttpClient httpClient, string[] invoices)
        {
            string path = Config.webServiceUrl + "/Invoice/Resend";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(invoices);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetInvoiceXml

        /*
         *  Método para Obtener el Xml de una factura
        */
        static async public Task<Response.Response> GetXml(string invoiceId, string token)
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
                return await GetXmlAsync(httpClient, invoiceId);
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
         *  Método para realizar el Request de la obtención del Xml de la factura y procesar la respuesta 
        */
        static async Task<Response.Response> GetXmlAsync(HttpClient httpClient, string invoiceId)
        {
            string path = Config.webServiceUrl + "Invoice/GetXML/" + invoiceId;
            Response.Response methodResponse = new Response.Response();
            UriBuilder builder = new UriBuilder(path);
            builder.Query = invoiceId;
            // HttpResponseMessage response = httpClient.GetAsync(builder.Uri).GetAwaiter().GetResult();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetInvoicePDF

        /*
         *  Método para Obtener el pedf de una factura
        */
        static async public Task<Response.Response> GetInvoicePDF(string invoiceId, string token)
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
                return await GetInvoicePDFAsync(httpClient, invoiceId);
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
         *  Método para realizar el Request de la obtención del pdf de la factura y procesar la respuesta 
        */
        static async Task<Response.Response> GetInvoicePDFAsync(HttpClient httpClient, string invoiceId)
        {
            string path = Config.webServiceUrl + "Invoice/GetInvoicePDF/" + invoiceId;
            Response.Response methodResponse = new Response.Response();
            UriBuilder builder = new UriBuilder(path);
            builder.Query = invoiceId;
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion


        #region UpdateInvoiceXml

        /*
         *  Método para actualizar el Xml Firmado de la factura
        */
        static async public Task<Response.Response> UpdateInvoiceXml(string invoiceId, string invoiceXml, string token)
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
                return await UpdateInvoiceXmlAsync(httpClient, invoiceId, invoiceXml);
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
         *  Método para realizar el Request de la actualización del Xml de la factura y procesar la respuesta 
        */
        static async Task<Response.Response> UpdateInvoiceXmlAsync(HttpClient httpClient, string invoiceId, string invoiceXml)
        {
            string path = Config.webServiceUrl + "/Invoice/UpdateXML/"+ invoiceId;
            UriBuilder builder = new UriBuilder(path);
            builder.Query = invoiceId;
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(invoiceXml);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion
        
    }

    #region Json Responses

    public class JsonInvoices
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public Invoice[] listObjectResponse;
    }

    public class JsonCreateInvoice
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public Invoice objectResponse;
    }
    public class JsonInvoicePDF
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public PDF objectResponse;
    }

    #endregion

    #region Parameters and Additional Clases

    public class InvoiceSearchConfiguration
    {
        public string StartDueDate;
        public string EndDueDate;
        public string ClientId;
        public string InvoiceId;
        public string ExternalReferenceNumber;
        public InvoiceStatus? Status;
        public int? Page;
        public int? PageSize;
        public StatusFirmaDigital? EstatusFirma;
        public FirmType? TipoFirma;
        public bool? IsPOSSearch;
    }

    public enum StatusFirmaDigital
    {
        // Pendiente por firmar
        Pendiente = 0,
        // Firmada
        Firmada,
        // Error"
        Error,
    }

    public enum FirmType
    {
        // Firma Llave Criptográfica
        Llave = 0,
        // Firma Digital
        Firma,
        //Llave Criptográfica / Firma Digital
        Todos
    }

    public enum InvoiceStatus
    {
        Pagada = 0,
        Provisional = 1,
        Contabilizada = 2,
        Reversada = 3,
        Pendiente = 4,
        Anulada = 5,
    }

    #endregion
}
