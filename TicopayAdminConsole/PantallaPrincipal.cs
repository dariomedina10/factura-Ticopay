using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicoPayDll.Authentication;
using TicoPayDll.Response;

namespace TicopayAdminConsole
{
    public partial class PantallaPrincipal : Form
    {

        #region Inicializacion y configuración de la Interfaz

        private string _token = "";
        private string _tenant = "";
        private string _user = "";
        private string _password = "";
        private int _documentosPendientes = 0;
        private ServiceController _servicio = null;

        public PantallaPrincipal()
        {
            InitializeComponent();
            _tenant = ConfigurationManager.AppSettings["tenant"];
            if ((_tenant == null) || (_tenant.Length == 0))
            {
                MessageBox.Show("El Sub Dominio no puede estar vació, Contacte a soporte para solucionarlo");
            }
            else
            {
                this.Text = "Consola de Administración de Ticopay-ContaPyme Service " + _tenant;
            }
            pLogin.Visible = true;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if ((tbUsuario.Text.Length == 0) || (tbClave.Text.Length == 0))
            {
                MessageBox.Show("Debe introducir el nombre de Usuario y la Clave");
                return;
            }
            this._token = AutentificarUsuario(_tenant, tbUsuario.Text, tbClave.Text);
            if ((this._token != "") && (this._token != null))
            {
                this._user = tbUsuario.Text;
                this._password = tbClave.Text;
                this.pLogin.Visible = false;
                this.pMonitor.Visible = true;
                cbConfigurado.Checked = ServicioTicopayContaPymeConfigurado();
                _servicio = EstatusServicio();
                if (_servicio != null)
                {
                    if (_servicio.Status == ServiceControllerStatus.Running)
                    {
                        cbServicioActivo.Checked = true;
                        DetenerServicioButton.Enabled = true;
                        IniciarButton.Enabled = false;
                    }
                    else
                    {
                        cbServicioActivo.Checked = false;
                        DetenerServicioButton.Enabled = false;
                        IniciarButton.Enabled = true;
                    }                    
                }
                else
                {
                    cbServicioActivo.Checked = false;
                    DetenerServicioButton.Enabled = false;
                    IniciarButton.Enabled = false;
                    MessageBox.Show("El servicio de TicopayMiddleService no se encuentra instalado o esta desactivado");
                }
                cbBdContaPyme.Checked = BdContaPymeConfigurada();
                if (cbBdContaPyme.Checked)
                {
                    OpenBdLocationButton.Enabled = false;
                    tbUsuarioBD.ReadOnly = true;
                    tbPasswordBd.ReadOnly = true;
                    tbIp.ReadOnly = true;
                    cmbServerType.Enabled = false;
                    ConnectContaPymeButton.Enabled = false;
                    DisconnectBdButton.Enabled = true;
                    ObtenerConfiguracionContaPyme();
                    cbContaPymeConectado.Checked = ProbarConexionContaPyme();                    
                }
                else
                {
                    OpenBdLocationButton.Enabled = true;
                    tbUsuarioBD.ReadOnly = false;
                    tbPasswordBd.ReadOnly = false;
                    tbIp.ReadOnly = false;
                    cmbServerType.Enabled = true;
                    cmbServerType.SelectedItem = "Mono Usuario";
                    ConnectContaPymeButton.Enabled = true;
                    DisconnectBdButton.Enabled = false;
                }                
                UpdateLog();
            }
            else
            {
                MessageBox.Show("Usuario o Clave incorrectos");
                return;
            }
        }

        #endregion

        #region Metodos de comunicacion con el Servicio
        public bool BdContaPymeConfigurada()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=02;";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envio peticion
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                if (respuesta == "1")
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
                return false;
            }
        }

        public bool ObtenerConfiguracionContaPyme()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=05;";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envio peticion
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                string[] argumentos = new string[] { };
                argumentos = respuesta.Split(';');
                tbUbicacionBd.Text = argumentos[0];
                tbUsuarioBD.Text = argumentos[1];
                tbPasswordBd.Text = argumentos[2];
                if(argumentos[3] == "true")
                {
                    cmbServerType.SelectedItem = "Multi Usuario";
                }
                else
                {
                    cmbServerType.SelectedItem = "Mono Usuario";
                }
                tbIp.Text = argumentos[4];
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ServicioTicopayContaPymeConfigurado()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=00;";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envio peticion
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                if(respuesta == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }catch
            {
                return false;
            }
        }

        public bool EliminarCredencialesContaPyme()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=04;";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envio peticion
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                if (respuesta == "1")
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
                return false;
            }
        }

        public bool ProbarConexionContaPyme()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=06;";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envio peticion
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                if (respuesta == "1")
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
                return false;
            }
        }

        public bool ConfigurarServicio()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=01;"+_tenant+";"+_user+";"+_password+";";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envio peticion
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                if (respuesta == "1")
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
                return false;
            }
        }

        public bool EnviarDocumentos()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=08;";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envió petición
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                string[] argumentos = new string[] { };
                if(respuesta.Length > 1)
                {
                    argumentos = respuesta.Split(';');
                    MessageBox.Show(argumentos[0]);    
                }
                else
                {
                    MessageBox.Show("Proceso de envió de documentos terminado culminado sin errores");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string CantidadDocumentosPendientes()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=09;";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envio peticion
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return respuesta;
            }
            catch
            {
                return "0";
            }
        }

        public bool ConfigurarBD()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                // Preparar Petición
                string serverType = "false";
                if(cmbServerType.SelectedItem.ToString() == "Multi Usuario")
                {
                    serverType = "true";
                }
                string data = "idPeticion=03;" + tbUbicacionBd.Text + ";" + tbUsuarioBD.Text + ";" + tbPasswordBd.Text + ";" + serverType + ";" + tbIp.Text + ";";
                string respuesta = "";
                sender.Connect(remoteEP);

                //Envió petición
                byte[] msg = Encoding.ASCII.GetBytes(data);
                sender.Send(msg);

                //Recibo respuesta
                int bytesRec = sender.Receive(bytes);
                respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                if (respuesta == "1")
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
                return false;
            }
        }

        #endregion

        #region Llamadas al Web Api
        public string AutentificarUsuario(string tenancy, string user, string password)
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
                return null;
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

        #region Métodos para controlar el Servicio

        public ServiceController EstatusServicio()
        {
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();

            foreach (ServiceController scTemp in scServices)
            {
                if (scTemp.ServiceName == "TicopayMiddleService")
                {
                    return scTemp;
                }
            }
            return null;
        }

        private void ConfigurarServicioButton_Click(object sender, EventArgs e)
        {
            AddTenants form = new AddTenants(_tenant, _user, _password, cbConfigurado.Checked);
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                if(form._cambios)
                {
                    cbConfigurado.Checked = ServicioTicopayContaPymeConfigurado();
                    UpdateLog();
                }
            }
        }

        private void IniciarButton_Click(object sender, EventArgs e)
        {
            if(_servicio.Status != ServiceControllerStatus.Running)
            {
                _servicio.Start();
            }
            UpdateLog();
        }

        private void DetenerServicioButton_Click(object sender, EventArgs e)
        {
            if (_servicio.Status != ServiceControllerStatus.Stopped)
            {
                DialogResult dialogResult = MessageBox.Show("Esta seguro que desea detener el servicio de TicopayMiddleService ? , si lo hace cesará toda comunicación entre ContaPyme y Ticopay",
                    "Detener Servicio TicopayMiddleService", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    _servicio.Start();
                }               
            }
            UpdateLog();
        }

        #endregion

        private void cbServicioActivo_CheckedChanged(object sender, EventArgs e)
        {
            if(cbServicioActivo.Checked == true)
            {
                DetenerServicioButton.Enabled = true;
                IniciarButton.Enabled = false;
            }
            else
            {
                DetenerServicioButton.Enabled = false;
                IniciarButton.Enabled = true;
            }
        }

        private void OpenBdLocationButton_Click(object sender, EventArgs e)
        {
            if (OpenFileDialogBd.ShowDialog() == DialogResult.OK)
            {
                tbUbicacionBd.Text = OpenFileDialogBd.FileName;
            }
        }

        private void ConnectContaPymeButton_Click(object sender, EventArgs e)
        {
            if(tbUbicacionBd.Text != null && tbUbicacionBd.Text.Length > 3 && tbUsuarioBD.Text != null && tbUsuarioBD.Text.Length > 0 && tbPasswordBd.Text != null)
            {
                if (ConfigurarBD())
                {
                    cbBdContaPyme.Checked = true;
                    cbContaPymeConectado.Checked = true;
                    OpenBdLocationButton.Enabled = false;
                    tbUsuarioBD.ReadOnly = true;
                    tbPasswordBd.ReadOnly = true;
                    ConnectContaPymeButton.Enabled = false;
                    DisconnectBdButton.Enabled = true;
                    cbContaPymeConectado.Checked = true;                    
                }
                else
                {
                    MessageBox.Show("Los datos proporcionados para la conexión con la BD de ContaPyme son incorrectos");
                }
            }
            else
            {
                MessageBox.Show("Debe indicar la ubicación del archivo de Base de datos de ContaPyme , el usuario y la clave, en caso de no tener clave deje el campo vació");
            }
            UpdateLog();
        }

        private void UpdateLogButton_Click(object sender, EventArgs e)
        {
            UpdateLog();
        }

        public void UpdateLog()
        {
            try
            {
                EventLog myLog = new EventLog();
                myLog.Log = "Application";
                lvEventos.Items.Clear();
                foreach (EventLogEntry entry in myLog.Entries)
                {
                    if(entry.Source == "TicopayMiddleService")
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
                _documentosPendientes = int.Parse(CantidadDocumentosPendientes());
                if (_documentosPendientes > 0)
                {
                    SendDocumentsButton.Enabled = true;
                }
                else
                {
                    SendDocumentsButton.Enabled = false;
                }
                lDocumentosPendientes.Text = _documentosPendientes.ToString() + " Documentos pendientes por enviar";
            }
            catch
            {

            }            
        }

        private void DisconnectBdButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Esta seguro que desea eliminar las Credenciales de Conexión a la BD de ContaPyme ? , si lo hace cesará toda comunicación entre ContaPyme y Ticopay",
                    "Eliminar Credenciales ContaPyme", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (EliminarCredencialesContaPyme())
                {
                    cbBdContaPyme.Checked = false;
                    cbContaPymeConectado.Checked = false;
                    OpenBdLocationButton.Enabled = true;
                    tbUsuarioBD.ReadOnly = false;
                    tbPasswordBd.ReadOnly = false;
                    ConnectContaPymeButton.Enabled = true;
                    DisconnectBdButton.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Error al Eliminar las Credenciales de acceso a la BD de ContaPyme");
                }
                UpdateLog();
            }            
        }

        private void cmbServerType_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbServerType.SelectedItem.ToString() == "Mono Usuario")
            {
                tbIp.Text = "";
                tbIp.ReadOnly = true;
            }
            else
            {
                tbIp.ReadOnly = false;
            }
        }

        private void SendDocumentsButton_Click(object sender, EventArgs e)
        {
            EnviarDocumentos();
            UpdateLog();
        }

        private void Test_Click(object sender, EventArgs e)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
            // Crear el socket.  
            Socket test = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            byte[] bytes = new byte[1024];
            // string data = "idPeticion=07;Doc;cli;emp;";
            string data = "idPeticion=07;5;17884166;1;";
            string respuesta = "";
            test.Connect(remoteEP);

            //Envió petición
            byte[] msg = Encoding.ASCII.GetBytes(data);
            test.Send(msg);

            //Recibo respuesta
            int bytesRec = test.Receive(bytes);
            respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

            test.Shutdown(SocketShutdown.Both);
            test.Close();

            MessageBox.Show(respuesta);
        }
    }
}
