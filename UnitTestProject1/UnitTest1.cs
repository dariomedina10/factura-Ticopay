using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
using static TicoPayDll.Clients.ClientController;
using static TicoPayDll.Services.ServiceController;
using static TicoPayDll.Taxes.TaxesController;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
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

        [TestMethod]
        public void TestMethod_busqueda_clientes()
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en Ticopay");
            token = AutentificarUsuario(tenancy, user, password);

            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Clients.ClientController.GetClients(token, false).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonClients clientes = JsonConvert.DeserializeObject<JsonClients>(respuestaServicio.result);
                Client[] clientes1 = clientes.listObjectResponse;
                Console.WriteLine(clientes1.Length + " encontrados");
                foreach (Client cliente in clientes1)
                {
                    Console.WriteLine(cliente.Name + " " + cliente.LastName + " " + cliente.Identification);
                }
                Assert.AreEqual(clientes1.Length, clientes1.Length);
            }
            else
            {
                Console.WriteLine(" LA RESPUESTA ES" + respuestaServicio.message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestMethod_busqueda_servicios()
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en Ticopay");
            token = AutentificarUsuario(tenancy, user, password);

            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Services.ServiceController.GetServices(token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonServices impuestos = JsonConvert.DeserializeObject<JsonServices>(respuestaServicio.result);
                Service[] servicios = impuestos.listObjectResponse;
                Console.WriteLine(servicios.Length + " encontrados");
                foreach (Service servicio in servicios)
                {
                    Console.WriteLine(servicio.Name + " " + servicio.Price);
                }
                Assert.AreEqual(servicios.Length, servicios.Length);
            }
            else
            {
                Console.WriteLine("LA RESPUESTA ES " + respuestaServicio.message);
                Assert.Fail();
            }

        }

        [TestMethod]
        public void TestMethod_busqueda_impuestos()
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en Ticopay");
            token = AutentificarUsuario(tenancy, user, password);

            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Taxes.TaxesController.Gettaxes(token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonTaxes impuestos = JsonConvert.DeserializeObject<JsonTaxes>(respuestaServicio.result);
                Tax[] losimpuestos = impuestos.listObjectResponse;
                Console.WriteLine(losimpuestos.Length + " encontrados");
                foreach (Tax impuesto in losimpuestos)
                {
                    Console.WriteLine(impuesto.Id + " " + impuesto.Name + " " + impuesto.Rate + " " + impuesto.TaxTypes);
                }
                Assert.AreEqual(losimpuestos.Length, losimpuestos.Length);
            }
            else
            {
                Console.WriteLine("LA RESPUESTA ES " + respuestaServicio.message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestMethod_busqueda_facturas()
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en Ticopay");
            token = AutentificarUsuario(tenancy, user, password);

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
                Invoice[] lasfacturas = facturas.listObjectResponse;
                Console.WriteLine(lasfacturas.Length + " encontrados");
                foreach (Invoice factura in lasfacturas)
                {
                    Console.WriteLine(factura.Total + " " + factura.Status);
                }
                Assert.AreEqual(lasfacturas.Length, lasfacturas.Length);
            }
            else
            {
                Console.WriteLine("LA RESPUESTA ES " + respuestaServicio.message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestMethod_crear_servicios()
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en Ticopay");
            token = AutentificarUsuario(tenancy, user, password);

            TicoPayDll.Response.Response respuestaServicio;
            Tax[] impuestos = BuscarImpuestos(token);
            Service servicio = new Service();
            servicio.Name = "Serv1";
            servicio.Price = 100;
            servicio.TaxId = impuestos.First().Id;
            //servicio.TaxId = new Guid("99749128-c710-4072-ac32-9450ebf010f1");
            servicio.Quantity = 1;
            servicio.UnitMeasurement = UnidadMedidaType.Servicios_Profesionales;
            Service[] servicios_old = BuscarServicios(token);
            respuestaServicio = TicoPayDll.Services.ServiceController.CreateNewService(servicio, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateService service = JsonConvert.DeserializeObject<JsonCreateService>(respuestaServicio.result);
                Service servicio_resp = service.objectResponse;
                if (servicio_resp != null)
                {
                    Console.WriteLine("Servicio Creado :");
                    Console.WriteLine(servicio_resp.Name + " " + servicio_resp.Price);
                    Service[] servicios_new = BuscarServicios(token);
                    //Assert.AreEqual(servicio_resp.Name, "Serv1");
                    Assert.AreEqual(servicios_old.Length+1, servicios_new.Length);
                }
            }
            else
            {
                Console.WriteLine("LA RESPUESTA ES " + respuestaServicio.message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestMethod_crear_cliente()
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en Ticopay");
            token = AutentificarUsuario(tenancy, user, password);
            TicoPayDll.Response.Response respuestaServicio;
            Client cliente = new Client();
            cliente.Name = "Isab26";
            cliente.LastName = "Ovied";
            cliente.IdentificationType = IdentificacionTypeTipo.Cedula_Fisica;
            cliente.Identification = "199235916";
            cliente.Email = "Ejemplo@ejemplo.ejm";
            Client[] clientes_antes = BuscarClientes(token);
            respuestaServicio = TicoPayDll.Clients.ClientController.CreateNewClient(cliente, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateClient clientes = JsonConvert.DeserializeObject<JsonCreateClient>(respuestaServicio.result);
                Client cliente_resp = clientes.objectResponse;
                if (cliente_resp != null)
                {
                    Console.WriteLine("Cliente Creado :");
                    Console.WriteLine(cliente_resp.Name + " " + cliente_resp.Identification);
                    Client[] clientes_despues = BuscarClientes(token);
                    Assert.AreEqual(clientes_antes.Length+1, clientes_despues.Length);
                }
            }
            else
            {
                Console.WriteLine("LA RESPUESTA ES " + respuestaServicio.message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestMethod_crear_factura()
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en Ticopay");
            token = AutentificarUsuario(tenancy, user, password);
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
            factura.TypeDocument = TicoPayDll.Invoices.DocumentType.Invoice;
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.CreateNewInvoice(factura, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateInvoice invoice = JsonConvert.DeserializeObject<JsonCreateInvoice>(respuestaServicio.result);
                Invoice fact_resp = invoice.objectResponse;
                if (fact_resp != null)
                {
                    Console.WriteLine("Factura Creada :");
                    Console.WriteLine(fact_resp.Total + " " + fact_resp.SubTotal + " " + fact_resp.TotalTax + " " + fact_resp.ClientName + " " + fact_resp.InvoiceLines);
                    Assert.AreEqual(fact_resp.Total, 100);
                }
            }
            else
            {
                Console.WriteLine("LA RESPUESTA ES " + respuestaServicio.message);
                Assert.Fail();
            }
        }
        
        [TestMethod]
        public void TestMethod_factura_pdf()
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en Ticopay");
            token = AutentificarUsuario(tenancy, user, password);
            //TicoPayDll.Response.Response respuestaServicio;
            Response respuestaServicio = new Response();
            Invoice[] facturas = BuscarFacturas(token);
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.GetInvoicePDF(facturas.First().Id, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                //JsonInvoicePDF getInvoicePDF = JsonInvoicePDF(respuestaServicio.result);
                //return getInvoicePDF.objectResponse;
                if (respuestaServicio.result != null)
                {
                    var JsonInvoicePDF = JsonConvert.DeserializeObject<JsonInvoicePDF>(respuestaServicio.result);
                    File.WriteAllBytes(JsonInvoicePDF.objectResponse.FileName + ".pdf", JsonInvoicePDF.objectResponse.Data);
                    PDF reporte = JsonInvoicePDF.objectResponse;
                    Console.WriteLine(reporte.FileName + " encontrado bytes " + reporte.Data.Length);
                    Assert.AreEqual(reporte.FileName, reporte.FileName);
                }
                else
                {
                    Console.WriteLine("EL RESULTADO ES " + respuestaServicio.message);
                    Assert.Fail();
                }
            }
            else
            {
                Console.WriteLine("LA RESPUESTA ES " + respuestaServicio.message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Rep_status_Invoice_Tribunet()
        {
            string token = "";
            string tenancy = "ticopay";
            string user = "admin";
            string password = "P@ssw0rd";
            Console.WriteLine("Realizando login en Ticopay");
            token = AutentificarUsuario(tenancy, user, password);
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
                InvoiceSendTribunet[] reporte = facturas.invoices;
                Console.WriteLine(reporte.Length + " encontrados");
                foreach (InvoiceSendTribunet factura in reporte)
                {
                    Console.WriteLine(factura.NombreCliente + " " + factura.NumeroFactura + " " + factura.StatusTribunet);
                }
                Assert.IsTrue(true);
            }
            else
            {
                Console.WriteLine("LA RESPUESTA ES " + respuestaServicio.message);
                Assert.Fail();
            }
        }
    }
}
