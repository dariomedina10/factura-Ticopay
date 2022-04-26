using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicopayUniversalConnectorService.Conectores;
using TicopayUniversalConnectorService.Entities;
using TicopayUniversalConnectorService.Log;
using TicopayUniversalConnectorService.Reports;
using UniversalTrayApp.Contexto;
using UniversalTrayApp.Forms_de_Conectores;

namespace UniversalTrayApp
{
    public partial class Principal : Form
    {
        RegistroDeEventos _eventos;
        UniversalConnectorDB _contexto;

        public Principal()
        {
            InitializeComponent();
            // Instan ciar del registrador de eventos
            _eventos = new RegistroDeEventos();
            // Instan ciar el Contexto de la BD
            _contexto = new UniversalConnectorDB();
            // Si existen Configuraciones creadas al iniciar el servicio se Programan los Jobs de Conexión
            if (_contexto.Configuraciones.Count() > 0)
            {
                foreach (Configuracion Job in _contexto.Configuraciones)
                {
                    StarConnectorJob(Job);
                }
                StarReportJob();
            }
            _contexto.Dispose();
            UpdateConfigurations();
            TiposDeConectorCb.Items.Clear();
            TiposDeConectorCb.DataSource = Enum.GetValues(typeof(Conector));
            UpdateLog();
        }

        #region Bandeja de sistema

        private void Principal_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                Notificacion.Visible = true;
            }
        }

        private void Notificacion_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            Notificacion.Visible = false;
        }

        #endregion

        public bool StarConnectorJob(Configuracion JobAEjecutar, bool Forzado = false)
        {
            try
            {
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);

                int tiempoEjecucion = 5;
                try
                {
                    if (ConfigurationManager.AppSettings["SyncPeriod"].ToString() != null && ConfigurationManager.AppSettings["SyncPeriod"].ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(ConfigurationManager.AppSettings["SyncPeriod"].ToString()) >= 2)
                        {
                            tiempoEjecucion = Convert.ToInt32(ConfigurationManager.AppSettings["SyncPeriod"].ToString());
                        }
                        else
                        {
                            _eventos.Advertencia("App Universal de Conexión", "Programar la ejecución de un Job", "La configuración del periodo de sincronzación debe ser mayor a 2, se usa 5 min por defecto");
                        }
                    }
                    else
                    {
                        _eventos.Advertencia("App Universal de Conexión", "Programar la ejecución de un Job", "La configuración del periodo de sincronzación es incorrecta, se usa 5 min por defecto");
                    }
                }
                catch
                {
                    _eventos.Advertencia("App Universal de Conexión", "Programar la ejecución de un Job", "La configuración del periodo de sincronzación es incorrecta, se usa 5 min por defecto");
                }                      

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
                            .WithIdentity(JobAEjecutar.TipoConector.ToString() + "Job" + JobAEjecutar.SubDominioTicopay, JobAEjecutar.TipoConector.ToString() + "group" + Forzado.ToString())
                            .UsingJobData("IdConfiguracion", JobAEjecutar.Id.ToString())
                            .Build();
                        break;
                    case Conector.QuickbooksEnterprise:
                        job = JobBuilder.Create<QuickBooksEnterpriseJob>()
                            .WithIdentity(JobAEjecutar.TipoConector.ToString() + "Job" + JobAEjecutar.SubDominioTicopay, JobAEjecutar.TipoConector.ToString() + "group" + Forzado.ToString())
                            .UsingJobData("IdConfiguracion", JobAEjecutar.Id.ToString())
                            .Build();
                        break;
                }
                // Creamos un disparador que ejecuta el job
                if (Forzado)
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
                    // cada 5 min por defecto
                    trigger = TriggerBuilder.Create()
                      .WithIdentity(JobAEjecutar.TipoConector.ToString() + JobAEjecutar.SubDominioTicopay + "Trigger5Min", JobAEjecutar.TipoConector.ToString() + "group" + Forzado.ToString())
                      .StartNow()
                      .WithSimpleSchedule(x => x
                          .WithIntervalInMinutes(tiempoEjecucion)
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

        /// <summary>
        /// Actualiza el Control del Log para mostrar los últimos eventos del servicio.
        /// </summary>
        private void UpdateLog()
        {
            try
            {
                EventLog myLog = new EventLog();
                myLog.Log = "Application";
                lvEventos.Items.Clear();
                foreach (EventLogEntry entry in myLog.Entries)
                {
                    if (entry.Source.StartsWith("TicopayUniversalService ") == true)
                    {
                        if (entry.TimeWritten >= DateTime.Now.AddDays(-1))
                        {
                            ListViewItem nuevoItem = new ListViewItem(entry.Message);
                            nuevoItem.SubItems.Add(entry.EntryType.ToString());
                            nuevoItem.SubItems.Add(entry.TimeGenerated.ToShortTimeString());
                            lvEventos.Items.Add(nuevoItem);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Actualiza el componente de listview con los conectores configurados en el servicio.
        /// </summary>
        private void UpdateConfigurations()
        {
            lvConfiguraciones.Items.Clear();
            try
            {
                _contexto = new UniversalConnectorDB();
                List<Configuracion> JobsConfigurados = _contexto.Configuraciones.ToList();
                if (JobsConfigurados.Count > 0)
                {
                    foreach (Configuracion Job in JobsConfigurados)
                    {                        
                        ListViewItem nuevoItem = new ListViewItem(Job.Id.ToString());
                        nuevoItem.SubItems.Add(Job.SubDominioTicopay);
                        nuevoItem.SubItems.Add(Job.UsuarioTicopay);
                        nuevoItem.SubItems.Add(Job.TipoConector.ToString());
                        lvConfiguraciones.Items.Add(nuevoItem);
                    }
                    _contexto.Dispose();
                }
                else
                {
                    _eventos.Advertencia("Servicio Universal de Conexión", "Obtener Jobs Configurados", "No existen Jobs en la BD");
                }
            }
            catch
            {
                _eventos.Error("Servicio Universal de Conexión", "Obtener Jobs Configurados", "Error al Leer los Job de la BD");
                MessageBox.Show("Imposible obtener la lista de conectores configurados");
            }              
        }

        /// <summary>
        /// Elimina las operaciones en estado de error de un conector.
        /// </summary>
        /// <returns></returns>
        private bool EliminarTrabajosOperacionesError(Guid idConector)
        {
            try
            {
                _contexto = new UniversalConnectorDB();
                Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == idConector).First();
                List<Operacion> operacionesAEliminar = _contexto.Operaciones.Where(o => o.EstadoOperacion == Estado.Error).ToList();
                List<Operacion> operacionesPorEliminar = operacionesAEliminar.Where(o => o.Configuracion == Job).ToList();
                foreach (Operacion op in operacionesPorEliminar)
                {
                    // var delete = _contexto.Operaciones.Single(a => a.IdOperacion == op.IdOperacion);
                    var entidadesSinCambiar = _contexto.ChangeTracker.Entries();
                    var delete = _contexto.Operaciones.Find(op.IdOperacion);                    
                    _contexto.Operaciones.Remove(delete);
                    var entidades = _contexto.ChangeTracker.Entries();
                    int result = _contexto.SaveChanges();
                    var entidadesGuardando = _contexto.ChangeTracker.Entries();
                }                
                _contexto.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                _eventos.Error("Servicio Universal de Conexión", "Eliminar Operaciones Error", "Imposible eliminar las operaciones en error, " + ex.Message);
                return false;
            }
        }

        private void ActualizarEventosBt_Click(object sender, EventArgs e)
        {
            UpdateLog();
        }

        private void EjecucionForzadaBtn_Click(object sender, EventArgs e)
        {
            if (lvConfiguraciones.SelectedItems.Count == 1)
            {
                DialogResult dialogResult = MessageBox.Show("Esta seguro que desea forzar la ejecución de este conector ?", "Ejecutar el conector " +
                    lvConfiguraciones.SelectedItems[0].SubItems[3].Text + " para el Sub Dominio " + lvConfiguraciones.SelectedItems[0].SubItems[1].Text + "?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string idJob = lvConfiguraciones.SelectedItems[0].SubItems[0].Text;
                    try
                    {
                        Guid _idConfiguracion = Guid.Parse(idJob);
                        _contexto = new UniversalConnectorDB();
                        Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == _idConfiguracion).Single();
                        if (Job != null)
                        {
                            if(StarConnectorJob(Job, true))
                            {
                                MessageBox.Show("Iniciando la ejecución del Conector");
                            }
                            else
                            {
                                MessageBox.Show("Imposible ejecutar el conector");
                            }
                        }
                        else
                        {
                            _eventos.Advertencia("Servicio Universal de Conexión", "Forzar un Job a Correr fuera de Tiempo", "Configuración de Job no encontrada");
                        }                        
                    }
                    catch (Exception ex)
                    {
                        _eventos.Error("Servicio Universal de Conexión", "Forzar un Job a Correr fuera de Tiempo", "Error desconocido");
                        MessageBox.Show("Imposible ejecutar el conector");
                    }                    
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar el conector ha ejecutar");
            }
        }

        private void EliminarConectorBt_Click(object sender, EventArgs e)
        {
            if (lvConfiguraciones.SelectedItems.Count == 1)
            {
                DialogResult dialogResult = MessageBox.Show("Esta seguro que desea eliminar la configuración de este Conector del Servicio de Sincronización ? , si lo hace no podrá facturar usando ese conector" +
                    lvConfiguraciones.SelectedItems[0].SubItems[3].Text + " " + lvConfiguraciones.SelectedItems[0].SubItems[1].Text, "Eliminar configuración de este conector ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string idJob = lvConfiguraciones.SelectedItems[0].SubItems[0].Text;
                    try
                    {
                        Guid _idConfiguracion = Guid.Parse(idJob);
                        _contexto = new UniversalConnectorDB();
                        Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == _idConfiguracion).First();
                        if (Job != null)
                        {
                            if (_contexto.Operaciones.ToList().Where(o => o.Configuracion.Id == _idConfiguracion 
                            && (o.EstadoOperacion == Estado.Error || o.EstadoOperacion == Estado.NoProcesado) 
                            && o.Registrado < DateTime.Now.AddHours(-DateTime.Now.Hour)).Count() == 0)
                            {
                                _contexto.Configuraciones.Remove(Job);
                                _contexto.SaveChanges();
                                _contexto.Dispose();
                                _eventos.Confirmacion("Servicio Universal de Conexión", "Eliminar la configuración de un Job", "Configuración de Job eliminada");
                                MessageBox.Show("Conector eliminado, Detenga el servicio e inicielo de nuevo para aplicar los cambios o reinicie el equipo");
                                UpdateConfigurations();
                            }
                            else
                            {
                                _eventos.Advertencia("Servicio Universal de Conexión", "Eliminar la configuración de un Job", "El Job contiene operaciones pendientes por corrección");
                                MessageBox.Show("Imposible eliminar el Conector, El Job contiene operaciones pendientes por corrección");
                            }                            
                        }
                        else
                        {
                            _eventos.Advertencia("Servicio Universal de Conexión", "Eliminar la configuración de un Job", "Configuración de Job a eliminar no encontrada");
                        }
                    }
                    catch (Exception ex)
                    {
                        _eventos.Error("Servicio Universal de Conexión", "Eliminar un Job de la Configuración", "Error desconocido");
                        MessageBox.Show("Imposible eliminar el Conector");
                    }                    
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar la configuración del conector ha eliminar");
            }
        }

        private void AgregarConectorBt_Click(object sender, EventArgs e)
        {
            switch ((Conector)TiposDeConectorCb.SelectedItem)
            {
                case Conector.Contapyme:
                    ContaPyme form = new ContaPyme();
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (form._configurado)
                        {
                            UpdateConfigurations();
                            MessageBox.Show("Conector configurado");
                        }
                        UpdateLog();
                    }
                    break;
                case Conector.QuickbooksEnterprise:
                    QuickbooksEnterpriseDesktop formQB = new QuickbooksEnterpriseDesktop();
                    var resultQB = formQB.ShowDialog();
                    if (resultQB == DialogResult.OK)
                    {
                        if (formQB._configurado)
                        {
                            UpdateConfigurations();
                            MessageBox.Show("Conector configurado");
                        }
                        UpdateLog();
                    }
                    break;
            }
        }

        private void GenerarReportesButton_Click(object sender, EventArgs e)
        {
            StarReportJob(true);
            MessageBox.Show("Generando Reportes en C:\\Reportes Conector Ticopay\\ ");
        }

        private void EliminarErroresButton_Click(object sender, EventArgs e)
        {
            if (lvConfiguraciones.SelectedItems.Count == 1)
            {
                DialogResult dialogResult = MessageBox.Show("Esta seguro que desea eliminar las operaciones en estado de Error de este Conector del Servicio de Sincronización ? , si lo hace las mismas seran eliminadas del sincronizador" +
                    lvConfiguraciones.SelectedItems[0].SubItems[3].Text + " " + lvConfiguraciones.SelectedItems[0].SubItems[1].Text, "Eliminar operaciones en error de este conector ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string idJob = lvConfiguraciones.SelectedItems[0].SubItems[0].Text;
                    try
                    {
                        _contexto = new UniversalConnectorDB();
                        Guid _idConfiguracion = Guid.Parse(idJob);
                        Configuracion Job = _contexto.Configuraciones.ToList().Where(c => c.Id == _idConfiguracion).First();
                        if (Job != null)
                        {
                            _contexto.Dispose();
                            if (EliminarTrabajosOperacionesError(_idConfiguracion))
                            {
                                _eventos.Confirmacion("Servicio Universal de Conexión", "Eliminar operaciones de error de un Job", "Operaciones en estado de error eliminadas");
                                MessageBox.Show("Operaciones en estado de error eliminadas");
                            }
                            else
                            {
                                MessageBox.Show("Imposible eliminar las operaciones en estado de Error");
                            }
                        }
                        else
                        {
                            _eventos.Advertencia("Servicio Universal de Conexión", "Eliminar operaciones de error de un Job", "Conector no encontrado");
                        }
                    }
                    catch (Exception ex)
                    {
                        _eventos.Error("Servicio Universal de Conexión", "Eliminar operaciones de error", "Error desconocido");
                        MessageBox.Show("Imposible eliminar operaciones del conector");
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar el conector al que se le eliminaran las operaciones de error");
            }
        }
    }
}
