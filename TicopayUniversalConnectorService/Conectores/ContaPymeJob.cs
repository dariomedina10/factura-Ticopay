using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Quartz;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Services;
using TicoPayDll.Taxes;
using TicopayUniversalConnectorService.ConexionTicopay;
using TicopayUniversalConnectorService.Contexto;
using TicopayUniversalConnectorService.Entities;
using TicopayUniversalConnectorService.Interfaces;
using TicopayUniversalConnectorService.Log;

namespace TicopayUniversalConnectorService.Conectores
{
    /// <summary>
    /// Job Creado para ContaPyme Basado en la Interfaz IConector
    /// </summary>
    /// <seealso cref="TicopayUniversalConnectorService.Interfaces.IConector" />
    [DisallowConcurrentExecution]
    public class ContaPymeJob : IConector
    {
        public Tax BuscarImpuesto(Operacion operacion, Configuracion configuracion, string porcentajeImpuesto = null, string nombreImpuesto = null)
        {
            Ticopay _conexionTicopay = new Ticopay();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            if (porcentajeImpuesto == null && nombreImpuesto == null)
            {
                _eventos.Error("ContaPymeJob", "Buscar Impuesto", "No se ha especificado información del impuesto a buscar");
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
                _eventos.Error("ContaPymeJob", "Buscar Impuesto", "Error al Buscar la lista de impuestos de Ticopay");
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
            FbConnection myConnection = new FbConnection(configuracion.DatosConexion);
            
            try
            {
                myConnection.Open();
                factura = new CreateInvoice();
                FbCommand readCommand;
                FbDataReader myreader;
                factura.ClientId = cliente.Id;
                factura.ExternalReferenceNumber = operacion.ConsecutivoConector;
                #region Moneda y Detalle de factura
                // Busca los datos basicos de la factura en el maestro de documentos OPRMAEST
                readCommand =
                  new FbCommand("Select FSOPORT,SNUMSOP,IMONEDA,TDETALLE From OPRMAEST Where INUMOPER =" + operacion.IdDocumento + "  and IEMP =" + operacion.IdEmpresa, myConnection);
                myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    // Moneda
                    switch (myreader[2].ToString())
                    {
                        // Colones 10
                        case "10":
                            factura.CodigoMoneda = CodigoMoneda.CRC;
                            break;
                        // Dolares 20
                        case "20":
                            factura.CodigoMoneda = CodigoMoneda.USD;
                            break;
                        default:      
                            break;
                    }
                    // Detalle de factura (Orden de compra)
                    if (myreader[3].ToString() != null)
                    {
                        factura.GeneralObservation = myreader[3].ToString().Replace("\r\n", "");
                    }
                    // Fecha no puede cambiarse
                    // Numero de operacion no puede cambiarse
                    // Moneda
                }
                myreader.Close();
                #endregion
                // Busca los detalles de la factura en el OprIng1_Base
                #region Notas de la factura y Descuento General
                readCommand =
                  new FbCommand("Select INIT,QPORCDESCUENTO,SOBSERV From OPRING1_BASE Where INUMOPER =" + operacion.IdDocumento + "  and IEMP =" + operacion.IdEmpresa, myConnection);
                myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    // Campos de la consulta  myreader[0]
                    if (myreader[1].ToString() != null && decimal.Parse(myreader[1].ToString()) != 0)
                    {
                        factura.DiscountGeneral = Decimal.Parse(myreader[1].ToString());
                        factura.TypeDiscountGeneral = 0;
                    }
                    else
                    {
                        factura.DiscountGeneral = null;
                        factura.TypeDiscountGeneral = 0;
                    }
                    if (myreader[2].ToString() != null)
                    {
                        factura.GeneralObservation = factura.GeneralObservation + " " + myreader[2].ToString().Replace("\r\n","");
                    }
                }
                myreader.Close();
                #endregion
                // Busca las lineas o Servicios que contiene la factura
                #region Lineas de la factura
                factura.InvoiceLines = new List<ItemInvoice>();
                decimal subTotal = 0;
                decimal descuento = 0;
                readCommand =
                  new FbCommand("Select d.IRECURSO,d.QPRODUCTO,d.MPRECIO,d.QPORCDESCUENTO,d.SOBSERV,d.MVRTOTAL,d.QPORCIVA,i.NRECURSO,i.NUNIDAD,i.BSERVICIO From OPRVENTAS d join INVMREC i on d.IRECURSO = i.IRECURSO Where d.INUMOPER =" + operacion.IdDocumento + "  and d.IEMP =" + operacion.IdEmpresa, myConnection);
                myreader = readCommand.ExecuteReader();
                int Linea = 1;
                while (myreader.Read())
                {
                    // Campos de la consulta  myreader[0]
                    ItemInvoice lineaFactura = new ItemInvoice();
                    lineaFactura.IdService = null;                    
                    if (myreader[7] != DBNull.Value && myreader[7] != null && myreader[7].ToString().Length > 0)
                    {
                        lineaFactura.Servicio = myreader[7].ToString();
                    }
                    else
                    {
                        throw new Exception("Linea " + Linea.ToString() + " Debe colocar la descripción del Producto o Servicio a facturar");
                    }
                    if (myreader[1] != DBNull.Value && myreader[1] != null && myreader[1].ToString().Length > 0)
                    {
                        if(decimal.Parse(myreader[1].ToString()) > 0)
                        {
                            lineaFactura.Cantidad = Decimal.Round(decimal.Parse(myreader[1].ToString()),2);
                        }
                        else
                        {
                            throw new Exception("Linea " + Linea.ToString() + " La cantidad del Producto o Servicio a facturar debe ser mayor a 0");
                        }                        
                    }
                    else
                    {
                        throw new Exception("Linea " + Linea.ToString() + " Debe colocar la cantidad del Producto o Servicio a facturar");
                    }
                    if (myreader[2] != DBNull.Value && myreader[2] != null && myreader[2].ToString().Length > 0)
                    {
                        if (decimal.Parse(myreader[2].ToString()) > 0)
                        {
                            lineaFactura.Precio = Decimal.Round(decimal.Parse(myreader[2].ToString()),2);
                        }
                        else
                        {
                            throw new Exception("Linea " + Linea.ToString() + " El precio del Producto o Servicio a facturar debe ser mayor a 0");
                        }
                    }
                    else
                    {
                        throw new Exception("Linea " + Linea.ToString() + " Debe colocar el precio del Producto o Servicio a facturar");
                    }                    
                    if(myreader[4].ToString() != null)
                    {
                        if(myreader[4].ToString().Contains("DESCRIP_ORIGINAL") == false)
                        {
                            lineaFactura.Note = myreader[4].ToString();
                        }                        
                    }                    
                    Tax impuesto = new Tax();
                    if (myreader[6] != DBNull.Value && myreader[6] != null && myreader[6].ToString().Length > 0)
                    {                        
                        if (decimal.Parse(myreader[6].ToString()) >= 0)
                        {
                            impuesto = this.BuscarImpuesto(operacion, configuracion, myreader[6].ToString(), null);
                        }
                        else
                        {
                            throw new Exception("Linea " + Linea.ToString() + " El porcentaje de impuesto debe ser mayor o igual a cero");
                        }
                    }
                    else
                    {
                        throw new Exception("Linea " + Linea.ToString() + " El porcentaje de impuesto no puede estar en blanco");
                    }                        
                    lineaFactura.IdImpuesto = impuesto.Id;
                    if (myreader[3] != DBNull.Value && myreader[3] != null && myreader[3].ToString().Length > 0)
                    {                        
                        if(decimal.Parse(myreader[3].ToString()) >= 0 && decimal.Parse(myreader[3].ToString()) < 100)
                        {
                            lineaFactura.Descuento = Decimal.Round(decimal.Parse(myreader[3].ToString()), 2);
                        }
                        else
                        {
                            throw new Exception("Linea " + Linea.ToString() + " El porcentaje de descuento no puede ser negativo, ni mayor a 99.99%");
                        }
                    }
                    else
                    {
                        lineaFactura.Descuento = 0;
                    }
                    if (lineaFactura.Descuento > 0)
                    {
                        descuento = (Decimal.Round(lineaFactura.Cantidad * lineaFactura.Precio, 2) * lineaFactura.Descuento) / 100;
                        subTotal = Decimal.Round(Decimal.Round(lineaFactura.Cantidad * lineaFactura.Precio, 2) - descuento, 2);
                    }
                    else
                    {
                        subTotal = Decimal.Round(lineaFactura.Cantidad * lineaFactura.Precio, 2);
                        
                    }
                    lineaFactura.Impuesto = Decimal.Round((impuesto.Rate * subTotal) / 100, 2);
                    lineaFactura.Total = Decimal.Round(subTotal + lineaFactura.Impuesto,2);
                    // Unidad de Medida
                    if (myreader[8].ToString() != null)
                    {
                        lineaFactura.UnidadMedida = UnidadDeMedida(myreader[8].ToString());
                        if (lineaFactura.UnidadMedida == UnidadMedidaType.Otros)
                        {
                            lineaFactura.UnidadMedidaOtra = myreader[8].ToString();
                        }
                    }
                    else
                    {
                        lineaFactura.UnidadMedida = UnidadMedidaType.Unidad;
                    }
                    // Tipo de linea  Producto o Servicio
                    if (myreader[9] != DBNull.Value && myreader[9] != null && myreader[9].ToString().Length > 0)
                    {
                        if (myreader[9].ToString() == "T")
                        {
                            lineaFactura.Tipo = LineType.Service;
                        }
                        else
                        {
                            lineaFactura.Tipo = LineType.Product;
                        }
                    }
                    else
                    {
                        lineaFactura.Tipo = LineType.Product;
                    }
                    Linea++;
                    factura.InvoiceLines.Add(lineaFactura);
                }
                myreader.Close();
                #endregion
                //  Busca los detalles del pago que contiene la factura
                #region Medios de pago de la factura
                factura.ListPaymentType = new List<PaymentInvoce>();
                readCommand =
                  new FbCommand("Select ITIPOFCOBRO,ITIPOTRANSACCION,ITRANSACCION,MVALOR,QDIASCXC,INIT From OPRFCOBRO Where  INUMOPER =" + operacion.IdDocumento + "  and IEMP =" + operacion.IdEmpresa, myConnection);
                myreader = readCommand.ExecuteReader();
                int LineaPagos = 1;
                bool Credito = false;
                while (myreader.Read())
                {
                    PaymentInvoce formaPago;                    
                    switch (myreader[0].ToString())
                    {
                        // Credito
                        case "3":
                            Credito = true;
                            if (int.Parse(myreader[4].ToString()) == 0)
                            {
                                factura.CreditTerm = 1;
                            }
                            else
                            {
                                factura.CreditTerm = int.Parse(myreader[4].ToString());
                            }
                            break;
                        // Efectivo
                        case "1":
                            if(Credito == true)
                            {
                                throw new Exception("Una factura de crédito no puede tener pagos , Debe ser pagada en su totalidad o ser de crédito");
                            }
                            formaPago = new PaymentInvoce();
                            formaPago.Trans = null;
                            formaPago.Balance = Decimal.Round(decimal.Parse(myreader[3].ToString()),2);
                            formaPago.TypePayment = 0;
                            factura.ListPaymentType.Add(formaPago);
                            break;
                        // Deposito , Cheque, Tarjeta
                        case "2":
                            if (Credito == true)
                            {
                                throw new Exception("Una factura de crédito no puede tener pagos , Debe ser pagada en su totalidad o ser de crédito");
                            }
                            formaPago = new PaymentInvoce();
                            switch (myreader[1].ToString())
                            {
                                // Tarjeta de Débito o Crédito
                                case "TC":
                                    formaPago.TypePayment = 1;
                                    break;
                                case "TD":
                                    formaPago.TypePayment = 1;
                                    break;
                                // Cheque
                                case "CH":
                                    formaPago.TypePayment = 2;
                                    break;
                                case "CHF":
                                    formaPago.TypePayment = 2;
                                    break;
                                // Transferencia o Deposito
                                case "CE":
                                    formaPago.TypePayment = 3;
                                    break;
                                case "CI":
                                    formaPago.TypePayment = 3;
                                    break;
                                default:
                                    throw new Exception("Forma de pago invalida (Los códigos aceptados para pagos de banco son TC,TD,CH,CHF,CE,CI )");
                            }
                            formaPago.Trans = myreader[2].ToString();
                            if (myreader[3] != DBNull.Value && myreader[3] != null && myreader[3].ToString().Length > 0)
                            {
                                if (decimal.Parse(myreader[3].ToString()) > 0)
                                {
                                    formaPago.Balance = Decimal.Round(decimal.Parse(myreader[3].ToString()),2);
                                }
                                else
                                {
                                    throw new Exception("Linea " + LineaPagos.ToString() + " El monto del pago debe ser mayor a 0");
                                }
                            }
                            else
                            {
                                throw new Exception("Linea " + LineaPagos.ToString() + " Debe colocar el monto pagado");
                            }
                            LineaPagos++;
                            factura.ListPaymentType.Add(formaPago);
                            break;
                        default:
                            break;
                    }
                }
                myreader.Close();
                #endregion
                // Chequear que el monto pagado sea igual al total                
                decimal totalinvoice = decimal.Round(factura.InvoiceLines.Sum(x => x.Total), 2);
                if(factura.ListPaymentType.Count > 0 && Credito == true)
                {
                    throw new Exception("Los pagos parciales no son validos , la factura debe ser a crédito o se debe pagar en su totalidad");
                }
                if (factura.ListPaymentType.Count > 0 && Credito == false)
                {
                    decimal totalPayments = decimal.Round(factura.ListPaymentType.Sum(x => x.Balance), 2);
                    if (totalPayments != totalinvoice)
                    {
                        throw new Exception("El monto pagado no coincide con el total de la factura");                        
                    }
                }
                factura.TipoFirma = FirmType.Llave;
                return factura;
            }
            catch (Exception x)
            {
                // _eventos.Error("ContaPymeJob", "Elaborar Factura", "Imposible elaborar la factura con la Data extraída de Contapyme: " + x.Message);
                throw x;
            }
            finally
            {
                myConnection.Close();
                factura = null;
            }
        }

        public CompleteNote ElaborarNotaCreditoDevolucion(Operacion operacion, Configuracion configuracion)
        {
            // Inicialización de Variables
            Ticopay _conexionTicopay = new Ticopay();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            UniversalConnectorDB _contexto = new UniversalConnectorDB();

            FbConnection myConnection = new FbConnection(configuracion.DatosConexion);
            CompleteNote nota = new CompleteNote();
            try
            {
                myConnection.Open();
                FbCommand readCommand;
                FbDataReader myreader;
                // Busca los datos de la Factura a aplicar la devolución en OPREREFERENCIAS
                string idFacturaOriginal = null;
                Invoice facturaCreada = null;
                readCommand =
                  new FbCommand("Select m.IEMP,m.INUMOPER From OPRMAEST m Join OPRREFERENCIAS r on m.SNUMSOP = r.IREFERENCIA Where r.INUMOPER =" + operacion.IdDocumento + "  and r.IEMP =" + operacion.IdEmpresa, myConnection);
                myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    idFacturaOriginal = myreader[1].ToString();
                }
                myreader.Close();
                string IdFacturaAReversar = _contexto.Operaciones.Where(faR => faR.IdEmpresa == operacion.IdEmpresa && faR.IdDocumento == idFacturaOriginal &&
                           (faR.TipoDeOperacion == TipoOperacion.Factura || faR.TipoDeOperacion == TipoOperacion.Ticket) && faR.Configuracion.TipoConector == operacion.Configuracion.TipoConector).First().IdTicopayDocument;
                string _token = null;
                try
                {
                    _token = _conexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                    facturaCreada = _conexionTicopay.BuscarFactura(IdFacturaAReversar, _token);
                }
                catch (Exception ex)
                {
                    _eventos.Error("ContaPymeJob", "Elaborar Nota Crédito Devolución", "Imposible obtener factura a aplicar devolución de Ticopay");
                }
                if (facturaCreada != null)
                {
                    if(facturaCreada.ClientId != null)
                    {
                        nota.ClientId = facturaCreada.ClientId;
                    }                    
                    nota.InvoiceId = Guid.Parse(facturaCreada.Id);
                    nota.NumberInvoiceRef = facturaCreada.ConsecutiveNumber;
                    nota.ExternalReferenceNumber = operacion.ConsecutivoConector;
                    // nota.ClientName = facturaCreada.Client.Name;
                    // Busca los datos basicos de la nota en el maestro de documentos OPRMAEST
                    readCommand =
                      new FbCommand("Select FSOPORT,SNUMSOP,IMONEDA From OPRMAEST Where INUMOPER =" + operacion.IdDocumento + "  and IEMP =" + operacion.IdEmpresa, myConnection);
                    myreader = readCommand.ExecuteReader();
                    while (myreader.Read())
                    {
                        // Campos de la consulta  myreader[0] 
                        // Moneda
                        switch (myreader[2].ToString())
                        {
                            // Colones 10
                            case "10":
                                nota.CodigoMoneda = CodigoMoneda.CRC;
                                break;
                            // Dolares 20
                            case "20":
                                nota.CodigoMoneda = CodigoMoneda.USD;
                                break;
                            default:
                                nota.CodigoMoneda = facturaCreada.CodigoMoneda;
                                break;
                        }
                        // Fecha no puede cambiarse
                        // Numero de operación no puede cambiarse
                        // Moneda
                    }
                    myreader.Close();                    
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
                    readCommand =
                      new FbCommand("SELECT x.NRECURSO, r.QRECURSO, r.MVRUNIT, r.MVRTOTAL, r.QPORCIVA, x.NUNIDAD,r.TOBSERVACION,x.BSERVICIO FROM OPRDEVOLVTA r Join INVMREC x on r.IRECURSO = x.IRECURSO where r.IEMP = " + operacion.IdEmpresa + " and r.INUMOPER = " + operacion.IdDocumento, myConnection);
                    myreader = readCommand.ExecuteReader();
                    while (myreader.Read())
                    {
                        lineaNotaDetalle = new NoteLineDto();
                        if (myreader[2] != DBNull.Value && myreader[2] != null && myreader[2].ToString().Length > 0)
                        {
                            if (decimal.Parse(myreader[2].ToString()) > 0)
                            {
                                lineaNotaDetalle.PricePerUnit = decimal.Parse(myreader[2].ToString());
                            }
                            else
                            {
                                throw new Exception("Linea " + numeroLinea.ToString() + " El precio del Producto o Servicio a devolver debe ser mayor a 0");
                            }
                        }
                        else
                        {
                            throw new Exception("Linea " + numeroLinea.ToString() + " Debe colocar el precio del Producto o Servicio a devolver");
                        }
                        
                        if (myreader[1] != DBNull.Value && myreader[1] != null && myreader[1].ToString().Length > 0)
                        {
                            if (decimal.Parse(myreader[1].ToString()) > 0)
                            {
                                lineaNotaDetalle.Quantity = decimal.Parse(myreader[1].ToString());
                            }
                            else
                            {
                                throw new Exception("Linea " + numeroLinea.ToString() + " La cantidad del Producto o Servicio a devolver debe ser mayor a 0");
                            }
                        }
                        else
                        {
                            throw new Exception("Linea " + numeroLinea.ToString() + " Debe colocar la cantidad del Producto o Servicio a devolver");
                        }
                        if (myreader[4] != DBNull.Value && myreader[4] != null && myreader[4].ToString().Length > 0)
                        {
                            if (decimal.Parse(myreader[4].ToString()) >= 0)
                            {
                                impuesto = BuscarImpuesto(operacion, configuracion, myreader[4].ToString(), null);
                            }
                            else
                            {
                                throw new Exception("Linea " + numeroLinea.ToString() + " El porcentaje de impuesto debe ser mayor o igual a cero");
                            }
                        }
                        else
                        {
                            throw new Exception("Linea " + numeroLinea.ToString() + " El porcentaje de impuesto no puede estar en blanco");
                        }                        
                        subTotalLinea = decimal.Round(lineaNotaDetalle.PricePerUnit * lineaNotaDetalle.Quantity,2);
                        lineaNotaDetalle.SubTotal = subTotalLinea;
                        subTotalNota = decimal.Round(subTotalNota + subTotalLinea,2);
                        impuestoTotalLinea = decimal.Round((subTotalLinea * impuesto.Rate) / 100,2);
                        lineaNotaDetalle.TaxAmount = impuestoTotalLinea;
                        impuestoTotalNota = decimal.Round(impuestoTotalNota + impuestoTotalLinea,2);
                        totalConImpuestoLinea = decimal.Round(subTotalLinea + impuestoTotalLinea,2);
                        lineaNotaDetalle.Total = totalConImpuestoLinea;
                        lineaNotaDetalle.LineTotal = totalConImpuestoLinea;
                        totalConImpuestoNota = decimal.Round(totalConImpuestoNota + totalConImpuestoLinea,2);
                        lineaNotaDetalle.DiscountPercentage = 0;
                        lineaNotaDetalle.Note = null;
                        if (myreader[6].ToString() != null && myreader[6].ToString().Length > 1)
                        {
                            lineaNotaDetalle.Note = myreader[6].ToString();
                        }
                        if (myreader[0] != DBNull.Value && myreader[0] != null && myreader[0].ToString().Length > 0)
                        {
                            lineaNotaDetalle.Title = myreader[0].ToString();
                        }
                        else
                        {
                            throw new Exception("Linea " + numeroLinea.ToString() + " Debe colocar la descripción del Producto o Servicio a devolver");
                        }
                        // Tipo de linea  Producto o Servicio
                        if (myreader[7] != DBNull.Value && myreader[7] != null && myreader[7].ToString().Length > 0)
                        {
                            if (myreader[7].ToString() == "T")
                            {
                                lineaNotaDetalle.LineType = LineType.Service;
                            }
                            else
                            {
                                lineaNotaDetalle.LineType = LineType.Product;
                            }
                        }
                        else
                        {
                            lineaNotaDetalle.LineType = LineType.Product;
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
                        // Unidad de Medida
                        if (myreader[5].ToString() != null)
                        {
                            lineaNotaDetalle.UnitMeasurement = UnidadDeMedida(myreader[5].ToString());
                            lineaNotaDetalle.UnitMeasurementOthers = null;
                            if (lineaNotaDetalle.UnitMeasurement == UnidadMedidaType.Otros)
                            {
                                lineaNotaDetalle.UnitMeasurementOthers = myreader[5].ToString();
                            }
                        }
                        else
                        {
                            lineaNotaDetalle.UnitMeasurement = UnidadMedidaType.Unidad;
                            lineaNotaDetalle.UnitMeasurementOthers = null;
                        }
                        nota.NotesLines.Add(lineaNotaDetalle);
                        numeroLinea++;
                    }
                    // Totales de la Nota
                    nota.Amount = subTotalNota;
                    nota.TaxAmount = impuestoTotalNota;
                    nota.DiscountAmount = 0;
                    nota.Total = totalConImpuestoNota;
                    // Moneda
                    // nota.CodigoMoneda = facturaCreada.CodigoMoneda;
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
                    // nota.StatusFirmaDigital = StatusFirmaDigital.Pendiente;
                    nota.ValidateHacienda = false;
                    nota.Status = Status.Completed;
                    nota.DueDate = DateTime.UtcNow;
                    nota.CreditTerm = 0;
                    nota.ConditionSaleType = TicoPayDll.Notes.FacturaElectronicaCondicionVenta.Otros;
                    myreader.Close();                    
                    return nota;
                }
                _eventos.Error("ContaPymeJob", "Elaborar Nota Crédito Devolución", "La factura a la que hace referencia la nota no ha sido procesada todavía o no puede ser ubicada en Ticopay");
                throw new Exception("La factura a la que hace referencia la nota no ha sido procesada todavía o no puede ser ubicada en Ticopay");
                // return null;
            }
            catch (Exception x)
            {
                // _eventos.Error("ContaPymeJob", "Elaborar Nota Crédito Devolución", "Imposible elaborar la nota con la Data de Contapyme: " + x.Message);
                throw x;
            }
            finally
            {
                myConnection.Close();
            }
        }

        public CompleteNote ElaborarNotaCreditoReverso(Operacion operacion, Configuracion configuracion)
        {
            // Inicialización de Variables
            Ticopay _conexionTicopay = new Ticopay();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            Invoice facturaCreada = null;
            string _token = null;
            try
            {
                string IdFacturaAReversar = _contexto.Operaciones.Where(faR => faR.IdEmpresa == operacion.IdEmpresa && faR.IdDocumento == operacion.IdDocumento &&
                           (faR.TipoDeOperacion == TipoOperacion.Factura || faR.TipoDeOperacion == TipoOperacion.Ticket) && faR.Configuracion.TipoConector == operacion.Configuracion.TipoConector).First().IdTicopayDocument;
                _token = _conexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                facturaCreada = _conexionTicopay.BuscarFactura(IdFacturaAReversar, _token);
            }
            catch(Exception ex)
            {
                // _eventos.Error("ContaPymeJob", "Elaborar Nota Crédito Reverso", "Imposible obtener factura a Reversar de Ticopay");
                throw new Exception("Elaborar Nota Crédito Reverso, Imposible obtener factura a Reversar de Ticopay " + ex.Message);
            }
            if(facturaCreada != null)
            {
                try
                {
                    CompleteNote nota = _conexionTicopay.ReversarFacturaOTiquete(_token, facturaCreada.Id, operacion.ConsecutivoConector); ;
                    return nota;
                }
                catch (Exception ex)
                {
                    //_eventos.Error("ContaPymeJob", "Elaborar Nota Crédito Reverso", "Imposible elaborar nota basada en factura de Ticopay" + ex.Message);
                    throw new Exception("Imposible elaborar la Nota basada en la data de la factura: " + ex.Message);
                }                
            }
            // _eventos.Error("ContaPymeJob", "Elaborar Nota Crédito Reverso", "La factura a la que hace referencia la nota no ha sido procesada todavía o no puede ser ubicada en Ticopay");
            throw new Exception("La factura a la que hace referencia la nota no ha sido procesada todavía o no puede ser ubicada en Ticopay");            
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
            FbConnection myConnection = new FbConnection(configuracion.DatosConexion);

            try
            {
                myConnection.Open();
                factura = new CreateInvoice();
                factura.ExternalReferenceNumber = operacion.ConsecutivoConector;
                FbCommand readCommand;
                FbDataReader myreader;
                // Busca los datos basicos de la factura en el maestro de documentos OPRMAEST
                #region Moneda y Detalle de tiquete
                readCommand =
                  new FbCommand("Select FSOPORT,SNUMSOP,IMONEDA,TDETALLE From OPRMAEST Where INUMOPER =" + operacion.IdDocumento + "  and IEMP =" + operacion.IdEmpresa, myConnection);
                myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    // Campos de la consulta  myreader[0] 
                    switch (myreader[2].ToString())
                    {
                        // Colones 10
                        case "10":
                            factura.CodigoMoneda = CodigoMoneda.CRC;
                            break;
                        // Dolares 20
                        case "20":
                            factura.CodigoMoneda = CodigoMoneda.USD;
                            break;
                        default:
                            break;
                    }
                    // Detalle de factura (Orden de compra)
                    if (myreader[3].ToString() != null)
                    {
                        factura.GeneralObservation = myreader[3].ToString().Replace("\r\n", "");
                    }
                    // Fecha no puede cambiarse
                    // Numero de operacion no puede cambiarse
                    // Moneda
                }
                myreader.Close();
                #endregion
                // Busca los detalles de la factura en el OprIng1_Base
                #region Notas del tiquete y descuento General
                readCommand =
                  new FbCommand("Select INIT,QPORCDESCUENTO,SOBSERV From OPRING1_BASE Where INUMOPER =" + operacion.IdDocumento + "  and IEMP =" + operacion.IdEmpresa, myConnection);
                myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    // Campos de la consulta  myreader[0]
                    if (myreader[1].ToString() != null && decimal.Parse(myreader[1].ToString()) != 0)
                    {
                        factura.DiscountGeneral = Decimal.Parse(myreader[1].ToString());
                        factura.TypeDiscountGeneral = 0;
                    }
                    else
                    {
                        factura.DiscountGeneral = null;
                        factura.TypeDiscountGeneral = 0;
                    }
                    if (myreader[2].ToString() != null)
                    {
                        factura.GeneralObservation = factura.GeneralObservation + " " + myreader[2].ToString().Replace("\r\n", "");
                    }
                }
                myreader.Close();
                #endregion
                // Busca las lineas o Servicios que contiene la factura
                #region Lineas del Tiquete
                factura.InvoiceLines = new List<ItemInvoice>();
                decimal subTotal = 0;
                decimal descuento = 0;
                readCommand =
                  new FbCommand("Select d.IRECURSO,d.QPRODUCTO,d.MPRECIO,d.QPORCDESCUENTO,d.SOBSERV,d.MVRTOTAL,d.QPORCIVA,i.NRECURSO,i.NUNIDAD,i.BSERVICIO From OPRVENTAS d join INVMREC i on d.IRECURSO = i.IRECURSO Where d.INUMOPER =" + operacion.IdDocumento + "  and d.IEMP =" + operacion.IdEmpresa, myConnection);
                myreader = readCommand.ExecuteReader();
                int Linea = 1;
                while (myreader.Read())
                {
                    // Campos de la consulta  myreader[0]
                    ItemInvoice lineaFactura = new ItemInvoice();
                    lineaFactura.IdService = null;
                    if (myreader[7] != DBNull.Value && myreader[7] != null && myreader[7].ToString().Length > 0)
                    {
                        lineaFactura.Servicio = myreader[7].ToString();
                    }
                    else
                    {
                        throw new Exception("Linea " + Linea.ToString() + " Debe colocar la descripción del Producto o Servicio a facturar");
                    }
                    if (myreader[1] != DBNull.Value && myreader[1] != null && myreader[1].ToString().Length > 0)
                    {
                        if (decimal.Parse(myreader[1].ToString()) > 0)
                        {
                            lineaFactura.Cantidad = Decimal.Round(decimal.Parse(myreader[1].ToString()),2);
                        }
                        else
                        {
                            throw new Exception("Linea " + Linea.ToString() + " La cantidad del Producto o Servicio a facturar debe ser mayor a 0");
                        }
                    }
                    else
                    {
                        throw new Exception("Linea " + Linea.ToString() + " Debe colocar la cantidad del Producto o Servicio a facturar");
                    }
                    if (myreader[2] != DBNull.Value && myreader[2] != null && myreader[2].ToString().Length > 0)
                    {
                        if (decimal.Parse(myreader[2].ToString()) > 0)
                        {
                            lineaFactura.Precio = Decimal.Round(decimal.Parse(myreader[2].ToString()),2);
                        }
                        else
                        {
                            throw new Exception("Linea " + Linea.ToString() + " El precio del Producto o Servicio a facturar debe ser mayor a 0");
                        }
                    }
                    else
                    {
                        throw new Exception("Linea " + Linea.ToString() + " Debe colocar el precio del Producto o Servicio a facturar");
                    }
                    if (myreader[4].ToString() != null)
                    {
                        if (myreader[4].ToString().Contains("DESCRIP_ORIGINAL") == false)
                        {
                            lineaFactura.Note = myreader[4].ToString();
                        }
                    }
                    Tax impuesto = new Tax();
                    if (myreader[6] != DBNull.Value && myreader[6] != null && myreader[6].ToString().Length > 0)
                    {
                        if (decimal.Parse(myreader[6].ToString()) >= 0)
                        {
                            impuesto = this.BuscarImpuesto(operacion, configuracion, myreader[6].ToString(), null);
                        }
                        else
                        {
                            throw new Exception("Linea " + Linea.ToString() + " El porcentaje de impuesto debe ser mayor o igual a cero");
                        }
                    }
                    else
                    {
                        throw new Exception("Linea " + Linea.ToString() + " El porcentaje de impuesto no puede estar en blanco");
                    }
                    lineaFactura.IdImpuesto = impuesto.Id;
                    if (myreader[3] != DBNull.Value && myreader[3] != null && myreader[3].ToString().Length > 0)
                    {
                        if (decimal.Parse(myreader[3].ToString()) >= 0 && decimal.Parse(myreader[3].ToString()) < 100)
                        {
                            lineaFactura.Descuento = Decimal.Round(decimal.Parse(myreader[3].ToString()), 2);
                        }
                        else
                        {
                            throw new Exception("Linea " + Linea.ToString() + " El porcentaje de descuento no puede ser negativo, ni mayor a 99.99%");
                        }
                    }
                    else
                    {
                        lineaFactura.Descuento = 0;
                    }
                    if (lineaFactura.Descuento > 0)
                    {
                        descuento = (Decimal.Round(lineaFactura.Cantidad * lineaFactura.Precio, 2) * lineaFactura.Descuento) / 100;
                        subTotal = Decimal.Round(Decimal.Round(lineaFactura.Cantidad * lineaFactura.Precio, 2) - descuento, 2);
                    }
                    else
                    {
                        subTotal = Decimal.Round(lineaFactura.Cantidad * lineaFactura.Precio, 2);

                    }
                    lineaFactura.Impuesto = Decimal.Round((impuesto.Rate * subTotal) / 100, 2);
                    lineaFactura.Total = Decimal.Round(subTotal + lineaFactura.Impuesto, 2);
                    // Unidad de Medida
                    if (myreader[8].ToString() != null)
                    {
                        lineaFactura.UnidadMedida = UnidadDeMedida(myreader[8].ToString());
                        if (lineaFactura.UnidadMedida == UnidadMedidaType.Otros)
                        {
                            lineaFactura.UnidadMedidaOtra = myreader[8].ToString();
                        }
                    }
                    else
                    {
                        lineaFactura.UnidadMedida = UnidadMedidaType.Unidad;
                    }
                    // Tipo de linea  Producto o Servicio
                    if (myreader[9] != DBNull.Value && myreader[9] != null && myreader[9].ToString().Length > 0)
                    {
                        if (myreader[9].ToString() == "T")
                        {
                            lineaFactura.Tipo = LineType.Service;
                        }
                        else
                        {
                            lineaFactura.Tipo = LineType.Product;
                        }
                    }
                    else
                    {
                        lineaFactura.Tipo = LineType.Product;
                    }
                    Linea++;
                    factura.InvoiceLines.Add(lineaFactura);
                }
                myreader.Close();
                #endregion
                //  Busca los detalles del pago que contiene la factura
                #region Formas de pago del Tiquete
                factura.ListPaymentType = new List<PaymentInvoce>();
                readCommand =
                  new FbCommand("Select ITIPOFCOBRO,ITIPOTRANSACCION,ITRANSACCION,MVALOR,QDIASCXC,INIT From OPRFCOBRO Where  INUMOPER =" + operacion.IdDocumento + "  and IEMP =" + operacion.IdEmpresa, myConnection);
                myreader = readCommand.ExecuteReader();
                int LineaPagos = 1;
                bool Credito = false;
                while (myreader.Read())
                {
                    PaymentInvoce formaPago;
                    switch (myreader[0].ToString())
                    {
                        // Credito
                        case "3":
                            Credito = true;
                            if (int.Parse(myreader[4].ToString()) == 0)
                            {
                                factura.CreditTerm = 1;
                            }
                            else
                            {
                                factura.CreditTerm = int.Parse(myreader[4].ToString());
                            }
                            break;
                        // Efectivo
                        case "1":
                            if (Credito == true)
                            {
                                throw new Exception("Una factura de crédito no puede tener pagos , Debe ser pagada en su totalidad o ser de crédito");
                            }
                            formaPago = new PaymentInvoce();
                            formaPago.Trans = null;
                            formaPago.Balance = Decimal.Round(decimal.Parse(myreader[3].ToString()),2);
                            formaPago.TypePayment = 0;
                            factura.ListPaymentType.Add(formaPago);
                            break;
                        // Deposito , Cheque, Tarjeta
                        case "2":
                            if (Credito == true)
                            {
                                throw new Exception("Una factura de crédito no puede tener pagos , Debe ser pagada en su totalidad o ser de crédito");
                            }
                            formaPago = new PaymentInvoce();
                            switch (myreader[1].ToString())
                            {
                                // Tarjeta de Débito o Crédito
                                case "TC":
                                    formaPago.TypePayment = 1;
                                    break;
                                case "TD":
                                    formaPago.TypePayment = 1;
                                    break;
                                // Cheque
                                case "CH":
                                    formaPago.TypePayment = 2;
                                    break;
                                case "CHF":
                                    formaPago.TypePayment = 2;
                                    break;
                                // Transferencia o Deposito
                                case "CE":
                                    formaPago.TypePayment = 3;
                                    break;
                                case "CI":
                                    formaPago.TypePayment = 3;
                                    break;
                                default:
                                    throw new Exception("Forma de pago invalida (Los códigos aceptados para pagos de banco son TC,TD,CH,CHF,CE,CI )");
                            }
                            formaPago.Trans = myreader[2].ToString();
                            if (myreader[3] != DBNull.Value && myreader[3] != null && myreader[3].ToString().Length > 0)
                            {
                                if (decimal.Parse(myreader[3].ToString()) > 0)
                                {
                                    formaPago.Balance = Decimal.Round(decimal.Parse(myreader[3].ToString()),2);
                                }
                                else
                                {
                                    throw new Exception("Linea " + LineaPagos.ToString() + " El monto del pago debe ser mayor a 0");
                                }
                            }
                            else
                            {
                                throw new Exception("Linea " + LineaPagos.ToString() + " Debe colocar el monto pagado");
                            }
                            LineaPagos++;
                            factura.ListPaymentType.Add(formaPago);
                            break;
                        default:
                            break;
                    }
                }
                myreader.Close();
                #endregion
                // Chequear que el monto pagado sea igual al total                
                decimal totalinvoice = decimal.Round(factura.InvoiceLines.Sum(x => x.Total), 2);
                if (factura.ListPaymentType.Count > 0 && Credito == true)
                {
                    throw new Exception("Los pagos parciales no son validos , la factura debe ser a crédito o se debe pagar en su totalidad");
                }
                if (factura.ListPaymentType.Count > 0 && Credito == false)
                {
                    decimal totalPayments = decimal.Round(factura.ListPaymentType.Sum(x => x.Balance), 2);
                    if (totalPayments != totalinvoice)
                    {
                        throw new Exception("El monto pagado no coincide con el total de la factura");
                    }
                }
                factura.TipoFirma = FirmType.Llave;
                // De aqui en adelante se define como tiquete
                #region Definicion del Tiquete
                factura.TypeDocument = DocumentType.Ticket;
                if (operacion.IdCliente != "1" && operacion.TipoDeOperacion == TipoOperacion.Ticket)
                {
                    if (cliente.Name != null)
                    {
                        factura.ClientName = cliente.Name;
                        if (cliente.LastName != null)
                        {
                            factura.ClientName = cliente.Name + " " + cliente.LastName;
                        }
                    }
                    if (cliente.IdentificationType == IdentificacionTypeTipo.NoAsignada)
                    {
                        if (cliente.IdentificacionExtranjero != null)
                        {
                            factura.ClientIdentification = cliente.IdentificacionExtranjero;
                        }
                    }
                    else
                    {
                        if (cliente.Identification != null)
                        {
                            factura.ClientIdentification = cliente.Identification;
                        }
                    }
                    if (cliente.Email != null)
                    {
                        factura.ClientEmail = cliente.Email;
                    }
                }
                #endregion
                return factura;
            }
            catch (Exception x)
            {
                // _eventos.Error("ContaPymeJob", "Elaborar Tiquete", "Imposible elaborar el Tiquete con la Data extraída de Contapyme: " + x.Message);
                throw x;
            }
            finally
            {
                myConnection.Close();
                factura = null;
            }
        }

        public Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            Ticopay ConexionTicopay = new Ticopay();

            // Obtengo Id de la Configuración del Job
            string parametros = dataMap.GetString("IdConfiguracion");
            Guid _idConfiguracion = Guid.Parse(parametros);
            Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == _idConfiguracion).Single();
            if (Job == null)
            {
                _eventos.Error("ContaPymeJob", "Ejecutar", "Imposible obtener configuración del Job");
                return Task.CompletedTask;
            }
            FbConnection myConnection = new FbConnection(Job.DatosConexion);
            myConnection.Open();
            try
            {
                // Preparo conexión para buscar documentos recientes                
                FbCommand readCommand;
                FbDataReader myreader;
                if (Job.FechaCreacion > DateTime.Now.AddDays(-7))
                {
                    // Busca los Documentos en el maestro de documentos OPRMAEST de el ultimo día
                    string Dias = Convert.ToInt32(Math.Round((DateTime.Now - Job.FechaCreacion).TotalDays, 0)).ToString();
                    readCommand =
                      new FbCommand("Select IEMP,ITDSOP,INUMOPER,INIT,BANULADA,IPROCESS,SNUMSOP From OPRMAEST Where FPROCESAM > dateadd (day, -"+ Dias + ", current_date) and FCREACION > dateadd (day, -30, current_date) and SVALORADIC1 is distinct from 'true' order by Fprocesam", myConnection);
                }
                else
                {
                    // Busca los Documentos en el maestro de documentos OPRMAEST de los últimos 7 días
                    readCommand =
                      new FbCommand("Select IEMP,ITDSOP,INUMOPER,INIT,BANULADA,IPROCESS,SNUMSOP From OPRMAEST Where FPROCESAM > dateadd (day, -7, current_date) and FCREACION > dateadd (day, -30, current_date) and SVALORADIC1 is distinct from 'true' order by Fprocesam", myConnection);
                }
                
                myreader = readCommand.ExecuteReader();
                string companyCode = null;
                string idDocumento = null;
                string idCliente = null;
                string token = ConexionTicopay.AutentificarUsuario(Job.SubDominioTicopay, Job.UsuarioTicopay, Job.ClaveTicopay);
                while (myreader.Read())
                {
                    try
                    {
                        // Empresa 
                        companyCode = myreader[0].ToString();
                        // ID Documento ContaPyme
                        idDocumento = myreader[2].ToString();
                        // ID Cliente ContaPyme
                        idCliente = myreader[3].ToString();
                        // Tipo Documento
                        // 10  Factura de Venta
                        // 20  Devolución de Venta
                        // 230 Nota de Crédito
                        // 220 Nota de Débito
                        // 
                        switch (myreader[1].ToString())
                        {
                            case "10":
                                if (myreader[5].ToString() == "2")
                                { // Es una factura procesada     
                                    if (_contexto.Operaciones.Where(t => t.IdDocumento == idDocumento && t.IdEmpresa == companyCode && t.TipoDeOperacion == TipoOperacion.Factura && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                                    {
                                        try
                                        {
                                            Invoice facturaExistente = ConexionTicopay.BuscarFacturaPorReferencia(myreader[6].ToString(), token);
                                            if (facturaExistente == null)
                                            {
                                                Operacion factura = new Operacion(idDocumento, idCliente, companyCode, TipoOperacion.Factura, Job);
                                                factura.ConsecutivoConector = myreader[6].ToString();
                                                _contexto.Operaciones.Add(factura);
                                            }
                                            facturaExistente = null;
                                        }
                                        catch (Exception ex)
                                        {
                                            _eventos.Error("ContaPymeJob", "Ejecutar " + Job.SubDominioTicopay, " Servicio de Ticopay Temporalmente no disponible: " + ex.Message);
                                        }   
                                    } 
                                    else
                                    {
                                        Operacion operacionACorregir = _contexto.Operaciones.Where(t => t.IdDocumento == idDocumento && t.IdEmpresa == companyCode && t.TipoDeOperacion == TipoOperacion.Factura && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).First();
                                        if(idCliente.Equals(operacionACorregir.IdCliente) == false && operacionACorregir.EstadoOperacion == Estado.Error)
                                        {
                                            operacionACorregir.IdCliente = idCliente;
                                            _contexto.Entry(operacionACorregir).CurrentValues.SetValues(operacionACorregir);
                                        }                                        
                                    }
                                }
                                if (myreader[5].ToString() == "2" && myreader[4].ToString() == "T")
                                { // Es una anulación de una factura

                                    if (_contexto.Operaciones.Where(t => t.IdDocumento == idDocumento && t.IdEmpresa == companyCode && t.TipoDeOperacion == TipoOperacion.Reverso && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                                    {
                                        Operacion nota = new Operacion(idDocumento, idCliente, companyCode, TipoOperacion.Reverso, Job, idDocumento);
                                        nota.ConsecutivoConector = myreader[6].ToString();
                                        _contexto.Operaciones.Add(nota);
                                    }
                                }
                                break;
                            case "20":
                                if (myreader[5].ToString() == "2")
                                { // Es una Devolución de Factura
                                    if (_contexto.Operaciones.Where(t => t.IdDocumento == idDocumento && t.IdEmpresa == companyCode && t.TipoDeOperacion == TipoOperacion.DevolucionFactura && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                                    {
                                        Operacion nota = new Operacion(idDocumento, idCliente, companyCode, TipoOperacion.DevolucionFactura, Job);
                                        nota.ConsecutivoConector = myreader[6].ToString();
                                        _contexto.Operaciones.Add(nota);
                                    }
                                }
                                break;
                            case "11":
                                if (myreader[5].ToString() == "2")
                                { // Es una tiquete procesada
                                    if (_contexto.Operaciones.Where(t => t.IdDocumento == idDocumento && t.IdEmpresa == companyCode && t.TipoDeOperacion == TipoOperacion.Ticket && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                                    {
                                        try
                                        {
                                            Invoice facturaExistente = ConexionTicopay.BuscarFacturaPorReferencia(myreader[6].ToString(), token);
                                            if (facturaExistente == null)
                                            {
                                                Operacion factura = new Operacion(idDocumento, idCliente, companyCode, TipoOperacion.Ticket, Job);
                                                factura.ConsecutivoConector = myreader[6].ToString();
                                                _contexto.Operaciones.Add(factura);
                                            }
                                            facturaExistente = null;
                                        }
                                        catch (Exception ex)
                                        {
                                            _eventos.Error("ContaPymeJob", "Ejecutar " + Job.SubDominioTicopay, " Servicio de Ticopay no disponible: " + ex.Message);
                                        }                                        
                                    }

                                    else
                                    {
                                        Operacion operacionACorregir = _contexto.Operaciones.Where(t => t.IdDocumento == idDocumento && t.IdEmpresa == companyCode && t.TipoDeOperacion == TipoOperacion.Ticket && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).First();
                                        if (idCliente.Equals(operacionACorregir.IdCliente) == false && operacionACorregir.EstadoOperacion == Estado.Error)
                                        {
                                            operacionACorregir.IdCliente = idCliente;
                                            _contexto.Entry(operacionACorregir).CurrentValues.SetValues(operacionACorregir);
                                        }
                                    }
                                }
                                break;
                            case "220":
                                // To Do
                                break;
                            case "230":
                                // To Do
                                break;
                        }
                        _contexto.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _eventos.Error("ContaPymeJob", "Ejecutar " + Job.SubDominioTicopay, "Error al Grabar una operación en la BD: " + ex.Message);
                    }
                }
                myreader.Close();                
            }
            catch(Exception ex)
            {
                _eventos.Error("ContaPymeJob", "Ejecutar " + Job.SubDominioTicopay, "Error al intentar leer las operaciones de Contapyme " + ex.Message);
            }
            finally
            {
                myConnection.Close();
            }
            List<Operacion> OperacionesAProcesar = _contexto.Operaciones.Where(O => O.EstadoOperacion == Estado.NoProcesado || O.EstadoOperacion == Estado.Error).ToList();
            List<Operacion> OperacionesDelConector = OperacionesAProcesar.Where(O => O.Configuracion.Id == Job.Id).OrderBy(O => O.IdOperacion).ToList();
            if (OperacionesDelConector.Count > 0)
            {
                ProcesarOperaciones(OperacionesDelConector, Job);
            }
            else
            {
                _eventos.Advertencia("ContaPymeJob", "Ejecutar " + Job.SubDominioTicopay, "No existen Operaciones por Procesar");
            }
            _contexto.Dispose();
            return Task.CompletedTask;
        }

        public Client IngresarCliente(Operacion operacion, Configuracion configuracion)
        {
            // Inicialización de Variables
            Ticopay _conexionTicopay = new Ticopay();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            Client cliente = null;
            // Inicio sesion en Ticopay
            string _token = _conexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
            // Busco si el Cliente ya existe en Ticopay
            cliente = _conexionTicopay.BuscarCliente(_token, false, operacion.IdCliente.Replace("-", string.Empty).Trim());
            if(cliente != null)
            {
                return cliente;
            }
            // Si el Cliente no existe , Busco los datos en ContaPyme
            FbConnection myConnection = new FbConnection(configuracion.DatosConexion);
            try
            {
                // Consulta de los datos del cliente en la Bd de ContaPyme
                string consulta = "Select NTERCERO,NAPELLIDO,SEMAILFE,ITDDOCUM,QDIASPLAZOCXC,INIT,BEMPRESA From ABANITS where Init = '" + operacion.IdCliente + "'";
                myConnection.Open();
                FbCommand readCommand = new FbCommand(consulta, myConnection);
                FbDataReader myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    cliente = new Client();                    
                    switch (myreader[3].ToString())
                    {
                        // Códigos de Tipo de cliente según ContaPyme , Tabla Abadtit
                        // 13  Cédula Física
                        // 22  Cédula de extranjería (DIMEX)
                        // 31  Cédula Jurídica
                        // 41  Pasaporte
                        // 51  NITE
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
                        case "51":
                            cliente.IdentificationType = IdentificacionTypeTipo.NITE;
                            break;
                        default:
                            if(operacion.TipoDeOperacion != TipoOperacion.Ticket)
                            {
                                throw new Exception("Debe seleccionar un tipo de identificación valida");
                            }
                            break;                            
                    }
                    if(cliente.IdentificationType != IdentificacionTypeTipo.Cedula_Juridica)
                    {
                        if (operacion.TipoDeOperacion != TipoOperacion.Ticket)
                        {
                            if(myreader[0] != DBNull.Value && myreader[0] != null && myreader[0].ToString().Length > 0)
                            {
                                cliente.Name = myreader[0].ToString();
                            }
                            else
                            {
                                throw new Exception("Faltan datos del cliente : Debe colocar el Nombre del Cliente");
                            }
                            if (myreader[1] != DBNull.Value && myreader[1] != null && myreader[1].ToString().Length > 0)
                            {
                                cliente.LastName = myreader[1].ToString();
                            }
                            else
                            {
                                throw new Exception("Faltan datos del cliente : Debe colocar el Apellido del Cliente");
                            }
                        }
                        else
                        {
                            cliente.Name = myreader[0].ToString();
                            cliente.LastName = myreader[1].ToString();                            
                        }                            
                    }
                    else
                    {
                        if (operacion.TipoDeOperacion != TipoOperacion.Ticket)
                        {
                            if (myreader[0] != DBNull.Value && myreader[0] != null && myreader[0].ToString().Length > 0)
                            {
                                cliente.Name = myreader[0].ToString();
                            }
                            else
                            {
                                throw new Exception("Faltan datos del cliente : Debe colocar el Nombre de la Empresa");
                            }
                        }
                        else
                        {
                            cliente.Name = myreader[0].ToString();
                            cliente.LastName = myreader[1].ToString();
                        }
                    }
                    if(operacion.IdCliente.Replace("-", string.Empty).Trim() != "1" && operacion.TipoDeOperacion != TipoOperacion.Ticket)
                    {
                        if(cliente.IdentificationType == IdentificacionTypeTipo.NoAsignada)
                        {
                            if (operacion.IdCliente.Replace("-", string.Empty).Trim().Length < 5)
                            {
                                throw new Exception("Faltan datos del cliente : El numero de identificación Extranjero debe tener al menos 5 caracteres");
                            }
                            else
                            {
                                cliente.IdentificacionExtranjero = operacion.IdCliente.Replace("-", string.Empty).Trim();
                            }
                        }
                        if (cliente.IdentificationType == IdentificacionTypeTipo.Cedula_Fisica)
                        {
                            if (operacion.IdCliente.Replace("-", string.Empty).Trim().Length != 9 || operacion.IdCliente.Replace("-", string.Empty).Trim().StartsWith("0"))
                            {
                                throw new Exception("Faltan datos del cliente : El numero de identificación Física debe ser de 9 caracteres, y no puede comenzar con 0");
                            }
                            else
                            {
                                if(Regex.Matches(operacion.IdCliente.Replace("-", string.Empty).Trim(), @"[a-zA-Z]").Count == 0)
                                {
                                    cliente.Identification = operacion.IdCliente.Replace("-", string.Empty).Trim();
                                }
                                else
                                {
                                    throw new Exception("Faltan datos del cliente : El numero de identificación no puede contener letras");
                                }                                
                            }
                        }
                        if (cliente.IdentificationType == IdentificacionTypeTipo.Cedula_Juridica)
                        {
                            if (operacion.IdCliente.Replace("-", string.Empty).Trim().Length != 10)
                            {
                                throw new Exception("Faltan datos del cliente : El numero de identificación Jurídico debe ser de 10 caracteres");
                            }
                            else
                            {
                                if (Regex.Matches(operacion.IdCliente.Replace("-", string.Empty).Trim(), @"[a-zA-Z]").Count == 0)
                                {
                                    cliente.Identification = operacion.IdCliente.Replace("-", string.Empty).Trim();
                                }
                                else
                                {
                                    throw new Exception("Faltan datos del cliente : El numero de identificación no puede contener letras");
                                }
                            }
                        }
                        if (cliente.IdentificationType == IdentificacionTypeTipo.DIMEX)
                        {
                            if ((operacion.IdCliente.Replace("-", string.Empty).Trim().Length < 11 && operacion.IdCliente.Replace("-", string.Empty).Trim().Length > 12) || operacion.IdCliente.Replace("-", string.Empty).Trim().StartsWith("0"))
                            {
                                throw new Exception("Faltan datos del cliente : El numero de identificación DIMEX debe tener entre 11 y 12 caracteres, y no puede comenzar con 0");
                            }
                            else
                            {
                                if (Regex.Matches(operacion.IdCliente.Replace("-", string.Empty).Trim(), @"[a-zA-Z]").Count == 0)
                                {
                                    cliente.Identification = operacion.IdCliente.Replace("-", string.Empty).Trim();
                                }
                                else
                                {
                                    throw new Exception("Faltan datos del cliente : El numero de identificación no puede contener letras");
                                }
                            }
                        }
                        if (cliente.IdentificationType == IdentificacionTypeTipo.NITE)
                        {
                            if (operacion.IdCliente.Replace("-", string.Empty).Trim().Length != 10)
                            {
                                throw new Exception("Faltan datos del cliente : El numero de identificación NITE debe ser de 10 caracteres");
                            }
                            else
                            {
                                if (Regex.Matches(operacion.IdCliente.Replace("-", string.Empty).Trim(), @"[a-zA-Z]").Count == 0)
                                {
                                    cliente.Identification = operacion.IdCliente.Replace("-", string.Empty).Trim();
                                }
                                else
                                {
                                    throw new Exception("Faltan datos del cliente : El numero de identificación no puede contener letras");
                                }
                            }
                        }
                    }
                    if (myreader[2] != DBNull.Value && myreader[2] != null && myreader[2].ToString().Length > 0)
                    {
                        cliente.Email = myreader[2].ToString();
                    }
                    else
                    {
                        if (operacion.TipoDeOperacion != TipoOperacion.Ticket)
                        {
                            throw new Exception("Faltan datos del cliente : Debe colocar una dirección de email en el Campo Email FE");
                        }
                        else
                        {
                            cliente.Email = myreader[2].ToString();
                        }
                    }                                           
                    if (myreader[4] != DBNull.Value && myreader[4] != null && myreader[4].ToString().Length > 0)
                    {
                        cliente.CreditDays = int.Parse(myreader[4].ToString());
                    }
                    else
                    {
                        cliente.CreditDays = 1;
                    }
                }
                myreader.Close();
                if(operacion.TipoDeOperacion != TipoOperacion.Ticket)
                {
                    try
                    {
                        cliente = _conexionTicopay.CrearCliente(_token, cliente);
                    }
                    catch(Exception ex)
                    {
                        // _eventos.Error("ContaPymeJob", "Ingresar Cliente", "Imposible Crear el Cliente en Ticopay: " + ex.Message);
                        throw ex;
                    }                    
                }
                if (cliente != null)
                {
                    return cliente;
                }
                else
                {
                    // _eventos.Error("ContaPymeJob", "Ingresar Cliente", "Imposible Crear el Cliente en Ticopay);
                    throw new Exception("Imposible Crear el Cliente en Ticopay");
                }                
            }
            catch (Exception x)
            {
                // _eventos.Error("ContaPymeJob", "Ingresar Cliente", "Imposible obtener los datos del cliente de la BD de ContaPyme");
                throw x;
            }
            finally
            {
                myConnection.Close();
            }
        }

        public void ProcesarOperaciones(List<Operacion> operacionesPendientes, Configuracion configuracion)
        {
            // Inicialización de Variables
            Ticopay ConexionTicopay = new Ticopay();
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            RegistroDeEventos _eventos = new RegistroDeEventos();

            if (ConexionTicopay.VerificarPermisoConector(configuracion.SubDominioTicopay,configuracion.TipoConector.ToString()) == false)
            {
                _eventos.Error("ContaPymeJob", "Procesar Operaciones", "Sub Dominio no tiene permiso de usar el Conector");
                return;
            }
            string token = null;
            foreach (Operacion operacionAProcesar in operacionesPendientes)
            {
                Client cliente = null;
                CreateInvoice factura = null;
                Invoice facturaEnviada = null;
                CreateInvoice tiquete = null;
                Invoice tiqueteEnviado = null;
                CompleteNote nota = null;
                CompleteNote notaEnviada = null;
                Invoice facturaCredito = null;
                switch (operacionAProcesar.TipoDeOperacion)
                {
                    case TipoOperacion.Factura:
                        try
                        {
                            token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                            Invoice facturaExistente = ConexionTicopay.BuscarFacturaPorReferencia(operacionAProcesar.ConsecutivoConector, token);
                            if (facturaExistente == null)
                            {
                                cliente = IngresarCliente(operacionAProcesar, configuracion);
                                if (cliente != null)
                                {
                                    factura = ElaborarFactura(operacionAProcesar, configuracion, cliente);
                                    // token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                                    facturaEnviada = ConexionTicopay.EnviarFactura(token, factura);
                                }
                                if (facturaEnviada != null)
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                    operacionAProcesar.IdTicopayDocument = facturaEnviada.Id;
                                    operacionAProcesar.ConsecutivoTicopay = facturaEnviada.ConsecutiveNumber;
                                    operacionAProcesar.VoucherTicopay = facturaEnviada.VoucherKey;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                            }
                            else
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                operacionAProcesar.IdTicopayDocument = facturaExistente.Id;
                                operacionAProcesar.ConsecutivoTicopay = facturaExistente.ConsecutiveNumber;
                                operacionAProcesar.VoucherTicopay = facturaExistente.VoucherKey;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                                Invoice facturaExistente = ConexionTicopay.BuscarFacturaPorReferencia(operacionAProcesar.ConsecutivoConector, token);
                                if (facturaExistente == null)
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Error;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                    operacionAProcesar.Errores = ex.Message;
                                }
                                else
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                    operacionAProcesar.IdTicopayDocument = facturaExistente.Id;
                                    operacionAProcesar.ConsecutivoTicopay = facturaExistente.ConsecutiveNumber;
                                    operacionAProcesar.VoucherTicopay = facturaExistente.VoucherKey;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                            }
                            catch(Exception exEnvio)
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Error;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                                operacionAProcesar.Errores = "Servicio de Ticopay no disponible";
                            }                                                   
                        }
                        break;
                    case TipoOperacion.Ticket:
                        try
                        {
                            token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                            Invoice tiqueteExistente = ConexionTicopay.BuscarFacturaPorReferencia(operacionAProcesar.ConsecutivoConector, token);
                            if (tiqueteExistente == null)
                            {
                                cliente = IngresarCliente(operacionAProcesar, configuracion);
                                if (cliente != null)
                                {
                                    tiquete = ElaborarTiquete(operacionAProcesar, configuracion, cliente);
                                    // token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                                    tiqueteEnviado = ConexionTicopay.EnviarTiquete(token, tiquete);
                                }
                                if (tiqueteEnviado != null)
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                    operacionAProcesar.IdTicopayDocument = tiqueteEnviado.Id;
                                    operacionAProcesar.ConsecutivoTicopay = tiqueteEnviado.ConsecutiveNumber;
                                    operacionAProcesar.VoucherTicopay = tiqueteEnviado.VoucherKey;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                            }
                            else
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                operacionAProcesar.IdTicopayDocument = tiqueteExistente.Id;
                                operacionAProcesar.ConsecutivoTicopay = tiqueteExistente.ConsecutiveNumber;
                                operacionAProcesar.VoucherTicopay = tiqueteExistente.VoucherKey;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                token = ConexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                                Invoice tiqueteExistente = ConexionTicopay.BuscarFacturaPorReferencia(operacionAProcesar.ConsecutivoConector, token);
                                if (tiqueteExistente == null)
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Error;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                    operacionAProcesar.Errores = ex.Message;
                                }
                                else
                                {
                                    operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                    operacionAProcesar.IdTicopayDocument = tiqueteExistente.Id;
                                    operacionAProcesar.ConsecutivoTicopay = tiqueteExistente.ConsecutiveNumber;
                                    operacionAProcesar.VoucherTicopay = tiqueteExistente.VoucherKey;
                                    operacionAProcesar.EnviadoEl = DateTime.Now;
                                }
                            }
                            catch
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Error;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                                operacionAProcesar.Errores = "Servicio de Ticopay no disponible";
                            }                                          
                        }                        
                        break;
                    case TipoOperacion.Reverso:
                        try
                        {
                            notaEnviada = ElaborarNotaCreditoReverso(operacionAProcesar, configuracion);                            
                            if (notaEnviada != null)
                            {
                                operacionAProcesar.IdDocumentoAfectado = notaEnviada.InvoiceId.ToString();
                                operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                operacionAProcesar.IdTicopayDocument = notaEnviada.Id.ToString();
                                operacionAProcesar.ConsecutivoTicopay = notaEnviada.ConsecutiveNumber;
                                operacionAProcesar.VoucherTicopay = notaEnviada.VoucherKey;
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
                    case TipoOperacion.NotaDebito:
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
                            }
                            else
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Error;
                                operacionAProcesar.EnviadoEl = DateTime.Now;
                                operacionAProcesar.Errores = "Error al Enviar la nota a Ticopay";
                            }
                        }
                        catch(Exception ex)
                        {
                            operacionAProcesar.EstadoOperacion = Estado.Error;
                            operacionAProcesar.EnviadoEl = DateTime.Now;
                            operacionAProcesar.Errores = ex.Message;
                        }
                        
                        break;
                    case TipoOperacion.PagoFactura:
                        break;
                }
                Operacion actualizarOperacion = _contexto.Operaciones.ToList().Where(op => op.IdOperacion == operacionAProcesar.IdOperacion).First();
                _contexto.Entry(actualizarOperacion).CurrentValues.SetValues(operacionAProcesar);
                _contexto.SaveChanges();
            }
            _contexto.Dispose();
        }

        public UnidadMedidaType UnidadDeMedida(string siglas)
        {
            switch (siglas)
            {
                case "Sp":
                    return UnidadMedidaType.Servicios_Profesionales;
                case "m":
                    return UnidadMedidaType.Metro;
                case "kg":
                    return UnidadMedidaType.Kilogramo;
                case "s":
                    return UnidadMedidaType.Segundo;
                case "A":
                    return UnidadMedidaType.Ampere;
                case "K":
                    return UnidadMedidaType.Kelvin;
                case "mol":
                    return UnidadMedidaType.Mol;
                case "cd":
                    return UnidadMedidaType.Candela;
                case "m²":
                    return UnidadMedidaType.Metro_Cuadrado;
                case "m³":
                    return UnidadMedidaType.Metro_Cubico;
                case "m/s":
                    return UnidadMedidaType.Metro_por_Segundo;
                case "m/s²":
                    return UnidadMedidaType.Metro_por_Segundo_Cuadrado;
                case "1/m":
                    return UnidadMedidaType.Uno_por_Metro;
                case "kg/m³":
                    return UnidadMedidaType.kilogramo_por_Metro_Cubico;
                case "A/m²":
                    return UnidadMedidaType.Ampere_por_Metro_Cuadrado;
                case "A/m":
                    return UnidadMedidaType.Ampere_por_Metro;
                case "mol/m³":
                    return UnidadMedidaType.Mol_por_Metro_Cubico;
                case "cd/m²":
                    return UnidadMedidaType.Candela_por_Metro_Cuadrado;
                case "1":
                    return UnidadMedidaType.Uno;
                case "rad":
                    return UnidadMedidaType.Radian;
                case "sr":
                    return UnidadMedidaType.Estereorradian;
                case "Hz":
                    return UnidadMedidaType.Hertz;
                case "N":
                    return UnidadMedidaType.Newton;
                case "Pa":
                    return UnidadMedidaType.Pascal;
                case "J":
                    return UnidadMedidaType.Joule;
                case "W":
                    return UnidadMedidaType.Watt;
                case "C":
                    return UnidadMedidaType.Coulomb;
                case "V":
                    return UnidadMedidaType.Volt;
                case "F":
                    return UnidadMedidaType.Farad;
                case "Ω":
                    return UnidadMedidaType.Ohm;
                case "S":
                    return UnidadMedidaType.Siemens;
                case "Wb":
                    return UnidadMedidaType.Weber;
                case "T":
                    return UnidadMedidaType.Tesla;
                case "H":
                    return UnidadMedidaType.Henry;
                case "°C":
                    return UnidadMedidaType.Grado_Celsius;
                case "lm":
                    return UnidadMedidaType.Lumen;
                case "lx":
                    return UnidadMedidaType.Lux;
                case "Bq":
                    return UnidadMedidaType.Becquerel;
                case "Gy":
                    return UnidadMedidaType.Gray;
                case "Sv":
                    return UnidadMedidaType.Sievert;
                case "kat":
                    return UnidadMedidaType.Katal;
                case "Pa·s":
                    return UnidadMedidaType.Pascal_Segundo;
                case "N·m":
                    return UnidadMedidaType.Newton_Metro;
                case "N/m":
                    return UnidadMedidaType.Newton_por_Metro;
                case "rad/s":
                    return UnidadMedidaType.Radian_por_Segundo;
                case "rad/s²":
                    return UnidadMedidaType.Radian_por_Segund_Cuadrado;
                case "W/m²":
                    return UnidadMedidaType.Watt_por_Metro_Cuadrado;
                case "J/K":
                    return UnidadMedidaType.Joule_por_Kelvin;
                case "J/(kg·K)":
                    return UnidadMedidaType.Joule_por_Kilogramo_Kelvin;
                case "J/kg":
                    return UnidadMedidaType.Joule_por_Kilogramo;
                case "W/(m·K)":
                    return UnidadMedidaType.Watt_por_Metro_Kevin;
                case "J/m³":
                    return UnidadMedidaType.Joule_por_Metro_Cubico;
                case "V/m":
                    return UnidadMedidaType.Volt_por_Metro;
                case "C/m³":
                    return UnidadMedidaType.Coulomb_por_Metro_Cubico;
                case "C/m²":
                    return UnidadMedidaType.Coulomb_por_Metro_Cuadrado;
                case "F/m":
                    return UnidadMedidaType.Farad_por_Metro;
                case "H/m":
                    return UnidadMedidaType.Henry_por_Metro;
                case "J/mol":
                    return UnidadMedidaType.Joule_por_Mol;
                case "J/(mol·K)":
                    return UnidadMedidaType.Joule_por_Mol_Kelvin;
                case "C/kg":
                    return UnidadMedidaType.Coulomb_por_Kilogramo;
                case "Gy/s":
                    return UnidadMedidaType.Gray_por_Segundo;
                case "W/sr":
                    return UnidadMedidaType.Watt_por_Estereorradian;
                case "W/(m²·sr)":
                    return UnidadMedidaType.Watt_por_Metro_Cuadrado_Estereorradian;
                case "kat/m³":
                    return UnidadMedidaType.Katal_por_Metro_Cubico;
                case "min":
                    return UnidadMedidaType.Minuto;
                case "h":
                    return UnidadMedidaType.Hora;
                case "d":
                    return UnidadMedidaType.Dia;
                case "º":
                    return UnidadMedidaType.Grado;
                case "´":
                    return UnidadMedidaType.Minuto_;
                case "´´":
                    return UnidadMedidaType.Segundo_;
                case "L":
                    return UnidadMedidaType.Litro;
                case "t":
                    return UnidadMedidaType.Tonelada;
                case "Np":
                    return UnidadMedidaType.Neper;
                case "B":
                    return UnidadMedidaType.Bel;
                case "eV":
                    return UnidadMedidaType.Electronvolt;
                case "u":
                    return UnidadMedidaType.Unidad_de_Masa_Atomica_Unificada;
                case "ua":
                    return UnidadMedidaType.Unidad_Astronomica;
                case "Unid":
                    return UnidadMedidaType.Unidad;
                case "Gal":
                    return UnidadMedidaType.Galon;
                case "g":
                    return UnidadMedidaType.Gramo;
                case "Km":
                    return UnidadMedidaType.Kilometro;
                case "ln":
                    return UnidadMedidaType.Pulgada;
                case "cm":
                    return UnidadMedidaType.Centimetro;
                case "mL":
                    return UnidadMedidaType.Mililitro;
                case "mm":
                    return UnidadMedidaType.Milimetro;
                case "Oz":
                    return UnidadMedidaType.Onzas;
                default:
                    return UnidadMedidaType.Otros;
            }
        }
    }
}
