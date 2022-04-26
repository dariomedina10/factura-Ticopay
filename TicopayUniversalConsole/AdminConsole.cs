using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicopayUniversalConnectorService.Entities;
using TicopayUniversalConsole.Forms_de_Conectores;

namespace TicopayUniversalConsole
{
    public partial class AdminConsole : Form
    {
        #region Variables y métodos para el control del formulario y controles
        private ServiceController _servicio = null;

        public AdminConsole()
        {
            InitializeComponent();
            string version = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = this.Text + " " + version;
            _servicio = EstatusServicio();
            if (_servicio != null)
            {
                if (_servicio.Status == ServiceControllerStatus.Running)
                {
                    ServicioActivo.Checked = true;
                }
                else
                {
                    ServicioActivo.Checked = false;
                    DetenerServicioBt.Enabled = false;
                    IniciarServicioBt.Enabled = true;
                }
            }
            UpdateLog();
            UpdateConfigurations();
            TiposDeConectorCb.Items.Clear();
            TiposDeConectorCb.DataSource = Enum.GetValues(typeof(Conector));
            #if (DEBUG)
            // Activar solo para Debug
            ServicioActivo.Checked = true;
            UpdateConfigurations();
            #else

            #endif
            ActivacionDesactivacionControles();            
        }

        /// <summary>
        /// Si el Servicio esta activo habilita los botones para su control, sino los deshabilita.
        /// </summary>
        private void ActivacionDesactivacionControles()
        {
            if (ServicioActivo.Checked)
            {
                AgregarConectorBt.Enabled = true;
                EliminarConectorBt.Enabled = true;
                EjecucionForzadaBtn.Enabled = true;
            }
            else
            {
                AgregarConectorBt.Enabled = false;
                EliminarConectorBt.Enabled = false;
                EjecucionForzadaBtn.Enabled = false;
            }
        }

        #endregion

        #region Métodos para el Manejo del Servicio

        /// <summary>
        /// Verifica que el servicio de Ticopay este instalado.
        /// </summary>
        /// <returns>Servicio de windows de Ticopay , o null si no esta instalado</returns>
        public ServiceController EstatusServicio()
        {
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();

            foreach (ServiceController scTemp in scServices)
            {
                if (scTemp.ServiceName == "UniversalConnectorService")
                {
                    return scTemp;
                }
            }
            return null;
        }

        /// <summary>
        /// Maneja el evento Click del Botón IniciarServicioBt (Inicia el Servicio de Ticopay Universal Connector).
        /// </summary>
        /// <param name="sender">Fuente del Evento.</param>
        /// <param name="e">The <see cref="EventArgs"/> Instancia que contiene la data del evento.</param>
        private void IniciarServicioBt_Click(object sender, EventArgs e)
        {
            _servicio = EstatusServicio();
            if (_servicio != null && _servicio.Status != ServiceControllerStatus.Running)
            {
                _servicio.Start();
                ServicioActivo.Checked = true;
            }
            UpdateConfigurations();
            UpdateLog();            
            ActivacionDesactivacionControles();
        }

        /// <summary>
        /// Maneja el evento Click del Botón DetenerServicioBt (Detiene el Servicio de Ticopay Universal Connector).
        /// </summary>
        /// <param name="sender">Fuente del Evento.</param>
        /// <param name="e">The <see cref="EventArgs"/> Instancia que contiene la data del evento.</param>
        private void DetenerServicioBt_Click(object sender, EventArgs e)
        {
            _servicio = EstatusServicio();
            if (_servicio != null && _servicio.Status != ServiceControllerStatus.Stopped)
            {
                DialogResult dialogResult = MessageBox.Show("Esta seguro que desea detener el servicio de Ticopay Universal Connector ? , si lo hace cesará toda comunicación entre sus sistemas y Ticopay",
                    "Detener Servicio Ticopay Universal Connector", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    _servicio.Stop();
                    ServicioActivo.Checked = false;
                }
            }
            UpdateLog();
            ActivacionDesactivacionControles();
        }

        /// <summary>
        /// Maneja el evento CheckedChanged del control ServicioActivo (Desactivar o Activar los Botones de Iniciar o Detener el Servicio).
        /// </summary>
        /// <param name="sender">Fuente del Evento.</param>
        /// <param name="e">The <see cref="EventArgs"/> Instancia que contiene la data del evento.</param>
        private void ServicioActivo_CheckedChanged(object sender, EventArgs e)
        {
            if (ServicioActivo.Checked == true)
            {
                DetenerServicioBt.Enabled = true;
                IniciarServicioBt.Enabled = false;
            }
            else
            {
                DetenerServicioBt.Enabled = false;
                IniciarServicioBt.Enabled = true;
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
                //_documentosPendientes = int.Parse(CantidadDocumentosPendientes());
                //if (_documentosPendientes > 0)
                //{
                //    SendDocumentsButton.Enabled = true;
                //}
                //else
                //{
                //    SendDocumentsButton.Enabled = false;
                //}
                //lDocumentosPendientes.Text = _documentosPendientes.ToString() + " Documentos pendientes por enviar";
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Maneja el evento Click del Botón ActualizarEventosBt (Actualiza el Registro de Eventos de la Consola).
        /// </summary>
        /// <param name="sender">Fuente del Evento.</param>
        /// <param name="e">The <see cref="EventArgs"/> Instancia que contiene la data del evento.</param>
        private void ActualizarEventosBt_Click(object sender, EventArgs e)
        {
            UpdateLog();
        }       


        #endregion

        #region Administración de Conectores

        /// <summary>
        /// Actualiza el componente de listview con los conectores configurados en el servicio.
        /// </summary>
        private void UpdateConfigurations()
        {
            if (ServicioActivo.Checked)
            {
                lvConfiguraciones.Items.Clear();
                string respuesta = "";
                try
                {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipAddress = ipHostInfo.AddressList[0];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                    // Crear el socket.  
                    Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    byte[] bytes = new byte[1024];
                    string data = "idPeticion=01" + Convert.ToChar(30);
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
                    MessageBox.Show("Imposible obtener la lista de conectores configurados");
                }
                if (respuesta.Length > 2)
                {
                    string[] Lineas = new string[] { };
                    Lineas = respuesta.Split(Convert.ToChar(30));
                    foreach (string item in Lineas)
                    {
                        if (item.Length > 0)
                        {
                            string[] argumentos = new string[] { };
                            argumentos = item.Split(Convert.ToChar(29));
                            ListViewItem nuevoItem = new ListViewItem(argumentos[0]);
                            nuevoItem.SubItems.Add(argumentos[1]);
                            nuevoItem.SubItems.Add(argumentos[2]);
                            nuevoItem.SubItems.Add(argumentos[3]);
                            lvConfiguraciones.Items.Add(nuevoItem);
                        }
                    }
                }
            }            
        }

        /// <summary>
        /// Maneja el evento Click del Botón AgregarConectorBt (Levanta el Pantalla de Configuración del Conector seleccionado).
        /// </summary>
        /// <param name="sender">Fuente del Evento.</param>
        /// <param name="e">The <see cref="EventArgs"/> Instancia que contiene la data del evento.</param>
        private void AgregarConectorBt_Click(object sender, EventArgs e)
        {
            switch ((Conector) TiposDeConectorCb.SelectedItem)
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
                case Conector.TicopayContapyme:
                    TicopayContaPyme formQB = new TicopayContaPyme();
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

        /// <summary>
        /// Maneja el evento Click del Botón EliminarConectorBt (Elimina la Configuración de Conexión seleccionada).
        /// </summary>
        /// <param name="sender">Fuente del Evento.</param>
        /// <param name="e">The <see cref="EventArgs"/> Instancia que contiene la data del evento.</param>
        private void EliminarConectorBt_Click(object sender, EventArgs e)
        {
            if (lvConfiguraciones.SelectedItems.Count == 1)
            {
                DialogResult dialogResult = MessageBox.Show("Esta seguro que desea eliminar la configuración de este Conector del Servicio de Sincronización ? , si lo hace no podrá facturar usando ese conector" +
                    lvConfiguraciones.SelectedItems[0].SubItems[3].Text + " " + lvConfiguraciones.SelectedItems[0].SubItems[1].Text, "Eliminar configuración de este conector ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string respuesta = "";
                    string idJob = lvConfiguraciones.SelectedItems[0].SubItems[0].Text;
                    try
                    {
                        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                        IPAddress ipAddress = ipHostInfo.AddressList[0];
                        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                        // Crear el socket.  
                        Socket senderDelete = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        byte[] bytes = new byte[1024];
                        string data = "idPeticion=02"+ Convert.ToChar(30) + idJob + Convert.ToChar(30);
                        senderDelete.Connect(remoteEP);

                        //Envió petición
                        byte[] msg = Encoding.ASCII.GetBytes(data);
                        senderDelete.Send(msg);

                        //Recibo respuesta
                        int bytesRec = senderDelete.Receive(bytes);
                        respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        senderDelete.Shutdown(SocketShutdown.Both);
                        senderDelete.Close();
                        if (respuesta == "1")
                        {
                            MessageBox.Show("Conector eliminado, Detenga el servicio e inicielo de nuevo para aplicar los cambios o reinicie el equipo");
                            UpdateConfigurations();
                        }
                        else
                        {
                            MessageBox.Show("Imposible eliminar el Conector");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Imposible obtener la lista de conectores configurados");
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar la configuración del conector ha eliminar");
            }            
        }

        /// <summary>
        /// Maneja el evento Click del Botón EjecucionForzadaBtn (Fuerza la ejecución del Job seleccionado).
        /// </summary>
        /// <param name="sender">Fuente del Evento.</param>
        /// <param name="e">The <see cref="EventArgs"/> Instancia que contiene la data del evento.</param>
        private void EjecucionForzadaBtn_Click(object sender, EventArgs e)
        {
            if (lvConfiguraciones.SelectedItems.Count == 1)
            {
                DialogResult dialogResult = MessageBox.Show("Esta seguro que desea forzar la ejecución de este conector ?", "Ejecutar el conector " +
                    lvConfiguraciones.SelectedItems[0].SubItems[3].Text + " para el Sub Dominio " + lvConfiguraciones.SelectedItems[0].SubItems[1].Text + "?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string respuesta = "";
                    string idJob = lvConfiguraciones.SelectedItems[0].SubItems[0].Text;
                    try
                    {
                        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                        IPAddress ipAddress = ipHostInfo.AddressList[0];
                        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                        // Crear el socket.  
                        Socket senderDelete = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        byte[] bytes = new byte[1024];
                        string data = "idPeticion=03"+ Convert.ToChar(30) + idJob + Convert.ToChar(30);
                        senderDelete.Connect(remoteEP);

                        //Envió petición
                        byte[] msg = Encoding.ASCII.GetBytes(data);
                        senderDelete.Send(msg);

                        //Recibo respuesta
                        int bytesRec = senderDelete.Receive(bytes);
                        respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        senderDelete.Shutdown(SocketShutdown.Both);
                        senderDelete.Close();
                        if (respuesta == "1")
                        {
                            MessageBox.Show("Preparando la ejecución del Conector");
                        }
                        else
                        {
                            MessageBox.Show("Imposible ejecutar el conector");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Imposible ejecutar el conector");
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar el conector ha ejecutar");
            }
        }


        #endregion

        private void GenerarReportesButton_Click(object sender, EventArgs e)
        {
            string respuesta = "";
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket senderDelete = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=04";
                senderDelete.Connect(remoteEP);

                //Envió petición
                byte[] msg = Encoding.ASCII.GetBytes(data);
                senderDelete.Send(msg);

                //Recibo respuesta
                int bytesRec = senderDelete.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                senderDelete.Shutdown(SocketShutdown.Both);
                senderDelete.Close();
                if (respuesta == "1")
                {
                    MessageBox.Show("Generando Reportes en C:\\Reportes Conector Ticopay\\ ");
                }
                else
                {
                    MessageBox.Show("Imposible Generar los reportes");
                }
            }
            catch
            {
                MessageBox.Show("Imposible Generar los reportes");
            }

        }
    }
}
