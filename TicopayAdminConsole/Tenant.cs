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

namespace TicopayAdminConsole
{
    public partial class Tenant : Form
    {
        public Tenant()
        {
            InitializeComponent();
        }

        private void ContinuarButton_Click(object sender, EventArgs e)
        {
            if (tbTenant.Text.Length >= 1)
            {
                ConfigurationManager.AppSettings["tenant"] = tbTenant.Text;
                this.Close();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Debe introducir un Sub Dominio valido para continuar");
                return;
            }
        }
    }
}
