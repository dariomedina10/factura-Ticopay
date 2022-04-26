using FirebirdSql.Data.FirebirdClient;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayContaPymeMiddleService
{
    public class JobContaPymeCheck : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            // Cadena de Conexión a ContaPyme
            string CadenaConexion = dataMap.GetString("ContaPymeConnection");

            // Preparo conexión para buscar documentos recientes
            FbConnection myConnection = new FbConnection(CadenaConexion);
            myConnection.Open();
            FbCommand readCommand;
            FbDataReader myreader;
            ModeloServicio contexto = new ModeloServicio();

            // Busca los Documentos en el maestro de documentos OPRMAEST de el ultimo día
            readCommand =
              new FbCommand("Select IEMP,ITDSOP,INUMOPER,INIT,BANULADA,IPROCESS From OPRMAEST Where FPROCESAM > dateadd (day, -7, current_date)", myConnection);
            myreader = readCommand.ExecuteReader();
            string companyCode = null;
            string idDocumento = null;
            string idCliente = null;
            while (myreader.Read())
            {
                // Empresa 
                companyCode = myreader[0].ToString();
                // ID Documento ContaPyme
                idDocumento = myreader[2].ToString();
                // ID Cliente ContaPyme
                idCliente = myreader[3].ToString();
                Tenant tenantConfig = contexto.Tenants.Where(t => t.CompanyContaPyme == companyCode).Single();                
                // Tipo Documento
                // 10  Factura de Venta
                // 20  Devolución de Venta
                // 230 Nota de Crédito
                // 220 Nota de Débito
                // 
                switch (myreader[1].ToString())
                {
                    case "10":
                        if(myreader[5].ToString() == "2")
                        { // Es una factura procesada
                            if (contexto.Documentos.Where(t => t.IdDocumento == idDocumento && t.IdEmpresa == companyCode && t.TipoDeOperacion == TipoOperacion.Factura).Count() == 0)
                            {
                                Operaciones factura = new Operaciones(idDocumento, idCliente, companyCode, TipoOperacion.Factura, tenantConfig);
                                CrearFactura(factura, CadenaConexion);
                            }
                        }
                        if (myreader[5].ToString() == "0" && myreader[4].ToString() == "T")
                        { // Es una anulación de una factura
                            
                            if (contexto.Documentos.Where(t => t.IdDocumento == idDocumento && t.IdEmpresa == companyCode && t.TipoDeOperacion == TipoOperacion.NotaCredito).Count() == 0)
                            {
                                Operaciones nota = new Operaciones(idDocumento, idCliente, companyCode, TipoOperacion.NotaCredito, tenantConfig);
                                CrearNota(nota, CadenaConexion, false);
                            }
                        }                        
                        break;
                    case "20":
                        if (myreader[5].ToString() == "2")
                        { // Es una Devolución de Factura
                            if (contexto.Documentos.Where(t => t.IdDocumento == idDocumento && t.IdEmpresa == companyCode && t.TipoDeOperacion == TipoOperacion.DevolucionFactura).Count() == 0)
                            {
                                Operaciones nota = new Operaciones(idDocumento, idCliente, companyCode, TipoOperacion.DevolucionFactura, tenantConfig);
                                CrearNota(nota, CadenaConexion, true);
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
            }
            myreader.Close();
            contexto.Dispose();
            return Task.CompletedTask;
        }

        public bool CrearFactura(Operaciones _factura, string ConexionCP)
        {
            EventLog eventLog = new EventLog("Application");
            eventLog.Source = "Job Actualización ContaPyme";
            eventLog.Log = "Application";
                try
                {
                    ModeloServicio contexto = new ModeloServicio();
                    string resultado = null;
                    if (_factura.EstadoOperacion == Estado.NoProcesado)
                    {
                        Operaciones nuevaFactura = _factura;
                        nuevaFactura.Cfg = contexto.Tenants.Find(_factura.Cfg.Id);
                        contexto.Documentos.Add(nuevaFactura);
                        contexto.SaveChanges();
                    }
                    Operaciones factura = contexto.Documentos.Find(_factura.IdOperacion);
                    resultado = factura.EnviarFactura(ConexionCP);
                    if (resultado == null)
                    {
                        _factura.EstadoOperacion = Estado.Error;
                        eventLog.WriteEntry("Error al crear Factura " + _factura.IdDocumento + "en Ticopay, Error = " + resultado, EventLogEntryType.Error);
                    }
                    else
                    {
                        _factura.EstadoOperacion = Estado.Procesado;
                        _factura.IdTicopayDocument = resultado;
                        // eventLog.WriteEntry("Factura " + _factura.IdDocumento + " enviada a Ticopay.", EventLogEntryType.Information);
                    }
                    contexto.Entry(factura).CurrentValues.SetValues(_factura);
                    contexto.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry("Error al crear Factura " + _factura.IdDocumento + "en Ticopay, Error = Guardar Factura en la BD del Servicio", EventLogEntryType.Error);
                    return false;
                }
        }

        public bool CrearNota(Operaciones _nota, string ConexionCP, bool Devolucion)
        {
            EventLog eventLog = new EventLog("Application");
            eventLog.Source = "Job Actualización ContaPyme";
            eventLog.Log = "Application";
            try
            {
                ModeloServicio contexto = new ModeloServicio();
                string resultado = null;
                if (_nota.EstadoOperacion == Estado.NoProcesado)
                {
                    Operaciones nuevaNota = _nota;
                    nuevaNota.Cfg = contexto.Tenants.Find(_nota.Cfg.Id);
                    contexto.Documentos.Add(nuevaNota);
                    contexto.SaveChanges();
                }
                Operaciones nota = contexto.Documentos.Find(_nota.IdOperacion);
                resultado = nota.EnviarNotaCredito(ConexionCP, Devolucion);
                if (resultado == null)
                {
                    _nota.EstadoOperacion = Estado.Error;
                    eventLog.WriteEntry("Error al crear Nota " + _nota.IdDocumento + "en Ticopay, Error = " + resultado, EventLogEntryType.Error);
                }
                else
                {
                    _nota.EstadoOperacion = Estado.Procesado;
                    _nota.IdTicopayDocument = resultado;
                    // eventLog.WriteEntry("Nota " + _nota.IdDocumento + " enviada a Ticopay.", EventLogEntryType.Information);
                }
                contexto.Entry(nota).CurrentValues.SetValues(_nota);
                contexto.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Error al crear Nota " + _nota.IdDocumento + "en Ticopay, Error = Guardar Nota en la BD del Servicio", EventLogEntryType.Error);
                return false;
            }
        }
    }
}
