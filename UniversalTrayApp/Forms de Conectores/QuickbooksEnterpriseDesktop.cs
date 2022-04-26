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
using TicopayUniversalConnectorService.Log;
using UniversalTrayApp.Contexto;

namespace UniversalTrayApp.Forms_de_Conectores
{
    public partial class QuickbooksEnterpriseDesktop : Form
    {
        public bool _configurado = false;
        RegistroDeEventos _eventos;
        UniversalConnectorDB _contexto;

        public QuickbooksEnterpriseDesktop()
        {
            InitializeComponent();
            // Instan ciar del registrador de eventos
            _eventos = new RegistroDeEventos();
            // Instan ciar el Contexto de la BD
            _contexto = new UniversalConnectorDB();
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
                    string Tiquete = "false";
                    if (SalesReceiptCheckBox.Checked)
                    {
                        Tiquete = "true";
                    }
                    Configuracion newJob = new Configuracion(SubDominioTxb.Text.Trim(), UsuarioTbx.Text.Trim(), ClaveTxb.Text.Trim(), IdEmpresaQuickbooksTxb.Text.Trim(), DireccionArchivoCompaniaTxb.Text + Convert.ToChar(30) + Tiquete + Convert.ToChar(30), Conector.QuickbooksEnterprise);
                    _contexto.Configuraciones.Add(newJob);
                    _contexto.SaveChanges();
                    _eventos.Confirmacion("Servicio Universal de Conexión", "Configuración de Job", "Nuevo Job agregado, Tenant " + SubDominioTxb.Text.Trim() + " Conector " + Conector.QuickbooksEnterprise.ToString());
                    _configurado = true;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch(Exception ex)
                {
                    _eventos.Error("Servicio Universal de Conexión", "Configuración de Job", "Job Imposible de agregar, Tenant " + SubDominioTxb.Text.Trim() + " Conector " + Conector.QuickbooksEnterprise.ToString());
                    MessageBox.Show("Imposible configurar el conector");
                    //MessageBox.Show(ex.Message);
                    //if(ex.InnerException != null)
                    //{
                    //    MessageBox.Show(ex.InnerException.Message);
                    //    if(ex.InnerException.InnerException != null)
                    //    {
                    //        MessageBox.Show(ex.InnerException.InnerException.Message);
                    //    }
                    //}
                }                
            }
        }
    }
}
