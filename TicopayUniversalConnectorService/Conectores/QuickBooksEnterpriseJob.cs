using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Services;
using TicoPayDll.Taxes;
using TicopayUniversalConnectorService.ConexionesExternas.QuickBooks.Dto;
using TicopayUniversalConnectorService.ConexionTicopay;
using TicopayUniversalConnectorService.Contexto;
using TicopayUniversalConnectorService.Entities;
using TicopayUniversalConnectorService.Interfaces;
using TicopayUniversalConnectorService.Log;

namespace TicopayUniversalConnectorService.Conectores
{
    [DisallowConcurrentExecution]
    public class QuickBooksEnterpriseJob : IConector
    {
        public Tax BuscarImpuesto(Operacion operacion, Configuracion configuracion, string porcentajeImpuesto = null, string nombreImpuesto = null)
        {
            Ticopay _conexionTicopay = new Ticopay();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            if (porcentajeImpuesto == null && nombreImpuesto == null)
            {
                _eventos.Error("QuickBooksEnterpriseJob", "Buscar Impuesto", "No se ha especificado información del impuesto a buscar");
                return null;
            }
            try
            {
                string _token = _conexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                Tax[] impuestos = _conexionTicopay.BuscarImpuestos(_token);
                if (nombreImpuesto != null)
                {
                    return impuestos.Where(i => i.Name == nombreImpuesto).First();
                }
                if (porcentajeImpuesto != null)
                {
                    decimal rate = decimal.Parse(porcentajeImpuesto);
                    return impuestos.Where(i => i.Rate == rate).First();
                }
                return null;
            }
            catch
            {
                _eventos.Error("QuickBooksEnterpriseJob", "Buscar Impuesto", "Error al Buscar la lista de impuestos de Ticopay");
                return null;
            }
        }

        public CreateInvoice ElaborarFactura(Operacion operacion, Configuracion configuracion, Client cliente)
        {
            // Inicialización de Variables
            Ticopay ConexionTicopay = new Ticopay();
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            CreateInvoice factura = null;
            string[] argumentos = new string[] { };
            argumentos = configuracion.DatosConexion.Split(Convert.ToChar(30));
            // Entablo comunicación con QuickBooks Enterprise
            ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop ConexionQuickBooks =
                    new ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop("TicopayConnector", argumentos[0], "TicopayC1");
            try
            {
                List<QuickbooksInvoice> invoiceList;
                if(operacion.TipoDeOperacion == TipoOperacion.FacturaContado)
                {
                    // Busca la factura en caso de se de Contado
                    invoiceList = ConexionQuickBooks.BuscarfacturasContado(configuracion.FechaCreacion, operacion.IdDocumento);
                }
                else
                {
                    // Busca la factura en caso de ser de Crédito
                    invoiceList = ConexionQuickBooks.BuscarfacturasCredito(configuracion.FechaCreacion, operacion.IdDocumento);
                }
                QuickbooksInvoice invoice;
                if (invoiceList.Count > 0)
                {
                    invoice = invoiceList.First();
                }
                else
                {
                    return null;
                }
                foreach (QuickbooksItemInvoice item in invoice.InvoiceLines)
                {
                    bool esServicio = false;
                    esServicio = ConexionQuickBooks.EsServicio(item.ItemId);
                    if (esServicio == true)
                    {
                        item.ItemType = TipoItem.Servicio;
                    }
                }
                factura = new CreateInvoice();
                // Asigno el cliente a la factura
                factura.ClientId = cliente.Id;
                factura.CodigoMoneda = invoice.CodigoMoneda;
                factura.ExternalReferenceNumber = invoice.NumeroReferencia;
                // Descuentos generales
                //factura.DiscountGeneral = Decimal.Parse(myreader[1].ToString());
                //factura.TypeDiscountGeneral = 0;
                factura.InvoiceLines = new List<ItemInvoice>();
                decimal subTotal = 0;
                // Relleno los items de la factura
                foreach (QuickbooksItemInvoice item in invoice.InvoiceLines)
                {
                    ItemInvoice lineaFactura = new ItemInvoice();
                    lineaFactura.IdService = null;
                    lineaFactura.Servicio = item.Servicio;
                    lineaFactura.Cantidad = item.Cantidad;
                    lineaFactura.Precio = item.Precio;
                    subTotal = Math.Round(lineaFactura.Cantidad * lineaFactura.Precio,2);
                    Tax impuesto = new Tax();
                    impuesto = this.BuscarImpuesto(operacion, configuracion, item.TasaImpuesto.ToString(), null);
                    lineaFactura.IdImpuesto = impuesto.Id;
                    lineaFactura.Descuento = item.Descuento;
                    lineaFactura.Impuesto = Math.Round((impuesto.Rate * subTotal) / 100,2);
                    lineaFactura.Total = Math.Round(lineaFactura.Impuesto + subTotal,2);
                    lineaFactura.UnidadMedida = UnidadMedidaType.Unidad;
                    if(item.ItemType == TipoItem.Servicio)
                    {
                        lineaFactura.Tipo = LineType.Service;
                    }
                    else
                    {
                        lineaFactura.Tipo = LineType.Product;
                    }
                    factura.InvoiceLines.Add(lineaFactura);
                }     
                factura.ListPaymentType = new List<PaymentInvoce>();
                // Si la factura es crédito
                if(operacion.TipoDeOperacion == TipoOperacion.FacturaCredito)
                {
                    if(invoice.CreditTerm == 0)
                    {
                        if(cliente.CreditDays > 0)
                        {
                            factura.CreditTerm = cliente.CreditDays;
                        }
                        else
                        {
                            factura.CreditTerm = 1;
                        }
                    }
                    else
                    {
                        factura.CreditTerm = invoice.CreditTerm;
                    }                    
                }
                else
                {
                    // Si es Contado obtengo los pagos
                    factura.CreditTerm = 0;
                    foreach (QuickbooksPaymentInvoce payment in invoice.ListPaymentType)
                    {
                        PaymentInvoce pago = new PaymentInvoce();
                        pago.Balance = payment.Balance;
                        pago.Trans = payment.Trans;
                        pago.TypePayment = payment.TypePayment;
                        factura.ListPaymentType.Add(pago);
                    }
                }               
                factura.TipoFirma = FirmType.Llave;
                if(invoice.GeneralObservation != null)
                {
                    factura.GeneralObservation = invoice.GeneralObservation;
                }
                return factura;
            }
            catch (Exception x)
            {
                //_eventos.Error("QuickBooksEnterpriseJob", "Elaborar factura", "Imposible obtener los datos de la factura de QuickBooks Enterprise");
                throw x;
            }
            finally
            {
                factura = null;
            }
        }

        public CompleteNote ElaborarNotaCreditoDevolucion(Operacion operacion, Configuracion configuracion)
        {
            // Inicialización de Variables
            Ticopay _conexionTicopay = new Ticopay();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            string[] argumentos = new string[] { };
            argumentos = configuracion.DatosConexion.Split(Convert.ToChar(30));
            // Entablo comunicación con QuickBooks Enterprise
            ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop ConexionQuickBooks =
                    new ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop("TicopayConnector", argumentos[0], "TicopayC1");
            CompleteNote nota = new CompleteNote();
            try
            {                
                // Busca los datos de la Factura a aplicar la devolución en OPREREFERENCIAS                
                Invoice facturaCreada = null;                
                string IdFacturaAReversar = _contexto.Operaciones.Where(faR => faR.IdEmpresa == operacion.IdEmpresa && faR.IdDocumento == operacion.IdDocumentoAfectado &&
                           faR.TipoDeOperacion == TipoOperacion.FacturaCredito && faR.Configuracion.TipoConector == operacion.Configuracion.TipoConector).First().IdTicopayDocument;
                string _token = null;
                try
                {
                    _token = _conexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                    facturaCreada = _conexionTicopay.BuscarFactura(IdFacturaAReversar, _token);
                }
                catch (Exception ex)
                {
                    _eventos.Error("QuickBooksEnterpriseJob", "Elaborar Nota Crédito Devolución", "Imposible obtener factura a aplicar devolución de Ticopay");
                    throw new Exception("Imposible obtener factura a aplicar devolución desde Ticopay");
                }
                if (facturaCreada != null)
                {
                    nota.ClientId = facturaCreada.ClientId;
                    nota.InvoiceId = Guid.Parse(facturaCreada.Id);
                    nota.NumberInvoiceRef = facturaCreada.ConsecutiveNumber;
                    nota.ClientName = facturaCreada.Client.Name;
                    nota.ExternalReferenceNumber = operacion.ConsecutivoConector;
                    // Busca las lineas o Servicios que contiene la devolución
                    nota.NotesLines = new List<NoteLineDto>();
                    NoteLineDto lineaNotaDetalle;
                    int numeroLinea = 1;
                    decimal subTotalNota = 0;
                    decimal subTotalLinea = 0;
                    decimal impuestoTotalNota = 0;
                    decimal impuestoTotalLinea = 0;
                    decimal totalConImpuestoNota = 0;
                    decimal totalConImpuestoLinea = 0;
                    Tax impuesto = null;
                    QuickbooksNote note;
                    List<QuickbooksNote> noteList = ConexionQuickBooks.BuscarNotas(configuracion.FechaCreacion, operacion.IdDocumento);
                    if (noteList.Count > 0)
                    {
                        note = noteList.First();
                    }
                    else
                    {
                        return null;
                    }
                    foreach (QuickbooksNoteLine item in note.NotesLines)
                    {
                        bool esServicio = false;
                        esServicio = ConexionQuickBooks.EsServicio(item.ItemId);
                        if (esServicio == true)
                        {
                            item.ItemType = TipoItem.Servicio;
                        }
                    }
                    foreach (QuickbooksNoteLine noteLine in note.NotesLines)
                    {
                        lineaNotaDetalle = new NoteLineDto();
                        lineaNotaDetalle.PricePerUnit = noteLine.PricePerUnit;
                        lineaNotaDetalle.Quantity = noteLine.Quantity;
                        impuesto = BuscarImpuesto(operacion, configuracion, noteLine.TaxRate.ToString(), null);
                        subTotalLinea = Decimal.Round(lineaNotaDetalle.PricePerUnit * lineaNotaDetalle.Quantity, 2);
                        lineaNotaDetalle.SubTotal = subTotalLinea;
                        subTotalNota = Decimal.Round(subTotalNota + subTotalLinea, 2);
                        impuestoTotalLinea = Decimal.Round((subTotalLinea * impuesto.Rate) / 100, 2);
                        lineaNotaDetalle.TaxAmount = impuestoTotalLinea;
                        impuestoTotalNota = Decimal.Round(impuestoTotalNota + impuestoTotalLinea, 2);
                        totalConImpuestoLinea = Decimal.Round(subTotalLinea + impuestoTotalLinea, 2);
                        lineaNotaDetalle.Total = totalConImpuestoLinea;
                        lineaNotaDetalle.LineTotal = totalConImpuestoLinea;
                        totalConImpuestoNota = Decimal.Round(totalConImpuestoNota + totalConImpuestoLinea, 2);
                        lineaNotaDetalle.DiscountPercentage = 0;
                        lineaNotaDetalle.Note = null;
                        if (noteLine.Note != null && noteLine.Note.Length > 1)
                        {
                            lineaNotaDetalle.Note = noteLine.Note;
                        }
                        lineaNotaDetalle.Title = noteLine.Title;
                        if(noteLine.ItemType == TipoItem.Producto)
                        {
                            lineaNotaDetalle.LineType = LineType.Product;
                        }
                        else
                        {
                            lineaNotaDetalle.LineType = LineType.Service;
                        }                        
                        lineaNotaDetalle.ServiceId = null;
                        lineaNotaDetalle.ProductId = null;
                        lineaNotaDetalle.LineNumber = numeroLinea;
                        lineaNotaDetalle.CodeTypes = TicoPayDll.Invoices.CodigoTypeTipo.Otros;
                        lineaNotaDetalle.DescriptionDiscount = null;
                        lineaNotaDetalle.ExonerationId = null;
                        lineaNotaDetalle.Service = null;
                        lineaNotaDetalle.Tax = null;
                        lineaNotaDetalle.TaxId = impuesto.Id;
                        lineaNotaDetalle.UnitMeasurement = TicoPayDll.Services.UnidadMedidaType.Unidad;
                        lineaNotaDetalle.UnitMeasurementOthers = null;
                        nota.NotesLines.Add(lineaNotaDetalle);
                        numeroLinea++;
                    }
                    // Totales de la Nota
                    nota.Amount = subTotalNota;
                    nota.TaxAmount = impuestoTotalNota;
                    nota.DiscountAmount = 0;
                    nota.Total = totalConImpuestoNota;
                    // Moneda
                    nota.CodigoMoneda = facturaCreada.CodigoMoneda;
                    // Tipo de nota
                    nota.NoteType = NoteType.Crédito;
                    nota.NoteReasons = NoteReason.Corregir_Monto_Factura;
                    nota.NoteReasonsOthers = null;
                    // Tipo de Firma 
                    nota.TipoFirma = facturaCreada.TipoFirma;
                    // Data para rellenar
                    nota.ConsecutiveNumber = null;
                    nota.SendInvoice = false;
                    nota.StatusTribunet = StatusTaxAdministration.NoEnviada;
                    nota.IsNoteReceptionConfirmed = false;
                    nota.StatusFirmaDigital = null;
                    nota.ValidateHacienda = false;
                    nota.Status = Status.Completed;
                    nota.DueDate = facturaCreada.DueDate;
                    nota.CreditTerm = 0;
                    nota.ConditionSaleType = TicoPayDll.Notes.FacturaElectronicaCondicionVenta.Contado;
                    return nota;
                }
                else
                {
                    // _eventos.Error("QuickBooksEnterpriseJob", "Elaborar Nota Crédito Devolución", "La factura a aplicarle la devolución no fue encontrada");
                    throw new Exception("La factura a aplicarle la devolución no fue encontrada");
                }                
            }
            catch (Exception x)
            {
                // _eventos.Error("QuickBooksEnterpriseJob", "Elaborar Nota Crédito Devolución", "Error al Elaborar la Nota con la Data de QuickBooks Enterprise");
                throw x;
            }
        }

        public CompleteNote ElaborarNotaCreditoReverso(Operacion operacion, Configuracion configuracion)
        {
            throw new NotImplementedException();
        }

        public CompleteNote ElaborarNotaDebito(Operacion operacion, Configuracion configuracion)
        {
            throw new NotImplementedException();
        }

        public CreateInvoice ElaborarTiquete(Operacion operacion, Configuracion configuracion, Client cliente)
        {
            // Inicialización de Variables
            Ticopay ConexionTicopay = new Ticopay();
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            CreateInvoice factura = null;
            string[] argumentos = new string[] { };
            argumentos = configuracion.DatosConexion.Split(Convert.ToChar(30));
            // Entablo comunicación con QuickBooks Enterprise
            ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop ConexionQuickBooks =
                    new ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop("TicopayConnector", argumentos[0], "TicopayC1");
            try
            {
                List<QuickbooksInvoice> invoiceList;                
                invoiceList = ConexionQuickBooks.BuscarfacturasContado(configuracion.FechaCreacion, operacion.IdDocumento);
                QuickbooksInvoice invoice;
                if (invoiceList.Count > 0)
                {
                    invoice = invoiceList.First();
                }
                else
                {
                    return null;
                }
                foreach (QuickbooksItemInvoice item in invoice.InvoiceLines)
                {
                    bool esServicio = false;
                    esServicio = ConexionQuickBooks.EsServicio(item.ItemId);
                    if (esServicio == true)
                    {
                        item.ItemType = TipoItem.Servicio;
                    }
                }
                factura = new CreateInvoice();
                // Asigno datos del cliente al tiquete
                factura.ClientIdentificationType = cliente.IdentificationType;
                if (cliente.Identification != null)
                {
                    factura.ClientIdentification = cliente.Identification;
                }                                
                if(cliente.Name != null)
                {
                    factura.ClientName = cliente.Name;
                    if(cliente.LastName != null)
                    {
                        factura.ClientName = factura.ClientName + cliente.LastName;
                    }
                }
                if(cliente.Email != null)
                {
                    factura.ClientEmail = cliente.Email;
                }
                if(cliente.PhoneNumber != null)
                {
                    factura.ClientPhoneNumber = cliente.PhoneNumber;
                }                
                factura.CodigoMoneda = invoice.CodigoMoneda;
                factura.ExternalReferenceNumber = invoice.NumeroReferencia;
                // Descuentos generales
                //factura.DiscountGeneral = Decimal.Parse(myreader[1].ToString());
                //factura.TypeDiscountGeneral = 0;
                factura.InvoiceLines = new List<ItemInvoice>();
                decimal subTotal = 0;
                // Relleno los items de la factura
                foreach (QuickbooksItemInvoice item in invoice.InvoiceLines)
                {
                    ItemInvoice lineaFactura = new ItemInvoice();
                    lineaFactura.IdService = null;
                    lineaFactura.Servicio = item.Servicio;
                    lineaFactura.Cantidad = item.Cantidad;
                    lineaFactura.Precio = item.Precio;
                    subTotal = Math.Round(lineaFactura.Cantidad * lineaFactura.Precio,2);
                    Tax impuesto = new Tax();
                    impuesto = this.BuscarImpuesto(operacion, configuracion, item.TasaImpuesto.ToString(), null);
                    lineaFactura.IdImpuesto = impuesto.Id;
                    lineaFactura.Descuento = item.Descuento;
                    lineaFactura.Impuesto = Math.Round((impuesto.Rate * subTotal) / 100,2);
                    lineaFactura.Total = Math.Round(lineaFactura.Impuesto + subTotal,2);
                    lineaFactura.UnidadMedida = UnidadMedidaType.Unidad;
                    if (item.ItemType == TipoItem.Servicio)
                    {
                        lineaFactura.Tipo = LineType.Service;
                    }
                    else
                    {
                        lineaFactura.Tipo = LineType.Product;
                    }
                    factura.InvoiceLines.Add(lineaFactura);
                }
                factura.ListPaymentType = new List<PaymentInvoce>();
                // Si la factura es crédito
                if (operacion.TipoDeOperacion == TipoOperacion.FacturaCredito)
                {
                    factura.CreditTerm = invoice.CreditTerm;
                }
                else
                {
                    // Si es Contado obtengo los pagos
                    factura.CreditTerm = 0;
                    foreach (QuickbooksPaymentInvoce payment in invoice.ListPaymentType)
                    {
                        PaymentInvoce pago = new PaymentInvoce();
                        pago.Balance = payment.Balance;
                        pago.Trans = payment.Trans;
                        pago.TypePayment = payment.TypePayment;
                        factura.ListPaymentType.Add(pago);
                    }
                }
                if (invoice.GeneralObservation != null)
                {
                    factura.GeneralObservation = invoice.GeneralObservation;
                }
                factura.TipoFirma = FirmType.Llave;
                factura.TypeDocument = DocumentType.Ticket;
                return factura;
            }
            catch (Exception x)
            {
                // _eventos.Error("QuickBooksEnterpriseJob", "Elaborar factura", "Imposible obtener los datos de la factura de QuickBooks Enterprise");
                throw x;
            }
            finally
            {
                factura = null;
            }
        }

        public Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            bool _ticket = false;
            // Obtengo Id de la Configuración del Job
            string parametros = dataMap.GetString("IdConfiguracion");
            Guid _idConfiguracion = Guid.Parse(parametros);
            Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == _idConfiguracion).Single();
            if (Job == null)
            {
                _eventos.Error("QuickBooksEnterprise", "Ejecutar", "Imposible obtener configuración del Job");
                return Task.CompletedTask;
            }
            string[] argumentos = new string[] { };
            argumentos = Job.DatosConexion.Split(Convert.ToChar(30));
            if(argumentos[1] == "true")
            {
                _ticket = true;
            }
            // Entablo comunicación con QuickBooks Enterprise
            ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop ConexionQuickBooks =
                    new ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop("TicopayConnector", argumentos[0], "TicopayConnector");
            try
            {                
                // Extraigo las facturas de Contado
                List<QuickbooksInvoice> facturasContado = new List<QuickbooksInvoice>();
                facturasContado = ConexionQuickBooks.BuscarfacturasContado(Job.FechaCreacion);
                foreach (QuickbooksInvoice factura in facturasContado)
                {
                    if (_ticket)
                    {
                        if (_contexto.Operaciones.Where(t => t.IdDocumento == factura.invoiceID && t.IdEmpresa == Job.IdEmpresa && t.TipoDeOperacion == TipoOperacion.Ticket && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                        {
                            Operacion oFactura = new Operacion(factura.invoiceID, factura.ClientId, Job.IdEmpresa, TipoOperacion.Ticket, Job);
                            oFactura.ConsecutivoConector = factura.NumeroReferencia;
                            _contexto.Operaciones.Add(oFactura);
                            _contexto.SaveChanges();
                        }
                    }
                    else
                    {
                        if (_contexto.Operaciones.Where(t => t.IdDocumento == factura.invoiceID && t.IdEmpresa == Job.IdEmpresa && t.TipoDeOperacion == TipoOperacion.FacturaContado && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                        {
                            Operacion oFactura = new Operacion(factura.invoiceID, factura.ClientId, Job.IdEmpresa, TipoOperacion.FacturaContado, Job);
                            oFactura.ConsecutivoConector = factura.NumeroReferencia;
                            _contexto.Operaciones.Add(oFactura);
                            _contexto.SaveChanges();
                        }
                    }
                                      
                }
                facturasContado.Clear();
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseJob", "Ejecutar", "Error al extraer la información de las Facturas de Contado -> " + ex.Message);
            }
            try
            {
                // Extraigo las facturas de Crédito
                List<QuickbooksInvoice> facturasCredito = new List<QuickbooksInvoice>();
                facturasCredito = ConexionQuickBooks.BuscarfacturasCredito(Job.FechaCreacion);
                foreach (QuickbooksInvoice factura in facturasCredito)
                {
                    if (_contexto.Operaciones.Where(t => t.IdDocumento == factura.invoiceID && t.IdEmpresa == Job.IdEmpresa && t.TipoDeOperacion == TipoOperacion.FacturaCredito && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                    {
                        Operacion oFactura = new Operacion(factura.invoiceID, factura.ClientId, Job.IdEmpresa, TipoOperacion.FacturaCredito, Job);
                        oFactura.ConsecutivoConector = factura.NumeroReferencia;
                        _contexto.Operaciones.Add(oFactura);
                        _contexto.SaveChanges();
                    }                    
                }
                facturasCredito.Clear();
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseJob", "Ejecutar", "Error al extraer la información de las Facturas de Crédito -> " + ex.Message);
            }
            try
            {
                // Extraigo las notas de Crédito
                List<QuickbooksNote> notasCredito = new List<QuickbooksNote>();
                notasCredito = ConexionQuickBooks.BuscarNotas(Job.FechaCreacion);
                foreach (QuickbooksNote nota in notasCredito)
                {
                    if (_contexto.Operaciones.Where(t => t.IdDocumento == nota.CreditMemoId && t.IdEmpresa == Job.IdEmpresa && t.TipoDeOperacion == TipoOperacion.DevolucionFactura && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                    {
                        Operacion oNota = new Operacion(nota.CreditMemoId, nota.QbClientId, Job.IdEmpresa, TipoOperacion.DevolucionFactura, Job, nota.AffectedInvoiceId);
                        oNota.ConsecutivoConector = nota.NumeroReferencia;
                        _contexto.Operaciones.Add(oNota);
                        _contexto.SaveChanges();
                    }                    
                }
                notasCredito.Clear();
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseJob", "Ejecutar", "Error al extraer la información de las Notas de Crédito -> " + ex.Message);
            }
            try
            {
                // Extraigo los Pagos de facturas de crédito
                List<QuickbooksPayment> pagos = new List<QuickbooksPayment>();
                pagos = ConexionQuickBooks.BuscarPagos(Job.FechaCreacion);
                foreach (QuickbooksPayment pago in pagos)
                {
                    if (_contexto.Operaciones.Where(t => t.IdDocumento == pago.PaymentId && t.IdEmpresa == Job.IdEmpresa && t.TipoDeOperacion == TipoOperacion.PagoFactura && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                    {
                        if(_contexto.Operaciones.Where(O => O.EstadoOperacion == Estado.Procesado && O.TipoDeOperacion == TipoOperacion.FacturaCredito && O.IdDocumento == pago.AffectedInvoiceId).ToList().Count > 0)
                        {
                            Operacion oPago = new Operacion(pago.PaymentId, pago.ClientId, Job.IdEmpresa, TipoOperacion.PagoFactura, Job);
                            oPago.ConsecutivoConector = pago.NumeroControl;
                            _contexto.Operaciones.Add(oPago);
                            _contexto.SaveChanges();
                        }                        
                    }
                }
                pagos.Clear();
            }
            catch (Exception ex)
            {
                _eventos.Error("QuickBooksEnterpriseJob", "Ejecutar", "Error al extraer la información de los Pagos -> " + ex.Message);
            }       
            List<Operacion> OperacionesAProcesar = _contexto.Operaciones.Where(O => O.EstadoOperacion == Estado.NoProcesado || O.EstadoOperacion == Estado.Error).OrderBy(O => O.IdOperacion).ToList();
            List<Operacion> OperacionesDelConector = OperacionesAProcesar.Where(O => O.Configuracion.Id == Job.Id).OrderBy(O => O.IdOperacion).ToList();
            if (OperacionesDelConector.Count > 0)
            {
                ProcesarOperaciones(OperacionesDelConector, Job);
            }
            else
            {
                _eventos.Advertencia("QuickBooksEnterpriseJob", "Ejecutar", "No existen Operaciones por Procesar");
            }
            _contexto.Dispose();
            return Task.CompletedTask;
        }

        public Client IngresarCliente(Operacion operacion, Configuracion configuracion)
        {
            // Inicialización de Variables
            Ticopay _conexionTicopay = new Ticopay();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            string[] argumentos = new string[] { };
            argumentos = configuracion.DatosConexion.Split(Convert.ToChar(30));
            // Entablo comunicación con QuickBooks Enterprise
            ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop ConexionQuickBooks =
                    new ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop("TicopayConnector", argumentos[0], "TicopayC1");
            Client cliente = null;            
            try
            {
                QuickbooksClient clienteQB;
                if (operacion.TipoDeOperacion == TipoOperacion.Ticket)
                {
                    clienteQB = ConexionQuickBooks.BuscarCliente(operacion.IdCliente,true);
                }
                else
                {
                    clienteQB = ConexionQuickBooks.BuscarCliente(operacion.IdCliente);
                }
                                    
                try
                {
                    // Inicio sesion en Ticopay
                    string _token = _conexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                    if (_token == null)
                    {
                        // _eventos.Error("QuickBooksEnterpriseJob", "Ingresar Cliente", "Imposible conectar con Ticopay");
                        throw new Exception("Ingresar Cliente: Imposible conectar con Ticopay ");
                    }
                    if(operacion.TipoDeOperacion == TipoOperacion.Ticket)
                    {
                        // Se recopilan los datos existentes para el tiquete
                        cliente = new Client();
                        cliente.IdentificationType = clienteQB.IdentificationType;
                        if(clienteQB.IdentificationType == IdentificacionTypeTipo.NoAsignada)
                        {
                            cliente.Identification = clienteQB.IdentificacionExtranjero;
                        }
                        else
                        {
                            cliente.Identification = clienteQB.Identification;
                        }
                        cliente.Name = clienteQB.Name;
                        cliente.Email = clienteQB.Email;
                        cliente.LastName = clienteQB.LastName;
                        cliente.CreditDays = clienteQB.CreditDays;
                        cliente.PhoneNumber = clienteQB.PhoneNumber;
                        return cliente;
                    }
                    else
                    {
                        // Busco si el Cliente ya existe en Ticopay
                        cliente = _conexionTicopay.BuscarCliente(_token, true, clienteQB.Identification);
                        if (cliente != null)
                        {
                            return cliente;
                        }
                        // Si no existe procedo a crearlo
                        cliente = new Client();
                        cliente.Identification = clienteQB.Identification;
                        cliente.IdentificationType = clienteQB.IdentificationType;
                        cliente.IdentificacionExtranjero = clienteQB.IdentificacionExtranjero;
                        cliente.Name = clienteQB.Name;
                        cliente.Email = clienteQB.Email;
                        cliente.LastName = clienteQB.LastName;
                        cliente.Address = clienteQB.Address;
                        cliente.CreditDays = clienteQB.CreditDays;
                        cliente.PhoneNumber = clienteQB.PhoneNumber;
                        cliente = _conexionTicopay.CrearCliente(_token, cliente);
                        if (cliente != null)
                        {
                            return cliente;
                        }
                        else
                        {
                            // _eventos.Error("QuickBooksEnterpriseJob", "Ingresar Cliente", "Imposible Crear el Cliente en Ticopay");
                            throw new Exception("Imposible crear Cliente en Ticopays: ");
                        }
                    }                    
                }
                catch (Exception x)
                {
                    // _eventos.Error("QuickBooksEnterpriseJob", "Ingresar Cliente", "Error al buscar o crear el cliente en Ticopay");
                    throw x;
                }
            }
            catch(Exception ex)
            {
                // _eventos.Error("QuickBooksEnterpriseJob", "Ingresar Cliente", "Imposible obtener los datos del cliente de QuickBooks Enterprise");
                throw ex;
            }
            
        }

        public void ProcesarOperaciones(List<Operacion> operacionesPendientes, Configuracion configuracion)
        {
            // Inicialización de Variables
            Ticopay ConexionTicopay = new Ticopay();
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            RegistroDeEventos _eventos = new RegistroDeEventos();

            if (ConexionTicopay.VerificarPermisoConector(configuracion.SubDominioTicopay, configuracion.TipoConector.ToString()) == false)
            {
                _eventos.Error("QuickBooksEnterpriseJob", "Procesar Operaciones", "Sub Dominio no tiene permiso de usar el Conector");
                return;
            }
            string token = null;
            try
            {
                foreach (Operacion operacionAProcesar in operacionesPendientes)
                {
                    Client cliente = null;
                    CreateInvoice factura = null;
                    Invoice facturaEnviada = null;
                    CompleteNote nota = null;
                    CompleteNote notaEnviada = null;
                    Invoice facturaCredito = null;
                    QuickbooksPayment pago = null;
                    Invoice pagoEnviado = null;
                    switch (operacionAProcesar.TipoDeOperacion)
                    {
                        case TipoOperacion.FacturaContado:
                            try
                            {
                                cliente = IngresarCliente(operacionAProcesar, configuracion);
                                if (cliente != null)
                                {
                                    factura = ElaborarFactura(operacionAProcesar, configuracion, cliente);
                                    token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                                    facturaEnviada = ConexionTicopay.EnviarFactura(token, factura);
                                }
                                if (facturaEnviada != null)
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                    operacionAProcesar.IdTicopayDocument = facturaEnviada.Id;
                                    operacionAProcesar.ConsecutivoTicopay = facturaEnviada.ConsecutiveNumber;
                                    operacionAProcesar.VoucherTicopay = facturaEnviada.VoucherKey;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                    try
                                    {
                                        ActualizarFactura(operacionAProcesar, configuracion, facturaEnviada.ConsecutiveNumber);
                                    }
                                    catch (Exception ex)
                                    {
                                        _eventos.Error("QuickBooksEnterpriseJob", "Actualizar Factura de Contado", "Imposible Actualizar el Numero de Consecutivo en Quickbooks");
                                    }
                                }
                                else
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Error;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                            }
                            catch (Exception ex)
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Error;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                                operacionAProcesar.Errores = ex.Message;
                            }                            
                            break;
                        case TipoOperacion.FacturaCredito:
                            try
                            {
                                cliente = IngresarCliente(operacionAProcesar, configuracion);
                                if (cliente != null)
                                {
                                    factura = ElaborarFactura(operacionAProcesar, configuracion, cliente);
                                    token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                                    facturaEnviada = ConexionTicopay.EnviarFactura(token, factura);
                                }
                                if (facturaEnviada != null)
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                    operacionAProcesar.IdTicopayDocument = facturaEnviada.Id;
                                    operacionAProcesar.ConsecutivoTicopay = facturaEnviada.ConsecutiveNumber;
                                    operacionAProcesar.VoucherTicopay = facturaEnviada.VoucherKey;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                    try
                                    {
                                        ActualizarFactura(operacionAProcesar, configuracion, facturaEnviada.ConsecutiveNumber);
                                    }
                                    catch (Exception ex)
                                    {
                                        _eventos.Error("QuickBooksEnterpriseJob", "Actualizar Factura de Credito", "Imposible Actualizar el Numero de Consecutivo en Quickbooks");
                                    }
                                }
                                else
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Error;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                            }
                            catch (Exception ex)
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Error;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                                operacionAProcesar.Errores = ex.Message;
                            }
                            
                            break;
                        case TipoOperacion.DevolucionFactura:
                            try
                            {
                                nota = ElaborarNotaCreditoDevolucion(operacionAProcesar, configuracion);
                                if (nota != null)
                                {
                                    token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                                    facturaCredito = ConexionTicopay.BuscarFactura(nota.InvoiceId.ToString(), token);
                                    if (facturaCredito.Balance == 0)
                                    {
                                        notaEnviada = ConexionTicopay.EnviarNota(token, nota, true);
                                    }
                                    else
                                    {
                                        notaEnviada = ConexionTicopay.EnviarNota(token, nota, false);
                                    }
                                }
                                if (notaEnviada != null)
                                {
                                    operacionAProcesar.IdDocumentoAfectado = notaEnviada.InvoiceId.ToString();
                                    operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                    operacionAProcesar.IdTicopayDocument = notaEnviada.Id.ToString();
                                    operacionAProcesar.ConsecutivoTicopay = notaEnviada.ConsecutiveNumber;
                                    operacionAProcesar.VoucherTicopay = notaEnviada.VoucherKey;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                    try
                                    {
                                        ActualizarNotaCredito(operacionAProcesar, configuracion, notaEnviada.ConsecutiveNumber);
                                    }
                                    catch (Exception ex)
                                    {
                                        _eventos.Error("QuickBooksEnterpriseJob", "Actualizar Nota de Credito", "Imposible Actualizar el Numero de Consecutivo en Quickbooks");
                                    }
                                }
                                else
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Error;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                            }
                            catch (Exception ex)
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Error;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                                operacionAProcesar.Errores = ex.Message;
                            }                            
                            break;
                        case TipoOperacion.PagoFactura:
                            try
                            {
                                pago = ExtraerPago(operacionAProcesar, configuracion);
                                Operacion DatosFacturaCredito = _contexto.Operaciones.Where(O => O.EstadoOperacion == Estado.Procesado && O.TipoDeOperacion == TipoOperacion.FacturaCredito && O.IdDocumento == pago.AffectedInvoiceId).ToList().First(); // .OrderBy(O => O.IdOperacion)
                                if (pago != null && DatosFacturaCredito != null)
                                {
                                    token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                                    List<PaymentInvoce> pagosPendientes = new List<PaymentInvoce>();
                                    foreach (QuickbooksPaymentInvoce pagoPendiente in pago.ListPaymentType)
                                    {
                                        PaymentInvoce pagoArmado = new PaymentInvoce();
                                        pagoArmado.TypePayment = pagoPendiente.TypePayment;
                                        pagoArmado.Trans = pagoPendiente.Trans;
                                        pagoArmado.Balance = pagoPendiente.Balance;
                                        pagosPendientes.Add(pagoArmado);
                                    }
                                    pagoEnviado = ConexionTicopay.PagarFactura(token, pagosPendientes, DatosFacturaCredito.IdTicopayDocument);
                                }
                                if (pagoEnviado != null)
                                {
                                    operacionAProcesar.IdDocumentoAfectado = pagoEnviado.Id.ToString();
                                    operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                    operacionAProcesar.IdTicopayDocument = pagoEnviado.Id.ToString();
                                    operacionAProcesar.ConsecutivoTicopay = "Pago Factura " + pagoEnviado.ConsecutiveNumber;
                                    operacionAProcesar.VoucherTicopay = "N/A";
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                                else
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Error;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                            }
                            catch (Exception ex)
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Error;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                                operacionAProcesar.Errores = ex.Message;
                            }
                            break;
                        case TipoOperacion.Ticket:
                            try
                            {
                                cliente = IngresarCliente(operacionAProcesar, configuracion);
                                if (cliente != null)
                                {
                                    factura = ElaborarTiquete(operacionAProcesar, configuracion, cliente);
                                    token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                                    facturaEnviada = ConexionTicopay.EnviarTiquete(token, factura);
                                }
                                if (facturaEnviada != null)
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                    operacionAProcesar.IdTicopayDocument = facturaEnviada.Id;
                                    operacionAProcesar.ConsecutivoTicopay = facturaEnviada.ConsecutiveNumber;
                                    operacionAProcesar.VoucherTicopay = facturaEnviada.VoucherKey;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                    try
                                    {
                                        ActualizarFactura(operacionAProcesar, configuracion, facturaEnviada.ConsecutiveNumber);
                                    }
                                    catch (Exception ex)
                                    {
                                        _eventos.Error("QuickBooksEnterpriseJob", "Actualizar Factura de Contado", "Imposible Actualizar el Numero de Consecutivo en Quickbooks");
                                    }
                                }
                                else
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Error;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                            }
                            catch (Exception ex)
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Error;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                                operacionAProcesar.Errores = ex.Message;
                            }                            
                            break;
                    }
                    Operacion actualizarOperacion = _contexto.Operaciones.ToList().Where(op => op.IdOperacion == operacionAProcesar.IdOperacion).First();
                    _contexto.Entry(actualizarOperacion).CurrentValues.SetValues(operacionAProcesar);
                    _contexto.SaveChanges();                    
                }
                _contexto.Dispose();
            }
            catch
            {

            }            
        }

        public bool ActualizarFactura(Operacion operacion, Configuracion configuracion, string ConsecutivoTicopay)
        {
            try
            {
                string[] argumentos = new string[] { };
                argumentos = configuracion.DatosConexion.Split(Convert.ToChar(30));
                // Entablo comunicación con QuickBooks Enterprise
                ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop ConexionQuickBooks =
                        new ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop("TicopayConnector", argumentos[0], "TicopayC1");
                List<QuickbooksInvoice> invoiceList;
                if (operacion.TipoDeOperacion == TipoOperacion.FacturaContado)
                {
                    // Busca la factura en caso de se de Contado
                    invoiceList = ConexionQuickBooks.BuscarfacturasContado(configuracion.FechaCreacion, operacion.IdDocumento);
                }
                else
                {
                    // Busca la factura en caso de ser de Crédito
                    invoiceList = ConexionQuickBooks.BuscarfacturasCredito(configuracion.FechaCreacion, operacion.IdDocumento);
                }
                QuickbooksInvoice invoice;
                if (invoiceList.Count > 0)
                {
                    invoice = invoiceList.First();
                    if ((operacion.TipoDeOperacion == TipoOperacion.FacturaContado) || (operacion.TipoDeOperacion == TipoOperacion.Ticket))
                    {
                        ConexionQuickBooks.ActualizarSalesReceipt(invoice.invoiceID,invoice.EditSequence,ConsecutivoTicopay);
                        return true;
                    }
                    if (operacion.TipoDeOperacion == TipoOperacion.FacturaCredito)
                    {
                        ConexionQuickBooks.ActualizarFactura(invoice.invoiceID, invoice.EditSequence, ConsecutivoTicopay);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }            
        }

        public bool ActualizarNotaCredito(Operacion operacion, Configuracion configuracion, string ConsecutivoTicopay)
        {
            try
            {
                string[] argumentos = new string[] { };
                argumentos = configuracion.DatosConexion.Split(Convert.ToChar(30));
                // Entablo comunicación con QuickBooks Enterprise
                ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop ConexionQuickBooks =
                        new ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop("TicopayConnector", argumentos[0], "TicopayC1");
                QuickbooksNote note;
                List<QuickbooksNote> noteList = ConexionQuickBooks.BuscarNotas(configuracion.FechaCreacion, operacion.IdDocumento);
                if (noteList.Count > 0)
                {
                    note = noteList.First();
                    ConexionQuickBooks.ActualizarSalesReceipt(note.CreditMemoId, note.EditSequence, ConsecutivoTicopay);
                    return true;
                }                
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public QuickbooksPayment ExtraerPago(Operacion operacion, Configuracion configuracion)
        {
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            string[] argumentos = new string[] { };
            argumentos = configuracion.DatosConexion.Split(Convert.ToChar(30));
            // Entablo comunicación con QuickBooks Enterprise
            ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop ConexionQuickBooks =
                    new ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop("TicopayConnector", argumentos[0], "TicopayC1");
            try
            {
                List<QuickbooksPayment> paymentList;                
                paymentList = ConexionQuickBooks.BuscarPagos(configuracion.FechaCreacion, operacion.IdDocumento);
                QuickbooksPayment payment;
                if (paymentList.Count > 0)
                {
                    payment = paymentList.First();
                }
                else
                {
                    return null;
                }                
                return payment;
            }
            catch (Exception x)
            {
                // _eventos.Error("QuickBooksEnterpriseJob", "Extraer Pagos", "Imposible obtener los datos del Pago de QuickBooks");
                throw x;
            }
        }
    }
}
