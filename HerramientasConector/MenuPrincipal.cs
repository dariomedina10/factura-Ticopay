using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Response;

namespace HerramientasConector
{
    public partial class MenuPrincipal : Form
    {
        string _token;

        public MenuPrincipal(string token)
        {
            InitializeComponent();
            _token = token;
        }

        private void BuscarFacturasButton_Click(object sender, EventArgs e)
        {
            if(tb_NumeroReferencia.Text == null || tb_NumeroReferencia.Text == "")
            {
                MessageBox.Show("Introduzca un numero de Referencia externa de factura o tiquete");
            }
            try
            {
                LV_Facturas.Items.Clear();
                Invoice[] facturas = BuscarFacturas(_token,tb_NumeroReferencia.Text);
                foreach (Invoice factura in facturas)
                {
                    if(factura.Status != Status.Voided && factura.Status != Status.Returned && factura.Balance >= 0 && factura.Notes.Count == 0)
                    {
                        ListViewItem nuevoItem = new ListViewItem(factura.Id);
                        nuevoItem.SubItems.Add(factura.ConsecutiveNumber);
                        nuevoItem.SubItems.Add(factura.ExternalReferenceNumber);
                        nuevoItem.SubItems.Add(factura.DueDate.ToString("dd/MM/yyyy"));
                        LV_Facturas.Items.Add(nuevoItem);
                    }                    
                }
            }
            catch (Exception ex)
            {

            }
        }

        #region Métodos de Ticopay Dll

        private static Invoice[] BuscarFacturas(string token, string externalReferenceNumber)
        {
            TicoPayDll.Response.Response respuestaServicio;
            InvoiceSearchConfiguration parametrosBusqueda = new InvoiceSearchConfiguration();
            parametrosBusqueda.ClientId = null;
            parametrosBusqueda.InvoiceId = null;
            parametrosBusqueda.Status = null;
            parametrosBusqueda.EndDueDate = null;
            parametrosBusqueda.StartDueDate = null;
            parametrosBusqueda.ExternalReferenceNumber = externalReferenceNumber;
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.GetInvoices(parametrosBusqueda, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonInvoices facturas = JsonConvert.DeserializeObject<JsonInvoices>(respuestaServicio.result);
                return facturas.listObjectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        private static CompleteNote ReversarFacturaOTiquete(string token, string idInvoiceOrTicket, string externalReferenceNumber)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Notes.NoteController.ReverseInvoiceOrTicket(idInvoiceOrTicket, token, externalReferenceNumber).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonCreateNote nota = JsonConvert.DeserializeObject<JsonCreateNote>(respuestaServicio.result);
                return nota.objectResponse;
            }
            else
            {
                Console.WriteLine(respuestaServicio.message);
                return null;
            }
        }

        #endregion

        private void ReversarButton_Click(object sender, EventArgs e)
        {
            if (LV_Facturas.SelectedItems.Count == 1)
            {                
                DialogResult dialogResult = MessageBox.Show("Reversar factura o tiquete " + LV_Facturas.SelectedItems[0].SubItems[1].Text + " equivalente al documento " +
                    LV_Facturas.SelectedItems[0].SubItems[2].Text + " de su sistema ?", "Esta seguro que desea reversar la factura o tiquete seleccionado ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string idFacturaOTiquete = LV_Facturas.SelectedItems[0].SubItems[0].Text;
                    try
                    {
                        CompleteNote nota = ReversarFacturaOTiquete(_token, idFacturaOTiquete, "N/A");
                        if (nota != null)
                        {
                            LV_Facturas.Items.Clear();
                            MessageBox.Show("Documento Reversado " + nota.VoucherKey);                            
                        }
                        else
                        {
                            MessageBox.Show("Imposible Reversar la factura o tiquete");
                        }
                    }
                    catch (Exception ex)
                    {                        
                        MessageBox.Show("Imposible ejecutar el conector");
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar la factura o tiquete a reversar");
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (ArchivoTxtOFD.ShowDialog() == DialogResult.OK)
            {
                UbicacionTB.Text = ArchivoTxtOFD.FileName;
            }
        }

        private void ReversoLoteButton_Click(object sender, EventArgs e)
        {
            if(UbicacionTB.Text.Trim().Length > 0)
            {
                string direccionArchivoLectura = UbicacionTB.Text.Trim();
                int counter = 0;
                string line;
                TxtFile resultado = new TxtFile();
                resultado.NuevaLinea("Procesando Archivo " + direccionArchivoLectura);
                if (File.Exists(direccionArchivoLectura))
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(direccionArchivoLectura);
                    while ((line = file.ReadLine()) != null)
                    {
                        if(line.Length > 0)
                        {
                            Invoice[] facturas = BuscarFacturas(_token, line);
                            if (facturas.Length > 0)
                            {
                                foreach (Invoice factura in facturas)
                                {
                                    if (factura.Status != Status.Voided && factura.Status != Status.Returned && factura.Balance >= 0 && factura.Notes.Count == 0)
                                    {
                                        string idFacturaOTiquete = factura.Id;
                                        try
                                        {
                                            CompleteNote nota = ReversarFacturaOTiquete(_token, idFacturaOTiquete, "N/A");
                                            if (nota != null)
                                            {
                                                resultado.NuevaLinea(factura.ExternalReferenceNumber + " Consecutivo " + factura.ConsecutiveNumber + " Reversada - Voucher Reverso " + nota.VoucherKey);
                                            }
                                            else
                                            {
                                                resultado.NuevaLinea(factura.ExternalReferenceNumber + " Consecutivo " + factura.ConsecutiveNumber + " Imposible Reversar");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            resultado.NuevaLinea(factura.ExternalReferenceNumber + " Consecutivo " + factura.ConsecutiveNumber + " Error al contactar al Api");
                                        }
                                    }
                                    else
                                    {
                                        resultado.NuevaLinea(factura.ExternalReferenceNumber + " Consecutivo " + factura.ConsecutiveNumber + " La factura esta reversada o tiene notas aplicadas");
                                    }
                                    counter++;
                                }
                            }
                            else
                            {
                                resultado.NuevaLinea(line + " No se puede encontrar este numero de referencia en Ticopay");
                            }
                        }                           
                    }
                    file.Close();
                    resultado.NuevaLinea("Procesadas " + counter.ToString() + " Operaciones del archivo de entrada");
                    MessageBox.Show("Procesadas " + counter.ToString() + " Operaciones del archivo de entrada, Puede verificar el archivo TransaccionesAReversar.txt para chequear los resultados");
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un archivo de Texto que contenga por linea un numero de referencia de factura");
            }
        }
    }
}
