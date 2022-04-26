using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Invoices;
using TicoPayDll.Response;

namespace TicoPayDll.Notes
{
    public class NoteController
    {
        #region GetNotes

        /*
         *  Metodo para obtener las notas
        */
        static async public Task<Response.Response> GetNotes(NotesSearchConfiguration searchParameters, string token)
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
                return await GetNotesAsync(httpClient, searchParameters);
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
         *  Metodo para realizar el Request de Busqueda de notas y procesar la respuesta 
        */
        static async Task<Response.Response> GetNotesAsync(HttpClient httpClient, NotesSearchConfiguration parameters)
        {
            string path = Config.webServiceUrl + "/Invoice/GetNotes";
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region CreateNewNote

        /*
         *  Metodo para crear una factura
        */
        static async public Task<Response.Response> CreateNewNote(CompleteNote newNote, string token, bool afectarBalance)
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
                return await CreateNoteAsync(httpClient, newNote, afectarBalance);
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
        static async Task<Response.Response> CreateNoteAsync(HttpClient httpClient, CompleteNote note, bool afectarBalance)
        {
            string path = "";
            if (afectarBalance == true)
            {
                path = Config.webServiceUrl + "/Invoice/ApplyNote?dontModifyBalance=true";
            }
            else
            {
                path = Config.webServiceUrl + "/Invoice/ApplyNote?dontModifyBalance=false";
            }
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(note);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region ReverseInvoiceOrTicket

        /*
         *  Metodo para crear una factura
        */
        static async public Task<Response.Response> ReverseInvoiceOrTicket(string idInvoiceOrTicket, string token, string externalReferenceNumber)
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
                return await ReverseInvoiceOrTicketAsync(httpClient, idInvoiceOrTicket, externalReferenceNumber);
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
        static async Task<Response.Response> ReverseInvoiceOrTicketAsync(HttpClient httpClient, string idInvoiceOrTicket, string externalReferenceNumber)
        {
            string path = "";
            if(externalReferenceNumber != null)
            {
                path = Config.webServiceUrl + "/Invoice/ApplyReverse?idInvoiceOrTicket=" + idInvoiceOrTicket + "&NumReferenciaExterna=" + externalReferenceNumber;
            }
            else
            {
                path = Config.webServiceUrl + "/Invoice/ApplyReverse?idInvoiceOrTicket=" + idInvoiceOrTicket;
            }
                    
            Response.Response methodResponse = new Response.Response();
            // string json = JsonConvert.SerializeObject(note);
            var content = new StringContent("",Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(
                path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region GetNoteXml

        /*
         *  Metodo para Obtener el Xml de una nota
        */
        static async public Task<Response.Response> GetXml(string noteId, string token)
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
                return await GetNoteXmlAsync(httpClient, noteId);
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
         *  Metodo para realizar el Request de la obtencion del Xml de la nota y procesar la respuesta 
        */
        static async Task<Response.Response> GetNoteXmlAsync(HttpClient httpClient, string noteId)
        {
            string path = Config.webServiceUrl + "Invoice/GetNoteXML/" + noteId;
            Response.Response methodResponse = new Response.Response();
            UriBuilder builder = new UriBuilder(path);
            builder.Query = noteId;
            // HttpResponseMessage response = httpClient.GetAsync(builder.Uri).GetAwaiter().GetResult();
            HttpResponseMessage response = httpClient.GetAsync(path).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion

        #region UpdateNoteXml

        /*
         *  Metodo para actualizar el Xml Firmado de la nota
        */
        static async public Task<Response.Response> UpdateInvoiceXml(string noteId, string noteXml, string token)
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
                return await UpdateNoteXmlAsync(httpClient, noteId, noteXml);
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
         *  Metodo para realizar el Request de la actualizacion del Xml de la nota y procesar la respuesta 
        */
        static async Task<Response.Response> UpdateNoteXmlAsync(HttpClient httpClient, string noteId, string noteXml)
        {
            string path = Config.webServiceUrl + "/Invoice/UpdateNoteXML/" + noteId;
            UriBuilder builder = new UriBuilder(path);
            builder.Query = noteId;
            Response.Response methodResponse = new Response.Response();
            string json = JsonConvert.SerializeObject(noteXml);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(path, content).GetAwaiter().GetResult();
            methodResponse = await methodResponse.ProcessHttpResponse(response);
            return methodResponse;
        }

        #endregion
    }

    #region Notes Response Types and Search Parameters

    public class NotesSearchConfiguration
    {
        public string StartDueDate { get; set; }

        public string EndDueDate { get; set; }

        public string InvoiceId { get; set; }

        public string NoteId { get; set; }

        public StatusTaxAdministration? Status { get; set; }

        public int? Page { get; set; }

        public int? PageSize { get; set; }

        public StatusFirmaDigital? EstatusFirma { get; set; }

        public FirmType? TipoFirma { get; set; }
    }

    public class JsonNotes
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public Note[] listObjectResponse;
    }

    public class JsonCreateNote
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public CompleteNote objectResponse;
    }

    public class JsonNoteXml
    {
        public Boolean success;
        public int total_elements;
        public int statusCode;
        public string objectResponse;
    }

    #endregion

}
