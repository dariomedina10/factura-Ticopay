using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Invoices;
using TicoPayDll.Response;

namespace TicoPayDll.Reports
{

    public class ReportsController
    {
        /*
         *  Metodo para obtener las facturas enviadas a Tribunet
        */
        static async public Task<Response.Response> GetInvoicesSendToTribunet(ReportInvoicesSentToTribunetSearchInput parameters, string token)
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
                return await GetInvoicesSendToTribunetAsync(httpClient, parameters);
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
         *  Metodo para realizar el Request de Busqueda de facturas enviadas a tribunet y procesar la respuesta 
        */
        static async Task<Response.Response> GetInvoicesSendToTribunetAsync(HttpClient httpClient, ReportInvoicesSentToTribunetSearchInput parameters)
        {
            string path = Config.webServiceUrl + "/reports/InvoicesSentToTribunetPost";            
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();            
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

    }

    public class JsonInvoicesSendToTribunet
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public InvoiceSendTribunet[] invoices;
    }

    public class ReportInvoicesSentToTribunetSearchInput
    {
        public Guid? ClienteId { get; set; }

        public string CedulaCliente { get; set; }

        public string NombreCliente { get; set; }

        public int? NumeroFactura { get; set; }

        public DateTime? FechaEmisionDesde { get; set; }

        public DateTime? FechaEmisionHasta { get; set; }

        public bool? RecepcionConfirmada { get; set; }

        public Status? Status { get; set; }

        public StatusTaxAdministration? StatusTribunet { get; set; }

        public PaymetnMethodType? MedioPago { get; set; }

        public FacturaElectronicaCondicionVenta? CondicionVenta { get; set; }

        // json or pdf
        public string type;
    }
}
