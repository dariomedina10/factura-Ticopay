using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Ticopay.BNModel;
using log4net;
using TicoPay.Invoices;
using TicoPay.Clients;
using TicoPay.MultiTenancy;
using TicoPay.Services;
using Abp.Domain.Repositories;
using System.Reflection;
using Abp.Dependency;
using TicoPay.Users;
using Abp.Domain.Uow;
using Abp.Modules;
using System.Text;
using Abp;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TicoPay.Invoices.Dto;
using System.Globalization;
using TicoPay.Clients.Dto;

namespace TicoPay.Socket
{
    public class Program
    {
        private static ILog _loggerForWeService;

        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            try
            {
                _loggerForWeService = SocketLogManager.GetLogger();

                string strHostName = Dns.GetHostName();
                IPHostEntry ipHostInfo = Dns.GetHostEntry(strHostName);
                IPAddress ipAddress = ipHostInfo.AddressList[1];

                foreach (IPAddress ipa in ipHostInfo.AddressList)
                {
                    Console.WriteLine("Iniciando Socket Direcciones IP: " + ipa);
                    _loggerForWeService.Debug("Iniciando Socked Direcciones IP: " + ipa);

                    if (ipa.ToString().Equals("172.17.4.243") || ipa.ToString().Equals("172.17.4.242"))
                    {
                        ipAddress = ipa;
                    }
                }

                IList<IPEndPoint> ipPointsList = new List<IPEndPoint>();

                string TenantDefault = "";
                // string consultarecibo = "";
                //InvoicePendingPayBN factura;
                ////IList<InvoicePendingPayBN> facturasPendientes;
                //int result = -1;
                //ClientBN clientdt;
                List<AgreementConectivity> listTenant = null;
                using (var bootstrapper = AbpBootstrapper.Create<SocketModule>())
                {
                    bootstrapper.Initialize();

                    var tenantService = IocManager.Instance.Resolve<tenantService>();
                    var client = IocManager.Instance.Resolve<clientService>();
                    var facturaService = IocManager.Instance.Resolve<invoiceService>();

                    listTenant = tenantService.GetTenantsByPort();
                    //TenantDefault = tenantService.TenantJob();
                    //facturaService.ApplyNCRGeneral(Guid.Parse("9DA5C65D-7C0E-48C0-A9A0-A1B8D27D35AE"));
                    //string consultarecibo = tenantService.ConsultaReciboTrama("02000015100000131000000796500111053                             201707271115320000", facturaService, client, tenantService, listTenant);
                    //string consultarecibo = tenantService.ConsultaReciboTrama("02000015100000131000000337200111056                             201710231321550000", facturaService, client, tenantService, listTenant);
                    ////var listTenant = tenantService.GetTenantsByPort(5103);
                    //TipoLLaveAcceso tipo = tenantService.GetTenantTipoAcceso(listTenant);
                    //var aplicacionPago = AplicacionPago.ParserAplicacionPago("0200001510002288000000033740017                             002017080000000000005650002017082900000000000000000016020171023122008000001151057000000000000000000000000000000000000000000");
                    //clientdt = client.GetExistClientByCode((MultiTenancy.TipoLLaveAcceso)tipo, Convert.ToInt64(aplicacionPago.ValorServicio));
                    //factura = facturaService.GetInvoicesByNumber(clientdt, aplicacionPago.NumeroFactura); 
                    ////Obtiene la factura segun el numero de cuenta y el numero de factura
                    ////facturasPendientes = facturaService.GetInvoicesPendingOld(clientdt, factura.DueDate);
                    //// facturasPendientes = facturaService.GetInvoicesPendingPay(clientdt); //obtiene las facturas del cliente pendientes por pago
                    //result = facturaService.PayInvoiceBn(factura,aplicacionPago.CodigoAgencia, "0200001510002288000000020700013002017060000000000006050002017061400000000000000000001020170718103001000001149207000000000000000000000000000000000000000000"); // Paga la factura
                    // bootstrapper.Dispose();
                }

                foreach (var endPoint in listTenant)
                {
                    ipPointsList.Add(new IPEndPoint(ipAddress, endPoint.Port)); 
                }

                if (ipPointsList.Any())
                {
                    IPEndPoint[] ipPoints = ipPointsList.ToArray();
                    ListenPorts lp = new ListenPorts(ipPoints);

                    //Console.WriteLine("En Tenant Default: {0}", TenantDefault);

                    //Console.WriteLine("En Tenant Default: {0}", consultarecibo);
                    //Console.WriteLine("Name Client: {0} ", clientdt.Name);
                    //Console.WriteLine("factura: {0} ", factura.Balance);

                    //if (facturasPendientes != null && facturasPendientes.Count() > 0)
                    //{
                    //    Console.WriteLine("Facturas Pendientes, la primera: {0} ", facturasPendientes.First().Number);
                    //}

                    //Console.WriteLine("Name Client: {0} ", facturasPendientes.First().Number);

                    Console.WriteLine("Empezando a Escuchar Puertos.....");

                    _loggerForWeService.Debug("Empezando a Escuchar Puertos.....");
                    foreach (var ipEndPoint in ipPoints)
                    {
                        Console.WriteLine("En Puerto: {0}", ipEndPoint.Port);
                        _loggerForWeService.Debug("En Puerto: " + ipEndPoint.Port);
                    }
                    lp.BeginListen();
                }
                else
                {
                    Console.WriteLine("No existen tenant con puertos");
                    _loggerForWeService.Debug("No existen tenant con puertos");
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
            Console.Read();
        }

    }
}
