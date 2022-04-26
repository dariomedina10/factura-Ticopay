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
    public partial class QuickbooksEnterpriseDesktop : Form
    {
        public bool _configurado = false;

        public QuickbooksEnterpriseDesktop()
        {
            InitializeComponent();
        }

        #region Configuración Ticopay

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
                    if (!sinConfirmacion)
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
            return _conexionTicopay.VerificarPermisoConector(SubDominioTxb.Text.Trim(), Conector.QuickbooksEnterprise.ToString());
        }

        private string VerificarCredenciales()
        {
            Ticopay _conexionTicopay = new Ticopay();
            return _conexionTicopay.AutentificarUsuario(SubDominioTxb.Text.Trim(), UsuarioTbx.Text.Trim(), ClaveTxb.Text.Trim());
        }

        #endregion

        private void BuscarBDBtn_Click(object sender, EventArgs e)
        {
            ArchivoCompaniaOFD.ValidateNames = false;
            if (ArchivoCompaniaOFD.ShowDialog() == DialogResult.OK)
            {
                DireccionArchivoCompaniaTxb.Text = ArchivoCompaniaOFD.FileName;
            }
        }

        private void ProbarConexionContaPymeBtn_Click(object sender, EventArgs e)
        {
            ProbarConexion();
        }

        private bool ProbarConexion(bool sinConfirmar = false)
        {
            try
            {
                TicopayUniversalConnectorService.ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop prueba = 
                    new TicopayUniversalConnectorService.ConexionesExternas.QuickBooks.QuickbooksEnterpriseDesktop("TicopayConnector", DireccionArchivoCompaniaTxb.Text, "TicopayConnector");
                if (prueba.ProbarConexion())
                {
                    if (!sinConfirmar)
                    {
                        MessageBox.Show("Éxito, Conexión con QuickBooks verificada");
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("Verifique la dirección del archivo de compañía, y asegurese de que QuickBooks este configurado para Multi Usuario");
                    return false;
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Verifique la dirección del archivo de compañía, Datos incorrectos");
                return false;
            }
        }

        private void CancelarBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AgregarBtn_Click(object sender, EventArgs e)
        {
            if (ProbarTicopay(true) == true && ProbarConexion(true) == true)
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

                    string data = "idPeticion=00" + Convert.ToChar(30) + SubDominioTxb.Text.Trim() + Convert.ToChar(30) + UsuarioTbx.Text.Trim() + Convert.ToChar(30) +
                        ClaveTxb.Text.Trim() + Convert.ToChar(30) + IdEmpresaQuickbooksTxb.Text.Trim() + Convert.ToChar(30) + DireccionArchivoCompaniaTxb.Text + Convert.ToChar(30) + Conector.QuickbooksEnterprise.ToString() + Convert.ToChar(30);
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
                    if (respuesta == "1")
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
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar contactar al Servicio Universal de Conexión, Verifique que este en estatus Iniciado");
                }
            }
        }
    }
}
