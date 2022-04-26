using Interop.QBXMLRP2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Services;
using TicopayUniversalConnectorService.ConexionesExternas.QuickBooks.Dto;
using TicopayUniversalConnectorService.Log;

namespace TicopayUniversalConnectorService.ConexionesExternas.QuickBooks
{
    public class QuickbooksEnterpriseDesktop
    {
        private string ticket;
        private RequestProcessor2 rp;
        private string maxVersion;
        private string companyFile = "";
        private QBFileMode mode = QBFileMode.qbFileOpenDoNotCare;
        RegistroDeEventos _eventos;
        FileLog _errorLog;

        private string appID = "";
        private string appName = "";

        public QuickbooksEnterpriseDesktop(string _appName, string _companyFile, string _appId)
        {
            appID = _appId;
            appName = _appName;
            companyFile = _companyFile;
            _eventos = new RegistroDeEventos();
            _errorLog = new FileLog();
            // ConnectToQB();
        }

        #region Consultas

        public bool ProbarConexion()
        {
            try
            {                
                ConnectToQB();                
                disconnectFromQB();
                return true;
            }
            catch (Exception ex)
            {
                rp.CloseConnection();
                return false;
            }
        }

        public List<QuickbooksClient> BuscarClientes(string ClientId = null, bool isTicket = false)
        {
            try
            {
                List<QuickbooksClient> clientes = new List<QuickbooksClient>();
                ConnectToQB();
                string response = processRequestFromQB(buildCustomerQueryRqXML(new string[] { "FullName",
                "CompanyName" , "BillAddress", "Phone", "Email" ,"DataExtRet"}, ClientId));
                clientes = ParsearXMLClientes(response, isTicket,false);
                disconnectFromQB();
                return clientes;
            }
            catch (Exception ex)
            {
                disconnectFromQB();
                _eventos.Error("QuickBooksEnterpriseDesktop", "BuscarClientes", " Error-> " + ex.Message);
                throw new Exception("Imposible obtener los datos de los Clientes");
            }
        }

        public QuickbooksClient BuscarCliente(string ClientId = null, bool isTicket = false)
        {
            try
            {
                List<QuickbooksClient> clientes = new List<QuickbooksClient>();
                string request = "CustomerQueryRq";
                ConnectToQB();
                // int count = getCount(request);
                string response = processRequestFromQB(buildCustomerQueryRqXML(new string[] { "FullName",
                "CompanyName" , "BillAddress", "FirstName", "LastName" , "Phone", "Email" ,"DataExtRet"}, ClientId));
                clientes = ParsearXMLClientes(response, isTicket,true);
                disconnectFromQB();
                if (clientes.Count > 0)
                {
                    return clientes.First();
                }
                else
                {
                    return null;
                }
            }            
            catch (Exception ex)
            {
                disconnectFromQB();
                // _eventos.Error("QuickBooksEnterpriseDesktop", "BuscarClienteEspecifico", " Error-> " + ex.Message);
                throw ex;
            }
        }

        public List<QuickbooksInvoice> BuscarfacturasCredito(DateTime FechaJob, string invoiceId = null)
        {
            try
            {
                List<QuickbooksInvoice> facturas = new List<QuickbooksInvoice>();
                ConnectToQB();
                string response = processRequestFromQB(buildInvoiceQueryRqXML(FechaJob, invoiceId));
                if (invoiceId == null)
                {
                    facturas = ParsearXMLFacturasCredito(response, false);
                }
                else
                {
                    facturas = ParsearXMLFacturasCredito(response, true);
                }
                disconnectFromQB();
                return facturas;
            }
            catch (Exception ex)
            {
                disconnectFromQB();
                // _eventos.Error("QuickBooksEnterpriseDesktop", "BuscarFacturasCredito", " Error-> " + ex.Message);
                throw ex;
            }
        }

        public List<QuickbooksPayment> BuscarPagos(DateTime FechaJob, string paymentId = null)
        {
            try
            {
                List<QuickbooksPayment> facturas = new List<QuickbooksPayment>();
                ConnectToQB();
                string response = processRequestFromQB(buildPaymentQueryRqXML(FechaJob, paymentId));
                if(paymentId == null)
                {
                    facturas = ParsearXMLPagos(response, false);
                }
                else
                {
                    facturas = ParsearXMLPagos(response, true);
                }
                disconnectFromQB();
                return facturas;
            }
            catch (Exception ex)
            {
                disconnectFromQB();
                // _eventos.Error("QuickBooksEnterpriseDesktop", "BuscarPagos", " Error-> " + ex.Message);
                throw ex;
            }
        }

        public bool ActualizarFactura(string invoiceId, string EditSequence, string ConsecutiveNumber)
        {
            try
            {
                ConnectToQB();
                string response = processRequestFromQB(buildInvoiceModXML(invoiceId,EditSequence,ConsecutiveNumber));
                // facturas = ParsearXMLFacturasCredito(response, true);
                disconnectFromQB();
                return true;
            }
            catch (Exception ex)
            {
                disconnectFromQB();
                _eventos.Error("QuickBooksEnterpriseDesktop", "Actualizar Factura Crédito", " Error-> " + ex.Message);
                throw new Exception("Imposible actualizar el Numero de Consecutivo de factura de Crédito");
            }
        }

        public bool ActualizarSalesReceipt(string SalesReceiptId, string EditSequence, string ConsecutiveNumber)
        {
            try
            {
                ConnectToQB();
                string response = processRequestFromQB(buildSalesReceiptModXML(SalesReceiptId, EditSequence, ConsecutiveNumber));
                // facturas = ParsearXMLFacturasCredito(response, true);
                disconnectFromQB();
                return true;
            }
            catch (Exception ex)
            {
                disconnectFromQB();
                _eventos.Error("QuickBooksEnterpriseDesktop", "Actualizar Factura de Contado", " Error-> " + ex.Message);
                throw new Exception("Imposible actualizar el Numero de Consecutivo de factura de Contado");
            }
        }

        public bool ActualizarCreditMemo(string CreditMemoId, string EditSequence, string ConsecutiveNumber)
        {
            try
            {
                ConnectToQB();
                string response = processRequestFromQB(buildCreditMemoModXML(CreditMemoId, EditSequence, ConsecutiveNumber));
                // facturas = ParsearXMLFacturasCredito(response, true);
                disconnectFromQB();
                return true;
            }
            catch (Exception ex)
            {
                disconnectFromQB();
                _eventos.Error("QuickBooksEnterpriseDesktop", "Actualizar Nota de Crédito", " Error-> " + ex.Message);
                throw new Exception("Imposible actualizar el Numero de Consecutivo de Nota de Crédito");
            }
        }

        public List<QuickbooksInvoice> BuscarfacturasContado(DateTime FechaJob, string invoiceId = null)
        {
            try
            {
                List<QuickbooksInvoice> facturas = new List<QuickbooksInvoice>();
                ConnectToQB();
                string response = processRequestFromQB(buildSalesReceiptQueryRqXML(FechaJob, invoiceId));
                if (invoiceId == null)
                {
                    facturas = ParsearXMLFacturasContado(response, false);
                }
                else
                {
                    facturas = ParsearXMLFacturasContado(response, true);
                }                    
                disconnectFromQB();
                return facturas;
            }
            catch (Exception ex)
            {
                disconnectFromQB();
                // _eventos.Error("QuickBooksEnterpriseDesktop", "BuscarFacturasContado", " Error-> " + ex.Message);
                throw ex;
            }
        }

        public List<QuickbooksNote> BuscarNotas(DateTime FechaJob, string noteId = null)
        {
            try
            {
                List<QuickbooksNote> notas = new List<QuickbooksNote>();                
                ConnectToQB();
                string response = processRequestFromQB(buildNoteQueryRqXML(FechaJob, noteId));
                if(noteId == null)
                {
                    notas = ParsearXMLNotas(response, false);
                }
                else
                {
                    notas = ParsearXMLNotas(response, true);
                }                
                disconnectFromQB();
                return notas;
            }
            catch (Exception ex)
            {
                disconnectFromQB();
                // _eventos.Error("QuickBooksEnterpriseDesktop", "BuscarNotas", " Error-> " + ex.Message);
                throw ex;
            }
        }

        public bool EsServicio(string itemId)
        {
            try
            {
                ConnectToQB();
                bool Servicio = false;
                string response = processRequestFromQB(buildItemServiceQueryRqXML(itemId));
                Servicio = ParsearXMLServiceItem(response);
                disconnectFromQB();
                return Servicio;
            }
            catch (Exception ex)
            {
                disconnectFromQB();
                _eventos.Error("QuickBooksEnterpriseDesktop", "Identificar si es servicio", " Error-> " + ex.Message);
                throw new Exception("Imposible identificar si es un servicio");
            }
        }

        #endregion

        #region Conexión a Quickbooks

        private void ConnectToQB()
        {            
            try
            {
                rp = new RequestProcessor2();
                rp.OpenConnection(appID, appName);
                ticket = rp.BeginSession(companyFile, mode);
                string[] versions = rp.get_QBXMLVersionsForSession(ticket);
                maxVersion = versions[versions.Length - 1];
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "ConnectToQB", "Error de Conexión -> " + ex.Message + " Cod " + ex.HResult);
                throw new Exception("Imposible conectar con Quickbooks");                
            }
        }

        private void disconnectFromQB()
        {
            if (ticket != null)
            {
                try
                {
                    rp.EndSession(ticket);
                    ticket = null;
                    rp.CloseConnection();
                }
                catch (Exception ex)
                {
                    _eventos.Error("QuickBooksEnterpriseDesktop", "disconnectFromQB", "Error de Conexión -> " + ex.Message);
                    throw new Exception("Imposible desconectar Quickbooks)");
                }
            }
        }

        #endregion

        #region Procesado de las Consultas

        private int getCount(string request)
        {
            string response = processRequestFromQB(buildDataCountQuery(request));
            int count = parseRsForCount(response, request);
            return count;
        }

        private string processRequestFromQB(string request)
        {
            try
            {
                return rp.ProcessRequest(ticket, request);
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "processRequestFromQB", "Error de procesado -> " + ex.Message);
                throw new Exception("Imposible obtener los datos de Quickbooks", ex);
            }
        }

        private int parseRsForCount(string xml, string request)
        {
            int ret = -1;
            try
            {
                XmlNodeList RsNodeList = null;
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(xml);
                string tagname = request.Replace("Rq", "Rs");
                RsNodeList = Doc.GetElementsByTagName(tagname);
                System.Text.StringBuilder popupMessage = new System.Text.StringBuilder();
                XmlAttributeCollection rsAttributes = RsNodeList.Item(0).Attributes;
                XmlNode retCount = rsAttributes.GetNamedItem("retCount");
                ret = Convert.ToInt32(retCount.Value);
            }
            catch (Exception e)
            {
                // MessageBox.Show("Error encountered: " + e.Message);
                ret = -1;
            }
            return ret;
        }

        #endregion

        #region Armado de las consultas

        private string buildDataCountQuery(string request)
        {
            string input = "";
            XmlDocument inputXMLDoc = new XmlDocument();
            XmlElement qbXMLMsgsRq = buildRqEnvelope(inputXMLDoc, maxVersion);
            XmlElement queryRq = inputXMLDoc.CreateElement(request);
            queryRq.SetAttribute("metaData", "MetaDataOnly");
            qbXMLMsgsRq.AppendChild(queryRq);
            input = inputXMLDoc.OuterXml;
            return input;
        }

        private XmlElement buildRqEnvelope(XmlDocument doc, string maxVer)
        {
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
            doc.AppendChild(doc.CreateProcessingInstruction("qbxml", "version=\"" + maxVer + "\""));
            XmlElement qbXML = doc.CreateElement("QBXML");
            doc.AppendChild(qbXML);
            XmlElement qbXMLMsgsRq = doc.CreateElement("QBXMLMsgsRq");
            qbXML.AppendChild(qbXMLMsgsRq);
            qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
            return qbXMLMsgsRq;
        }

        private string buildCustomerQueryRqXML(string[] includeRetElement, string listId)
        {
            try
            {
                string xml = "";
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement qbXMLMsgsRq = buildRqEnvelope(xmlDoc, maxVersion);
                qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
                XmlElement CustomerQueryRq = xmlDoc.CreateElement("CustomerQueryRq");
                qbXMLMsgsRq.AppendChild(CustomerQueryRq);
                // Para filtrar por el FullName
                if (listId != null)
                {
                    XmlElement fullNameElement = xmlDoc.CreateElement("ListID");
                    CustomerQueryRq.AppendChild(fullNameElement).InnerText = listId;
                }
                // Para Agregar los campos que se quieren consultar
                for (int x = 0; x < includeRetElement.Length; x++)
                {
                    XmlElement includeRet = xmlDoc.CreateElement("IncludeRetElement");
                    CustomerQueryRq.AppendChild(includeRet).InnerText = includeRetElement[x];
                }
                // Para obtener CustomFields creados
                XmlElement DataExtensions = xmlDoc.CreateElement("OwnerID");
                CustomerQueryRq.AppendChild(DataExtensions).InnerText = "0";
                CustomerQueryRq.SetAttribute("requestID", "1");
                xml = xmlDoc.OuterXml;
                return xml;
            }
            catch(Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "Armar Query Customer", " Error-> " + ex.Message);
                return null;                
            }            
        }

        private string buildInvoiceQueryRqXML(DateTime FechaJob, string invoiceId = null)
        {
            try
            {
                string xml = "";
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement qbXMLMsgsRq = buildRqEnvelope(xmlDoc, maxVersion);
                qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
                XmlElement InvoiceQueryRq = xmlDoc.CreateElement("InvoiceQueryRq");
                qbXMLMsgsRq.AppendChild(InvoiceQueryRq);
                if (invoiceId != null)
                {
                    XmlElement TxnIDElement = xmlDoc.CreateElement("TxnID");
                    InvoiceQueryRq.AppendChild(TxnIDElement).InnerText = invoiceId;
                }
                else
                {
                    XmlElement TxnDateRangeFilter = xmlDoc.CreateElement("TxnDateRangeFilter");
                    InvoiceQueryRq.AppendChild(TxnDateRangeFilter);
                    XmlElement DateMacro = xmlDoc.CreateElement("FromTxnDate");
                    if (FechaJob > DateTime.Now.AddDays(-7))
                    {
                        int dias = Convert.ToInt32(Math.Round((DateTime.Now - FechaJob).TotalDays, 0));
                        TxnDateRangeFilter.AppendChild(DateMacro).InnerText = DateTime.Now.AddDays(-dias).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        TxnDateRangeFilter.AppendChild(DateMacro).InnerText = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                    }
                }
                //XmlElement PaidStatus = xmlDoc.CreateElement("PaidStatus");
                //InvoiceQueryRq.AppendChild(PaidStatus).InnerText = "true";
                XmlElement IncludeLineItems = xmlDoc.CreateElement("IncludeLineItems");
                InvoiceQueryRq.AppendChild(IncludeLineItems).InnerText = "true";
                XmlElement IncludeLinkedTxns = xmlDoc.CreateElement("IncludeLinkedTxns");
                InvoiceQueryRq.AppendChild(IncludeLinkedTxns).InnerText = "true";
                InvoiceQueryRq.SetAttribute("requestID", "1");
                xml = xmlDoc.OuterXml;
                return xml;
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "Armar Query Customer", " Error-> " + ex.Message);
                return null;
            }            
        }

        private string buildInvoiceModXML(string invoiceId, string EditSequence, string ConsecutiveNumber)
        {
            try
            {
                string xml = "";
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement qbXMLMsgsRq = buildRqEnvelope(xmlDoc, maxVersion);
                qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
                XmlElement InvoiceModRq = xmlDoc.CreateElement("InvoiceModRq");
                qbXMLMsgsRq.AppendChild(InvoiceModRq);
                XmlElement InvoiceMod = xmlDoc.CreateElement("InvoiceMod");
                InvoiceModRq.AppendChild(InvoiceMod);
                XmlElement TxnIDElement = xmlDoc.CreateElement("TxnID");
                InvoiceMod.AppendChild(TxnIDElement).InnerText = invoiceId;
                XmlElement EditSequenceElement = xmlDoc.CreateElement("EditSequence");
                InvoiceMod.AppendChild(EditSequenceElement).InnerText = EditSequence;
                XmlElement OtherElement = xmlDoc.CreateElement("Other");
                InvoiceMod.AppendChild(OtherElement).InnerText = ConsecutiveNumber;    
                xml = xmlDoc.OuterXml;
                return xml;
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "Armar Query Mod Invoice", " Error-> " + ex.Message);
                return null;
            }
        }

        private string buildSalesReceiptModXML(string SalesReceiptId, string EditSequence, string ConsecutiveNumber)
        {
            try
            {
                string xml = "";
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement qbXMLMsgsRq = buildRqEnvelope(xmlDoc, maxVersion);
                qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
                XmlElement SalesReceiptModRq = xmlDoc.CreateElement("SalesReceiptModRq");
                qbXMLMsgsRq.AppendChild(SalesReceiptModRq);
                XmlElement SalesReceiptMod = xmlDoc.CreateElement("SalesReceiptMod");
                SalesReceiptModRq.AppendChild(SalesReceiptMod);
                XmlElement TxnIDElement = xmlDoc.CreateElement("TxnID");
                SalesReceiptMod.AppendChild(TxnIDElement).InnerText = SalesReceiptId;
                XmlElement EditSequenceElement = xmlDoc.CreateElement("EditSequence");
                SalesReceiptMod.AppendChild(EditSequenceElement).InnerText = EditSequence;                
                XmlElement OtherElement = xmlDoc.CreateElement("Other");
                SalesReceiptMod.AppendChild(OtherElement).InnerText = ConsecutiveNumber;
                xml = xmlDoc.OuterXml;
                return xml;
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "Armar Query Mod Invoice", " Error-> " + ex.Message);
                return null;
            }
        }

        private string buildCreditMemoModXML(string CreditMemoId, string EditSequence, string ConsecutiveNumber)
        {
            try
            {
                string xml = "";
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement qbXMLMsgsRq = buildRqEnvelope(xmlDoc, maxVersion);
                qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
                XmlElement SalesReceiptModRq = xmlDoc.CreateElement("SalesReceiptModRq");
                qbXMLMsgsRq.AppendChild(SalesReceiptModRq);
                XmlElement SalesReceiptMod = xmlDoc.CreateElement("SalesReceiptMod");
                SalesReceiptModRq.AppendChild(SalesReceiptMod);
                XmlElement TxnIDElement = xmlDoc.CreateElement("TxnID");
                SalesReceiptMod.AppendChild(TxnIDElement).InnerText = CreditMemoId;
                XmlElement EditSequenceElement = xmlDoc.CreateElement("EditSequence");
                SalesReceiptMod.AppendChild(EditSequenceElement).InnerText = EditSequence;
                XmlElement OtherElement = xmlDoc.CreateElement("Other");
                SalesReceiptMod.AppendChild(OtherElement).InnerText = ConsecutiveNumber;
                xml = xmlDoc.OuterXml;
                return xml;
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "Armar Query Mod Invoice", " Error-> " + ex.Message);
                return null;
            }
        }

        private string buildSalesReceiptQueryRqXML(DateTime FechaJob, string invoiceId = null)
        {
            try
            {
                string xml = "";
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement qbXMLMsgsRq = buildRqEnvelope(xmlDoc, maxVersion);
                qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
                XmlElement InvoiceQueryRq = xmlDoc.CreateElement("SalesReceiptQueryRq");
                qbXMLMsgsRq.AppendChild(InvoiceQueryRq);
                if (invoiceId != null)
                {
                    XmlElement TxnIDElement = xmlDoc.CreateElement("TxnID");
                    InvoiceQueryRq.AppendChild(TxnIDElement).InnerText = invoiceId;
                }
                else
                {
                    XmlElement TxnDateRangeFilter = xmlDoc.CreateElement("TxnDateRangeFilter");
                    InvoiceQueryRq.AppendChild(TxnDateRangeFilter);
                    XmlElement DateMacro = xmlDoc.CreateElement("FromTxnDate");
                    if (FechaJob > DateTime.Now.AddDays(-7))
                    {
                        int dias = Convert.ToInt32(Math.Round((DateTime.Now - FechaJob).TotalDays, 0));
                        TxnDateRangeFilter.AppendChild(DateMacro).InnerText = DateTime.Now.AddDays(-dias).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        TxnDateRangeFilter.AppendChild(DateMacro).InnerText = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                    }
                }
                //XmlElement PaidStatus = xmlDoc.CreateElement("PaidStatus");
                //InvoiceQueryRq.AppendChild(PaidStatus).InnerText = "true";
                XmlElement IncludeLineItems = xmlDoc.CreateElement("IncludeLineItems");
                InvoiceQueryRq.AppendChild(IncludeLineItems).InnerText = "true";
                //XmlElement IncludeLinkedTxns = xmlDoc.CreateElement("IncludeLinkedTxns");
                //InvoiceQueryRq.AppendChild(IncludeLinkedTxns).InnerText = "true";
                InvoiceQueryRq.SetAttribute("requestID", "1");
                xml = xmlDoc.OuterXml;
                return xml;
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "Armar Query Customer", " Error-> " + ex.Message);
                return null;
            }            
        }

        private string buildNoteQueryRqXML(DateTime FechaJob, string noteId = null)
        {
            try
            {
                string xml = "";
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement qbXMLMsgsRq = buildRqEnvelope(xmlDoc, maxVersion);
                qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
                XmlElement InvoiceQueryRq = xmlDoc.CreateElement("CreditMemoQueryRq");
                qbXMLMsgsRq.AppendChild(InvoiceQueryRq);
                if (noteId != null)
                {
                    XmlElement TxnIDElement = xmlDoc.CreateElement("TxnID");
                    InvoiceQueryRq.AppendChild(TxnIDElement).InnerText = noteId;
                }
                else
                {
                    XmlElement TxnDateRangeFilter = xmlDoc.CreateElement("TxnDateRangeFilter");
                    InvoiceQueryRq.AppendChild(TxnDateRangeFilter);
                    XmlElement DateMacro = xmlDoc.CreateElement("FromTxnDate");
                    if (FechaJob > DateTime.Now.AddDays(-7))
                    {
                        int dias = Convert.ToInt32(Math.Round((DateTime.Now - FechaJob).TotalDays, 0));
                        TxnDateRangeFilter.AppendChild(DateMacro).InnerText = DateTime.Now.AddDays(-dias).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        TxnDateRangeFilter.AppendChild(DateMacro).InnerText = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                    }
                }
                XmlElement IncludeLineItems = xmlDoc.CreateElement("IncludeLineItems");
                InvoiceQueryRq.AppendChild(IncludeLineItems).InnerText = "true";
                XmlElement IncludeLinkedTxns = xmlDoc.CreateElement("IncludeLinkedTxns");
                InvoiceQueryRq.AppendChild(IncludeLinkedTxns).InnerText = "true";
                InvoiceQueryRq.SetAttribute("requestID", "1");
                xml = xmlDoc.OuterXml;
                return xml;
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "Armar Query Customer", " Error-> " + ex.Message);
                return null;
            }            
        }

        private string buildPaymentQueryRqXML(DateTime FechaJob, string paymentId = null)
        {
            try
            {
                string xml = "";
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement qbXMLMsgsRq = buildRqEnvelope(xmlDoc, maxVersion);
                qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
                XmlElement ReceivePaymentQueryRq = xmlDoc.CreateElement("ReceivePaymentQueryRq");
                qbXMLMsgsRq.AppendChild(ReceivePaymentQueryRq);
                if (paymentId != null)
                {
                    XmlElement TxnIDElement = xmlDoc.CreateElement("TxnID");
                    ReceivePaymentQueryRq.AppendChild(TxnIDElement).InnerText = paymentId;
                }
                else
                {
                    XmlElement TxnDateRangeFilter = xmlDoc.CreateElement("TxnDateRangeFilter");
                    ReceivePaymentQueryRq.AppendChild(TxnDateRangeFilter);
                    XmlElement DateMacro = xmlDoc.CreateElement("FromTxnDate");
                    if (FechaJob > DateTime.Now.AddDays(-7))
                    {
                        int dias = Convert.ToInt32(Math.Round((DateTime.Now - FechaJob).TotalDays, 0));
                        TxnDateRangeFilter.AppendChild(DateMacro).InnerText = DateTime.Now.AddDays(-dias).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        TxnDateRangeFilter.AppendChild(DateMacro).InnerText = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                    }
                }
                XmlElement IncludeLineItems = xmlDoc.CreateElement("IncludeLineItems");
                ReceivePaymentQueryRq.AppendChild(IncludeLineItems).InnerText = "true";
                //XmlElement IncludeLinkedTxns = xmlDoc.CreateElement("IncludeLinkedTxns");
                //ReceivePaymentQueryRq.AppendChild(IncludeLinkedTxns).InnerText = "true";
                ReceivePaymentQueryRq.SetAttribute("requestID", "1");
                xml = xmlDoc.OuterXml;
                return xml;
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "Armar Query Customer", " Error-> " + ex.Message);
                return null;
            }            
        }

        private string buildItemServiceQueryRqXML(string itemId = null)
        {
            try
            {
                string xml = "";
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement qbXMLMsgsRq = buildRqEnvelope(xmlDoc, maxVersion);
                qbXMLMsgsRq.SetAttribute("onError", "stopOnError");
                XmlElement ReceivePaymentQueryRq = xmlDoc.CreateElement("ItemServiceQueryRq");
                qbXMLMsgsRq.AppendChild(ReceivePaymentQueryRq);
                XmlElement TxnIDElement = xmlDoc.CreateElement("ListID");
                ReceivePaymentQueryRq.AppendChild(TxnIDElement).InnerText = itemId;
                ReceivePaymentQueryRq.SetAttribute("requestID", "1");
                xml = xmlDoc.OuterXml;
                return xml;
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "Armar Query Service Item", " Error-> " + ex.Message);
                return null;
            }
        }

        #endregion

        #region Procesar y devolver entidades

        private List<QuickbooksClient> ParsearXMLClientes(string xml, bool isTicket, bool individualSearch)
        {
            try
            {
                List<QuickbooksClient> clientes = new List<QuickbooksClient>();
                QuickbooksClient cliente;
                if (xml != null)
                {
                    XDocument doc = XDocument.Parse(xml);
                    var data = doc.Root.Element("QBXMLMsgsRs").Element("CustomerQueryRs").Elements("CustomerRet").ToList();
                    foreach (XElement customer in data)
                    {
                        cliente = new QuickbooksClient();
                        try
                        {
                            if (customer.Elements("DataExtRet") != null)
                            {
                                if(customer.Elements("DataExtRet").Count() == 0)
                                {
                                    if (isTicket == false)
                                    {
                                        throw new Exception("Faltan datos del cliente : Debe colocar un tipo de identificación , el numero de identificación y los días de crédito");
                                    }
                                }
                                if(customer.Elements("DataExtRet").Where(c => c.Element("DataExtName").Value == "IdentificationType").Count() == 0)
                                {
                                    if (isTicket == false)
                                    {
                                        throw new Exception("Faltan datos del cliente : Debe colocar un tipo de identificación");
                                    }
                                }
                                if (customer.Elements("DataExtRet").Where(c => c.Element("DataExtName").Value == "Identification").Count() == 0)
                                {
                                    if (isTicket == false)
                                    {
                                        throw new Exception("Faltan datos del cliente : Debe colocar un numero de identificación");
                                    }
                                }
                                if (customer.Elements("DataExtRet").Where(c => c.Element("DataExtName").Value == "CreditDays").Count() == 0)
                                {
                                    cliente.CreditDays = 1;
                                }
                                foreach (XElement CustomFields in customer.Elements("DataExtRet").OrderByDescending(c => c.Element("DataExtName").Value))
                                {
                                    if (CustomFields.Element("DataExtName") != null)
                                    {
                                        if (CustomFields.Element("DataExtName").Value == "IdentificationType")
                                        {
                                            string valor = CustomFields.Element("DataExtValue").Value;
                                            switch (valor)
                                            {
                                                case "Cedula Fisica":
                                                    cliente.IdentificationType = IdentificacionTypeTipo.Cedula_Fisica;
                                                    break;
                                                case "Cedula Juridica":
                                                    cliente.IdentificationType = IdentificacionTypeTipo.Cedula_Juridica;
                                                    break;
                                                case "DIMEX":
                                                    cliente.IdentificationType = IdentificacionTypeTipo.DIMEX;
                                                    break;
                                                case "NITE":
                                                    cliente.IdentificationType = IdentificacionTypeTipo.NITE;
                                                    break;
                                                case "Pasaporte":
                                                    cliente.IdentificationType = IdentificacionTypeTipo.NoAsignada;
                                                    break;
                                                default:
                                                    if (isTicket == false)
                                                    {
                                                        throw new Exception("Faltan datos del cliente : Debe colocar un tipo de identificación valido , revise la lista de tipos de identificación");
                                                    }
                                                    else
                                                    {
                                                        cliente.IdentificationType = IdentificacionTypeTipo.Cedula_Fisica;
                                                    }
                                                    break;
                                            }
                                        }
                                        if (CustomFields.Element("DataExtName").Value == "Identification")
                                        {
                                            if (cliente.IdentificationType == IdentificacionTypeTipo.NoAsignada)
                                            {
                                                cliente.IdentificacionExtranjero = CustomFields.Element("DataExtValue").Value;                                                
                                            }
                                            else
                                            {
                                                cliente.Identification = CustomFields.Element("DataExtValue").Value;
                                            }
                                            if (CustomFields.Element("DataExtValue").Value == null || CustomFields.Element("DataExtValue").Value == string.Empty)
                                            {
                                                if (isTicket == false)
                                                {
                                                    throw new Exception("Faltan datos del cliente : Debe colocar un numero de identificación");
                                                }
                                            }
                                            else
                                            {
                                                if (cliente.IdentificationType != IdentificacionTypeTipo.NoAsignada)
                                                {                                                    
                                                    if(cliente.IdentificationType == IdentificacionTypeTipo.Cedula_Juridica)
                                                    {
                                                        if (CustomFields.Element("DataExtValue").Value.Length != 10)
                                                        {
                                                            throw new Exception("Faltan datos del cliente : El numero de identificación Jurídico debe ser de 10 caracteres");
                                                        }
                                                    }
                                                    if (cliente.IdentificationType == IdentificacionTypeTipo.Cedula_Fisica)
                                                    {
                                                        if (CustomFields.Element("DataExtValue").Value.Length != 9 || CustomFields.Element("DataExtValue").Value.StartsWith("0"))
                                                        {
                                                            throw new Exception("Faltan datos del cliente : El numero de identificación Física debe ser de 9 caracteres, y no puede comenzar con 0");
                                                        }
                                                    }
                                                    if (cliente.IdentificationType == IdentificacionTypeTipo.DIMEX)
                                                    {
                                                        if ((CustomFields.Element("DataExtValue").Value.Length < 11 && CustomFields.Element("DataExtValue").Value.Length > 12) || CustomFields.Element("DataExtValue").Value.StartsWith("0"))
                                                        {
                                                            throw new Exception("Faltan datos del cliente : El numero de identificación DIMEX debe tener entre 11 y 12 caracteres, y no puede comenzar con 0");
                                                        }
                                                    }
                                                    if (cliente.IdentificationType == IdentificacionTypeTipo.NITE)
                                                    {
                                                        if (CustomFields.Element("DataExtValue").Value.Length != 10)
                                                        {
                                                            throw new Exception("Faltan datos del cliente : El numero de identificación NITE debe ser de 10 caracteres");
                                                        }
                                                    }
                                                } 
                                                else
                                                {
                                                    if (CustomFields.Element("DataExtValue").Value.Length < 5)
                                                    {
                                                        throw new Exception("Faltan datos del cliente : El numero de identificación Extranjero debe tener al menos 5 caracteres");
                                                    }
                                                }
                                            }
                                        }
                                        if (CustomFields.Element("DataExtName").Value == "CreditDays")
                                        {
                                            int diasCredito = 1;
                                            try
                                            {
                                                diasCredito = int.Parse(CustomFields.Element("DataExtValue").Value);
                                            }
                                            catch
                                            {
                                                diasCredito = 1;
                                            }
                                            cliente.CreditDays = diasCredito;
                                        }
                                    }
                                    else
                                    {
                                        if (isTicket == false)
                                        {
                                            throw new Exception("Faltan datos del cliente : Debe colocar un tipo de identificación , el numero de identificación y los días de crédito");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if(isTicket == false)
                                {
                                    throw new Exception("Faltan datos del cliente : Debe colocar un tipo de identificación , el numero de identificación y los días de crédito");
                                }
                            }
                            cliente.Address = null;
                            if (customer.Element("BillAddress") != null)
                            {
                                if (customer.Element("Addr1") != null)
                                {
                                    cliente.Address = customer.Element("BillAddress").Element("Addr1").Value;
                                }
                            }
                            cliente.PhoneNumber = null;
                            if (customer.Element("Phone") != null)
                            {
                                cliente.PhoneNumber = customer.Element("Phone").Value;
                            }
                            cliente.Email = null;
                            if (customer.Element("Email") != null)
                            {
                                cliente.Email = customer.Element("Email").Value;
                                if (customer.Element("Email").Value == null || customer.Element("Email").Value == string.Empty)
                                {
                                    if (isTicket == false)
                                    {
                                        throw new Exception("Faltan datos del cliente : Debe colocar una dirección de email");
                                    }
                                }
                            }
                            else
                            {
                                if(isTicket == false)
                                {
                                    throw new Exception("Faltan datos del cliente : Debe colocar una dirección de email");
                                }
                            }
                            cliente.Name = null;
                            if (cliente.IdentificationType != IdentificacionTypeTipo.Cedula_Juridica)
                            {
                                if (customer.Element("FirstName") != null && customer.Element("LastName") != null)
                                {
                                    cliente.Name = customer.Element("FirstName").Value;
                                    cliente.LastName = customer.Element("LastName").Value;
                                    if ((customer.Element("FirstName").Value == null || customer.Element("LastName").Value == null) || (customer.Element("FirstName").Value == string.Empty || customer.Element("LastName").Value == string.Empty))
                                    {
                                        if (isTicket == false)
                                        {
                                            throw new Exception("Faltan datos del cliente : Debe colocar un nombre y apellido");
                                        }
                                    }
                                }
                                else
                                {
                                    if (isTicket == false)
                                    {
                                        throw new Exception("Faltan datos del cliente : Debe colocar un nombre y apellido");
                                    }
                                }
                            }
                            else
                            {
                                if (customer.Element("CompanyName") != null)
                                {
                                    cliente.Name = customer.Element("CompanyName").Value;
                                    if (customer.Element("CompanyName").Value == null || customer.Element("CompanyName").Value == string.Empty)
                                    {
                                        if (isTicket == false)
                                        {
                                            throw new Exception("Faltan datos del cliente : Debe colocar el nombre de la compañía");
                                        }
                                    }
                                }
                                else
                                {
                                    if (isTicket == false)
                                    {
                                        throw new Exception("Faltan datos del cliente : Debe colocar el nombre de la compañía");
                                    }
                                }
                            }
                            clientes.Add(cliente);
                        }
                        catch(Exception ex)
                        {
                            if(individualSearch == true)
                            {
                                throw ex;
                            }
                            // _eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLClientes", "Error -> " + ex.Message);
                            // _errorLog.NuevaLinea("XML Fallo cliente : " + customer.ToString());
                        }
                        
                    }
                }
                return clientes;
            }
            catch(Exception ex)
            {
                // _eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLClientes", "Error -> " + ex.Message);
                // _errorLog.NuevaLinea("XML Clientes : " + xml);
                throw ex;
            }            
        }

        private List<QuickbooksInvoice> ParsearXMLFacturasCredito(string xml, bool detalle = false)
        {
            try
            {
                List<QuickbooksInvoice> facturas = new List<QuickbooksInvoice>();
                QuickbooksInvoice factura;
                if (xml != null)
                {
                    XDocument doc = XDocument.Parse(xml);
                    var data = doc.Root.Element("QBXMLMsgsRs").Element("InvoiceQueryRs").Elements("InvoiceRet").ToList();
                    foreach (XElement invoice in data)
                    {
                        try
                        {
                            factura = new QuickbooksInvoice();
                            factura.invoiceID = invoice.Element("TxnID").Value;                            
                            if (invoice.Element("CustomerRef").Element("ListID") != null)
                            {
                                factura.ClientId = invoice.Element("CustomerRef").Element("ListID").Value;
                            }
                            else
                            {
                                throw new Exception("Debe especificar un cliente para la factura");
                            }
                            factura.NumeroReferencia = invoice.Element("RefNumber").Value;
                            factura.EditSequence = invoice.Element("EditSequence").Value;
                            // Credito
                            factura.ListPaymentType = new List<QuickbooksPaymentInvoce>();
                            factura.Tipo = TipoFactura.Credito;
                            if (detalle)
                            {
                                decimal SalesTax = Decimal.Parse(invoice.Element("SalesTaxPercentage").Value);
                                if (invoice.Element("CurrencyRef") != null)
                                {
                                    if (invoice.Element("CurrencyRef").Element("FullName") != null)
                                    {
                                        switch (invoice.Element("CurrencyRef").Element("FullName").Value)
                                        {
                                            case "Costa Rican Colon":
                                                // Costa Rican Colones
                                                factura.CodigoMoneda = CodigoMoneda.CRC;
                                                break;
                                            case "US Dollar":
                                                // American Dollars
                                                factura.CodigoMoneda = CodigoMoneda.USD;
                                                break;
                                        }
                                    }
                                }
                                if(invoice.Element("DueDate") != null)
                                {
                                    DateTime fechaVencimiento = DateTime.Parse(invoice.Element("DueDate").Value);
                                    //DateTime fechaCreacion = DateTime.Parse(invoice.Element("TimeCreated").Value);
                                    DateTime fechaCreacion = DateTime.Now;
                                    if (fechaVencimiento > fechaCreacion)
                                    {
                                        double diasCredito = (fechaVencimiento - fechaCreacion).TotalDays;
                                        factura.CreditTerm = (int) Decimal.Round((decimal) diasCredito,0,MidpointRounding.AwayFromZero);
                                        if(factura.CreditTerm <= 0)
                                        {
                                            factura.CreditTerm = 1;
                                        }
                                    }
                                    else
                                    {
                                        factura.CreditTerm = 1;
                                    }
                                }
                                else
                                {
                                    factura.CreditTerm = 0;
                                }
                                factura.InvoiceLines = new List<QuickbooksItemInvoice>();
                                decimal SubtotalFactura = 0;
                                decimal TotalImpuesto = 0;
                                int numLinea = 1;
                                foreach (XElement invoiceLine in invoice.Elements("InvoiceLineRet"))
                                {
                                    QuickbooksItemInvoice linea = new QuickbooksItemInvoice();
                                    if (invoiceLine.Element("ItemRef") != null)
                                    {
                                        if (invoiceLine.Element("ItemRef").Element("FullName") != null)
                                        {
                                            linea.Servicio = invoiceLine.Element("ItemRef").Element("FullName").Value;
                                            linea.ItemId = invoiceLine.Element("ItemRef").Element("ListID").Value;
                                        }
                                        else
                                        {
                                            throw new Exception("Linea " + numLinea + "- Debe especificar el código valido del servicio o producto, Recuerde que no es valido enviar lineas vacías");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Linea " + numLinea + "- Debe especificar el código valido del servicio o producto, Recuerde que no es valido enviar lineas vacías");
                                    }
                                    linea.ItemType = TipoItem.Producto;
                                    if (invoiceLine.Element("Desc") != null)
                                    {
                                        linea.Servicio = linea.Servicio + " " + invoiceLine.Element("Desc").Value;
                                    }
                                    else
                                    {
                                        throw new Exception("Linea " + numLinea + "- Debe especificar una descripción del servicio o producto");
                                    }
                                    if(invoiceLine.Element("Quantity") != null)
                                    {
                                        if (Decimal.Parse(invoiceLine.Element("Quantity").Value) > 0)
                                        {
                                            linea.Cantidad = Decimal.Parse(invoiceLine.Element("Quantity").Value);
                                        }
                                        else
                                        {
                                            throw new Exception("Linea " + numLinea + "- La cantidad del servicio o producto debe ser mayor a 0");
                                        }                                        
                                    }
                                    else
                                    {
                                        // linea.Cantidad = 1;
                                        throw new Exception("Linea " + numLinea + "- Debe especificar la cantidad de items del servicio o producto a facturar");
                                    }                                    
                                    linea.UnidadMedida = TicoPayDll.Services.UnidadMedidaType.Unidad;
                                    // linea.UnidadMedida = invoiceLine.Element("UnitOfMeasure").Value;
                                    if (invoiceLine.Element("Rate") != null)
                                    {                                        
                                        if (Decimal.Parse(invoiceLine.Element("Rate").Value) > 0)
                                        {
                                            linea.Precio = Decimal.Parse(invoiceLine.Element("Rate").Value);
                                        }
                                        else
                                        {
                                            throw new Exception("Linea " + numLinea + "- El precio del servicio o producto debe ser mayor a 0");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Linea " + numLinea + "- Debe especificar el precio del servicio o producto a facturar");
                                    }                                    
                                    linea.Descuento = 0;
                                    if (invoiceLine.Element("SalesTaxCodeRef").Element("FullName").Value != "Non")
                                    {
                                        decimal subTotal = Decimal.Round(linea.Precio * linea.Cantidad, 2);
                                        linea.TasaImpuesto = SalesTax;
                                        linea.Impuesto = Decimal.Round((SalesTax * subTotal) / 100, 2);
                                        linea.Total = Decimal.Round(subTotal + linea.Impuesto, 2);
                                        SubtotalFactura = Decimal.Round(SubtotalFactura + subTotal, 2);
                                        TotalImpuesto = Decimal.Round(TotalImpuesto + linea.Impuesto, 2);
                                    }
                                    else
                                    {
                                        linea.TasaImpuesto = 0;
                                        linea.Impuesto = 0;
                                        linea.Total = Decimal.Round(linea.Precio * linea.Cantidad, 2);
                                        SubtotalFactura = Decimal.Round(SubtotalFactura + linea.Total, 2);
                                        TotalImpuesto = Decimal.Round(TotalImpuesto + 0, 2);
                                    }                                    
                                    factura.InvoiceLines.Add(linea);
                                    numLinea++;
                                }
                                if (invoice.Element("Memo") != null)
                                {
                                    factura.GeneralObservation = invoice.Element("Memo").Value;
                                }
                            }
                            facturas.Add(factura);
                        }
                        catch (Exception ex)
                        {
                            if (detalle)
                            {
                                throw ex;
                            }
                            else
                            {
                                _eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLFacturasCredito", "Error -> " + ex.Message);
                                // _errorLog.NuevaLinea("XML Factura Credito Fallida : " + invoice.ToString());
                            }                            
                        }
                    }
                }
                return facturas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<QuickbooksInvoice> ParsearXMLFacturasContado(string xml, bool detalle = false)
        {
            try
            {
                List<QuickbooksInvoice> facturas = new List<QuickbooksInvoice>();
                QuickbooksInvoice factura;
                if (xml != null)
                {
                    XDocument doc = XDocument.Parse(xml);
                    var data = doc.Root.Element("QBXMLMsgsRs").Element("SalesReceiptQueryRs").Elements("SalesReceiptRet").ToList();
                    foreach (XElement invoice in data)
                    {
                        try
                        {
                            factura = new QuickbooksInvoice();
                            factura.invoiceID = invoice.Element("TxnID").Value;
                            if (invoice.Element("CustomerRef").Element("ListID") != null)
                            {
                                factura.ClientId = invoice.Element("CustomerRef").Element("ListID").Value;
                            }
                            else
                            {
                                throw new Exception("Debe especificar un cliente para la factura");
                            }
                            factura.NumeroReferencia = invoice.Element("RefNumber").Value;
                            factura.EditSequence = invoice.Element("EditSequence").Value;
                            if (detalle)
                            {
                                // Contado
                                factura.CreditTerm = 0;
                                factura.ListPaymentType = new List<QuickbooksPaymentInvoce>();
                                QuickbooksPaymentInvoce pago = new QuickbooksPaymentInvoce();
                                if (invoice.Element("CurrencyRef") != null)
                                {
                                    if (invoice.Element("CurrencyRef").Element("FullName") != null)
                                    {
                                        switch (invoice.Element("CurrencyRef").Element("FullName").Value)
                                        {
                                            case "Costa Rican Colon":
                                                // Costa Rican Colones
                                                factura.CodigoMoneda = CodigoMoneda.CRC;
                                                break;
                                            case "US Dollar":
                                                // American Dollars
                                                factura.CodigoMoneda = CodigoMoneda.USD;
                                                break;
                                        }
                                    }
                                }
                                if (invoice.Element("PaymentMethodRef") != null)
                                {
                                    switch (invoice.Element("PaymentMethodRef").Element("FullName").Value)
                                    {
                                        case "Cash":
                                            // Efectivo
                                            pago.TypePayment = 0;
                                            break;
                                        case "Check":
                                            // Cheque
                                            pago.TypePayment = 2;
                                            if (invoice.Element("CheckNumber") != null)
                                            {
                                                pago.Trans = invoice.Element("CheckNumber").Value;
                                            }
                                            break;
                                        case "E-Check":
                                            // Deposito o Transferencia 3
                                            pago.TypePayment = 3;
                                            if (invoice.Element("CheckNumber") != null)
                                            {
                                                pago.Trans = invoice.Element("CheckNumber").Value;
                                            }
                                            break;
                                        default:
                                            // Tarjeta de Débito o Crédito 1
                                            // Debit Card ,MasterCard , American Express , Discover, Visa
                                            pago.TypePayment = 1;
                                            if (invoice.Element("CheckNumber") != null)
                                            {
                                                pago.Trans = invoice.Element("CheckNumber").Value;
                                            }
                                            break;
                                    }
                                    pago.Balance = Decimal.Parse(invoice.Element("TotalAmount").Value);
                                    factura.ListPaymentType.Add(pago);
                                }
                                else
                                {
                                    pago.TypePayment = 0;
                                    pago.Balance = Decimal.Parse(invoice.Element("TotalAmount").Value);
                                    factura.ListPaymentType.Add(pago);
                                }
                                decimal SalesTax = Decimal.Parse(invoice.Element("SalesTaxPercentage").Value);
                                factura.InvoiceLines = new List<QuickbooksItemInvoice>();
                                decimal SubtotalFactura = 0;
                                decimal TotalImpuesto = 0;
                                int numLinea = 1;
                                foreach (XElement invoiceLine in invoice.Elements("SalesReceiptLineRet"))
                                {
                                    QuickbooksItemInvoice linea = new QuickbooksItemInvoice();
                                    if (invoiceLine.Element("ItemRef") != null)
                                    {
                                        if (invoiceLine.Element("ItemRef").Element("FullName") != null)
                                        {
                                            linea.Servicio = invoiceLine.Element("ItemRef").Element("FullName").Value;
                                            linea.ItemId = invoiceLine.Element("ItemRef").Element("ListID").Value;
                                        }
                                        else
                                        {
                                            throw new Exception("Linea " + numLinea + "- Debe especificar el código valido del servicio o producto, Recuerde que no es valido enviar lineas vacías");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Linea " + numLinea + "- Debe especificar el código valido del servicio o producto, Recuerde que no es valido enviar lineas vacías");
                                    }
                                    linea.ItemType = TipoItem.Producto;
                                    if (invoiceLine.Element("Desc") != null)
                                    {
                                        linea.Servicio = linea.Servicio + " " + invoiceLine.Element("Desc").Value;
                                    }
                                    else
                                    {
                                        throw new Exception("Linea " + numLinea + "- Debe especificar una descripción del servicio o producto");
                                    }
                                    if (invoiceLine.Element("Quantity") != null)
                                    {
                                        if (Decimal.Parse(invoiceLine.Element("Quantity").Value) > 0)
                                        {
                                            linea.Cantidad = Decimal.Parse(invoiceLine.Element("Quantity").Value);
                                        }
                                        else
                                        {
                                            throw new Exception("Linea " + numLinea + "- La cantidad del servicio o producto debe ser mayor a 0");
                                        }
                                    }
                                    else
                                    {
                                        // linea.Cantidad = 1;
                                        throw new Exception("Linea " + numLinea + "- Debe especificar la cantidad de items del servicio o producto a facturar");
                                    }                                    
                                    linea.UnidadMedida = TicoPayDll.Services.UnidadMedidaType.Unidad;
                                    // linea.UnidadMedida = invoiceLine.Element("UnitOfMeasure").Value;
                                    if (invoiceLine.Element("Rate") != null)
                                    {
                                        if (Decimal.Parse(invoiceLine.Element("Rate").Value) > 0)
                                        {
                                            linea.Precio = Decimal.Parse(invoiceLine.Element("Rate").Value);
                                        }
                                        else
                                        {
                                            throw new Exception("Linea " + numLinea + "- El precio del servicio o producto debe ser mayor a 0");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Linea " + numLinea + "- Debe especificar el precio del servicio o producto a facturar");
                                    }
                                    linea.Descuento = 0;
                                    if (invoiceLine.Element("SalesTaxCodeRef").Element("FullName").Value != "Non")
                                    {
                                        decimal subTotal = Decimal.Round(linea.Precio * linea.Cantidad, 2);
                                        linea.TasaImpuesto = SalesTax;
                                        linea.Impuesto = Decimal.Round((SalesTax * subTotal) / 100, 2);
                                        linea.Total = Decimal.Round(subTotal + linea.Impuesto, 2);
                                        SubtotalFactura = Decimal.Round(SubtotalFactura + subTotal, 2);
                                        TotalImpuesto = Decimal.Round(TotalImpuesto + linea.Impuesto, 2);
                                    }
                                    else
                                    {
                                        linea.TasaImpuesto = 0;
                                        linea.Impuesto = 0;
                                        linea.Total = Decimal.Round(linea.Precio * linea.Cantidad, 2);
                                        SubtotalFactura = Decimal.Round(SubtotalFactura + linea.Total, 2);
                                        TotalImpuesto = Decimal.Round(TotalImpuesto + 0, 2);
                                    }
                                    factura.InvoiceLines.Add(linea);
                                    numLinea++;
                                }
                                if (invoice.Element("Memo") != null)
                                {
                                    factura.GeneralObservation = invoice.Element("Memo").Value;
                                }
                            }
                            facturas.Add(factura);
                        }
                        catch(Exception ex)
                        {
                            if (detalle)
                            {
                                throw ex;
                            }
                            else
                            {
                                _eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLFacturasContado ", "Error -> " + ex.Message);
                                // _errorLog.NuevaLinea("XML Factura Contado Fallida: " + invoice.ToString());
                            }
                        }                        
                    }
                }
                return facturas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<QuickbooksNote> ParsearXMLNotas(string xml,bool detalle = false)
        {
            try
            {
                List<QuickbooksNote> notas = new List<QuickbooksNote>();
                QuickbooksNote nota;
                if (xml != null)
                {
                    XDocument doc = XDocument.Parse(xml);
                    var data = doc.Root.Element("QBXMLMsgsRs").Element("CreditMemoQueryRs").Elements("CreditMemoRet").ToList();
                    foreach (XElement note in data)
                    {
                        try
                        {
                            nota = new QuickbooksNote();
                            if (note.Element("LinkedTxn") != null)
                            {
                                if (note.Element("LinkedTxn").Element("TxnType") != null)
                                {
                                    if (note.Element("LinkedTxn").Element("TxnType").Value == "Invoice")
                                    {
                                        nota.CreditMemoId = note.Element("TxnID").Value;
                                        nota.NumeroReferencia = note.Element("RefNumber").Value;
                                        nota.EditSequence = note.Element("EditSequence").Value;
                                        nota.QbClientId = note.Element("CustomerRef").Element("ListID").Value;                                        
                                        if (detalle)
                                        {
                                            if (note.Element("LinkedTxn") != null)
                                            {
                                                if (note.Element("LinkedTxn").Element("TxnID") != null)
                                                {
                                                    nota.AffectedInvoiceId = note.Element("LinkedTxn").Element("TxnID").Value;
                                                }
                                                else
                                                {
                                                    throw new Exception("La nota debe afectar a una factura especifica");
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("La nota debe afectar a una factura especifica");
                                            }
                                        }
                                        else
                                        {
                                            nota.AffectedInvoiceId = note.Element("LinkedTxn").Element("TxnID").Value;
                                        }                                        
                                        if (detalle)
                                        {
                                            decimal SalesTax = Decimal.Parse(note.Element("SalesTaxPercentage").Value);
                                            decimal SubtotalNota = 0;
                                            decimal TotalImpuesto = 0;
                                            nota.NotesLines = new List<QuickbooksNoteLine>();
                                            int numLinea = 1;
                                            foreach (XElement noteLine in note.Elements("CreditMemoLineRet"))
                                            {
                                                QuickbooksNoteLine linea = new QuickbooksNoteLine();
                                                if (noteLine.Element("ItemRef") != null)
                                                {
                                                    if (noteLine.Element("ItemRef").Element("FullName").Value != null)
                                                    {
                                                        linea.Title = noteLine.Element("ItemRef").Element("FullName").Value;
                                                        linea.ItemId = noteLine.Element("ItemRef").Element("ListID").Value;
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Linea " + numLinea + "- Debe especificar el código valido del servicio o producto, Recuerde que no es valido enviar lineas vacías");
                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception("Linea " + numLinea + "- Debe especificar el código valido del servicio o producto, Recuerde que no es valido enviar lineas vacías");
                                                }                                                
                                                linea.ItemType = TipoItem.Producto;
                                                if (noteLine.Element("Desc") != null)
                                                {
                                                    linea.Title = linea.Title + " " + noteLine.Element("Desc").Value;
                                                }
                                                else
                                                {
                                                    throw new Exception("Linea " + numLinea + "- Debe especificar una descripción del servicio o producto");
                                                }
                                                if (noteLine.Element("Quantity") != null)
                                                {
                                                    if (Decimal.Parse(noteLine.Element("Quantity").Value) > 0)
                                                    {
                                                        linea.Quantity = Decimal.Parse(noteLine.Element("Quantity").Value);
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Linea " + numLinea + "- La cantidad del servicio o producto debe ser mayor a 0");
                                                    }
                                                }
                                                else
                                                {
                                                    // linea.Quantity = 1;
                                                    throw new Exception("Linea " + numLinea + "- Debe especificar la cantidad de items del servicio o producto a devolver");
                                                }
                                                linea.UnitMeasurement = UnidadMedidaType.Unidad;
                                                if (noteLine.Element("Rate") != null)
                                                {
                                                    if (Decimal.Parse(noteLine.Element("Rate").Value) > 0)
                                                    {
                                                        linea.PricePerUnit = Decimal.Parse(noteLine.Element("Rate").Value);
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Linea " + numLinea + "- El precio del servicio o producto debe ser mayor a 0");
                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception("Linea " + numLinea + "- Debe especificar el precio del servicio o producto a devolver");
                                                }
                                                
                                                linea.DiscountPercentage = 0;
                                                linea.UnitMeasurementOthers = null;
                                                if (noteLine.Element("SalesTaxCodeRef").Element("FullName").Value != "Non")
                                                {
                                                    decimal subTotal = Decimal.Round(linea.PricePerUnit * linea.Quantity, 2);
                                                    linea.TaxRate = SalesTax;
                                                    linea.TaxAmount = Decimal.Round((SalesTax * subTotal) / 100, 2);
                                                    linea.Total = Decimal.Round(subTotal + linea.TaxAmount, 2);
                                                    SubtotalNota = Decimal.Round(SubtotalNota + subTotal, 2);
                                                    TotalImpuesto = Decimal.Round(TotalImpuesto + linea.TaxAmount, 2);
                                                }
                                                else
                                                {
                                                    linea.TaxRate = 0;
                                                    linea.TaxAmount = 0;
                                                    linea.Total = Decimal.Round(linea.PricePerUnit * linea.Quantity, 2);
                                                    SubtotalNota = Decimal.Round(SubtotalNota + linea.Total, 2);
                                                    TotalImpuesto = Decimal.Round(TotalImpuesto + 0, 2);
                                                }
                                                nota.NotesLines.Add(linea);
                                            }
                                        }
                                        notas.Add(nota);
                                    }
                                }
                            }
                        } 
                        catch(Exception ex)
                        {
                            if (detalle)
                            {
                                throw ex;
                            }
                            _eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLNotas", "Error -> " + ex.Message);
                            // _errorLog.NuevaLinea("XML Nota Fallida : " + note.ToString());
                        }                        
                    }
                }
                return notas;
            }
            catch (Exception ex)
            {
                //_eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLNotas", "Error -> " + ex.Message);
                //_errorLog.NuevaLinea("XML Notas : " + xml);
                throw ex;
            }
        }

        private List<QuickbooksPayment> ParsearXMLPagos(string xml, bool detalle = false)
        {
            try
            {
                List<QuickbooksPayment> pagos = new List<QuickbooksPayment>();
                QuickbooksPayment pago;
                if (xml != null)
                {
                    XDocument doc = XDocument.Parse(xml);
                    var data = doc.Root.Element("QBXMLMsgsRs").Element("ReceivePaymentQueryRs").Elements("ReceivePaymentRet").ToList();
                    foreach (XElement payment in data)
                    {
                        try
                        {
                            pago = new QuickbooksPayment();
                            pago.PaymentId = payment.Element("TxnID").Value; ;
                            pago.NumeroControl = payment.Element("TxnNumber").Value; ;
                            pago.ClientId = payment.Element("CustomerRef").Element("ListID").Value;                            
                            if (detalle)
                            {
                                if (payment.Element("AppliedToTxnRet") != null)
                                {
                                    if (payment.Element("AppliedToTxnRet").Element("TxnID") != null)
                                    {
                                        pago.AffectedInvoiceId = payment.Element("AppliedToTxnRet").Element("TxnID").Value;
                                    }
                                    else
                                    {
                                        throw new Exception("El pago debe afectar a una factura especifica");
                                    }
                                }
                                else
                                {
                                    throw new Exception("El pago debe afectar a una factura especifica");
                                }
                            }
                            else
                            {
                                pago.AffectedInvoiceId = payment.Element("AppliedToTxnRet").Element("TxnID").Value;
                            }
                            if (detalle)
                            {                                                                
                                pago.ListPaymentType = new List<QuickbooksPaymentInvoce>();
                                QuickbooksPaymentInvoce datoPago = new QuickbooksPaymentInvoce();                               
                                if (payment.Element("PaymentMethodRef") != null)
                                {
                                    switch (payment.Element("PaymentMethodRef").Element("FullName").Value)
                                    {
                                        case "Cash":
                                            // Efectivo
                                            datoPago.TypePayment = 0;
                                            break;
                                        case "Efectivo":
                                            // Efectivo
                                            datoPago.TypePayment = 0;
                                            break;
                                        case "Check":
                                            // Cheque
                                            datoPago.TypePayment = 2;
                                            if (payment.Element("RefNumber") != null)
                                            {
                                                datoPago.Trans = payment.Element("RefNumber").Value;
                                            }
                                            break;
                                        case "Cheque":
                                            // Cheque
                                            datoPago.TypePayment = 2;
                                            if (payment.Element("RefNumber") != null)
                                            {
                                                datoPago.Trans = payment.Element("RefNumber").Value;
                                            }
                                            break;
                                        case "E-Check":
                                            // Deposito o Transferencia 3
                                            datoPago.TypePayment = 3;
                                            if (payment.Element("RefNumber") != null)
                                            {
                                                datoPago.Trans = payment.Element("RefNumber").Value;
                                            }
                                            break;
                                        case "Transferencia":
                                            // Deposito o Transferencia 3
                                            datoPago.TypePayment = 3;
                                            if (payment.Element("RefNumber") != null)
                                            {
                                                datoPago.Trans = payment.Element("RefNumber").Value;
                                            }
                                            break;
                                        default:
                                            // Tarjeta de Débito o Crédito 1
                                            // Debit Card ,MasterCard , American Express , Discover, Visa
                                            datoPago.TypePayment = 1;
                                            if (payment.Element("RefNumber") != null)
                                            {
                                                datoPago.Trans = payment.Element("RefNumber").Value;
                                            }
                                            break;
                                    }
                                    if (payment.Element("TotalAmount") != null)
                                    {
                                        if (Decimal.Parse(payment.Element("TotalAmount").Value) > 0)
                                        {
                                            datoPago.Balance = Decimal.Parse(payment.Element("TotalAmount").Value);
                                        }
                                        else
                                        {
                                            throw new Exception("El monto del pago debe ser mayor a 0");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("El monto del pago debe ser mayor a 0");
                                    }                                    
                                    pago.ListPaymentType.Add(datoPago);
                                }
                                else
                                {
                                    datoPago.TypePayment = 0;
                                    datoPago.Balance = Decimal.Parse(payment.Element("TotalAmount").Value);
                                    pago.ListPaymentType.Add(datoPago);
                                }
                            }
                            pagos.Add(pago);
                        }
                        catch (Exception ex)
                        {
                            if (detalle)
                            {
                                throw ex;
                            }
                            _eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLPagos", "Error -> " + ex.Message);
                            // _errorLog.NuevaLinea("XML Nota Fallida : " + payment.ToString());
                        }
                    }
                }
                return pagos;
            }
            catch (Exception ex)
            {
                //_eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLpagos", "Error -> " + ex.Message);
                //_errorLog.NuevaLinea("XML Pagos : " + xml);
                throw ex;
            }
        }

        private bool ParsearXMLServiceItem(string xml)
        {
            try
            {
                bool esServicio = false;
                if (xml != null)
                {
                    XDocument doc = XDocument.Parse(xml);
                    if(doc.Root.Element("QBXMLMsgsRs").Element("ItemServiceQueryRs") != null)
                    {
                        var data = doc.Root.Element("QBXMLMsgsRs").Element("ItemServiceQueryRs").Elements("ItemServiceRet").ToList();
                        foreach (XElement service in data)
                        {
                            try
                            {
                                if (service.Element("Name") != null)
                                {
                                    esServicio = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                // _eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLPagos", "Error -> " + ex.Message);
                                // _errorLog.NuevaLinea("XML Nota Fallida : " + payment.ToString());
                            }
                        }
                    }                    
                }
                return esServicio;
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseDesktop", "ParsearXMLServiceItems", "Error -> " + ex.Message);
                _errorLog.NuevaLinea("XML ServiceItem : " + xml);
                throw new Exception("Imposible definir si es servicio / " + ex.Message, ex);
            }
        }

        #endregion
    }
}
