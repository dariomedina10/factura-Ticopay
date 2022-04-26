using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Authentication;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Response;
using TicoPayDll.Services;
using TicoPayDll.Taxes;
using static TicoPayDll.Clients.ClientController;
using static TicoPayDll.Taxes.TaxesController;

namespace TicopayContaPymeMiddleService
{
    public class Operaciones
    {
        [Key]
        public Guid IdOperacion { get; set; }
        public string IdDocumento { get; set; }
        public string IdCliente { get; set; }
        public string IdEmpresa { get; set; }
        public string IdTicopayDocument { get; set; }
        public TipoOperacion TipoDeOperacion { get; set; }
        public Estado EstadoOperacion { get; set; }
        public Tenant Cfg { get; set; }

        public Operaciones()
        {
            IdOperacion = Guid.NewGuid();
        }

        public Operaciones(string idDocumento, string idCliente,string idEmpresa,TipoOperacion tipoOperacion,Tenant tenant)
        {
            IdOperacion = Guid.NewGuid();
            IdDocumento = idDocumento;
            IdCliente = idCliente;
            IdEmpresa = idEmpresa;
            TipoDeOperacion = tipoOperacion; 
            EstadoOperacion = Estado.NoProcesado;
            Cfg = tenant;
        }

        public string EnviarFactura(string connectionString)
        {
            string token = "";
            Client cliente = null;
            if (VerificarPermisoConector(this.Cfg.TenantTicopay))
            {

            }
            token = AutentificarUsuario(this.Cfg.TenantTicopay, this.Cfg.UserTicopay, this.Cfg.PasswordTicopay);
            cliente = BuscarCliente(token, false, this.IdCliente);
            if (cliente == null)
            {
                cliente = BuscarInfoCliente(connectionString, this.IdCliente);
                cliente = CrearCliente(token, cliente);
            }
            Tax[] impuestos = BuscarImpuestos(token);
            CreateInvoice factura = ElaborarFactura(connectionString,this.IdDocumento, this.IdEmpresa,cliente, impuestos.ToList());
            Invoice facturaEnviada = CrearFactura(token, factura);
            if(facturaEnviada != null)
            {
                return facturaEnviada.ConsecutiveNumber;
            }
            else
            {
                return null;
            }            
        }

        public string EnviarNotaCredito(string connectionString, bool devolucion)
        {
            string token = "";
            Client cliente = null;
            if (VerificarPermisoConector(this.Cfg.TenantTicopay))
            {

            }
            token = AutentificarUsuario(this.Cfg.TenantTicopay, this.Cfg.UserTicopay, this.Cfg.PasswordTicopay);
            cliente = BuscarCliente(token, false, this.IdCliente);
            if (cliente == null)
            {
                cliente = BuscarInfoCliente(connectionString, this.IdCliente);
                cliente = CrearCliente(token, cliente);
            }
            Tax[] impuestos = BuscarImpuestos(token);
            Invoice invoice = BuscarFactura(this.IdTicopayDocument, token);
            CompleteNote nota = ElaborarNota(connectionString, this.IdDocumento, this.IdEmpresa, cliente, impuestos.ToList(),NoteType.Crédito, devolucion, invoice);
            CompleteNote notaEnviada = CrearNota(token, nota);
            if (notaEnviada != null)
            {
                return notaEnviada.ConsecutiveNumber;
            }
            else
            {
                return null;
            }
        }

        public string EnviarNotaDebito(string connectionString)
        {
            return null;
        }

        public string PagarFacturaCredito(string connectionString)
        {
            return null;
        }

        #region Métodos para recopilar la información de ContaPyme

        public Client BuscarInfoCliente(string connectionString, string clientId)
        {
            FbConnection myConnection = new FbConnection(connectionString);
            Client cliente = null;
            try
            {
                string consulta = "Select NTERCERO,NAPELLIDO,SEMAIL,ITDDOCUM,QDIASPLAZOCXC,INIT From ABANITS where Init = '" + clientId + "'";
                myConnection.Open();
                FbCommand readCommand = new FbCommand(consulta, myConnection);                
                FbDataReader myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {                    
                        cliente = new Client();
                        cliente.Name = myreader[0].ToString();
                        cliente.LastName = myreader[1].ToString();
                        cliente.Email = myreader[2].ToString();
                        switch (myreader[3].ToString())
                        {
                            // Abadtit Codes
                            // 13  Cédula/RUT
                            // 22  Cédula de extranjería
                            // 31  NIT / RUC
                            // 41  Pasaporte
                            // 99  Código interno
                            case "13":
                                cliente.IdentificationType = IdentificacionTypeTipo.Cedula_Fisica;
                                break;
                            case "31":
                                cliente.IdentificationType = IdentificacionTypeTipo.Cedula_Juridica;
                                break;
                            case "22":
                                cliente.IdentificationType = IdentificacionTypeTipo.DIMEX;
                                break;
                            case "99":
                                cliente.IdentificationType = IdentificacionTypeTipo.NoAsignada;
                                break;
                            case "41":
                                cliente.IdentificationType = IdentificacionTypeTipo.NoAsignada;
                                break;
                            default:
                                cliente.IdentificationType = IdentificacionTypeTipo.NoAsignada;
                                break;
                        }
                        cliente.Identification = clientId;
                        if (myreader[4].ToString() != null || myreader[4].ToString() != "")
                        {
                            cliente.CreditDays = int.Parse(myreader[4].ToString());
                        }
                        else
                        {
                            cliente.CreditDays = 0;
                        }
                }
                myreader.Close();
                return cliente;
            }
            catch (Exception x)
            {
                return null;
            }
            finally
            {
                myConnection.Close();
            }
        }

        public Tax BuscarImpuesto(string connectionString, string taxPercentage, List<Tax> impuestos)
        {
            try
            {
                decimal rate = decimal.Parse(taxPercentage);
                return impuestos.Where(i => i.Rate == rate).First();
            }
            catch
            {
                return null;
            }
        }

        public CreateInvoice ElaborarFactura(string connectionString,string invoiceId,string companyId, Client client, List<Tax> impuestos)
        {
            FbConnection myConnection = new FbConnection(connectionString);
            CreateInvoice factura = null;
            try
            {
                myConnection.Open();
                factura = new CreateInvoice();
                FbCommand readCommand;
                FbDataReader myreader;
                factura.ClientId = client.Id;
                // Busca los datos basicos de la factura en el maestro de documentos OPRMAEST
                readCommand =
                  new FbCommand("Select FSOPORT,SNUMSOP,IMONEDA From OPRMAEST Where INUMOPER =" + invoiceId + "  and IEMP =" + companyId, myConnection);
                myreader = readCommand.ExecuteReader();                
                while (myreader.Read())
                {
                    // Campos de la consulta  myreader[0]                    
                    // Fecha no puede cambiarse
                    // Numero de operacion no puede cambiarse
                    // Moneda
                }
                myreader.Close();
                // Busca los detalles de la factura en el OprIng1_Base
                readCommand =
                  new FbCommand("Select INIT,QPORCDESCUENTO From OPRING1_BASE Where INUMOPER =" + invoiceId + "  and IEMP =" + companyId, myConnection);
                myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    // Campos de la consulta  myreader[0]
                    if(myreader[1].ToString() != null)
                    {
                        factura.DiscountGeneral = Decimal.Parse(myreader[1].ToString());
                        factura.TypeDiscountGeneral = 0;
                    }
                    else
                    {
                        factura.DiscountGeneral = null;
                        factura.TypeDiscountGeneral = null;
                    }
                }
                myreader.Close();
                // Busca las lineas o Servicios que contiene la factura
                factura.InvoiceLines = new List<ItemInvoice>();
                decimal subTotal = 0;
                readCommand =
                  new FbCommand("Select IRECURSO,QPRODUCTO,MPRECIO,QPORCDESCUENTO,SOBSERV,MVRTOTAL,QPORCIVA From OPRVENTAS Where INUMOPER =" + invoiceId + "  and IEMP =" + companyId, myConnection);
                myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    // Campos de la consulta  myreader[0]
                    ItemInvoice lineaFactura = new ItemInvoice();
                    lineaFactura.IdService = null;
                    lineaFactura.Servicio = myreader[0].ToString();
                    lineaFactura.Cantidad = int.Parse(myreader[1].ToString());
                    lineaFactura.Precio = decimal.Parse(myreader[2].ToString());
                    subTotal = lineaFactura.Cantidad * lineaFactura.Precio;
                    Tax impuesto = new Tax();
                    impuesto = BuscarImpuesto(connectionString, myreader[6].ToString(), impuestos);
                    lineaFactura.IdImpuesto = impuesto.Id;
                    lineaFactura.Descuento = decimal.Parse(myreader[3].ToString());
                    lineaFactura.Impuesto = (impuesto.Rate * subTotal) / 100;
                    lineaFactura.Total = lineaFactura.Impuesto + subTotal;
                    lineaFactura.UnidadMedida = UnidadMedidaType.Unidad;
                    factura.InvoiceLines.Add(lineaFactura);
                }
                myreader.Close();
                //  Busca los detalles del pago que contiene la factura
                factura.ListPaymentType = new List<PaymentInvoce>();                
                readCommand =
                  new FbCommand("Select ITIPOFCOBRO,ITIPOTRANSACCION,ITRANSACCION,MVALOR,QDIASCXC,INIT From OPRFCOBRO Where  INUMOPER =" + invoiceId + "  and IEMP =" + companyId, myConnection);
                myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    // Crédito
                    if (myreader[0].ToString() == "3")
                    {
                            factura.CreditTerm = int.Parse(myreader[4].ToString());
                    }
                    else
                    {
                        PaymentInvoce formaPago = new PaymentInvoce();
                        // Efectivo
                        if (myreader[0].ToString() == "1")
                        {
                            formaPago.Trans = null;
                            formaPago.Balance = decimal.Parse(myreader[3].ToString());
                            formaPago.TypePayment = 0;
                            factura.ListPaymentType.Add(formaPago);
                        }
                        // Deposito , Cheque, Tarjeta
                        if (myreader[0].ToString() == "2")
                        {
                            // Tarjeta de Débito o Crédito
                            if (myreader[1].ToString() == "TC" || myreader[1].ToString() == "TD")
                            {
                                formaPago.TypePayment = 1;
                            }
                            // Cheque
                            if (myreader[1].ToString() == "CH" || myreader[1].ToString() == "CHF")
                            {
                                formaPago.TypePayment = 2;
                            }
                            // Transferencia o Deposito
                            if (myreader[1].ToString() == "CE" || myreader[1].ToString() == "CI")
                            {
                                formaPago.TypePayment = 3;
                            }
                            formaPago.Trans = myreader[2].ToString();
                            formaPago.Balance = decimal.Parse(myreader[3].ToString());
                            factura.ListPaymentType.Add(formaPago);
                        }                        
                    }                           
                }
                myreader.Close();
                return factura;
            }
            catch (Exception x)
            {
                return null;
            }
            finally
            {
                myConnection.Close();
                factura = null;
            }
        }

        public CompleteNote ElaborarNota(string connectionString, string noteId, string companyId, Client cliente, List<Tax> impuestos, NoteType tipoNota, bool devolucion, Invoice facturaCreada)
        {
            if (devolucion)
            {
                FbConnection myConnection = new FbConnection(connectionString);
                CompleteNote nota = null;
                try
                {
                    myConnection.Open();
                    FbCommand readCommand;
                    FbDataReader myreader;
                    nota.ClientId = facturaCreada.ClientId;
                    nota.InvoiceId = Guid.Parse(facturaCreada.Id);
                    nota.NumberInvoiceRef = facturaCreada.ConsecutiveNumber;
                    nota.ClientId = cliente.Id;
                    nota.ClientName = cliente.Name;
                    // Busca los datos basicos de la nota en el maestro de documentos OPRMAEST
                    readCommand =
                      new FbCommand("Select FSOPORT,SNUMSOP,IMONEDA From OPRMAEST Where INUMOPER =" + noteId + "  and IEMP =" + companyId, myConnection);
                    myreader = readCommand.ExecuteReader();
                    while (myreader.Read())
                    {
                        // Campos de la consulta  myreader[0]                    
                        // Fecha no puede cambiarse
                        // Numero de operacion no puede cambiarse
                        // Moneda
                    }
                    myreader.Close();
                    // Busca los detalles de la factura en el OprIng1_Base
                    readCommand =
                      new FbCommand("Select INIT,QPORCDESCUENTO From OPRING1_BASE Where INUMOPER =" + noteId + "  and IEMP =" + companyId, myConnection);
                    myreader = readCommand.ExecuteReader();
                    while (myreader.Read())
                    {
                        // Campos de la consulta  myreader[0]
                        if (myreader[1].ToString() != null)
                        {
                            //note.DiscountAmount
                            //note.DiscountGeneral = Decimal.Parse(myreader[1].ToString());
                            //note.TypeDiscountGeneral = 0;
                        }
                        else
                        {
                            //note.DiscountGeneral = null;
                            //note.TypeDiscountGeneral = null;
                        }
                    }
                    myreader.Close();
                    // Busca las lineas o Servicios que contiene la devolucion
                    nota.NotesLines = new List<NoteLineDto>();
                    NoteLineDto lineaNotaDetalle;
                    int numeroLinea = 1;
                    decimal subTotal = 0;
                    readCommand =
                      new FbCommand("SELECT r.IEMP, r.INUMOPER,x.NRECURSO, r.QRECURSO,r.MVRUNIT, r.MVRTOTAL, r.QPORCIVA FROM OPRDEVOLVTA r Join INVMREC x on r.IRECURSO = x.IRECURSO where r.IEMP = " + companyId + " and r.INUMOPER = " + noteId, myConnection);
                    myreader = readCommand.ExecuteReader();
                    while (myreader.Read())
                    {                        
                        lineaNotaDetalle = new NoteLineDto();
                        //lineaNotaDetalle.PricePerUnit = item.PricePerUnit;
                        //lineaNotaDetalle.TaxAmount = item.TaxAmount;
                        //lineaNotaDetalle.Total = item.Total;
                        //lineaNotaDetalle.DiscountPercentage = 0;
                        //lineaNotaDetalle.Note = item.Note;
                        //lineaNotaDetalle.Title = item.Title;
                        //lineaNotaDetalle.Quantity = item.Quantity;
                        //lineaNotaDetalle.LineType = LineType.Service;
                        //lineaNotaDetalle.ServiceId = null;
                        //lineaNotaDetalle.ProductId = null;
                        //lineaNotaDetalle.LineNumber = numeroLinea;
                        //lineaNotaDetalle.CodeTypes = TicoPayDll.Invoices.CodigoTypeTipo.Otros;
                        //lineaNotaDetalle.DescriptionDiscount = null;
                        //lineaNotaDetalle.SubTotal = item.SubTotal;
                        //lineaNotaDetalle.LineTotal = item.LineTotal;
                        //lineaNotaDetalle.ExonerationId = null;
                        //lineaNotaDetalle.Service = null;
                        //lineaNotaDetalle.Tax = null;
                        //lineaNotaDetalle.TaxId = item.TaxId;
                        //lineaNotaDetalle.UnitMeasurement = TicoPayDll.Services.UnidadMedidaType.Unidad;
                        //lineaNotaDetalle.UnitMeasurementOthers = null;
                        nota.NotesLines.Add(lineaNotaDetalle);
                        numeroLinea++;
                        // Campos de la consulta  myreader[0]                        
                        //lineaFactura.IdService = null;
                        //lineaFactura.Servicio = myreader[0].ToString();
                        //lineaFactura.Cantidad = int.Parse(myreader[1].ToString());
                        //lineaFactura.Precio = decimal.Parse(myreader[2].ToString());
                        //subTotal = lineaFactura.Cantidad * lineaFactura.Precio;
                        //Tax impuesto = new Tax();
                        //impuesto = BuscarImpuesto(connectionString, myreader[6].ToString(), impuestos);
                        //lineaFactura.IdImpuesto = impuesto.Id;
                        //lineaFactura.Descuento = decimal.Parse(myreader[3].ToString());
                        //lineaFactura.Impuesto = (impuesto.Rate * subTotal) / 100;
                        //lineaFactura.Total = lineaFactura.Impuesto + subTotal;
                        //lineaFactura.UnidadMedida = UnidadMedidaType.Servicios_Profesionales;
                    }
                    nota.Amount = facturaCreada.SubTotal;
                    nota.TaxAmount = facturaCreada.TotalTax;
                    nota.DiscountAmount = 0;
                    nota.Total = facturaCreada.Total;
                    nota.CodigoMoneda = (CodigoMoneda)facturaCreada.CodigoMoneda;
                    nota.ConsecutiveNumber = null;
                    nota.NoteType = NoteType.Crédito;
                    nota.SendInvoice = facturaCreada.SendInvoice;
                    nota.StatusTribunet = facturaCreada.StatusTribunet;
                    nota.IsNoteReceptionConfirmed = false;
                    nota.NoteReasons = NoteReason.Corregir_Monto_Factura;
                    nota.NoteReasonsOthers = null;
                    nota.TipoFirma = facturaCreada.TipoFirma;
                    nota.StatusFirmaDigital = null;                    
                    myreader.Close();
                    //  Busca los detalles del pago que contiene la factura                
                    // note.ListPaymentType = new List<PaymentInvoce>();
                    readCommand =
                      new FbCommand("Select ITIPOFCOBRO,ITIPOTRANSACCION,ITRANSACCION,MVALOR,QDIASCXC,INIT From OPRFCOBRO Where INUMOPER =" + noteId + "  and IEMP =" + companyId, myConnection);
                    myreader = readCommand.ExecuteReader();
                    while (myreader.Read())
                    {
                        // Crédito
                        if (myreader[0].ToString() == "3")
                        {
                            // factura.CreditTerm = int.Parse(myreader[4].ToString());
                        }
                        else
                        {
                            PaymentInvoce formaPago = new PaymentInvoce();
                            // Efectivo
                            if (myreader[0].ToString() == "1")
                            {
                                formaPago.Trans = null;
                                formaPago.Balance = decimal.Parse(myreader[3].ToString());
                                formaPago.TypePayment = 0;
                                // nota.ListPaymentType.Add(formaPago);
                            }
                            // Deposito , Cheque, Tarjeta
                            if (myreader[0].ToString() == "2")
                            {
                                // Tarjeta de Débito o Crédito
                                if (myreader[1].ToString() == "TC" || myreader[1].ToString() == "TD")
                                {
                                    formaPago.TypePayment = 1;
                                }
                                // Cheque
                                if (myreader[1].ToString() == "CH" || myreader[1].ToString() == "CHF")
                                {
                                    formaPago.TypePayment = 2;
                                }
                                // Transferencia o Deposito
                                if (myreader[1].ToString() == "CE" || myreader[1].ToString() == "CI")
                                {
                                    formaPago.TypePayment = 3;
                                }
                                formaPago.Trans = myreader[2].ToString();
                                formaPago.Balance = decimal.Parse(myreader[3].ToString());
                                // factura.ListPaymentType.Add(formaPago);
                            }
                        }
                    }
                    myreader.Close();
                    return nota;
                }
                catch (Exception x)
                {
                    return null;
                }
                finally
                {
                    myConnection.Close();
                    nota = null;
                }
            }
            else
            {
                try
                {
                    CompleteNote nota = null;
                    nota.ClientId = facturaCreada.ClientId;
                    nota.InvoiceId = Guid.Parse(facturaCreada.Id);
                    nota.NumberInvoiceRef = facturaCreada.ConsecutiveNumber;
                    nota.ClientId = cliente.Id;
                    nota.ClientName = cliente.Name;
                    nota.Amount = facturaCreada.SubTotal;
                    nota.TaxAmount = facturaCreada.TotalTax;
                    nota.DiscountAmount = 0;
                    nota.Total = facturaCreada.Total;
                    nota.CodigoMoneda =(CodigoMoneda) facturaCreada.CodigoMoneda;
                    nota.ConsecutiveNumber = null;                    
                    nota.NoteType = NoteType.Crédito;
                    nota.SendInvoice = facturaCreada.SendInvoice;
                    nota.StatusTribunet = facturaCreada.StatusTribunet;
                    nota.IsNoteReceptionConfirmed = false;
                    nota.NoteReasons = NoteReason.Reversa_documento;
                    nota.NoteReasonsOthers = null;
                    nota.TipoFirma = facturaCreada.TipoFirma;
                    nota.StatusFirmaDigital = null;
                    nota.NotesLines = new List<NoteLineDto>();
                    #region LineNote
                    NoteLineDto lineaNotaDetalle;
                    int numeroLinea = 1;
                    foreach (InvoiceLineApiDto item in facturaCreada.InvoiceLines)
                    {
                        lineaNotaDetalle = new NoteLineDto();
                        lineaNotaDetalle.PricePerUnit = item.PricePerUnit;
                        lineaNotaDetalle.TaxAmount = item.TaxAmount;
                        lineaNotaDetalle.Total = item.Total;
                        lineaNotaDetalle.DiscountPercentage = 0;
                        lineaNotaDetalle.Note = item.Note;
                        lineaNotaDetalle.Title = item.Title;
                        lineaNotaDetalle.Quantity = item.Quantity;
                        lineaNotaDetalle.LineType = LineType.Service;
                        lineaNotaDetalle.ServiceId = null;
                        lineaNotaDetalle.ProductId = null;
                        lineaNotaDetalle.LineNumber = numeroLinea;
                        lineaNotaDetalle.CodeTypes = TicoPayDll.Invoices.CodigoTypeTipo.Otros;
                        lineaNotaDetalle.DescriptionDiscount = null;
                        lineaNotaDetalle.SubTotal = item.SubTotal;
                        lineaNotaDetalle.LineTotal = item.LineTotal;
                        lineaNotaDetalle.ExonerationId = null;
                        lineaNotaDetalle.Service = null;
                        lineaNotaDetalle.Tax = null;
                        lineaNotaDetalle.TaxId = item.TaxId;
                        lineaNotaDetalle.UnitMeasurement = TicoPayDll.Services.UnidadMedidaType.Unidad;
                        lineaNotaDetalle.UnitMeasurementOthers = null;
                        nota.NotesLines.Add(lineaNotaDetalle);
                        numeroLinea++;
                    }
                    #endregion
                    nota.ValidateHacienda = false;
                    nota.Status = Status.Completed;
                    nota.DueDate = facturaCreada.DueDate;
                    nota.CreditTerm = 0;
                    nota.ConditionSaleType = TicoPayDll.Notes.FacturaElectronicaCondicionVenta.Contado;
                    return nota;
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
        }

        #endregion

        #region Métodos Web Api

        public Invoice CrearFactura(string token, CreateInvoice factura)
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
                return null;
            }
        }

        public CompleteNote CrearNota(string token, CompleteNote nota,bool afectarBalance = false)
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
                return null;
            }
        }

        public Invoice BuscarFactura(string invoiceId,string token)
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
                return null;
            }

        }

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
                return null;
            }

        }

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
                return null;
            }
        }

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
                return null;
            }
        }

        public bool VerificarPermisoConector(string tenant)
        {

            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Authentication.Authentication.VerifyConnector(tenant, "Contapyme").GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonConnector token = JsonConvert.DeserializeObject<JsonConnector>(respuestaServicio.result);
                return token.objectResponse;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }

    public enum TipoOperacion
    {
        Factura,
        NotaCredito,
        NotaDebito,
        DevolucionFactura,
        PagoFactura,
    }

    public enum Estado
    {
        NoProcesado,
        Procesado,
        Error,
    }
}
