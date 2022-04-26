using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicopayUniversalConnectorService.ConexionTicopay;
using TicopayUniversalConnectorService.Entities;

namespace TicopayUniversalConsole.Forms_de_Conectores
{
    public partial class ContaPyme : Form
    {
        public bool _configurado = false;

        public ContaPyme()
        {
            InitializeComponent();
            TipoInstalacionContaPymeCmb.Items.Clear();
            TipoInstalacionContaPymeCmb.DataSource = Enum.GetValues(typeof(TipoInstalacion));
            DireccionBdTxb.Text = null;
        }

        private void BuscarBDBtn_Click(object sender, EventArgs e)
        {
            if (ArchivoBdContaPyme.ShowDialog() == DialogResult.OK)
            {
                DireccionBdTxb.Text = ArchivoBdContaPyme.FileName;
            }
        }

        private string ArmarCadenaConexion()
        {
            if ((UsuarioBdContaPymeTxb.Text != null && ClaveBdContaPymeTxb.Text != null && DireccionBdTxb.Text != null && IpServidorContaPymeTxb.Text != null && IdEmpresaContaPymeTxb.Text != null) &&
                (UsuarioBdContaPymeTxb.Text.Length > 0 && ClaveBdContaPymeTxb.Text.Length > 0 && DireccionBdTxb.Text.Length > 0 && IpServidorContaPymeTxb.Text.Length > 0 && IdEmpresaContaPymeTxb.Text.Length > 0))
            {
                string cadena = null;
                string TipoServidor = null;
                switch ((TipoInstalacion)TipoInstalacionContaPymeCmb.SelectedItem)
                {
                    case TipoInstalacion.MultiUsuario:
                        TipoServidor = "0";
                        break;
                    case TipoInstalacion.MonoUsuario:
                        TipoServidor = "1";
                        break;
                }
                cadena = "User=" + UsuarioBdContaPymeTxb.Text.Trim() + ";" + "Password=" + ClaveBdContaPymeTxb.Text.Trim() + ";" + "Database=" + DireccionBdTxb.Text.Trim() + ";" + "DataSource=" + IpServidorContaPymeTxb.Text.Trim() + ";" +
                        "Port=3050;" + "Dialect=3;" + "Charset=NONE;" + "Role=;" + "Connection lifetime=15;" + "Pooling=true; MinPoolSize = 0; MaxPoolSize = 50; " +
                        "Packet Size=8192;" + "ServerType=" + TipoServidor;
                return cadena;
            }
            else
            {
                MessageBox.Show("Verifique los datos de conexión de ContaPyme, todos los campos son obligatorios");
                return null;
            }            
        }

        #region Conexion Ticopay

        private void ProbarTicopayBtn_Click(object sender, EventArgs e)
        {
            ProbarTicopay();            
        }

        private bool ProbarTicopay(bool sinConfirmacion = false)
        {
            if (VerificarPermisoConector())
            {
                if (VerificarCredenciales() == null)
                {
                    MessageBox.Show("Verifique sus credenciales de conexión");
                    return false;
                }
                else
                {
                    if(!sinConfirmacion)
                    {
                        MessageBox.Show("Éxito, credenciales verificadas");
                    }                    
                    return true;
                }
            }
            else
            {
                MessageBox.Show("Su Sub Dominio de Ticopay no tiene permiso para usar este conector");
                return false;
            }
        }

        private bool VerificarPermisoConector()
        {
            Ticopay _conexionTicopay = new Ticopay();
            return _conexionTicopay.VerificarPermisoConector(SubDominioTxb.Text.Trim(), Conector.Contapyme.ToString());
        }

        private string VerificarCredenciales()
        {
            Ticopay _conexionTicopay = new Ticopay();
            return _conexionTicopay.AutentificarUsuario(SubDominioTxb.Text.Trim(),UsuarioTbx.Text.Trim(),ClaveTxb.Text.Trim());
        }

        #endregion

        private void AgregarBtn_Click(object sender, EventArgs e)
        {
            if(ProbarTicopay(true) == true && ProbarCadenaConexion(true) == true)
            {
                try
                {
                    // Inicialización de Variables para el Socket
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ipAddress = ipHostInfo.AddressList[0];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                    // Crear el socket.  
                    Socket ServiceSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    byte[] bytes = new byte[1024];
                    
                    string data = "idPeticion=00"+ Convert.ToChar(30) + SubDominioTxb.Text.Trim() + Convert.ToChar(30) + UsuarioTbx.Text.Trim() + Convert.ToChar(30) + 
                        ClaveTxb.Text.Trim() + Convert.ToChar(30) + IdEmpresaContaPymeTxb.Text.Trim() + Convert.ToChar(30) + ArmarCadenaConexion() + Convert.ToChar(30) + Conector.Contapyme.ToString() + Convert.ToChar(30);
                    string respuesta = "";
                    ServiceSocket.Connect(remoteEP);

                    //Envió petición
                    byte[] msg = Encoding.ASCII.GetBytes(data);
                    ServiceSocket.Send(msg);

                    //Recibo respuesta
                    int bytesRec = ServiceSocket.Receive(bytes);
                    respuesta = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    ServiceSocket.Shutdown(SocketShutdown.Both);
                    ServiceSocket.Close();
                    if(respuesta == "1")
                    {                        
                        _configurado = true;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Imposible configurar el conector");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error al intentar contactar al Servicio Universal de Conexión, Verifique que este en estatus Iniciado");
                }               
            }
        }

        private void CancelarBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ContaPyme_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private bool ProbarCadenaConexion(bool sinConfirmar = false)
        {
            string cadena = ArmarCadenaConexion();
            if (cadena != null)
            {
                FbConnection myConnection = new FbConnection(cadena);

                try
                {
                    myConnection.Open();
                    myConnection.Close();
                    if (!sinConfirmar)
                    {
                        MessageBox.Show("Éxito, Conexión con ContaPyme verificada");
                    }                    
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Verifique los datos de conexión de ContaPyme, Datos incorrectos");
                    return false;                    
                }
            }
            return false;
        }

        private void ProbarConexionContaPymeBtn_Click(object sender, EventArgs e)
        {
            ProbarCadenaConexion();
        }
    }

    public enum TipoInstalacion
    {
        MultiUsuario,
        MonoUsuario,
    }
}
