using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Taxes;
using TicopayUniversalConnectorService.Contexto;
using TicopayUniversalConnectorService.Entities;
using TicopayUniversalConnectorService.Interfaces;
using TicopayUniversalConnectorService.Log;

namespace TicopayUniversalConnectorService.Reports
{
    public class ReportJob : IConector
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

            if(_contexto.Configuraciones.Count() > 0)
            {
                foreach (Configuracion job in _contexto.Configuraciones.ToList())
                {
                    if(_contexto.Operaciones.ToList().Where(o=> o.Configuracion.Id == job.Id).Count() > 0)
                    {
                        List<Operacion> operacionesAReportar = _contexto.Operaciones.ToList().Where(o => o.Configuracion.Id == job.Id && o.Registrado >= DateTime.Now.AddDays(-2)).ToList();
                        if(operacionesAReportar.Count > 0)
                        {
                            if (Directory.Exists(ConfigurationManager.AppSettings["ReportLocation"].ToString()) == false )
                            {
                                DirectoryInfo directory = Directory.CreateDirectory(ConfigurationManager.AppSettings["ReportLocation"].ToString());
                            }
                            string ruta = ConfigurationManager.AppSettings["ReportLocation"].ToString() + "ReporteOperacionesProcesadas"+ job.SubDominioTicopay + DateTime.Now.ToString("dd-MM-yyyy-HH") + ".pdf";
                            PdfReportGenerator Generador = new PdfReportGenerator();
                            Generador.GenerateOperationsSincronizedReport(operacionesAReportar, job, ruta);
                        }

                        // List<Operacion> operacionesConErrores = _contexto.Operaciones.ToList().Where(o => o.Configuracion.Id == job.Id && o.Registrado < DateTime.Now.AddDays(-1) && o.EstadoOperacion == Estado.Error).ToList();
                        List<Operacion> operacionesConErrores = _contexto.Operaciones.ToList().Where(o => o.Configuracion.Id == job.Id && o.EstadoOperacion == Estado.Error).ToList();
                        if (operacionesConErrores.Count > 0)
                        {
                            if (Directory.Exists(ConfigurationManager.AppSettings["ReportLocation"].ToString()) == false)
                            {
                                DirectoryInfo directory = Directory.CreateDirectory(ConfigurationManager.AppSettings["ReportLocation"].ToString());
                            }
                            string ruta = ConfigurationManager.AppSettings["ReportLocation"].ToString() + "ReporteOperacionesFallidas" + job.SubDominioTicopay + DateTime.Now.ToString("dd-MM-yyyy-HH") + ".pdf";
                            PdfReportGenerator Generador = new PdfReportGenerator();
                            Generador.GenerateOperationsErrorReport(operacionesConErrores, job, ruta);
                        }
                    }
                }
            }
            _contexto.Dispose();
            return Task.CompletedTask;
        }

        public Client IngresarCliente(Operacion operacion, Configuracion configuracion)
        {
            throw new NotImplementedException();
        }

        public void ProcesarOperaciones(List<Operacion> operacionesPendientes, Configuracion configuracion)
        {
            throw new NotImplementedException();
        }
    }
}
