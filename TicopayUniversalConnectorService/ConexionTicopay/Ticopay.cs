using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Authentication;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Response;
using TicoPayDll.Taxes;
using TicopayUniversalConnectorService.Log;
using static TicoPayDll.Clients.ClientController;
using static TicoPayDll.Taxes.TaxesController;

namespace TicopayUniversalConnectorService.ConexionTicopay
{
    public class Ticopay
    {
        /// <summary>
        /// Envía una factura a Ticopay.
        /// </summary>
        /// <param name="token">Token de seguridad.</param>
        /// <param name="factura">Factura.</param>
        /// <returns></returns>
        public Invoice EnviarFactura(string token, CreateInvoice factura)
        {
            TicoPayDll.Response.Response respuestaServicio;

            respuestaServicio = TicoPayDll.Invoices.InvoiceController.CreateNewInvoice(factura, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateInvoice invoice = JsonConvert.DeserializeObject<JsonCreateInvoice>(respuestaServicio.result);
                return invoice.objectResponse;
            }
            else
            {
                JsonError error = JsonConvert.DeserializeObject<JsonError>(respuestaServicio.result);
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Error("Ticopay", "Enviar Factura", respuestaServicio.status.ToString() + " " + error.error_msg + " " + error.innerExcepcion);
                throw new Exception("Error al enviar la factura " + factura.ExternalReferenceNumber + " a Ticopay " + " " + respuestaServicio.status.ToString() + " " + error.error_msg + " " + error.innerExcepcion);
            }
        }

        /// <summary>
        /// Envía un tiquete a Ticopay.
        /// </summary>
        /// <param name="token">Token de seguridad.</param>
        /// <param name="factura">Tiquete.</param>
        /// <returns></returns>
        public Invoice EnviarTiquete(string token, CreateInvoice tiquete)
        {
            TicoPayDll.Response.Response respuestaServicio;

            respuestaServicio = TicoPayDll.Invoices.InvoiceController.CreateNewTicket(tiquete, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateInvoice ticket = JsonConvert.DeserializeObject<JsonCreateInvoice>(respuestaServicio.result);
                return ticket.objectResponse;
            }
            else
            {
                JsonError error = JsonConvert.DeserializeObject<JsonError>(respuestaServicio.result);
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Error("Ticopay", "Enviar Tiquete", respuestaServicio.status.ToString() + " " + error.error_msg + " " + error.innerExcepcion);
                throw new Exception("Error al enviar el tiquete" + tiquete.ExternalReferenceNumber + " a Ticopay " + " " + respuestaServicio.status.ToString() + " " + error.error_msg + " " + error.innerExcepcion);
            }
        }

        /// <summary>
        /// Envía una Nota a Ticopay.
        /// </summary>
        /// <param name="token">Token de seguridad.</param>
        /// <param name="nota">Nota.</param>
        /// <param name="afectarBalance">Si <c>true</c> no afecta el balance ni estatus de nota.</param>
        /// <returns></returns>
        public CompleteNote EnviarNota(string token, CompleteNote nota, bool afectarBalance = false)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Notes.NoteController.CreateNewNote(nota, token, afectarBalance).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateNote note = JsonConvert.DeserializeObject<JsonCreateNote>(respuestaServicio.result);
                return note.objectResponse;
            }
            else
            {
                JsonError error = JsonConvert.DeserializeObject<JsonError>(respuestaServicio.result);
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Error("Ticopay", "Enviar Nota", "Error al enviar la nota de " + nota.NoteType + " a Ticopay " + " " + respuestaServicio.status.ToString() + " " + error.error_msg);
                throw new Exception("Error al enviar la nota de " + nota.NoteType + " a Ticopay " + " " + respuestaServicio.status.ToString() + " " + error.error_msg + " " + error.innerExcepcion);
            }
        }

        /// <summary>
        /// Busca una factura en Ticopay.
        /// </summary>
        /// <param name="invoiceId">Id de la factura (Invoice) en Ticopay.</param>
        /// <param name="token">Token de seguridad.</param>
        /// <returns></returns>
        public Invoice BuscarFactura(string invoiceId, string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            InvoiceSearchConfiguration parametrosBusqueda = new InvoiceSearchConfiguration();
            parametrosBusqueda.ClientId = null;
            parametrosBusqueda.InvoiceId = invoiceId;
            parametrosBusqueda.Status = null;
            parametrosBusqueda.EndDueDate = null;
            parametrosBusqueda.StartDueDate = null;
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.GetInvoices(parametrosBusqueda, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonInvoices facturas = JsonConvert.DeserializeObject<JsonInvoices>(respuestaServicio.result);
                return facturas.listObjectResponse.First();
            }
            else
            {
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Advertencia("Ticopay", "Obtener una Factura", "Factura no encontrada");
                return null;
            }

        }

        /// <summary>
        /// Reversa una factura o tiquete.
        /// </summary>
        /// <param name="token">Token de Seguridad.</param>
        /// <param name="idInvoiceOrTicket">Id de la factura o tiquete.</param>
        /// <param name="externalReferenceNumber">Numero de referencia externo de la nota.</param>
        /// <returns></returns>
        public CompleteNote ReversarFacturaOTiquete(string token, string idInvoiceOrTicket, string externalReferenceNumber)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Notes.NoteController.ReverseInvoiceOrTicket(idInvoiceOrTicket, token, externalReferenceNumber).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateNote nota = JsonConvert.DeserializeObject<JsonCreateNote>(respuestaServicio.result);
                return nota.objectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        /// <summary>
        /// Busca una factura en Ticopay.
        /// </summary>
        /// <param name="ExternalReferenceNumber">Numero externo de factura (Invoice) en Ticopay.</param>
        /// <param name="token">Token de seguridad.</param>
        /// <returns></returns>
        public Invoice BuscarFacturaPorReferencia(string ExternalReferenceNumber, string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            InvoiceSearchConfiguration parametrosBusqueda = new InvoiceSearchConfiguration();
            parametrosBusqueda.ClientId = null;
            parametrosBusqueda.InvoiceId = null;
            parametrosBusqueda.Status = null;
            parametrosBusqueda.ExternalReferenceNumber = ExternalReferenceNumber;
            parametrosBusqueda.EndDueDate = null;
            parametrosBusqueda.StartDueDate = null;
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.GetInvoices(parametrosBusqueda, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonInvoices facturas = JsonConvert.DeserializeObject<JsonInvoices>(respuestaServicio.result);
                if(facturas.listObjectResponse.Count() > 0)
                {
                    return facturas.listObjectResponse.First();
                }
                else
                {
                    return null;
                }                
            }
            else
            {
                RegistroDeEventos evento = new RegistroDeEventos();
                // evento.Advertencia("Ticopay", "Obtener una Factura", "Factura no encontrada");
                throw new Exception("Busqueda de facturas : Servicio de Ticopay no disponible");
            }

        }

        /// <summary>
        /// Busca facturas en Ticopay.
        /// </summary>
        /// <param name="desde">Fecha desde la cual se buscaran las facturas.</param>
        /// <param name="token">Token de seguridad.</param>
        /// <returns></returns>
        public List<Invoice> BuscarFacturas(DateTime desde, string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            InvoiceSearchConfiguration parametrosBusqueda = new InvoiceSearchConfiguration();
            parametrosBusqueda.ClientId = null;
            parametrosBusqueda.InvoiceId = null;
            parametrosBusqueda.Status = null;
            parametrosBusqueda.EndDueDate = null;
            parametrosBusqueda.StartDueDate = desde.ToString("dd/MM/yyyy");
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.GetInvoices(parametrosBusqueda, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonInvoices facturas = JsonConvert.DeserializeObject<JsonInvoices>(respuestaServicio.result);
                return facturas.listObjectResponse.ToList();
            }
            else
            {
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Advertencia("Ticopay", "Obtener Facturas", "Facturas no encontradas");
                return null;
            }

        }

        /// <summary>
        /// Busca notas en Ticopay.
        /// </summary>
        /// <param name="desde">Fecha desde la cual se buscaran las notas.</param>
        /// <param name="token">Token de seguridad.</param>
        /// <returns></returns>
        public List<Note> BuscarNotas(DateTime desde, string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            NotesSearchConfiguration parametrosBusqueda = new NotesSearchConfiguration();
            parametrosBusqueda.InvoiceId = null;
            parametrosBusqueda.Status = null;
            parametrosBusqueda.EndDueDate = null;
            parametrosBusqueda.StartDueDate = desde.ToString("dd/MM/aaaa");
            respuestaServicio = TicoPayDll.Notes.NoteController.GetNotes(parametrosBusqueda, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonNotes facturas = JsonConvert.DeserializeObject<JsonNotes>(respuestaServicio.result);
                return facturas.listObjectResponse.ToList();
            }
            else
            {
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Advertencia("Ticopay", "Obtener Notas", "Notas no encontradas");
                return null;
            }

        }

        /// <summary>
        /// Busca un Cliente por numero de identificación en Ticopay.
        /// </summary>
        /// <param name="token">Token de seguridad.</param>
        /// <param name="detallado">Si <c>true</c> trae la información completa del cliente, sino la básica.</param>
        /// <param name="id">Identificación de Cliente.</param>
        /// <returns></returns>
        public Client BuscarCliente(string token, bool detallado, string id)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Clients.ClientController.SearchClients(token, detallado, id).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonSearchClient clientes = JsonConvert.DeserializeObject<JsonSearchClient>(respuestaServicio.result);
                return clientes.objectResponse;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Crea un cliente en Ticopay.
        /// </summary>
        /// <param name="token">Token de seguridad.</param>
        /// <param name="cliente">Cliente.</param>
        /// <returns></returns>
        public Client CrearCliente(string token, Client cliente)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Clients.ClientController.CreateNewClient(cliente, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateClient clientes = JsonConvert.DeserializeObject<JsonCreateClient>(respuestaServicio.result);
                return clientes.objectResponse;
            }
            else
            {
                JsonError error = JsonConvert.DeserializeObject<JsonError>(respuestaServicio.result);
                if (error.innerExcepcion.Contains("El número de Cédula física, debe contener 9 dígitos") == true)
                {
                    throw new Exception("El número de Cédula física, debe contener 9 dígitos");
                }
                if (error.innerExcepcion.Contains("El número de cédula jurídica, debe contener 10 dígitos") == true)
                {
                    throw new Exception("El número de cédula jurídica, debe contener 10 dígitos");
                }
                if (error.innerExcepcion.Contains("El número DIMEX, debe contener 11 0 12 dígitos") == true)
                {
                    throw new Exception("El número DIMEX, debe contener 11 0 12 dígitos");
                }
                if (error.innerExcepcion.Contains("El número NITE, debe contener 10 dígitos") == true)
                {
                    throw new Exception("El número NITE, debe contener 10 dígitos");
                }
                if (error.innerExcepcion.Contains("El número identificación de extranjeros, No puede estar vacío y debe contener mínimo 5 dígitos") == true)
                {
                    throw new Exception("El número identificación de extranjeros, No puede estar vacío y debe contener mínimo 5 dígitos");
                }
                if (error.innerExcepcion.Contains("Existe un cliente con el mismo número de cédula.") == true)
                {
                    throw new Exception("Existe un cliente con el mismo número de cédula.");
                }
                if (error.innerExcepcion.Contains("Existe un cliente con el mismo número de cédula.") == true)
                {
                    throw new Exception("Existe un cliente con el mismo número de cédula.");
                }
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Error("Ticopay", "Crear nuevo Cliente", error.innerExcepcion);
                return null;
            }

        }

        /// <summary>
        /// Busca en Ticopay los Impuestos que maneja el SubDominio.
        /// </summary>
        /// <param name="token">Token de seguridad.</param>
        /// <returns></returns>
        public Tax[] BuscarImpuestos(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Taxes.TaxesController.Gettaxes(token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonTaxes impuestos = JsonConvert.DeserializeObject<JsonTaxes>(respuestaServicio.result);
                return impuestos.listObjectResponse;
            }
            else
            {
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Advertencia("Ticopay", "Obtener Impuestos", "No existen Impuestos creados en Ticopay para el Sub Dominio");
                return null;
            }
        }

        /// <summary>
        /// Autentifica en Ticopay un usuario y devuelve el token de sesión.
        /// </summary>
        /// <param name="tenancy">Tenant o Sub Dominio.</param>
        /// <param name="user">Usuario.</param>
        /// <param name="password">Clave.</param>
        /// <returns>Token de seguridad</returns>
        public string AutentificarUsuario(string tenancy, string user, string password)
        {

            TicoPayDll.Response.Response respuestaServicio;
            TicoPayDll.Authentication.UserCredentials credenciales = new TicoPayDll.Authentication.UserCredentials();
            credenciales.tenancyName = tenancy;
            credenciales.usernameOrEmailAddress = user;
            credenciales.password = password;
            respuestaServicio = TicoPayDll.Authentication.Authentication.Authenticate(credenciales).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonAuthentication token = JsonConvert.DeserializeObject<JsonAuthentication>(respuestaServicio.result);
                return token.objectResponse.tokenAuthenticate;
            }
            else
            {
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Advertencia("Ticopay", "Autentificar Sub Dominio", "Problemas al Autentificar al Sub Dominio " + respuestaServicio.message);
                return null;
            }
        }

        /// <summary>
        /// Verifica si el Tenant o Sub Dominio tiene permiso para usar el Conector .
        /// </summary>
        /// <param name="tenant">Tenant o Sub Dominio.</param>
        /// <param name="conector">Nombre del Conector (Igual a la Categoría en Ticopay).</param>
        /// <returns>Verdadero si tiene permiso de uso, Falso si no posee permiso</returns>
        public bool VerificarPermisoConector(string tenant, string conector)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Authentication.Authentication.VerifyConnector(tenant, conector).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonConnector token = JsonConvert.DeserializeObject<JsonConnector>(respuestaServicio.result);
                return token.objectResponse;
            }
            else
            {
                RegistroDeEventos evento = new RegistroDeEventos();
                evento.Advertencia("Ticopay", "Verificar Permiso Conector", "El Sub Dominio no posee permiso para utilizar el Conector");
                return false;
            }
        }

        public Invoice PagarFactura(string token, List<PaymentInvoce> Pagos, string IdFacturaOTiquete)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.PayInvoiceOrTicket(IdFacturaOTiquete,Pagos,token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateInvoice FacturaOTiquetePagado = JsonConvert.DeserializeObject<JsonCreateInvoice>(respuestaServicio.result);
                return FacturaOTiquetePagado.objectResponse;
            }
            else
            {
                JsonError error = JsonConvert.DeserializeObject<JsonError>(respuestaServicio.result);
                throw new Exception(error.innerExcepcion + " " + error.error_msg);
            }
        }
    }
}
