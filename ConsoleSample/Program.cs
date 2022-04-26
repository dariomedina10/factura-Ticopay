using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll;
using TicoPayDll.Authentication;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Reports;
using TicoPayDll.Response;
using TicoPayDll.Services;
using TicoPayDll.Taxes;
using TicoPayDll.Tenants.Dto;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Clients;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Invoices;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Items;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Opetations;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Security;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Responses;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Responses.Operations;
using static TicoPayDll.Clients.ClientController;
using static TicoPayDll.Services.ServiceController;
using static TicoPayDll.Taxes.TaxesController;
using static TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Opetations.Operation;

namespace ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en " + tenancy);
            token = AutentificarUsuario(tenancy,user,password);
            if (token != "")
            {
                string Opcion = "";
                bool Ejecutando = true;
                Console.WriteLine("Token asignado: " + token);
                Console.WriteLine("Este Token tiene una duracion de 20 min, para extenderlo utilize la funcion RefreshToken");
                Console.ReadKey();
                while (Ejecutando)
                {
                    Console.Clear();
                    Console.WriteLine("Seleccione Operacion a Realizar: ");
                    Console.WriteLine("1: Insertar Cliente Ejemplo ");
                    Console.WriteLine("2: Consultar Clientes ");
                    Console.WriteLine("3: Consultar Impuestos ");
                    Console.WriteLine("4: Consultar Servicios ");
                    Console.WriteLine("5: Crear Servicio Ejemplo ");
                    Console.WriteLine("6: Crear Factura Ejemplo ");
                    Console.WriteLine("7: Consultar Facturas ");
                    Console.WriteLine("8: Consultar Facturas Enviadas a Tribunet ");
                    Console.WriteLine("9: Consultar PDF");
                    Console.WriteLine("A: Autentificar Contapyme");
                    Console.WriteLine("B: Agregar Tercero Contapyme");
                    Console.WriteLine("C: Consultar Recursos Contapyme");
                    Console.WriteLine("D: Agregar Recurso Contapyme");
                    Console.WriteLine("E: Emitir Factura Contapyme");
                    Console.WriteLine("F: Consultar cliente");
                    Console.WriteLine("G: Consultar cliente");
                    Console.WriteLine("S: Salir ");
                    Opcion = Console.ReadKey().KeyChar.ToString();
                    if (Opcion.ToUpper().Contains("1"))
                    {
                        Console.WriteLine("Ejecutando");
                        Client cliente = CrearCliente(token);                        
                        if (cliente != null)
                        {
                            Console.WriteLine("Cliente Creado :");
                            Console.WriteLine(cliente.Name + " " + cliente.LastName + " " + cliente.Identification);
                        }                        
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("2"))
                    {
                        Console.WriteLine("Ejecutando");
                        Client[] clientes = BuscarClientes(token);
                        Console.WriteLine(clientes.Length + " encontrados");
                        foreach (Client cliente in clientes)
                        {
                            Console.WriteLine(cliente.Name + " " + cliente.LastName + " " + cliente.Identification);
                        }
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("3"))
                    {
                        Console.WriteLine("Ejecutando");
                        Tax[] impuestos = BuscarImpuestos(token);
                        Console.WriteLine(impuestos.Length + " encontrados");
                        foreach (Tax impuesto in impuestos)
                        {
                            Console.WriteLine(impuesto.Name + " " + impuesto.Rate + " " + impuesto.TaxTypes );
                        }
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("4"))
                    {
                        Console.WriteLine("Ejecutando");
                        Service[] servicios = BuscarServicios(token);
                        Console.WriteLine(servicios.Length + " encontrados");
                        foreach (Service servicio in servicios)
                        {
                            Console.WriteLine(servicio.Name + " " + servicio.Price);
                        }
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("5"))
                    {
                        Console.WriteLine("Ejecutando");
                        Service servicio = CrearServicio(token);
                        if (servicio != null)
                        {
                            Console.WriteLine("Servicio Creado :");
                            Console.WriteLine(servicio.Name + " " + servicio.Price);
                        }
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("6"))
                    {
                        Console.WriteLine("Ejecutando");
                        Invoice factura = CrearFactura(token);
                        if (factura != null)
                        {
                            Console.WriteLine("Factura Creada :");
                            Console.WriteLine(factura.Status + " " + factura.Client.Name);
                        }
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("7"))
                    {
                        Console.WriteLine("Ejecutando");
                        Invoice[] facturas = BuscarFacturas(token);
                        Console.WriteLine(facturas.Length + " encontrados");
                        foreach (Invoice factura in facturas)
                        {
                            Console.WriteLine(factura.Client.Name + " " + factura.Balance + " " + factura.Status);
                        }
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("8"))
                    {
                        Console.WriteLine("Ejecutando");
                        InvoiceSendTribunet[] reporte = ReporteEstatusFacturasTribunet(token);
                        Console.WriteLine(reporte.Length + " encontrados");
                        foreach (InvoiceSendTribunet factura in reporte)
                        {
                            Console.WriteLine(factura.NombreCliente + " " + factura.NumeroFactura + " " + factura.StatusTribunet);
                        }
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("9"))
                    {
                        Console.WriteLine("Ejecutando");
                        PDF reporte = GetInvoicePDF(token, "f8d073a7-d5f0-45c1-a53c-1fb64344f3df").objectResponse;
                        Console.WriteLine(reporte.FileName + " encontrado bytes " + reporte.Data.Length);
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("A"))
                    {
                        Console.WriteLine("Ejecutando");
                        MethodResponse respuestaServicio;
                        LoginJson credenciales = new LoginJson();
                        credenciales.dataJSON = new LoginInformation("desarrollo@asadacloud.com", "123", "//");
                        credenciales.controlkey = "";
                        credenciales.iapp = "1001";
                        Random rnd = new Random();
                        credenciales.random = rnd.Next(300,300000).ToString();
                        respuestaServicio = ContaPymeApi.Authenticate("http://localhost:9000",credenciales).GetAwaiter().GetResult();
                        GetAuthResult responseArray = JsonConvert.DeserializeObject<GetAuthResult>(respuestaServicio.result);
                        credenciales.controlkey = responseArray.result[0].respuesta.datos.keyagente;
                        Console.WriteLine("ApiKey: " + credenciales.controlkey);
                        OperationJson ConsultarClientes = new OperationJson();
                        ConsultarClientes.controlkey = credenciales.controlkey;
                        ConsultarClientes.iapp = credenciales.iapp;
                        ConsultarClientes.random = credenciales.random;
                        RequestParameters datosPeticion = new RequestParameters();
                        datosPeticion.datospagina = new PageData();
                        datosPeticion.datospagina.cantidadregistros = "99";
                        datosPeticion.datospagina.pagina = "1";
                        datosPeticion.camposderetorno = new string[] { "init", "ntercero", "napellido" };
                        ConsultarClientes.dataJSON = datosPeticion;
                        respuestaServicio = ContaPymeApi.GetClients("http://localhost:9000", ConsultarClientes).GetAwaiter().GetResult();
                        BasicOperationResponse ListadoClientes = JsonConvert.DeserializeObject<BasicOperationResponse>(respuestaServicio.result);
                        string datos = "{\"datos\":" +  ListadoClientes.result[0].respuesta.datos.ToString() + "}";
                        GetClients Clients = JsonConvert.DeserializeObject<GetClients>(datos);
                        respuestaServicio = ContaPymeApi.Logout("http://localhost:9000", credenciales).GetAwaiter().GetResult();
                        LogOutResponse responseClose = JsonConvert.DeserializeObject<LogOutResponse>(respuestaServicio.result);
                        Console.WriteLine("Cerrada la Sesión: " + responseClose.result[0].respuesta.datos.cerro);
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("B"))
                    {
                        Console.WriteLine("Ejecutando");
                        MethodResponse respuestaServicio;
                        LoginJson credenciales = new LoginJson();
                        credenciales.dataJSON = new LoginInformation("desarrollo@asadacloud.com", "123", "//");
                        credenciales.controlkey = "";
                        credenciales.iapp = "1001";
                        Random rnd = new Random();
                        credenciales.random = rnd.Next(300, 300000).ToString();
                        respuestaServicio = ContaPymeApi.Authenticate("http://localhost:9000", credenciales).GetAwaiter().GetResult();
                        GetAuthResult responseArray = JsonConvert.DeserializeObject<GetAuthResult>(respuestaServicio.result);
                        credenciales.controlkey = responseArray.result[0].respuesta.datos.keyagente;
                        Console.WriteLine("ApiKey: " + credenciales.controlkey);
                        OperationJson CrearCliente = new OperationJson();
                        CrearCliente.controlkey = credenciales.controlkey;
                        CrearCliente.iapp = credenciales.iapp;
                        CrearCliente.random = credenciales.random;
                        AddClient datosPeticion = new AddClient(TipoPersona.Natural);
                        datosPeticion.init = "65247300";
                        datosPeticion.infobasica.ntercero = "Christian";
                        datosPeticion.infobasica.napellido = "Diaz";
                        datosPeticion.infobasica.bempresa = "F";
                        datosPeticion.infobasica.itddocum = "13";
                        datosPeticion.infobasica.semail = "jguevara@asadacloud.com";
                        CrearCliente.dataJSON = datosPeticion;
                        respuestaServicio = ContaPymeApi.AddClient("http://localhost:9000", CrearCliente).GetAwaiter().GetResult();
                        BasicOperationResponse ClienteIngresado = JsonConvert.DeserializeObject<BasicOperationResponse>(respuestaServicio.result);
                        respuestaServicio = ContaPymeApi.Logout("http://localhost:9000", credenciales).GetAwaiter().GetResult();
                        LogOutResponse responseClose = JsonConvert.DeserializeObject<LogOutResponse>(respuestaServicio.result);
                        Console.WriteLine("Cerrada la Sesión: " + responseClose.result[0].respuesta.datos.cerro);
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("C"))
                    {
                        Console.WriteLine("Ejecutando");
                        MethodResponse respuestaServicio;
                        LoginJson credenciales = new LoginJson();
                        credenciales.dataJSON = new LoginInformation("desarrollo@asadacloud.com", "123", "//");
                        credenciales.controlkey = "";
                        credenciales.iapp = "1001";
                        Random rnd = new Random();
                        credenciales.random = rnd.Next(300, 300000).ToString();
                        respuestaServicio = ContaPymeApi.Authenticate("http://localhost:9000", credenciales).GetAwaiter().GetResult();
                        GetAuthResult responseArray = JsonConvert.DeserializeObject<GetAuthResult>(respuestaServicio.result);
                        credenciales.controlkey = responseArray.result[0].respuesta.datos.keyagente;
                        Console.WriteLine("ApiKey: " + credenciales.controlkey);
                        OperationJson ConsultarClientes = new OperationJson();
                        ConsultarClientes.controlkey = credenciales.controlkey;
                        ConsultarClientes.iapp = credenciales.iapp;
                        ConsultarClientes.random = credenciales.random;
                        RequestParameters datosPeticion = new RequestParameters();
                        datosPeticion.datospagina = new PageData();
                        datosPeticion.datospagina.cantidadregistros = "99";
                        datosPeticion.datospagina.pagina = "1";
                        datosPeticion.camposderetorno = new string[] { "irecurso", "nrecurso"};
                        ConsultarClientes.dataJSON = datosPeticion;
                        respuestaServicio = ContaPymeApi.GetItems("http://localhost:9000", ConsultarClientes).GetAwaiter().GetResult();
                        BasicOperationResponse ListadoClientes = JsonConvert.DeserializeObject<BasicOperationResponse>(respuestaServicio.result);
                        string datos = "{\"datos\":" + ListadoClientes.result[0].respuesta.datos.ToString() + "}";
                        GetItems Clients = JsonConvert.DeserializeObject<GetItems>(datos);
                        respuestaServicio = ContaPymeApi.Logout("http://localhost:9000", credenciales).GetAwaiter().GetResult();
                        LogOutResponse responseClose = JsonConvert.DeserializeObject<LogOutResponse>(respuestaServicio.result);
                        Console.WriteLine("Cerrada la Sesión: " + responseClose.result[0].respuesta.datos.cerro);
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("D"))
                    {
                        Console.WriteLine("Ejecutando");
                        MethodResponse respuestaServicio;
                        LoginJson credenciales = new LoginJson();
                        credenciales.dataJSON = new LoginInformation("desarrollo@asadacloud.com", "123", "//");
                        credenciales.controlkey = "";
                        credenciales.iapp = "1001";
                        Random rnd = new Random();
                        credenciales.random = rnd.Next(300, 300000).ToString();
                        respuestaServicio = ContaPymeApi.Authenticate("http://localhost:9000", credenciales).GetAwaiter().GetResult();
                        GetAuthResult responseArray = JsonConvert.DeserializeObject<GetAuthResult>(respuestaServicio.result);
                        credenciales.controlkey = responseArray.result[0].respuesta.datos.keyagente;
                        Console.WriteLine("ApiKey: " + credenciales.controlkey);
                        OperationJson CrearRecurso = new OperationJson();
                        CrearRecurso.controlkey = credenciales.controlkey;
                        CrearRecurso.iapp = credenciales.iapp;
                        CrearRecurso.random = credenciales.random;
                        AddItem datosPeticion = new AddItem();
                        datosPeticion.irecurso = "019";
                        datosPeticion.infobasica.nrecurso = "Vajilla";
                        datosPeticion.infobasica.nunidad = "und";
                        datosPeticion.infobasica.bvisible = "T";
                        datosPeticion.infobasica.bcontrolinv= "F";
                        datosPeticion.infobasica.bventa = "T";
                        datosPeticion.infobasica.igrupoinv = "1";
                        //datosPeticion.infobasica.irecurso = "019";
                        CrearRecurso.dataJSON = datosPeticion;
                        respuestaServicio = ContaPymeApi.AddItem("http://localhost:9000", CrearRecurso).GetAwaiter().GetResult();
                        BasicOperationResponse ItemIngresado = JsonConvert.DeserializeObject<BasicOperationResponse>(respuestaServicio.result);
                        respuestaServicio = ContaPymeApi.Logout("http://localhost:9000", credenciales).GetAwaiter().GetResult();
                        LogOutResponse responseClose = JsonConvert.DeserializeObject<LogOutResponse>(respuestaServicio.result);
                        Console.WriteLine("Cerrada la Sesión: " + responseClose.result[0].respuesta.datos.cerro);
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("E"))
                    {
                        Console.WriteLine("Ejecutando");
                        MethodResponse respuestaServicio;
                        LoginJson credenciales = new LoginJson();
                        credenciales.dataJSON = new LoginInformation("desarrollo@asadacloud.com", "123", "//");
                        credenciales.controlkey = "";
                        credenciales.iapp = "1001";
                        Random rnd = new Random();
                        credenciales.random = rnd.Next(300, 300000).ToString();
                        respuestaServicio = ContaPymeApi.Authenticate("http://localhost:9000", credenciales).GetAwaiter().GetResult();
                        GetAuthResult responseArray = JsonConvert.DeserializeObject<GetAuthResult>(respuestaServicio.result);
                        credenciales.controlkey = responseArray.result[0].respuesta.datos.keyagente;
                        Console.WriteLine("ApiKey: " + credenciales.controlkey);
                        OperationJson CrearFactura = new OperationJson();
                        CrearFactura.controlkey = credenciales.controlkey;
                        CrearFactura.iapp = credenciales.iapp;
                        CrearFactura.random = credenciales.random;
                        ComplexOperationInfo Factura = new ComplexOperationInfo("CREATE", "ING1");                        
                        AddInvoice datosPeticion = new AddInvoice(1,0,0,1,1);
                        // Encabezado
                        datosPeticion.encabezado.iemp = "1";
                        datosPeticion.encabezado.itdsop = "10";
                        datosPeticion.encabezado.fsoport = DateTime.Now.ToString("MM/dd/yyyy"); // "08/31/2018";
                        datosPeticion.encabezado.svaloradic1 = "true";
                        datosPeticion.encabezado.svaloradic2 = "00000000001234500001";
                        datosPeticion.encabezado.banulada = "F";
                        datosPeticion.encabezado.inumsop = "0";
                        datosPeticion.encabezado.snumsop = "<AUTO>";
                        // datosPeticion.encabezado.iprocess = "2";
                        // Datos Principales
                        datosPeticion.datosprincipales.init = "178841668";
                        datosPeticion.datosprincipales.iinventario = "1";
                        datosPeticion.datosprincipales.qprecisionprecio = "2";
                        datosPeticion.datosprincipales.qprecisionliquid = "2";
                        datosPeticion.datosprincipales.qregproductos = "1";
                        // Listado de Productos
                        datosPeticion.listaproductos[0] = new ListaProductosInvoice();
                        datosPeticion.listaproductos[0].icc = "1"; // Centro de Costos
                        datosPeticion.listaproductos[0].irecurso = "PT003";
                        // datosPeticion.listaproductos[0].iinventario = "1"; // Bodega
                        // datosPeticion.listaproductos[0].qporcdescuento = "10.00"; // Porcentaje de Descuento
                        datosPeticion.listaproductos[0].qproducto = "2.0";
                        datosPeticion.listaproductos[0].mprecio = "147.00";
                        datosPeticion.listaproductos[0].mvrtotal = "294.00"; // Cantidad por Precio , menos descuento
                        datosPeticion.listaproductos[0].qporciva = "13.00";                        
                        // Listado de impuestos
                        datosPeticion.liquidimpuestos[0] = new LiquidacionImpuestos();
                        datosPeticion.liquidimpuestos[0].iconcepto = "IVAV";
                        datosPeticion.liquidimpuestos[0].nconcepto = "IVA por Ventas";
                        datosPeticion.liquidimpuestos[0].icuenta = "2408";
                        datosPeticion.liquidimpuestos[0].isigno = "+";
                        datosPeticion.liquidimpuestos[0].mvalorbase = "294.00";
                        datosPeticion.liquidimpuestos[0].mvalor = "38.22";
                        datosPeticion.liquidimpuestos[0].qpercent = "13.00";
                        datosPeticion.liquidimpuestos[0].bautocalc = "S";
                        datosPeticion.liquidimpuestos[0].iasiento = "C";
                        // Pagos
                        datosPeticion.formacobro.mtotalpago = "332.22";
                        datosPeticion.formacobro.mtotalreg = "332.22";
                        // Pago en Efectivo
                        //datosPeticion.formacobro.fcobrocaja[0] = new CobroCajaCuenta();
                        //datosPeticion.formacobro.fcobrocaja[0].id = "1";
                        //datosPeticion.formacobro.fcobrocaja[0].ilineamov = "1";
                        //datosPeticion.formacobro.fcobrocaja[0].icuenta = "1105";
                        //datosPeticion.formacobro.fcobrocaja[0].mvalor = "332.22";
                        //datosPeticion.formacobro.fcobrocaja[0].icc = "1";
                        // Pago en Deposito , Tarjeta , Transferencia
                        //datosPeticion.formacobro.fcobrobanco[0] = new CobroBancoCuenta();
                        //datosPeticion.formacobro.fcobrobanco[0].icuenta = "1110";
                        //datosPeticion.formacobro.fcobrobanco[0].mvalor = "332.22";
                        //datosPeticion.formacobro.fcobrobanco[0].ilineamov = "1";
                        //datosPeticion.formacobro.fcobrobanco[0].id = "1";
                        // datosPeticion.formacobro.fcobrobanco[0].icc = "1"; // No necesario
                        //datosPeticion.formacobro.fcobrobanco[0].beditvrotramoneda = "F";
                        // TC y TD Tarjeta de credito o debido , CH y CHF Cheques , CI Deposito o Transferencia
                        //datosPeticion.formacobro.fcobrobanco[0].itipotransaccion = "CH";
                        //datosPeticion.formacobro.fcobrobanco[0].ftransaccion = DateTime.Now.ToString("MM/dd/yyyy");
                        //datosPeticion.formacobro.fcobrobanco[0].itransaccion = "0001254563";
                        // Pago Cuentas por Cobrar
                        datosPeticion.formacobro.fcobrocxc[0] = new CuentaCobrar();
                        datosPeticion.formacobro.fcobrocxc[0].id = "1";
                        datosPeticion.formacobro.fcobrocxc[0].icuenta = "1305";
                        datosPeticion.formacobro.fcobrocxc[0].init = "178841668";
                        datosPeticion.formacobro.fcobrocxc[0].qdiascxc = "5";
                        datosPeticion.formacobro.fcobrocxc[0].nconcepto = "Cuenta por cobrar Factura TicopayMovil";
                        datosPeticion.formacobro.fcobrocxc[0].mvalor = "332.22";
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas = new CuentaCobrarcuota[1];
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas[0] = new CuentaCobrarcuota();
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas[0].icuota = "1";
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas[0].fpagocuota = DateTime.Now.AddDays(5).ToString("MM/dd/yyyy");
                        datosPeticion.formacobro.fcobrocxc[0].slistcuotas[0].mcuota = "332.22";
                        datosPeticion.formacobro.fcobrocxc[0].bconceptochanged = "F";
                        datosPeticion.formacobro.fcobrocxc[0].beditvrotramoneda = "F";
                        // Encapsulado de la Peticion
                        Factura.oprdata = datosPeticion;
                        CrearFactura.dataJSON = Factura;
                        respuestaServicio = ContaPymeApi.AddInvoice("http://localhost:9000", CrearFactura).GetAwaiter().GetResult();
                        BasicOperationResponse FacturaIngresada = JsonConvert.DeserializeObject<BasicOperationResponse>(respuestaServicio.result);
                        string datos = "{\"datos\":" + FacturaIngresada.result[0].respuesta.datos.ToString() + "}";
                        InvoiceCreationResponse DatosFacturaIngresada = JsonConvert.DeserializeObject<InvoiceCreationResponse>(datos);                         
                        // Procesar la operacion
                        ComplexOperationInfo ProcesarFactura = new ComplexOperationInfo("PROCESS", "ING1");
                        ProcesarFactura.operaciones[0].inumoper = DatosFacturaIngresada.datos.inumoper;
                        //ConfirmInvoice PeticionConfirmacion = new ConfirmInvoice(DatosFacturaIngresada.datos.inumoper, "ING1");                        
                        //ProcesarFactura.oprdata = PeticionConfirmacion;
                        ProcesarFactura.oprdata = "";
                        OperationJson ConfirmarFactura = new OperationJson();
                        ConfirmarFactura.controlkey = credenciales.controlkey;
                        ConfirmarFactura.iapp = credenciales.iapp;
                        ConfirmarFactura.random = credenciales.random;
                        ConfirmarFactura.dataJSON = ProcesarFactura;
                        respuestaServicio = ContaPymeApi.AddInvoice("http://localhost:9000", ConfirmarFactura).GetAwaiter().GetResult();
                        BasicOperationResponse FacturaProcesada = JsonConvert.DeserializeObject<BasicOperationResponse>(respuestaServicio.result);
                        // Cerrar Sesion
                        respuestaServicio = ContaPymeApi.Logout("http://localhost:9000", credenciales).GetAwaiter().GetResult();
                        LogOutResponse responseClose = JsonConvert.DeserializeObject<LogOutResponse>(respuestaServicio.result);
                        Console.WriteLine("Cerrada la Sesión: " + responseClose.result[0].respuesta.datos.cerro);
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("F"))
                    {
                        Client client = BuscarCliente(token, "");
                        Console.WriteLine("Cliente encontrado:" + client.Name);
                    }
                    if (Opcion.ToUpper().Contains("G"))
                    {
                        Console.WriteLine("Ejecutando");
                        Tenant subdominio = BuscarTenant(token);
                        if (subdominio != null)
                        {
                            Console.WriteLine("Tenant : " + subdominio.Edition.DisplayName);
                        }
                        Console.ReadKey();
                    }
                    if (Opcion.ToUpper().Contains("S"))
                    {
                        Ejecutando = false;
                    }
                }
                Console.WriteLine("Hasta luego ");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Error al Conectar con Ticopay");
                Console.ReadKey();
            }
            
        }

        private static InvoiceSendTribunet[] ReporteEstatusFacturasTribunet(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            ReportInvoicesSentToTribunetSearchInput parameter = new ReportInvoicesSentToTribunetSearchInput();
            parameter.StatusTribunet = TicoPayDll.Reports.StatusTaxAdministration.Aceptado;
            parameter.Status = null;
            parameter.RecepcionConfirmada = null;
            parameter.NumeroFactura = null;
            parameter.NombreCliente = null;
            parameter.MedioPago = null;
            parameter.FechaEmisionHasta = DateTime.Now.AddDays(-30);
            parameter.FechaEmisionDesde = null;
            parameter.CondicionVenta = null;
            parameter.ClienteId = null;
            parameter.CedulaCliente = null;
            parameter.type = "json";
            respuestaServicio = TicoPayDll.Reports.ReportsController.GetInvoicesSendToTribunet(parameter, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonInvoicesSendToTribunet facturas = JsonConvert.DeserializeObject<JsonInvoicesSendToTribunet>(respuestaServicio.result);
                return facturas.invoices;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        private static InvoiceSendTribunet[] ReporteEstatusFacturasTribunetPablo(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            ReportInvoicesSentToTribunetSearchInput parametrosBusqueda = new ReportInvoicesSentToTribunetSearchInput();
            parametrosBusqueda.StatusTribunet = TicoPayDll.Reports.StatusTaxAdministration.Rechazado;
            parametrosBusqueda.RecepcionConfirmada = true;
            parametrosBusqueda.type = "json";
            respuestaServicio = TicoPayDll.Reports.ReportsController.GetInvoicesSendToTribunet(parametrosBusqueda, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonInvoicesSendToTribunet facturas = JsonConvert.DeserializeObject<JsonInvoicesSendToTribunet>(respuestaServicio.result);
                return facturas.invoices;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        private static Invoice CrearFactura(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            Client[] clientes = BuscarClientes(token);
            Tax[] impuestos = BuscarImpuestos(token);
            CreateInvoice factura = new CreateInvoice();
            factura.ClientId = clientes.First().Id;
            ItemInvoice lineaFactura = new ItemInvoice();
            lineaFactura.Servicio = "Nombre del item a Facturar";
            lineaFactura.Cantidad = 1;
            lineaFactura.Precio = 100;
            lineaFactura.IdService = null;
            decimal subTotal = lineaFactura.Cantidad * lineaFactura.Precio;
            lineaFactura.IdImpuesto = impuestos.First().Id;
            lineaFactura.Descuento = 0;
            lineaFactura.Impuesto = (impuestos.First().Rate * subTotal) / 100;
            lineaFactura.Total = lineaFactura.Impuesto + subTotal;
            lineaFactura.UnidadMedida = UnidadMedidaType.Servicios_Profesionales;
            factura.InvoiceLines = new List<ItemInvoice>();
            factura.InvoiceLines.Add(lineaFactura);
            PaymentInvoce formaPago = new PaymentInvoce();
            formaPago.TypePayment = 0;
            formaPago.Balance = lineaFactura.Total;
            formaPago.Trans = null;
            factura.ListPaymentType = new List<PaymentInvoce>();
            factura.ListPaymentType.Add(formaPago);
            factura.DiscountGeneral = null;
            factura.TypeDiscountGeneral = null;
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.CreateNewInvoice(factura, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateInvoice invoice = JsonConvert.DeserializeObject<JsonCreateInvoice>(respuestaServicio.result);
                return invoice.objectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        private static Invoice[] BuscarFacturas(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            InvoiceSearchConfiguration parametrosBusqueda = new InvoiceSearchConfiguration();
            parametrosBusqueda.ClientId = null;
            parametrosBusqueda.InvoiceId = null;
            parametrosBusqueda.Status = InvoiceStatus.Pagada;
            parametrosBusqueda.EndDueDate = null;
            parametrosBusqueda.StartDueDate = null;
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.GetInvoices(parametrosBusqueda, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonInvoices facturas = JsonConvert.DeserializeObject<JsonInvoices>(respuestaServicio.result);
                return facturas.listObjectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        private static Service CrearServicio(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            Tax[] impuestos = BuscarImpuestos(token);
            Service servicio = new Service();
            servicio.Name = "Pedro";
            servicio.Price = 100;
            servicio.TaxId = impuestos.First().Id;
            servicio.Quantity = 1;
            servicio.UnitMeasurement = UnidadMedidaType.Servicios_Profesionales;
            respuestaServicio = TicoPayDll.Services.ServiceController.CreateNewService(servicio, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateService service = JsonConvert.DeserializeObject<JsonCreateService>(respuestaServicio.result);
                return service.objectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        private static Service[] BuscarServicios(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Services.ServiceController.GetServices(token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonServices impuestos = JsonConvert.DeserializeObject<JsonServices>(respuestaServicio.result);
                return impuestos.listObjectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        private static Tax[] BuscarImpuestos(string token)
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
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        private static Client CrearCliente(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            Client cliente = new Client();
            cliente.Name = "Pedro";
            cliente.LastName = "Perez";
            cliente.IdentificationType = IdentificacionTypeTipo.Cedula_Fisica;
            cliente.Identification = "923456789";
            cliente.Email = "Ejemplo@ejemplo.ejm";
            respuestaServicio = TicoPayDll.Clients.ClientController.CreateNewClient(cliente, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateClient clientes = JsonConvert.DeserializeObject<JsonCreateClient>(respuestaServicio.result);
                return clientes.objectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
            
        }

        public static Client[] BuscarClientes(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Clients.ClientController.GetClients(token, false).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonClients clientes = JsonConvert.DeserializeObject<JsonClients>(respuestaServicio.result);
                return clientes.listObjectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        public static Client BuscarCliente(string token,string identificacion)
        {
            Response respuestaServicio = new Response();
            respuestaServicio = SearchClients(token, true, identificacion).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonSearchClient clientes = JsonConvert.DeserializeObject<JsonSearchClient>(respuestaServicio.result);
                return clientes.objectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        public static Tenant BuscarTenant(string token)
        {
            Response respuestaServicio = new Response();
            respuestaServicio = TicoPayDll.Tenants.TenantController.GetTenant(token,true).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                TicoPayDll.Tenants.JsonResponses.JsonTenant subdominio = JsonConvert.DeserializeObject<TicoPayDll.Tenants.JsonResponses.JsonTenant>(respuestaServicio.result);
                return subdominio.objectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        private List<InvoiceSendTribunet> ObtenerFacturaTicoPayHacienda(ReportInvoicesSentToTribunetSearchInput parameter, string token)
        {
            Response respuestaServicio = new Response();
            respuestaServicio = TicoPayDll.Reports.ReportsController.GetInvoicesSendToTribunet(parameter, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonInvoicesSendToTribunet invoiceSendTribunet = JsonConvert.DeserializeObject<JsonInvoicesSendToTribunet>(respuestaServicio.result);
                return invoiceSendTribunet.invoices.ToList();
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        public static string AutentificarUsuario(string tenancy, string user, string password)
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
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        public static JsonInvoicePDF GetInvoicePDF(string token, string id)
        {

            Response respuestaServicio = new Response();
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.GetInvoicePDF(id, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                //JsonInvoicePDF getInvoicePDF = JsonInvoicePDF(respuestaServicio.result);
                //return getInvoicePDF.objectResponse;
                if (respuestaServicio.result!=null)
                {
                    var JsonInvoicePDF = JsonConvert.DeserializeObject<JsonInvoicePDF>(respuestaServicio.result);
                    File.WriteAllBytes(JsonInvoicePDF.objectResponse.FileName + ".pdf", JsonInvoicePDF.objectResponse.Data);
                    return JsonInvoicePDF;
                }
                return null;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }


    }
}
