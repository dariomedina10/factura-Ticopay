using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TicopayUniversalConnectorService.Conectores;
using TicopayUniversalConnectorService.Contexto;
using TicopayUniversalConnectorService.Entities;
using TicopayUniversalConnectorService.Log;
using TicopayUniversalConnectorService.Reports;

namespace TicopayUniversalConnectorService
{
    /// <summary>
    /// Servicio de Windows encargado de Sincronizar los Conectores con Ticopay
    /// </summary>
    /// <seealso cref="System.ServiceProcess.ServiceBase" />
    public partial class UniversalConnectorService : ServiceBase
    {
        #region Variables para el chequeo del funcionamiento del servicio

        bool _serviceState = false;
        Thread _listener;
        RegistroDeEventos _eventos;
        UniversalConnectorDB _contexto;
        CancellationTokenSource cts;

        #endregion

        #region Métodos Nativos del Servicio
        /// <summary>
        /// Inicia liza una instancia de la clase <see cref="UniversalConnectorService"/>.
        /// </summary>
        public UniversalConnectorService()
        {
            InitializeComponent();
            // Instan ciar del registrador de eventos
            _eventos = new RegistroDeEventos();
            // Instan ciar el Contexto de la BD
            _contexto = new UniversalConnectorDB();

            // Create the token source.
            cts = new CancellationTokenSource();

            // Inicializar la Escucha de TCP
            // _listener = new Thread(StartListenning);
            // _listener.Name = "Servidor Tcp Universal Ticopay";
        }

        /// <summary>
        /// Llamado en modo Debug para iniciar el Servicio sin necesidad de instalarlo.
        /// </summary>
        public void OnDebug()
        {
            OnStart(null);
        }

        /// <summary>
        /// Se ejecuta cuando el comando Iniciar es enviado por el Service Control Manager (SCM) O cuando inicia el sistema operativo. Especifica las acciones a realizar al iniciar el servicio.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            _serviceState = true;            

            // Pass the token to the cancelable operation.
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartListenning), cts.Token);

            // _listener.Start();

            _eventos.Confirmacion("Servicio Universal de Conexión", "Estatus del Servicio", "Servicio Iniciado");

            // Si existen Configuraciones creadas al iniciar el servicio se Programan los Jobs de Conexión
            if(_contexto.Configuraciones.Count() > 0)
            {
                foreach (Configuracion Job in _contexto.Configuraciones)
                {
                    StarConnectorJob(Job);
                }
                StarReportJob();
            }
        }

        /// <summary>
        /// Se ejecuta cuando el Comando Detener es enviado por el Service Control Manager (SCM). y especifica otras acciones a realizar al detener el servicio.
        /// </summary>
        protected override void OnStop()
        {
            if (_serviceState)
            {
                string respuesta = "";
                try
                {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipAddress = ipHostInfo.AddressList[0];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                    // Crear el socket.  
                    Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    byte[] bytes = new byte[1024];
                    string data = "idPeticion=99" + Convert.ToChar(30);
                    sender.Connect(remoteEP);

                    //Envió petición
                    byte[] msg = Encoding.ASCII.GetBytes(data);
                    sender.Send(msg);

                    //Recibo respuesta
                    int bytesRec = sender.Receive(bytes);
                    respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch
                {
                    
                }
            }
            _eventos.Confirmacion("Servicio Universal de Conexión", "Estatus del Servicio", "Servicio Detenido");
            // _eventos = null;
            // _contexto = null;
            cts.Cancel();
            Thread.Sleep(2500);
            cts.Dispose();            
        }

        #endregion

        #region Método para la Ejecución de Jobs de Connexion (Único Método a modificar al agregar Conectores)
        /// <summary>
        /// Inicia un Trabajo configurado.
        /// </summary>
        public bool StarConnectorJob(Configuracion JobAEjecutar, bool Forzado = false)
        {
            try
            {
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);

                // Obtenemos el proceso a despertar el job
                IScheduler sched = factory.GetScheduler().GetAwaiter().GetResult();
                sched.Start();
                IJobDetail job = null;
                ITrigger trigger = null;
                // Definimos el Job y lo asignamos a la clase de Job
                switch (JobAEjecutar.TipoConector)
                {
                    case Conector.Contapyme:
                        job = JobBuilder.Create<ContaPymeJob>()
                            .WithIdentity(JobAEjecutar.TipoConector.ToString()+"Job"+ JobAEjecutar.SubDominioTicopay, JobAEjecutar.TipoConector.ToString() + "group" + Forzado.ToString())
                            .UsingJobData("IdConfiguracion", JobAEjecutar.Id.ToString())
                            .Build();
                        break;
                    case Conector.QuickbooksEnterprise:
                        job = JobBuilder.Create<QuickBooksEnterpriseJob>()
                            .WithIdentity(JobAEjecutar.TipoConector.ToString() + "Job" + JobAEjecutar.SubDominioTicopay, JobAEjecutar.TipoConector.ToString() + "group" + Forzado.ToString())
                            .UsingJobData("IdConfiguracion", JobAEjecutar.Id.ToString())
                            .Build();
                        break;
                    case Conector.TicopayContapyme:
                        job = JobBuilder.Create<TicopayContaPymeJob>()
                            .WithIdentity(JobAEjecutar.TipoConector.ToString() + "Job" + JobAEjecutar.SubDominioTicopay, JobAEjecutar.TipoConector.ToString() + "group" + Forzado.ToString())
                            .UsingJobData("IdConfiguracion", JobAEjecutar.Id.ToString())
                            .Build();
                        break;
                }
                // Creamos un disparador que ejecuta el job
                if(Forzado)
                {
                    // Forzado (Una sola vez)
                    trigger = TriggerBuilder.Create()
                      .WithIdentity(JobAEjecutar.TipoConector.ToString() + JobAEjecutar.SubDominioTicopay + "TriggerForzado", JobAEjecutar.TipoConector.ToString() + "group" + Forzado.ToString())
                      .StartNow()
                      .WithSimpleSchedule(x => x                          
                          .WithRepeatCount(0))
                      .Build();
                }
                else
                {
                    // cada 5 min
                    trigger = TriggerBuilder.Create()
                      .WithIdentity(JobAEjecutar.TipoConector.ToString() + JobAEjecutar.SubDominioTicopay + "Trigger5Min", JobAEjecutar.TipoConector.ToString() + "group" + Forzado.ToString())
                      .StartNow()
                      .WithSimpleSchedule(x => x
                          .WithIntervalInMinutes(5)
                          .RepeatForever())
                      .Build();
                }
                sched.ScheduleJob(job, trigger);
                _eventos.Confirmacion("Servicio Universal de Conexión", "Ejecución de un Job", "Job " +job.Key.Name +" Iniciado");
                return true;
            }
            catch (Exception ex)
            {
                _eventos.Error("Servicio Universal de Conexión", "Programar la ejecución de un Job", "Imposible iniciar Job");
                return false;
            }
        }

        public bool StarReportJob(bool Forzado = false)
        {
            try
            {
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);

                // Obtenemos el proceso a despertar el job
                IScheduler sched = factory.GetScheduler().GetAwaiter().GetResult();
                sched.Start();
                IJobDetail job = null;
                ITrigger trigger = null;
                // Definimos el Job y lo asignamos a la clase de Job
                job = JobBuilder.Create<ReportJob>()
                            .WithIdentity("Report Job", "Report group" + Forzado.ToString())
                            .Build();
                // Creamos un disparador que ejecuta el job
                if (Forzado)
                {
                    // Forzado (Una sola vez)
                    trigger = TriggerBuilder.Create()
                      .WithIdentity("Report TriggerForzado", "Report group" + Forzado.ToString())
                      .StartNow()
                      .WithSimpleSchedule(x => x
                          .WithRepeatCount(0))
                      .Build();
                }
                else
                {
                    // cada 24 Horas
                    trigger = TriggerBuilder.Create()
                      .WithIdentity("Report Trigger24Horas", "Report group" + Forzado.ToString())
                      .StartNow()
                      .WithSimpleSchedule(x => x
                          .WithIntervalInHours(24)
                          .RepeatForever())
                      .Build();
                }
                sched.ScheduleJob(job, trigger);
                _eventos.Confirmacion("Servicio Universal de Conexión", "Ejecución de un Job", "Job " + job.Key.Name + " Iniciado");
                return true;
            }
            catch (Exception ex)
            {
                _eventos.Error("Servicio Universal de Conexión", "Programar la ejecución de un Job", "Imposible iniciar Job");
                return false;
            }
        }
        #endregion

        #region Métodos para la interacción con la Consola de Administración
        /// <summary>
        /// Comienza la escucha de TCP IP para comunicarse con la Consola de Administración.
        /// </summary>
        public void StartListenning(object token = null)
        {
            try
            {
                CancellationToken _token = (CancellationToken) token;

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                TcpListener tcpListener = new TcpListener(ipAddress, 5000);
                tcpListener.Start();

                byte[] bytes = new byte[1024];

                while (_serviceState == true)
                {
                    if (_token.IsCancellationRequested)
                    {
                        break;
                    }
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
                        string[] argumentos = new string[] { };
                        argumentos = data.Split(Convert.ToChar(30));
                        // this.EventLog.WriteEntry("Petición recibida", EventLogEntryType.Information);
                        if (argumentos.Length > 0)
                        {
                            string peticion = "";
                            peticion = argumentos[0].Substring(data.IndexOf('=') + 1, 2);                            
                            // Configurar Servicio
                            if (peticion.Contains("00"))
                            {
                                if (ConfigurarNuevoTrabajo(argumentos[1], argumentos[2], argumentos[3], argumentos[4], argumentos[5], argumentos[6]))
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Obtener Tenants Configurados
                            if (peticion.Contains("01"))
                            {
                                string configuraciones = ObtenerConfiguraciones();
                                if (configuraciones != null)
                                {
                                    respuesta = configuraciones;
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Eliminar Tenant de la configuración
                            if (peticion.Contains("02"))
                            {
                                if (EliminarConfiguracion(argumentos[1]))
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Forzar Sincronización
                            if (peticion.Contains("03"))
                            {
                                if (SincronizarJobEspecifico(argumentos[1]))
                                {
                                    respuesta = "1";
                                }
                                else
                                {
                                    respuesta = "0";
                                }
                            }
                            // Forzar Reportes
                            if (peticion.Contains("04"))
                            {
                                StarReportJob(true);
                                respuesta = "1";
                            }
                            // Detener Servicio
                            if (peticion.Contains("99"))
                            {
                                _serviceState = false;
                                respuesta = "1";
                            }
                        }
                        else
                        {
                            _eventos.Error("Servicio Universal de Conexión","Ejecutar Acción desde la Consola","Formato de la Petición incorrecto");
                        }
                    }
                    else
                    {
                        _eventos.Error("Servicio Universal de Conexión", "Ejecutar Acción desde la Consola", "Formato de la Petición incorrecto");
                    }

                    //Envió respuesta a la petición
                    byte[] msg = Encoding.ASCII.GetBytes(respuesta);
                    socket.Send(msg);                   

                    // Cierro el Socket
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }

                tcpListener.Stop();
                OnStop();
            }
            catch (Exception ex)
            {
                _eventos.Error("Servicio Universal de Conexión", "Escucha de Peticiones de la Consola", "Error desconocido");
            }
        }

        /// <summary>
        /// Programa un Job para que se ejecute de forma obligatoria fuera de tiempo.
        /// </summary>
        /// <param name="idConfiguracion">Id de Configuración del Job a Forzar.</param>
        /// <returns>Verdadero si el Job se intento programar, Falso de no haber logrado enviar el comando</returns>
        private bool SincronizarJobEspecifico(string idConfiguracion)
        {
            try
            {
                Guid _idConfiguracion = Guid.Parse(idConfiguracion);
                Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == _idConfiguracion).Single();
                if(Job != null)
                {
                    return StarConnectorJob(Job, true);                    
                }
                _eventos.Advertencia("Servicio Universal de Conexión", "Forzar un Job a Correr fuera de Tiempo", "Configuración de Job no encontrada");
                return false;
            }
            catch (Exception ex)
            {
                _eventos.Error("Servicio Universal de Conexión", "Forzar un Job a Correr fuera de Tiempo", "Error desconocido");
                return false;
            }
        }

        /// <summary>
        /// Elimina la configuración de un Job del servicio.
        /// </summary>
        /// <param name="idConfiguracion">Id de Configuración.</param>
        /// <returns>Verdadero si elimino la Configuración , o falso si no pudo ser eliminada</returns>
        private bool EliminarConfiguracion(string idConfiguracion)
        {
            try
            {
                Guid _idConfiguracion = Guid.Parse(idConfiguracion);
                Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == _idConfiguracion).First();
                if (Job != null)
                {
                    if (_contexto.Operaciones.ToList().Where(o => o.Configuracion.Id == _idConfiguracion 
                    && (o.EstadoOperacion == Estado.Error || o.EstadoOperacion == Estado.NoProcesado) 
                    && o.Registrado < DateTime.Now.AddHours(-DateTime.Now.Hour)).Count() == 0)
                    {
                        _contexto.Configuraciones.Remove(Job);
                        _contexto.SaveChanges();
                        _eventos.Confirmacion("Servicio Universal de Conexión", "Eliminar la configuración de un Job", "Configuración de Job eliminada");
                        return true;
                    }
                    else
                    {
                        _eventos.Advertencia("Servicio Universal de Conexión", "Eliminar la configuración de un Job", "El Job contiene operaciones pendientes por corrección");
                        return false;
                    }
                }
                _eventos.Advertencia("Servicio Universal de Conexión", "Eliminar la configuración de un Job", "Configuración de Job a eliminar no encontrada");
                return false;                    
            }
            catch (Exception ex)
            {
                _eventos.Error("Servicio Universal de Conexión", "Eliminar un Job de la Configuración", "Error desconocido");
                return false;
            }
        }

        /// <summary>
        /// Obtiene la lista de Jobs configurados en el Servicio.
        /// </summary>
        /// <returns>String con la Lista de Jobs para ser enviada por TCP IP a la Consola</returns>
        private string ObtenerConfiguraciones()
        {
            try
            {
                List<Configuracion> JobsConfigurados = _contexto.Configuraciones.ToList();
                string respuesta = "";
                if(JobsConfigurados.Count > 0)
                {
                    foreach (Configuracion Job in JobsConfigurados)
                    {
                        respuesta = respuesta + Job.Id + Convert.ToChar(29) + Job.SubDominioTicopay + Convert.ToChar(29) + Job.UsuarioTicopay + Convert.ToChar(29) + Job.TipoConector.ToString() + Convert.ToChar(29) + Convert.ToChar(30);
                    }
                    return respuesta;
                }
                else
                {
                    _eventos.Advertencia("Servicio Universal de Conexión", "Obtener Jobs Configurados", "No existen Jobs en la BD");
                    return respuesta;
                }
            }
            catch
            {
                _eventos.Error("Servicio Universal de Conexión", "Obtener Jobs Configurados", "Error al Leer los Job de la BD");
                return null;
            }
        }

        /// <summary>
        /// Agrega una nueva configuración de Conector al Servicio.
        /// </summary>
        /// <param name="SubDominioTicopay">Nombre del Sub Dominio de Ticopay.</param>
        /// <param name="UsuarioTicopay">Usuario de Sub Dominio ticopay.</param>
        /// <param name="ClaveTicopay">Contraseña del Usuario de ticopay.</param>
        /// <param name="IdEmpresa">Identificador de Empresa.</param>
        /// <param name="DatosConexion">Datos de Conexión.</param>
        /// <param name="TipoConector">Tipo de Conector a utilizar.</param>
        /// <returns>Verdadero si se agrego la configuración , falso si no</returns>
        private bool ConfigurarNuevoTrabajo(string SubDominioTicopay, string UsuarioTicopay, string ClaveTicopay, string IdEmpresa, string DatosConexion, string TipoConector)
        {
            try
            {                
                Configuracion newJob = new Configuracion(SubDominioTicopay,UsuarioTicopay,ClaveTicopay,IdEmpresa,DatosConexion, (Conector) Enum.Parse(typeof(Conector),TipoConector));
                _contexto.Configuraciones.Add(newJob);
                _contexto.SaveChanges();
                _eventos.Confirmacion("Servicio Universal de Conexión","Configuración de Job","Nuevo Job agregado, Tenant " + SubDominioTicopay + " Conector " + TipoConector);
                return true;
            }
            catch
            {
                _eventos.Error("Servicio Universal de Conexión", "Configuración de Job", "Job Imposible de agregar, Tenant " + SubDominioTicopay + " Conector " + TipoConector);
                return false;
            }
        }

        /// <summary>
        /// Elimina las operaciones en estado de error de un conector.
        /// </summary>
        /// <returns></returns>
        private bool EliminarTrabajosOperacionesError(string idConector)
        {
            try
            {
                Guid idJob = Guid.Parse(idConector);
                _contexto = new UniversalConnectorDB();
                Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == idJob).First();
                List<Operacion> operacionesAEliminar = _contexto.Operaciones.Where(o => o.EstadoOperacion == Estado.Error).ToList();
                List<Operacion> operacionesPorEliminar = operacionesAEliminar.Where(o => o.Configuracion == Job).ToList();
                _contexto.Operaciones.RemoveRange(operacionesPorEliminar);
                int result = _contexto.SaveChanges();
                _contexto.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                _eventos.Error("Servicio Universal de Conexión", "Eliminar Operaciones Error", "Imposible eliminar las operaciones en error, " + ex.Message);
                return false;
            }
        }
        #endregion
    }
}
