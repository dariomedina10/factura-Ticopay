using Abp.Domain.Repositories;
using Abp.UI;
using Neodynamic.SDK.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.Printers;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Web.Controllers
{
    public class WebClientPrintAPIController : Controller
    {
        private readonly TenantManager _tenantManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<Note, Guid> _noteRepository;

        public WebClientPrintAPIController(TenantManager tenantManager, IInvoiceManager invoiceManager, IRepository<Invoice, Guid> invoiceRepository, IRepository<Note, Guid> noteRepository)
        {
            _tenantManager = tenantManager;
            _invoiceManager = invoiceManager;
            _invoiceRepository = invoiceRepository;
            _noteRepository = noteRepository;
        }
        const string PRINTER_ID = "-PID";
        const string INSTALLED_PRINTER_NAME = "-InstalledPrinterName";
        const string NET_PRINTER_HOST = "-NetPrinterHost";
        const string NET_PRINTER_PORT = "-NetPrinterPort";
        const string PARALLEL_PORT = "-ParallelPort";
        const string SERIAL_PORT = "-SerialPort";
        const string SERIAL_PORT_BAUDS = "-SerialPortBauds";
        const string SERIAL_PORT_DATA_BITS = "-SerialPortDataBits";
        const string SERIAL_PORT_STOP_BITS = "-SerialPortStopBits";
        const string SERIAL_PORT_PARITY = "-SerialPortParity";
        const string SERIAL_PORT_FLOW_CONTROL = "-SerialPortFlowControl";
        const string PRINTER_COMMANDS = "-PrinterCommands";


        [AllowAnonymous]
        public void ProcessRequest()
        {
            //get session ID
            string sessionID = (HttpContext.Request["sid"] != null ? HttpContext.Request["sid"] : null);

            //get Query String
            string queryString = HttpContext.Request.Url.Query;

            if (Boolean.Parse(ConfigurationManager.AppSettings["RegisterWebClientPrint"]))
            {
                WebClientPrint.LicenseOwner = ConfigurationManager.AppSettings["WebClientPrint40LicenseOwner"];
                WebClientPrint.LicenseKey = ConfigurationManager.AppSettings["WebClientPrint40LicenseKey"];
            }

            try
            {
                //Determine and get the Type of Request 
                RequestType prType = WebClientPrint.GetProcessRequestType(queryString);

                if (prType == RequestType.GenPrintScript ||
                    prType == RequestType.GenWcppDetectScript)
                {
                    //Let WebClientPrint to generate the requested script
                    byte[] script = WebClientPrint.GenerateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), queryString);

                    HttpContext.Response.ContentType = "text/javascript";
                    HttpContext.Response.BinaryWrite(script);
                    HttpContext.Response.End();
                }
                else if (prType == RequestType.ClientSetWcppVersion)
                {
                    //This request is a ping from the WCPP utility
                    //so store the session ID indicating it has the WCPP installed
                    //also store the WCPP Version if available
                    string wcppVersion = HttpContext.Request["wcppVer"];
                    if (string.IsNullOrEmpty(wcppVersion))
                        wcppVersion = "1.0.0.0";

                    HttpContext.Application.Set(sessionID + "wcppInstalled", wcppVersion);
                }
                else if (prType == RequestType.ClientSetInstalledPrinters)
                {
                    //WCPP Utility is sending the installed printers at client side
                    //so store this info with the specified session ID
                    string printers = HttpContext.Request["printers"];
                    if (string.IsNullOrEmpty(printers) == false)
                        printers = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printers));

                    HttpContext.Application.Set(sessionID + "printers", printers);

                }
                else if (prType == RequestType.ClientSetInstalledPrintersInfo)
                {
                    //WCPP Utility is sending the client installed printers with detailed info
                    //so store this info with the specified session ID
                    //Printers Info is in JSON format
                    string printersInfo = HttpContext.Request.Form["printersInfoContent"];

                    if (string.IsNullOrEmpty(printersInfo) == false)
                        printersInfo = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printersInfo));

                    HttpContext.Application.Set(sessionID + "printersInfo", printersInfo);


                }
                else if (prType == RequestType.ClientGetWcppVersion)
                {
                    //return the WCPP version for the specified sid if any
                    bool sidWcppVersion = (HttpContext.Application.Get(sessionID + "wcppInstalled") != null);

                    HttpContext.Response.ContentType = "text/plain";
                    HttpContext.Response.Write((sidWcppVersion ? HttpContext.Application.Get(sessionID + "wcppInstalled") : ""));
                    HttpContext.Response.End();

                }
                else if (prType == RequestType.ClientGetInstalledPrinters)
                {
                    //return the installed printers for the specified sid if any
                    bool sidHasPrinters = (HttpContext.Application.Get(sessionID + "printers") != null);

                    HttpContext.Response.ContentType = "text/plain";
                    HttpContext.Response.Write((sidHasPrinters ? HttpContext.Application.Get(sessionID + "printers") : ""));
                    HttpContext.Response.End();
                }
                else if (prType == RequestType.ClientGetInstalledPrintersInfo)
                {
                    //return the installed printers with detailed info for the specified Session ID (sid) if any
                    bool sidHasPrinters = (HttpContext.Application[sessionID + "printersInfo"] != null);

                    HttpContext.Response.ContentType = "text/plain";
                    HttpContext.Response.Write(sidHasPrinters ? HttpContext.Application[sessionID + "printersInfo"] : "");

                }

            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.ContentType = "text/plain";
                HttpContext.Response.Write(ex.Message + " - StackTrace: " + ex.StackTrace);
                HttpContext.Response.End();
            }


        }


        [AllowAnonymous]
        public void PrintCommands(string id, string tenantId, TypeDocumentInvoice tipoDocumento)
        {
            string sid ="";
            Invoice invoice;
            Note note;
            Guid Id = new Guid(id);
            var TipoDocumento = tipoDocumento;
            var document = string.Empty;
            var tenant = _tenantManager.Get(Convert.ToInt32(tenantId));
            var printer = new Printer((PrinterTypes)tenant.PrinterType);

            if (TipoDocumento == TypeDocumentInvoice.Invoice || TipoDocumento == TypeDocumentInvoice.Ticket)
            {
                invoice = _invoiceRepository.GetAll().Where(x => x.Id == Id && x.TenantId == tenant.Id).FirstOrDefault();

                if (invoice != null)
                {
                    DocumentPrint docPrint = new DocumentPrint(invoice);
                    document = printer.print(docPrint);
                }
                else
                {
                    throw new UserFriendlyException("Error al buscar factura.");
                }
            }

            if (TipoDocumento == TypeDocumentInvoice.NotaCredito || TipoDocumento == TypeDocumentInvoice.NotaDebito)
            {
                note = _noteRepository.GetAll().Where(n => n.Id == Id && n.TenantId == tenant.Id).FirstOrDefault();

                if (note != null)
                {
                    DocumentPrint docPrint = new DocumentPrint(note);
                    document = printer.print(docPrint);
                }
                else
                {
                    throw new UserFriendlyException("Error al buscar la Nota.");
                }
            }



            if (Boolean.Parse(ConfigurationManager.AppSettings["RegisterWebClientPrint"]))
            {
                WebClientPrint.LicenseOwner = ConfigurationManager.AppSettings["WebClientPrint40LicenseOwner"];
                WebClientPrint.LicenseKey = ConfigurationManager.AppSettings["WebClientPrint40LicenseKey"];
            }

            if (WebClientPrint.ProcessPrintJob(System.Web.HttpContext.Current.Request.Url.Query))
            {
                HttpApplicationStateBase app = HttpContext.Application;

                //Create a ClientPrintJob obj that will be processed at the client side by the WCPP
                ClientPrintJob cpj = new ClientPrintJob();

                //get printer commands for this user id

                object printerCommands = null;
                try
                {
                    //printerCommands = app[sid2 + PRINTER_COMMANDS];
                    printerCommands = document;
                    if (printerCommands != null)
                    {
                        cpj.PrinterCommands = printerCommands.ToString();
                        cpj.FormatHexValues = true;
                        //get printer settings for this user id

                        cpj.ClientPrinter = new DefaultPrinter();

                        //Send ClientPrintJob back to the client
                        System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                        System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent());
                        System.Web.HttpContext.Current.Response.End();
                    }
                }
                finally
                {
                    //if (app[sid2 + PRINTER_COMMANDS] != null)
                    //{
                    //    app[sid2 + PRINTER_COMMANDS] = "";
                    //}
                    if (printerCommands != null)
                    {
                        printerCommands = "";
                    }
                }
            }
        }
    }
}