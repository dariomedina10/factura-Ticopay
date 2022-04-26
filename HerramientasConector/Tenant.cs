using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicoPayDll.Response;

namespace HerramientasConector
{
    public partial class Tenant : Form
    {
        // Login _ventanaPrincipal;

        public Tenant()
        {
            InitializeComponent();
        }

        private void ContinuarButton_Click(object sender, EventArgs e)
        {
            if(tbTenant.Text.Length >= 1 && VerificarTenant(tbTenant.Text))
            {                
                ConfigurationManager.AppSettings["tenant"] = tbTenant.Text;
                this.Close();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Debe introducir un Sub Dominio válido para continuar");
                return;
            }
        }

        public bool VerificarTenant(string tenantName)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Authentication.Authentication.VerifyDomain(tenantName).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                TicoPayDll.Authentication.JsonDomain tenant = JsonConvert.DeserializeObject<TicoPayDll.Authentication.JsonDomain>(respuestaServicio.result);
                return tenant.objectResponse;
            }
            else
            {
                return false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
