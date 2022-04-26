using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
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
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Security;
using TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Responses;
using TicopayUniversalConnectorService.ConexionTicopay;
using TicopayUniversalConnectorService.Entities;

namespace TicopayUniversalConsole.Forms_de_Conectores
{
    public partial class TicopayContaPyme : Form
    {
        public bool _configurado = false;

        public TicopayContaPyme()
        {
            InitializeComponent();
        }

        private string ArmarCadenaConexion()
        {
            if ((UsuarioBdContaPymeTxb.Text != null && ClaveBdContaPymeTxb.Text != null && TBIdApp.Text != null && IpServidorContaPymeTxb.Text != null && TBIdMaquina.Text != null
                && TBIdEmpresa.Text != null && TBCodigoCentroCostos.Text != null && TBCodigoCuentaBanco.Text != null && TBCodigoCuentaCaja.Text != null && TBCodigoCuentasXCobrar.Text != null
                && TBCodigoLiquidacionIva.Text != null) &&
                (UsuarioBdContaPymeTxb.Text.Length > 0 && ClaveBdContaPymeTxb.Text.Length > 0 && TBIdMaquina.Text.Length > 0 && IpServidorContaPymeTxb.Text.Length > 0 && TBIdApp.Text.Length > 0
                && TBIdEmpresa.Text.Length > 0 && TBCodigoCentroCostos.Text.Length > 0 && TBCodigoCuentaBanco.Text.Length > 0 && TBCodigoCuentaCaja.Text.Length > 0
                && TBCodigoCuentasXCobrar.Text.Length > 0 && TBCodigoLiquidacionIva.Text.Length > 0))
            {
                string cadena = null;
                cadena = IpServidorContaPymeTxb.Text.Trim() + ";" + UsuarioBdContaPymeTxb.Text.Trim() + ";" + ClaveBdContaPymeTxb.Text.Trim() + ";" + TBIdMaquina.Text.Trim() + ";" + TBIdApp.Text.Trim() + ";"
                    + TBIdEmpresa.Text.Trim() + ";" + TBCodigoCentroCostos.Text.Trim() + ";" + TBCodigoLiquidacionIva.Text.Trim() + ";" + TBCodigoCuentaCaja.Text.Trim() + ";" + TBCodigoCuentaBanco.Text.Trim() + ";"
                    + TBCodigoCuentasXCobrar.Text.Trim() + ";";
                return cadena;
            }
            else
            {
                MessageBox.Show("Verifique los datos de conexión del Api de ContaPyme y Códigos de Cuentas, todos los campos son obligatorios");
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
                        ClaveTxb.Text.Trim() + Convert.ToChar(30) + TBIdEmpresa.Text.Trim() + Convert.ToChar(30) + ArmarCadenaConexion() + Convert.ToChar(30) + Conector.TicopayContapyme.ToString() + Convert.ToChar(30);
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
                try
                {
                    MethodResponse respuestaServicio;
                    LoginJson credenciales = new LoginJson();
                    credenciales.dataJSON = new LoginInformation(UsuarioBdContaPymeTxb.Text, ClaveBdContaPymeTxb.Text, TBIdMaquina.Text);
                    credenciales.controlkey = "";
                    credenciales.iapp = TBIdApp.Text;
                    Random rnd = new Random();
                    credenciales.random = rnd.Next(300, 300000).ToString();
                    respuestaServicio = ContaPymeApi.Authenticate(IpServidorContaPymeTxb.Text, credenciales).GetAwaiter().GetResult();
                    GetAuthResult responseArray = JsonConvert.DeserializeObject<GetAuthResult>(respuestaServicio.result);
                    credenciales.controlkey = responseArray.result[0].respuesta.datos.keyagente;
                    respuestaServicio = ContaPymeApi.Logout(IpServidorContaPymeTxb.Text, credenciales).GetAwaiter().GetResult();
                    LogOutResponse responseClose = JsonConvert.DeserializeObject<LogOutResponse>(respuestaServicio.result);
                    if(responseClose.result[0].respuesta.datos.cerro == "true")
                    {
                        if (!sinConfirmar)
                        {
                            MessageBox.Show("Éxito, Conexión con Api de ContaPyme verificada");
                        }
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Verifique los datos de conexión para el Api de ContaPyme, Datos incorrectos");
                        return false;
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Verifique los datos de conexión para el Api de ContaPyme, Datos incorrectos");
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
}
