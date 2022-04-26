using Abp.Dependency;
using Abp.Domain.Repositories;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticopay.BNModel;
using TicoPay.Clients.Dto;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.Services;
using TicoPay.Users;

namespace TicoPay.Socket
{
    public class tenantService : ITransientDependency
    {
         private readonly ITenantAppService _tenantservice;
         public tenantService(ITenantAppService tenantservice)
        {
            _tenantservice = tenantservice;
        }

        public string TenantJob()
        {
            var tenant = _tenantservice.GetTenantsByPort(5103);
            return tenant.First().ToString();
        }

        public List<AgreementConectivity> GetTenantsByPort(int? port = null)
        {
            return _tenantservice.GetTenantsByPort(port);
        }

        public TipoLLaveAcceso GetTenantTipoAcceso(List<int> listTenant)
        {
            return (TipoLLaveAcceso)_tenantservice.GetTenantTipoAcceso(listTenant);
        }

        public string ConsultaReciboTrama(string trama, invoiceService facturaService, clientService clientService, tenantService tenantService, List<int> listTenant)
        {
            try
            {
                Console.WriteLine("Consulta Recibo de Pago");
                //_loggerForWeService.Debug("Consulta Recibo de Pago");
                var consultaRecibo = Ticopay.BNModel.ConsultaRecibo.ParserConsultaRecibo(trama); // Serializa la trama al objeto ConsultaRecibo
                return ConsultaRecibo(facturaService, clientService, tenantService, consultaRecibo, listTenant);//Para conectividad por numero de Cuenta  <----------------------------------------
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public string ConsultaRecibo(invoiceService facturaService, clientService clientService, tenantService tenantService, ConsultaRecibo consultaRecibo, List<int> listTenant)
        {
            var respuestaConsultaRecibo = new RespuestaConsultaRecibo(consultaRecibo);
            ClientBN client;
            try
            {
                TipoLLaveAcceso tipo = tenantService.GetTenantTipoAcceso(listTenant);
                client = clientService.GetExistClientByCode((MultiTenancy.TipoLLaveAcceso)tipo, long.Parse(consultaRecibo.LlaveAcceso));
                if (client == null)
                {
                    Console.WriteLine("Error, El Código del Cliente no existe");
                    respuestaConsultaRecibo.CodigoRespuesta = 3; //Por favor llamar a la asada
                    return respuestaConsultaRecibo.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error, procesando la consulta de facturas pendientes por favor llamar a TicoPay\n Exception: {0}", ex);
                respuestaConsultaRecibo.CodigoRespuesta = 1; //Por favor llamar a la asada
                return respuestaConsultaRecibo.ToString();
            }

            try
            {
                if (client != null)
                {
                    var facturas = facturaService.GetInvoicesPendingPay(client); //obtiene las facturas del cliente pendientes por pago
                    if (facturas != null && facturas.Count > 0)
                    {
                        Console.WriteLine("Consultando cliente con Codigo: {0}", client.Code);

                        string str = client.Name;
                        if (str.Length > 48)
                            str = str.Substring(0, 48);


                        respuestaConsultaRecibo.NombreCliente = str;
                        respuestaConsultaRecibo.CantidadServicios = facturas.Count(); //*** xq tiene 1???

                        var servicioPendiente = new ServicioPendiente { LlaveAcceso = consultaRecibo.LlaveAcceso };
                        foreach (var factura in facturas)
                        {
                            var recibo = new Recibo
                            {
                                Monto = (double)factura.Balance,
                                NumeroFactura = factura.Number,
                                Verificador = 0,
                                Vencimiento = factura.DueDate,
                                Periodo = factura.DueDate
                            };
                            servicioPendiente.Recibos.Add(recibo);
                        }
                        respuestaConsultaRecibo.Servicios.Add(servicioPendiente);
                    }
                    else
                    {
                        Console.WriteLine("El cliente con codigo {0} no tiene facturas pendientes", client.Code);
                        respuestaConsultaRecibo.CodigoRespuesta = 2; // El servicio no tiene recibos pendientes
                    }
                }
                else
                {
                    Console.WriteLine("El cliente con codigo {0} no existe", client.Code);
                    respuestaConsultaRecibo.CodigoRespuesta = 3; // Si el numero de cuenta no existe, por efecto la respuesta a retornar es 3 (El servicio no existe)
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ufff hubo un error procesando la consulta de facturas pendientes por favor llamar a TicoPay\n Exception: {0}", ex.ToString());
                respuestaConsultaRecibo.CodigoRespuesta = 1; // Si algo sale mal, un error no controlado, Por favor llamar a la ASADA

            }
            return respuestaConsultaRecibo.ToString();
        }


    
    }
}
