using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Taxes;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Invoices;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Items;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Opetations;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Security;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Responses;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Responses.Operations;
using TicopayUniversalConnectorService.ConexionTicopay;
using TicopayUniversalConnectorService.Contexto;
using TicopayUniversalConnectorService.Entities;
using TicopayUniversalConnectorService.Interfaces;
using TicopayUniversalConnectorService.Log;
using static TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Opetations.Operation;

namespace TicopayUniversalConnectorService.Conectores
{
    public class TicopayContaPymeJob : IConector
    {
        public Tax BuscarImpuesto(Operacion operacion, Configuracion configuracion, string porcentajeImpuesto = null, string nombreImpuesto = null)
        {
            throw new NotImplementedException();
        }

        public CreateInvoice ElaborarFactura(Operacion operacion, Configuracion configuracion, Client cliente)
        {
            throw new NotImplementedException();
        }

        public CompleteNote ElaborarNotaCreditoDevolucion(Operacion operacion, Configuracion configuracion)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            UniversalConnectorDB _contexto = new UniversalConnectorDB();
            RegistroDeEventos _eventos = new RegistroDeEventos();
            Ticopay _conexionTicopay = new Ticopay();

            // Obtengo Id de la Configuración del Job
            string parametros = dataMap.GetString("IdConfiguracion");
            Guid _idConfiguracion = Guid.Parse(parametros);
            Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == _idConfiguracion).Single();
            if (Job == null)
            {
                _eventos.Error("TicopayContapyme", "Ejecutar " + Job.UsuarioTicopay, "Imposible obtener configuración del Job");
                return Task.CompletedTask;
            }
            // Inicio sesion en Ticopay
            string _token = _conexionTicopay.AutentificarUsuario(Job.SubDominioTicopay, Job.UsuarioTicopay, Job.ClaveTicopay);
            try
            {
                // Extraigo las Facturas y tiquetes
                List<Invoice> facturas = new List<Invoice>();
                if(Job.FechaCreacion > DateTime.Now.AddDays(-7))
                {
                    facturas = _conexionTicopay.BuscarFacturas(Job.FechaCreacion , _token);
                }
                else
                {
                    facturas = _conexionTicopay.BuscarFacturas(DateTime.Now.AddDays(-7), _token);
                }
                if(facturas != null)
                {
                    foreach (Invoice factura in facturas)
                    {
                        if (factura.ExternalReferenceNumber == null || factura.ExternalReferenceNumber == "N/A" || factura.ExternalReferenceNumber == "")
                        {
                            if (_contexto.Operaciones.Where(t => t.IdDocumento == factura.Id && t.IdEmpresa == Job.IdEmpresa && t.TipoDeOperacion == TipoOperacion.Factura && t.Configuracion.SubDominioTicopay == Job.SubDominioTicopay).Count() == 0)
                            {
                                Operacion oFactura = null;
                                if (factura.typeDocument == DocumentType.Invoice)
                                {
                                    oFactura = new Operacion(factura.Id, factura.ClientId.ToString(), Job.IdEmpresa, TipoOperacion.Factura, Job);
                                }
                                else
                                {
                                    oFactura = new Operacion(factura.Id, factura.ClientId.ToString(), Job.IdEmpresa, TipoOperacion.Ticket, Job);
                                }
                                oFactura.ConsecutivoTicopay = factura.ConsecutiveNumber;
                                oFactura.VoucherTicopay = factura.VoucherKey;
                                oFactura.IdTicopayDocument = factura.Id;
                                _contexto.Operaciones.Add(oFactura);
                                _contexto.SaveChanges();
                            }
                        }
                    }
                    facturas.Clear();

                }
                else
                {
                    _eventos.Advertencia("TicopayContapyme", "Ejecutar " + Job.UsuarioTicopay, "No existen facturas en Ticopay");
                }                
            }
            catch (Exception ex)
            {
                _eventos.Error("TicopayContapyme" , "Ejecutar " + Job.UsuarioTicopay, "Error al extraer la información de las Facturas -> " + ex.Message);
            }
            //try
            //{
            //    Extraigo las Notas de Crédito
            //    List<Note> notas = new List<Note>();
            //    if (Job.FechaCreacion > DateTime.Now.AddDays(-7))
            //    {
            //        notas = _conexionTicopay.BuscarNotas(Job.FechaCreacion, _token);
            //    }
            //    else
            //    {
            //        notas = _conexionTicopay.BuscarNotas(DateTime.Now.AddDays(-7), _token);
            //    }
            //    foreach (Note nota in notas)
            //    {
            //        if (_contexto.Operaciones.Where(t => t.IdDocumento == nota.Id && t.IdEmpresa == Job.IdEmpresa && t.TipoDeOperacion == TipoOperacion.Reverso && t.Configuracion.TipoConector == Job.TipoConector).Count() == 0)
            //        {
            //            Operacion oNota = new Operacion(nota.Id, nota.InvoiceId, Job.IdEmpresa, TipoOperacion.Reverso, Job);
            //            _contexto.Operaciones.Add(oNota);
            //        }
            //    }
            //    notas.Clear();
            //}
            //catch (Exception ex)
            //{
            //    _eventos.Error("TicopayContapyme " + Job.UsuarioTicopay, "Ejecutar", "Error al extraer la información de las Notas -> " + ex.Message);
            //}
            List<Operacion> OperacionesAProcesar = _contexto.Operaciones.Where(O => O.EstadoOperacion == Estado.NoProcesado || O.EstadoOperacion == Estado.Error).ToList();
            List<Operacion> OperacionesDelConector = OperacionesAProcesar.Where(O => O.Configuracion.Id == Job.Id).OrderBy(O => O.IdOperacion).ToList();
            if (OperacionesDelConector.Count > 0)
            {
                ProcesarOperaciones(OperacionesDelConector, Job);
            }
            else
            {
                _eventos.Advertencia("TicopayContapyme", "Ejecutar " + Job.UsuarioTicopay, "No existen Operaciones por Procesar");
            }
            _contexto.Dispose();
            return Task.CompletedTask;
        }

        public Client IngresarCliente(Operacion operacion, Configuracion configuracion)
        {
            RegistroDeEventos _eventos = new RegistroDeEventos();
            try
            {
                
                Ticopay _conexionTicopay = new Ticopay();
                // Inicio sesion en Ticopay
                string _token = _conexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                Client cliente = _conexionTicopay.BuscarCliente(_token, true, operacion.IdCliente);
                if (cliente != null)
                {
                    return cliente;
                }
                else
                {
                    _eventos.Error("TicopayContaPyme", "Buscar Cliente " + configuracion.UsuarioTicopay, "Cliente no encontrado");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _eventos.Error("TicopayContaPyme", "Buscar Cliente " + configuracion.UsuarioTicopay, "Error al extraer la información del Cliente -> " + ex.Message);
                return null;
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
                _eventos.Error("TicopayContapyme", "Procesar Operaciones " + configuracion.UsuarioTicopay, "Sub Dominio no tiene permiso de usar el Conector");
                return;
            }
            string token = null;
            foreach (Operacion operacionAProcesar in operacionesPendientes)
            {
                string facturaEnviada = null;
                switch (operacionAProcesar.TipoDeOperacion)
                {
                    case TipoOperacion.Factura:
                        try
                        {
                            facturaEnviada = EnviarFactura(operacionAProcesar, configuracion);
                            if (facturaEnviada != null)
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                operacionAProcesar.ConsecutivoConector = facturaEnviada;
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
                            facturaEnviada = EnviarFactura(operacionAProcesar, configuracion,true);
                            if (facturaEnviada != null)
                            {
                                operacionAProcesar.EstadoOperacion = Estado.Procesado;
                                operacionAProcesar.ConsecutivoConector = facturaEnviada;
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
                    case TipoOperacion.Reverso:
                        break;
                    case TipoOperacion.NotaDebito:
                        break;
                    case TipoOperacion.DevolucionFactura:
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

        public string EnviarFactura(Operacion operacion, Configuracion configuracion, bool tiquete = false)
        {
            Ticopay _conexionTicopay = new Ticopay();
            string _token = null;
            Invoice ticopayInvoice = null;
            Tax[] ticopayTaxes = null;
            GetItems _productos = null;
            string[] argumentos = new string[] { };
            argumentos = configuracion.DatosConexion.Split(';'); // Convert.ToChar(30)
            string _direccionIpApi = argumentos[0];
            string _emailUsuario = argumentos[1];
            string _claveUsuario = argumentos[2];
            string _idMaquina = argumentos[3];
            string _idApp = argumentos[4];
            string _codCentroCostos = argumentos[6];
            string _codLiquidacionIva = argumentos[7];
            string _codCaja = argumentos[8];
            string _codBanco = argumentos[9];
            string _codCxC = argumentos[10];
            try
            {
                // Busco la factura especifica en Ticopay
                try
                {
                    _token = _conexionTicopay.AutentificarUsuario(configuracion.SubDominioTicopay, configuracion.UsuarioTicopay, configuracion.ClaveTicopay);
                    ticopayInvoice = _conexionTicopay.BuscarFactura(operacion.IdDocumento, _token);
                    ticopayTaxes = _conexionTicopay.BuscarImpuestos(_token);
                }
                catch (Exception ExcepcionTicopay)
                {
                    throw new Exception("Error al buscar la factura en Ticopay: " + ExcepcionTicopay.Message);
                }
                // Abro sesion en Contapyme y armo la factura a Enviar
                MethodResponse respuestaServicio;
                LoginJson credenciales = new LoginJson();
                try
                {
                    credenciales.dataJSON = new LoginInformation(_emailUsuario, _claveUsuario, _idMaquina);
                    credenciales.controlkey = "";
                    credenciales.iapp = _idApp;
                    Random rnd = new Random();
                    credenciales.random = rnd.Next(300, 300000).ToString();
                    respuestaServicio = ContaPymeApi.Authenticate(_direccionIpApi, credenciales).GetAwaiter().GetResult();
                    GetAuthResult responseArray = JsonConvert.DeserializeObject<GetAuthResult>(respuestaServicio.result);
                    credenciales.controlkey = responseArray.result[0].respuesta.datos.keyagente;
                }
                catch (Exception exLogin)
                {
                    throw new Exception("Imposible ingresar al Api de Contapyme: " + exLogin.Message);
                }
                try
                {
                    OperationJson ConsultarProductos = new OperationJson();
                    ConsultarProductos.controlkey = credenciales.controlkey;
                    ConsultarProductos.iapp = credenciales.iapp;
                    ConsultarProductos.random = credenciales.random;
                    RequestParameters peticionProductos = new RequestParameters();
                    peticionProductos.datospagina = new PageData();
                    peticionProductos.datospagina.cantidadregistros = "9999";
                    peticionProductos.datospagina.pagina = "1";
                    peticionProductos.camposderetorno = new string[] { "irecurso", "nrecurso" };
                    ConsultarProductos.dataJSON = peticionProductos;
                    respuestaServicio = ContaPymeApi.GetItems(_direccionIpApi, ConsultarProductos).GetAwaiter().GetResult();
                    BasicOperationResponse ListadoProductos = JsonConvert.DeserializeObject<BasicOperationResponse>(respuestaServicio.result);
                    string datos = "{\"datos\":" + ListadoProductos.result[0].respuesta.datos.ToString() + "}";
                    _productos = JsonConvert.DeserializeObject<GetItems>(datos);
                }
                catch(Exception exBusquedaProductos)
                {
                    throw new Exception("Imposible extraer data de productos del Api de Contapyme: " + exBusquedaProductos.Message);
                }
                // Armar la factura basado en lo obtenido de ticopay
                OperationJson CrearFactura = new OperationJson();
                CrearFactura.controlkey = credenciales.controlkey;
                CrearFactura.iapp = credenciales.iapp;
                CrearFactura.random = credenciales.random;
                ComplexOperationInfo Factura = null;
                AddInvoice datosPeticion = null;
                if (operacion.TipoDeOperacion == TipoOperacion.Factura)
                {
                    Factura = new ComplexOperationInfo("CREATE", "ING1");
                }
                else
                {
                    Factura = new ComplexOperationInfo("CREATE", "ING5");
                }
                try
                {
                    int cantpagosEfectivo = ticopayInvoice.InvoicePaymentTypes.Count(e => e.PaymetnMethodType == PaymetnMethodType.Cash);
                    int cantpagosBanco = ticopayInvoice.InvoicePaymentTypes.Count(e => e.PaymetnMethodType != PaymetnMethodType.Cash);
                    int cantidadArticulosConImpuesto = ticopayInvoice.InvoiceLines.Count(x => x.TaxAmount != 0);
                    if(ticopayInvoice.InvoicePaymentTypes.Count == 0)
                    {
                        datosPeticion = new AddInvoice(ticopayInvoice.InvoiceLines.Count, 0, 0, 1, cantidadArticulosConImpuesto);
                    }
                    else
                    {
                        datosPeticion = new AddInvoice(ticopayInvoice.InvoiceLines.Count, cantpagosEfectivo, cantpagosBanco, 0, cantidadArticulosConImpuesto);
                    }           
                    #region Encabezado de factura
                    datosPeticion.encabezado.iemp = operacion.IdEmpresa;
                    if (operacion.TipoDeOperacion == TipoOperacion.Factura)
                    {
                        datosPeticion.encabezado.itdsop = "10";
                    }
                    else
                    {
                        datosPeticion.encabezado.itdsop = "11";
                    }
                    datosPeticion.encabezado.fsoport = ticopayInvoice.DueDate.ToString("MM/dd/yyyy"); // "08/31/2018";
                    datosPeticion.encabezado.svaloradic1 = "true";
                    datosPeticion.encabezado.svaloradic2 = ticopayInvoice.ConsecutiveNumber;
                    datosPeticion.encabezado.banulada = "F";
                    datosPeticion.encabezado.inumsop = "0";
                    datosPeticion.encabezado.snumsop = "<AUTO>";
                    #endregion
                    #region Datos Principales de la factura
                    if (ticopayInvoice.Client != null)
                    {
                        if (ticopayInvoice.Client.IdentificationType == IdentificacionTypeTipo.NoAsignada)
                        {
                            datosPeticion.datosprincipales.init = ticopayInvoice.Client.IdentificacionExtranjero;
                        }
                        else
                        {
                            datosPeticion.datosprincipales.init = ticopayInvoice.Client.Identification;
                        }
                    }
                    else
                    {
                        datosPeticion.datosprincipales.init = ticopayInvoice.ClientIdentification;

                    }
                    // datosPeticion.datosprincipales.iinventario = "1"; // Bodega
                    datosPeticion.datosprincipales.qprecisionprecio = "2";
                    datosPeticion.datosprincipales.qprecisionliquid = "2";
                    datosPeticion.datosprincipales.qregproductos = ticopayInvoice.InvoiceLines.Count.ToString();
                    #endregion
                    #region Listado de Productos 
                    int itemConImpuesto = 0;
                    foreach (InvoiceLineApiDto linea in ticopayInvoice.InvoiceLines)
                    {
                        datosPeticion.listaproductos[0] = new ListaProductosInvoice();
                        datosPeticion.listaproductos[0].icc = _codCentroCostos; // Centro de Costos                            
                        datosPeticion.listaproductos[0].irecurso = _productos.datos.Where(i => i.nrecurso.Contains(linea.Title) == true).First().irecurso; ; // "PT003"
                                                                                                                                                             // datosPeticion.listaproductos[0].iinventario = "1"; // Bodega
                        datosPeticion.listaproductos[0].qporcdescuento = linea.DiscountPercentage.ToString(); // Porcentaje de Descuento
                        datosPeticion.listaproductos[0].qproducto = linea.Quantity.ToString();
                        datosPeticion.listaproductos[0].mprecio = linea.PricePerUnit.ToString();
                        datosPeticion.listaproductos[0].mvrtotal = linea.SubTotal.ToString(); // Cantidad por Precio , menos descuento
                        Tax impuestoLinea = null;
                        foreach (Tax taxItem in ticopayTaxes)
                        {
                            if (taxItem.Id == linea.TaxId)
                            {
                                datosPeticion.listaproductos[0].qporciva = taxItem.Rate.ToString();
                                impuestoLinea = taxItem;
                            }
                        }
                        #region Liquidación de Impuestos
                        if (linea.TaxAmount > 0)
                        {
                            datosPeticion.liquidimpuestos[itemConImpuesto] = new LiquidacionImpuestos();
                            datosPeticion.liquidimpuestos[itemConImpuesto].iconcepto = impuestoLinea.Name;
                            datosPeticion.liquidimpuestos[itemConImpuesto].nconcepto = impuestoLinea.Name + " por Ventas";
                            datosPeticion.liquidimpuestos[itemConImpuesto].icuenta = _codLiquidacionIva;
                            datosPeticion.liquidimpuestos[itemConImpuesto].isigno = "+";
                            datosPeticion.liquidimpuestos[itemConImpuesto].mvalorbase = linea.SubTotal.ToString();
                            datosPeticion.liquidimpuestos[itemConImpuesto].mvalor = linea.TaxAmount.ToString();
                            datosPeticion.liquidimpuestos[itemConImpuesto].qpercent = impuestoLinea.Rate.ToString();
                            datosPeticion.liquidimpuestos[itemConImpuesto].bautocalc = "S";
                            datosPeticion.liquidimpuestos[itemConImpuesto].iasiento = "C";
                            itemConImpuesto++;
                        }
                        #endregion
                    }
                    #endregion
                    #region Pagos
                    datosPeticion.formacobro.mtotalpago = ticopayInvoice.Total.ToString();
                    datosPeticion.formacobro.mtotalreg = ticopayInvoice.Total.ToString();
                    if (ticopayInvoice.InvoicePaymentTypes.Count == 0 && ticopayInvoice.ExpirationDate != null)
                    {
                        #region Factura a Crédito
                        // Pago Cuentas por Cobrar
                        datosPeticion.formacobro.fcobrocxc[0] = new CuentaCobrar();
                        datosPeticion.formacobro.fcobrocxc[0].id = "1";
                        datosPeticion.formacobro.fcobrocxc[0].icuenta = _codCxC;
                        datosPeticion.formacobro.fcobrocxc[0].init = datosPeticion.datosprincipales.init;
                        datosPeticion.formacobro.fcobrocxc[0].qdiascxc = Convert.ToInt16((ticopayInvoice.ExpirationDate - ticopayInvoice.DueDate).Value.TotalDays).ToString("");                        
                        datosPeticion.formacobro.fcobrocxc[0].nconcepto = "CxC Fact Ticopay " + ticopayInvoice.ConsecutiveNumber;
                        datosPeticion.formacobro.fcobrocxc[0].mvalor = ticopayInvoice.Total.ToString();
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas = new CuentaCobrarcuota[1];
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas[0] = new CuentaCobrarcuota();
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas[0].icuota = "1";
                        DateTime fecha = ticopayInvoice.ExpirationDate == null ? DateTime.UtcNow : (DateTime) ticopayInvoice.ExpirationDate;
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas[0].fpagocuota = fecha.ToString("MM/dd/yyyy");
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas[0].mcuota = ticopayInvoice.Total.ToString();
                        datosPeticion.formacobro.fcobrocxc[0].bconceptochanged = "F";
                        datosPeticion.formacobro.fcobrocxc[0].beditvrotramoneda = "F";
                        #endregion
                    }
                    else
                    {
                        int numPago = 0;
                        int pagoEfectivo = 0;
                        int pagoBanco = 0;
                        foreach (PaymentInvoiceDto pago in ticopayInvoice.InvoicePaymentTypes)
                        {
                            if (pago.PaymetnMethodType == PaymetnMethodType.Cash)
                            {
                                #region Pago Efectivo
                                // Pago en Efectivo
                                datosPeticion.formacobro.fcobrocaja[pagoEfectivo] = new CobroCajaCuenta();
                                datosPeticion.formacobro.fcobrocaja[pagoEfectivo].id = (numPago + 1).ToString();
                                datosPeticion.formacobro.fcobrocaja[pagoEfectivo].ilineamov = (numPago + 1).ToString();
                                datosPeticion.formacobro.fcobrocaja[pagoEfectivo].icuenta = _codCaja;
                                datosPeticion.formacobro.fcobrocaja[pagoEfectivo].mvalor = pago.Amount.ToString();
                                datosPeticion.formacobro.fcobrocaja[pagoEfectivo].icc = _codCentroCostos;
                                pagoEfectivo++;
                                numPago++;
                                #endregion
                            }
                            else
                            {
                                #region Pago a Banco
                                // Pago en Deposito , Tarjeta , Transferencia
                                datosPeticion.formacobro.fcobrobanco[pagoBanco] = new CobroBancoCuenta();
                                datosPeticion.formacobro.fcobrobanco[pagoBanco].icuenta = _codBanco;
                                datosPeticion.formacobro.fcobrobanco[pagoBanco].mvalor = pago.Amount.ToString();
                                datosPeticion.formacobro.fcobrobanco[pagoBanco].ilineamov = (numPago + 1).ToString(); ;
                                datosPeticion.formacobro.fcobrobanco[pagoBanco].id = (numPago + 1).ToString(); ;
                                datosPeticion.formacobro.fcobrobanco[pagoBanco].icc = _codCentroCostos; // No necesario
                                datosPeticion.formacobro.fcobrobanco[pagoBanco].beditvrotramoneda = "F";
                                //TC y TD Tarjeta de credito o debido, CH y CHF Cheques , CI Deposito o Transferencia
                                switch (pago.PaymetnMethodType)
                                {
                                    case PaymetnMethodType.Card:
                                        datosPeticion.formacobro.fcobrobanco[pagoBanco].itipotransaccion = "TD";
                                        break;
                                    case PaymetnMethodType.Check:
                                        datosPeticion.formacobro.fcobrobanco[pagoBanco].itipotransaccion = "CH";
                                        break;
                                    case PaymetnMethodType.Deposit:
                                        datosPeticion.formacobro.fcobrobanco[pagoBanco].itipotransaccion = "CI";
                                        break;
                                }
                                datosPeticion.formacobro.fcobrobanco[pagoBanco].ftransaccion = pago.PaymentDate.ToString("MM/dd/yyyy");
                                if (pago.Reference != null)
                                {
                                    if (pago.Reference.Length > 10)
                                    {
                                        datosPeticion.formacobro.fcobrobanco[pagoBanco].itransaccion = pago.Reference.Substring(0, 10);
                                    }
                                    else
                                    {
                                        datosPeticion.formacobro.fcobrobanco[pagoBanco].itransaccion = pago.Reference;
                                    }
                                }
                                numPago++;
                                pagoBanco++;
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ExcepcionArmadoFactura)
                {
                    throw new Exception("Error al armar la factura: " + ExcepcionArmadoFactura.Message);
                }
                #region Envió de Factura al Api
                InvoiceCreationResponse DatosFacturaIngresada = null;
                try
                {
                    // Encapsulado de la Petición
                    Factura.oprdata = datosPeticion;
                    CrearFactura.dataJSON = Factura;
                    respuestaServicio = ContaPymeApi.AddInvoice(_direccionIpApi, CrearFactura).GetAwaiter().GetResult();
                    BasicOperationResponse FacturaIngresada = JsonConvert.DeserializeObject<BasicOperationResponse>(respuestaServicio.result);
                    string datos = "{\"datos\":" + FacturaIngresada.result[0].respuesta.datos.ToString() + "}";
                    DatosFacturaIngresada = JsonConvert.DeserializeObject<InvoiceCreationResponse>(datos);
                }
                catch (Exception exEnvioFactura)
                {
                    throw new Exception("Error al enviar la factura: " + exEnvioFactura.Message);
                }
                #endregion
                #region Procesar factura
                if (DatosFacturaIngresada.datos != null)
                {
                    try
                    {
                        ComplexOperationInfo ProcesarFactura = null;
                        // Procesar la operacion
                        if (operacion.TipoDeOperacion == TipoOperacion.Factura)
                        {
                            ProcesarFactura = new ComplexOperationInfo("PROCESS", "ING1");
                        }
                        else
                        {
                            ProcesarFactura = new ComplexOperationInfo("PROCESS", "ING5");
                        }                        
                        ProcesarFactura.operaciones[0].inumoper = DatosFacturaIngresada.datos.inumoper;
                        ProcesarFactura.oprdata = "";
                        OperationJson ConfirmarFactura = new OperationJson();
                        ConfirmarFactura.controlkey = credenciales.controlkey;
                        ConfirmarFactura.iapp = credenciales.iapp;
                        ConfirmarFactura.random = credenciales.random;
                        ConfirmarFactura.dataJSON = ProcesarFactura;
                        respuestaServicio = ContaPymeApi.AddInvoice(_direccionIpApi, ConfirmarFactura).GetAwaiter().GetResult();
                        BasicOperationResponse FacturaProcesada = JsonConvert.DeserializeObject<BasicOperationResponse>(respuestaServicio.result);
                    }
                    catch (Exception)
                    {
                       
                    }
                    
                }
                else
                {
                    return null;
                }
                #endregion
                // Cerrar Sesion
                respuestaServicio = ContaPymeApi.Logout(_direccionIpApi, credenciales).GetAwaiter().GetResult();
                LogOutResponse responseClose = JsonConvert.DeserializeObject<LogOutResponse>(respuestaServicio.result);
                return DatosFacturaIngresada.datos.snumsop;
            }
            catch(Exception ex)
            {
                throw ex;
            }            
        }
    }
}
