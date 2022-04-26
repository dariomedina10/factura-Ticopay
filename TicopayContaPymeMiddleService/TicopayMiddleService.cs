using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TicoPayDll.Authentication;
using TicoPayDll.Response;

namespace TicopayContaPymeMiddleService
{
    public partial class TicopayMiddleService : ServiceBase
    {
        #region Variables de Configuración del Servicio
        // Variables para el chequeo del funcionamiento del servicio
        bool _serviceState = false;
        bool _serviceConfigurated = false;
        bool _bdContaPymeConfigurated = false;
        bool _MultiUser = false;
        Thread _listener;
        // Variables para el almacenamiento de la configuración
        string _bdLocation = "";
        string _bdUser = "";
        string _bdPassword = "";
        string _connectionString = "";
        string _bdIp = "";
        #endregion

        #region Métodos Nativos del Servicio de Windows
        public TicopayMiddleService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            this.EventLog.Source = this.ServiceName;
            this.EventLog.Log = "Application";
            // this.EventLog.Log = this.ServiceName;
            _serviceState = true;
            _bdLocation = ConfigurationManager.AppSettings["bdLocation"];
            _bdUser = ConfigurationManager.AppSettings["bdUser"];
            _bdPassword = ConfigurationManager.AppSettings["bdPassword"];
            string serverType = "";
            if (ConfigurationManager.AppSettings["bdMultiUser"] == "true")
            {
                _MultiUser = true;
                serverType = "0";
            }
            else
            {
                _MultiUser = false;
                serverType = "1";
            }
            ModeloServicio contexto = new ModeloServicio();
            if (contexto.Tenants.ToList().Count == 0)
            {
                _serviceConfigurated = false;
            }
            else
            {
                _serviceConfigurated = true;
            }
            if ((_bdLocation == null) || (_bdLocation.Length == 0) || (_bdUser == null) || (_bdUser.Length == 0) || (_bdPassword == null))
            {
                _bdContaPymeConfigurated = false;
            }
            else
            {
                _bdContaPymeConfigurated = true;
            }            
            _connectionString = "User=" + _bdUser + ";" + "Password=" + _bdPassword + ";" + "Database=" + _bdLocation + ";" + "DataSource=" + _bdIp + ";" +
                    "Port=3050;" + "Dialect=3;" + "Charset=NONE;" + "Role=;" + "Connection lifetime=15;" + "Pooling=true; MinPoolSize = 0; MaxPoolSize = 50; " +
                    "Packet Size=8192;" + "ServerType=" + serverType;
            this.EventLog.WriteEntry("Servicio Ticopay - Iniciado", EventLogEntryType.Information);
            _listener = new Thread(StartListenning);
            _listener.Name = "Servidor Tcp Ticopay - ContaPyme";
            _listener.Start();

            //if (_serviceConfigurated && _bdContaPymeConfigurated)
            if (_serviceConfigurated)
            {
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);

                // Obtenemos el proceso a despertar el job
                IScheduler sched = factory.GetScheduler().GetAwaiter().GetResult();
                sched.Start();

                // Definimos el Job y lo asignamos a la clase de JobContaPymeCheck
                IJobDetail job = JobBuilder.Create<JobContaPymeCheck>()
                    .WithIdentity("myJob", "group1")
                    .UsingJobData("ContaPymeConnection", _connectionString)
                    .Build();

                // Creamos un disparador que ejecuta el job cada 30 min
                ITrigger trigger = TriggerBuilder.Create()
                  .WithIdentity("myTrigger", "group1")
                  .StartNow()
                  .WithSimpleSchedule(x => x
                      .WithIntervalInMinutes(10)
                      .RepeatForever())
                  .Build();

                sched.ScheduleJob(job, trigger);
            }
        }

        public void StartListenning()
        {
            try
            {                
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                // IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
                TcpListener tcpListener = new TcpListener(ipAddress, 5000);
                tcpListener.Start();

                byte[] bytes = new byte[1024];

                while (_serviceState == true)
                {
                    string data = "";
                    string respuesta = "";
                    Socket socket = tcpListener.AcceptSocket();

                    // Recepción de petición
                    int result = socket.Receive(bytes);
                    ASCIIEncoding asen = new ASCIIEncoding();
                    data = asen.GetString(bytes);

                    // Determino el tipo de petición enviada

                    if (data.StartsWith("idPeticion="))
                    {
                        string[] argumentos = new string[] {};                        
                        argumentos = data.Split(';');
                        // this.EventLog.WriteEntry("Petición recibida", EventLogEntryType.Information);
                        if(argumentos.Length > 0)
                        {
                            string peticion = "";
                            peticion = argumentos[0].Substring(data.IndexOf('=')+1, 2);
                            // Chequear Estado de la Configuración del Servicio
                            if (peticion.Contains("00"))
                            {
                                if (_serviceConfigurated)
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Configurar Servicio
                            if (peticion.Contains("01"))
                            {
                                if(ConfigurarServicio(argumentos[1], argumentos[2], argumentos[3], argumentos[4]))
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Chequear Estado de la Configuración de la BD ContaPyme
                            if (peticion.Contains("02"))
                            {
                                if (_bdContaPymeConfigurated)
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Configurar BD ContaPyme
                            if (peticion.Contains("03"))
                            {
                                if (ConfigurarBdContaPyme(argumentos[1], argumentos[2], argumentos[3], argumentos[4], argumentos[5]))
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Eliminar Configuración de la BD ContaPyme
                            if (peticion.Contains("04"))
                            {
                                if (EliminarConfiguracionContaPyme())
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }                            
                            // Obtener Configuración de la BD ContaPyme
                            if (peticion.Contains("05"))
                            {
                                respuesta = ObtenerConfiguracionContaPyme();
                            }
                            // Probar Conexión de la BD ContaPyme
                            if (peticion.Contains("06"))
                            {
                                if (TestConectionContaPyme())
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Crear Factura
                            if (peticion.Contains("07"))
                            {
                                try
                                {
                                    ModeloServicio contexto = new ModeloServicio();
                                    string companyCode = argumentos[3];
                                    Tenant tenantConfig = contexto.Tenants.Where(t => t.CompanyContaPyme == companyCode).Single();
                                    contexto.Dispose();
                                    Operaciones factura = new Operaciones(argumentos[1], argumentos[2], argumentos[3], TipoOperacion.Factura, tenantConfig);                                    
                                    string resultado = null;
                                    resultado = CrearFactura(factura);
                                    if (resultado == null)
                                    {
                                        respuesta = "1";
                                    }
                                    else
                                    {
                                        respuesta = resultado;
                                        this.EventLog.WriteEntry("Error en la creación de Factura: " + resultado, EventLogEntryType.Warning);
                                    }
                                } 
                                catch(Exception ex)
                                {
                                    respuesta = "Imposible Acceder a BD Servicio";
                                    this.EventLog.WriteEntry("Error en la creación de Factura: Imposible Acceder a BD Servicio" , EventLogEntryType.Error);
                                }                                
                            }
                            // Enviar Documentos en espera
                            if (peticion.Contains("08"))
                            {
                                string resultado = null;
                                resultado = EnvioDocumentosEnEspera();
                                if (resultado == null)
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = resultado;
                                    this.EventLog.WriteEntry("Error durante el envió de documentos en espera: " + resultado, EventLogEntryType.Warning);
                                }
                            }
                            // Probar Conexión de la BD ContaPyme
                            if (peticion.Contains("09"))
                            {
                                int cantidad = CantidadDocumentosPendientes();
                                if (cantidad > 0)
                                {
                                    respuesta = cantidad.ToString();
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Obtener Tenants Configurados
                            if (peticion.Contains("10"))
                            {
                                string tenants = ObtenerTenantsConfigurados();
                                if (tenants != null)
                                {
                                    respuesta = tenants;
                                }
                                else
                                {
                                    respuesta = "0;";
                                }
                            }
                            // Eliminar Tenant de la configuración
                            if (peticion.Contains("11"))
                            {                                
                                if (EliminarTenant(argumentos[1]))
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0;";
                                }
                            }
                        }
                        else
                        {
                            this.EventLog.WriteEntry("Formato de la petición incorrecto", EventLogEntryType.Error);
                        }
                    }
                    else
                    {
                        this.EventLog.WriteEntry("Formato de la petición incorrecto", EventLogEntryType.Error);
                    }

                    //Envió respuesta a la petición
                    byte[] msg = Encoding.ASCII.GetBytes(respuesta);
                    socket.Send(msg);
                    // this.EventLog.WriteEntry("Petición respondida", EventLogEntryType.Information);

                    // Cierro el Socket
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }

                tcpListener.Stop();
            }
            catch (Exception ex)
            {                
                this.EventLog.WriteEntry("Error desconocido en Servicio Ticopay", EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            _serviceState = false;
            this.EventLog.WriteEntry("Servicio Ticopay - Detenido", EventLogEntryType.Information);
        }

        #endregion

        #region Métodos de Configuración del Servicio

        public bool ConfigurarServicio(string tenant, string user, string password, string empresa)
        {
            try
            {
                ModeloServicio contexto = new ModeloServicio();
                Tenant configuracion = new Tenant(tenant,user,password,empresa);
                contexto.Tenants.Add(configuracion);
                contexto.SaveChanges();
                _serviceConfigurated = true;
                this.EventLog.WriteEntry("Credenciales del Servicio Ticopay Configuradas", EventLogEntryType.Information);
                return true;
            }
            catch
            {
                _serviceConfigurated = false;
                return false;
            }
        }

        public bool ConfigurarBdContaPyme(string bdLocation, string bdUser, string bdPassword,string bdMultiUser, string bdIp)
        {
            _bdLocation = bdLocation;
            _bdUser = bdUser;
            _bdPassword = bdPassword;
            if(bdMultiUser == "true")
            {
                _MultiUser = true;
                _bdIp = bdIp;
            }
            else
            {
                _bdIp = "localhost";
                _MultiUser = false;
            }
            try
            {
                ConfigurationManager.AppSettings["bdLocation"] = _bdLocation;
                ConfigurationManager.AppSettings["bdUser"] = _bdUser;
                ConfigurationManager.AppSettings["bdPassword"] = _bdPassword;
                ConfigurationManager.AppSettings["bdIp"] = _bdIp;
                if (_MultiUser)
                {
                    ConfigurationManager.AppSettings["bdMultiUser"] = "true";
                    _connectionString = "User=" + _bdUser + ";" + "Password=" + _bdPassword + ";" + "Database=" + _bdLocation + ";" + "DataSource=" + _bdIp + ";" +
                    "Port=3050;" + "Dialect=3;" + "Charset=NONE;" + "Role=;" + "Connection lifetime=15;" + "Pooling=true; MinPoolSize = 0; MaxPoolSize = 50; " +
                    "Packet Size=8192;" + "ServerType=0";
                }
                else
                {
                    ConfigurationManager.AppSettings["bdMultiUser"] = "false";
                    _connectionString = "User=" + _bdUser + ";" + "Password=" + _bdPassword + ";" + "Database=" + _bdLocation + ";" + "DataSource=" + _bdIp + ";" +
                    "Port=3050;" + "Dialect=3;" + "Charset=NONE;" + "Role=;" + "Connection lifetime=15;" + "Pooling=true; MinPoolSize = 0; MaxPoolSize = 50; " +
                    "Packet Size=8192;" + "ServerType=1";
                }                
                _bdContaPymeConfigurated = true;

                this.EventLog.WriteEntry("Credenciales de BD ContaPyme Configuradas", EventLogEntryType.Information);
                if (TestConectionContaPyme())
                {
                    return true;
                }
                else
                {
                    return false;
                }               
            }
            catch
            {
                _bdContaPymeConfigurated = false;
                this.EventLog.WriteEntry("Error al configurar a BD de ContaPyme", EventLogEntryType.Warning);
                return false;
            }
        }

        public string ObtenerConfiguracionContaPyme()
        {
            string respuesta = _bdLocation + ";" + _bdUser + ";" + _bdPassword + ";";
            if (_MultiUser)
            {
                respuesta = respuesta + "true;" + _bdIp + ";";
            }
            else
            {
                respuesta = respuesta + "false;;";
            }
            return respuesta;
        }

        public bool TestConectionContaPyme()
        {
            string serverType = "";
            if(_MultiUser)
            {
                serverType = "0";
            }
            else
            {
                serverType = "1";
            }
            try
            {
                _connectionString = "User=" + _bdUser + ";" + "Password=" + _bdPassword + ";" + "Database=" + _bdLocation + ";" + "DataSource="+ _bdIp +";" +
                    "Port=3050;" + "Dialect=3;" + "Charset=NONE;" + "Role=;" + "Connection lifetime=15;" + "Pooling=true; MinPoolSize = 0; MaxPoolSize = 50; " +
                    "Packet Size=8192;" + "ServerType=" + serverType;
                FbConnection myConnection = new FbConnection(_connectionString);
                myConnection.Open();
                myConnection.Close();
                this.EventLog.WriteEntry("Conexion con BD ContaPyme lograda", EventLogEntryType.Information);
                return true;
            }
            catch(Exception ex)
            {
                this.EventLog.WriteEntry("Error al tratar de acceder a BD de ContaPyme", EventLogEntryType.Warning);
                return false;
            }            
        }

        public bool EliminarConfiguracionContaPyme()
        {
            _bdLocation = null;
            _bdUser = null;
            _bdPassword = null;
            try
            {
                ConfigurationManager.AppSettings["bdLocation"] = "";
                ConfigurationManager.AppSettings["bdUser"] = "";
                ConfigurationManager.AppSettings["bdPassword"] = "";
                ConfigurationManager.AppSettings["bdIp"] = "";
                ConfigurationManager.AppSettings["bdMultiUser"] = "false";
                _bdContaPymeConfigurated = false;
                this.EventLog.WriteEntry("Credenciales de BD ContaPyme Eliminadas", EventLogEntryType.Information);
                return true;
            }
            catch
            {
                _bdContaPymeConfigurated = false;
                this.EventLog.WriteEntry("Error al eliminar Credenciales de la BD de ContaPyme", EventLogEntryType.Warning);
                return false;
            }
        }

        public int CantidadDocumentosPendientes()
        {
            List<Operaciones> listaDocumentos = new List<Operaciones>();
            listaDocumentos = ObtenerListaOperaciones();
            if(listaDocumentos != null)
            {
                return listaDocumentos.Count();
            }
            else
            {
                return 0;
            }
        }

        public string ObtenerTenantsConfigurados()
        {
            ModeloServicio contexto = new ModeloServicio();
            List<Tenant> tenants = contexto.Tenants.ToList();
            string respuesta = "";
            foreach (Tenant item in tenants)
            {
                respuesta = respuesta + item.Id + "/" + item.TenantTicopay + "/" + item.UserTicopay + "/" + item.CompanyContaPyme + "/" + ";";
            }
            return respuesta;
        }

        public bool EliminarTenant(string tenantId)
        {
            try
            {
                ModeloServicio contexto = new ModeloServicio();                
                Tenant tenantEliminado = contexto.Tenants.Find(Guid.Parse(tenantId));
                contexto.Tenants.Remove(tenantEliminado);
                contexto.SaveChanges();
                if (contexto.Tenants.Count() == 0 )
                {
                    _serviceConfigurated = false;
                }
                this.EventLog.WriteEntry("Tenant " + tenantEliminado.TenantTicopay + " eliminado", EventLogEntryType.Information);
                return true;
            }
            catch (Exception ex)
            {
                this.EventLog.WriteEntry("Error al eliminar tenant = " + tenantId, EventLogEntryType.Error);
                return false;
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

        #region Métodos para procesar las operaciones

        public string EnvioDocumentosEnEspera()
        {
            if(_bdContaPymeConfigurated)
            {
                ModeloServicio contexto = new ModeloServicio();
                // Leo todas las operación pendientes por realizar
                List<Operaciones> listaDocumentos = new List<Operaciones>();
                int nErrores = 0;
                listaDocumentos = ObtenerListaOperaciones();
                // Proceso todas las operaciones
                foreach (Operaciones operacion in listaDocumentos)
                {
                    string resultado = null;
                    switch (operacion.TipoDeOperacion)
                    {
                        case TipoOperacion.Factura:
                            resultado = operacion.EnviarFactura(_connectionString);
                            break;
                        case TipoOperacion.NotaCredito:
                            resultado = operacion.EnviarNotaCredito(_connectionString,false);
                            break;
                        case TipoOperacion.NotaDebito:
                            resultado = operacion.EnviarNotaDebito(_connectionString);
                            break;
                        case TipoOperacion.DevolucionFactura:
                            resultado = operacion.EnviarNotaCredito(_connectionString,true);
                            break;
                        case TipoOperacion.PagoFactura:
                            resultado = operacion.PagarFacturaCredito(_connectionString);
                            break;
                    }
                    Operaciones documento = contexto.Documentos.Find(operacion.IdOperacion);
                    if (resultado == null)
                    {                        
                        operacion.EstadoOperacion = Estado.Procesado;                        
                    }
                    else
                    {
                        // Si se produce un error procesando la operación lo registro en el visor de eventos
                        operacion.EstadoOperacion = Estado.Error;
                        nErrores++;
                        this.EventLog.WriteEntry("Error al enviar el documento " + operacion.IdDocumento + " Razón : " + resultado, EventLogEntryType.Error);
                    }
                    contexto.Entry(documento).CurrentValues.SetValues(operacion);
                    contexto.SaveChanges();
                }
                if (nErrores > 0)
                {
                    return "Algunas operaciones no pudieron ser realizadas";
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return "No existen credenciales configuradas para conectar a la BD de ContaPyme";
            }           
        }

        public List<Operaciones> ObtenerListaOperaciones()
        {
            try
            {
                ModeloServicio contexto = new ModeloServicio();
                List<Operaciones> listado = new List<Operaciones>();
                listado = contexto.Documentos.Where(d => d.EstadoOperacion == Estado.NoProcesado || d.EstadoOperacion == Estado.Error).ToList();
                return listado;
            }
            catch(Exception ex)
            {
                this.EventLog.WriteEntry("Error al obtener el listado de operaciones pendientes ", EventLogEntryType.Error);
                return null;
            }
        }

        public string CrearFactura(Operaciones _factura)
        {
            if (_bdContaPymeConfigurated)
            {
                try
                {
                    ModeloServicio contexto = new ModeloServicio();
                    if (_MultiUser)
                    {
                        string resultado = null;
                        if (_factura.EstadoOperacion == Estado.NoProcesado)
                        {
                            Operaciones nuevaFactura = _factura;
                            nuevaFactura.Cfg = contexto.Tenants.Find(_factura.Cfg.Id);
                            contexto.Documentos.Add(nuevaFactura);
                            contexto.SaveChanges();
                        }
                        Operaciones factura = contexto.Documentos.Find(_factura.IdOperacion);
                        resultado = factura.EnviarFactura(_connectionString);
                        if (resultado != null)
                        {
                            _factura.EstadoOperacion = Estado.Error;
                            this.EventLog.WriteEntry("Error al crear Factura " + _factura.IdDocumento + "en Ticopay, Error = " + resultado, EventLogEntryType.Error);
                            
                        }
                        else
                        {
                            _factura.EstadoOperacion = Estado.Procesado;
                            this.EventLog.WriteEntry("Factura " + _factura.IdDocumento + " enviada a Ticopay.", EventLogEntryType.Information);
                        }                        
                        contexto.Entry(factura).CurrentValues.SetValues(_factura);
                        contexto.SaveChanges();
                        return resultado;
                    }
                    else
                    {
                        this.EventLog.WriteEntry("Factura " + _factura.IdDocumento + " puesta en espera.", EventLogEntryType.Information);
                        return null;
                    }                    
                }
                catch (Exception ex)
                {
                    this.EventLog.WriteEntry("Error al crear Factura " + _factura.IdDocumento + "en Ticopay, Error = Guardar Factura en la BD del Servicio", EventLogEntryType.Error);
                    return "Error = Guardar Factura en la BD del Servicio";
                }                
            }
            else
            {
                return "No existen credenciales configuradas para conectar a la BD de ContaPyme";
            }
        }

        #endregion
    }
}
