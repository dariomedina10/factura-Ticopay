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

namespace TicopayAdminConsole
{
    public partial class AddTenants : Form
    {
        public bool _cambios = false;
        public List<TenantsConfigurados> _compañias;

        public AddTenants(string tenant,string user,string password,bool configurado)
        {
            InitializeComponent();
            if(configurado == false)
            {
                tbTenant.Text = tenant;
                tbUser.Text = user;
                tbPassword.Text = password;
            }
            else
            {
                CargarListaTenants();
            }
        }

        #region Métodos de comunicación con el Servicio

        public bool ConfigurarServicio(string tenant, string user, string password, string company)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=01;" + tenant + ";" + user + ";" + password + ";" + company + ";";
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

        public string CantidadTenantConfigurados()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=10;";
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

        public bool EliminarTenant(string tenantId)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                // Crear el socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                byte[] bytes = new byte[1024];
                string data = "idPeticion=11;" + tenantId + ";";
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

        #endregion

        private void backButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AddTenants_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        public void ActualizarTenants()
        {
            _compañias = new List<TenantsConfigurados>();
            _compañias.Clear();
            string tenants = CantidadTenantConfigurados();
            if(tenants.Length > 2)
            {
                string[] Lineas = new string[] { };
               Lineas = tenants.Split(';');
                foreach (string item in Lineas)
                {
                    if(item.Length > 0)
                    {
                        string[] argumentos = new string[] { };
                        argumentos = item.Split('/');
                        TenantsConfigurados compania = new TenantsConfigurados(argumentos[0], argumentos[1], argumentos[2], argumentos[3]);
                        _compañias.Add(compania);
                    }                    
                }
            }
        }

        public void CargarListaTenants()
        {
            lvTenants.Items.Clear();
            ActualizarTenants();
            foreach (TenantsConfigurados item in _compañias)
            {
                ListViewItem nuevoItem = new ListViewItem(item.Id);
                nuevoItem.SubItems.Add(item.Name);
                nuevoItem.SubItems.Add(item.User);
                nuevoItem.SubItems.Add(item.Company);
                lvTenants.Items.Add(nuevoItem);
            }
        }
                
        private void addButton_Click(object sender, EventArgs e)
        {
            if(tbTenant.Text != null && tbTenant.Text.Length > 1 && tbUser.Text != null && tbUser.Text.Length > 1 && tbPassword.Text != null && tbPassword.Text.Length > 1 
                && tbCompany.Text != null && tbCompany.Text.Length > 0)
            {
                if (ConfigurarServicio(tbTenant.Text,tbUser.Text,tbPassword.Text,tbCompany.Text))
                {
                    _cambios = true;
                    CargarListaTenants();
                    MessageBox.Show("Tenant agregado a la configuración");
                }
                else
                {
                    MessageBox.Show("Error al configurar el servicio");
                }
            }
            else
            {
                MessageBox.Show("Ninguno de los campos de configuración puede estar vacío");
            }
            
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if(lvTenants.SelectedItems.Count == 1)
            {
                DialogResult dialogResult = MessageBox.Show("Esta seguro que desea eliminar este Tenant de Ticopay ? , si lo hace no podrá facturar usando el Tenant " +
                    lvTenants.SelectedItems[0].SubItems[1].Text , "Eliminar configuración de Tenant de Ticopay", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (EliminarTenant(lvTenants.SelectedItems[0].Text))
                    {
                        _cambios = true;
                        MessageBox.Show("El Tenant " + lvTenants.SelectedItems[0].SubItems[1].Text + " ha sido eliminado");
                        CargarListaTenants();                        
                    }
                    else
                    {
                        MessageBox.Show("Imposible eliminar el Tenant " + lvTenants.SelectedItems[0].SubItems[1].Text);
                    }
                }                    
            }
            else
            {
                MessageBox.Show("Debe seleccionar el Tenant ha eliminar");
            }
        }
    }

    public class TenantsConfigurados
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Company { get; set; }

        public TenantsConfigurados(string id,string name,string user,string company)
        {
            Id = id;
            Name = name;
            User = user;
            Company = company;
        }
    }
}
