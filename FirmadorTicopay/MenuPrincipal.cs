using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature.Parameters;
using FirmaXadesNet.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicoPayDll.Authentication;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Response;
using TicoPayDll.Vouchers;

namespace FirmadorTicopay
{
    public partial class MenuPrincipal : Form
    {
        string _token;
        Invoice[] _facturas;
        Note[] _notas;
        Voucher[] _comprobantes;

        public MenuPrincipal(string token)
        {            
            InitializeComponent();
            lTotalFacturas.Text = "";
            _token = token;
            MenuBarra.ThemeColor = RibbonTheme.Green;
            pFacturasEmitidas.Visible = false;
            pNotasEmitidas.Visible = false;
            pVouchers.Visible = false;
        }

        #region Panel Facturas

        private void facturasEmitidasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _token = this.RefreshToken(_token);
            pFacturasEmitidas.Visible = true;
            pNotasEmitidas.Visible = false;
            pVouchers.Visible = false;
        }

        private void CrearGrid(Invoice[] resultadoBusqueda)
        {
            // Inicializar el Grid
            dgFacturas.Columns.Clear();
            dgFacturas.DataSource = null;
            dgFacturas.AutoGenerateColumns = false;

            if (resultadoBusqueda != null)
            {
                lTotalFacturas.Text = resultadoBusqueda.Count().ToString() + " Facturas encontradas";

                //Agregar las columnas

                DataGridViewTextBoxColumn idFactura = new DataGridViewTextBoxColumn();
                idFactura.Name = "Id";
                idFactura.HeaderText = "Id";
                idFactura.DataPropertyName = "Id";
                idFactura.ValueType = typeof(string);
                idFactura.Visible = false;
                idFactura.ReadOnly = true;
                dgFacturas.Columns.Add(idFactura);

                DataGridViewTextBoxColumn nFactura = new DataGridViewTextBoxColumn();
                nFactura.Name = "NumeroFactura";
                nFactura.HeaderText = "Nº de Factura";
                nFactura.DataPropertyName = "NumeroFactura";
                nFactura.ValueType = typeof(string);
                nFactura.Width = 200;
                nFactura.ReadOnly = true;
                dgFacturas.Columns.Add(nFactura);

                DataGridViewTextBoxColumn fechaFactura = new DataGridViewTextBoxColumn();
                fechaFactura.Name = "Fecha";
                fechaFactura.HeaderText = "Fecha";
                fechaFactura.DataPropertyName = "Fecha";
                fechaFactura.ValueType = typeof(string);
                fechaFactura.ReadOnly = true;
                dgFacturas.Columns.Add(fechaFactura);

                DataGridViewTextBoxColumn clientName = new DataGridViewTextBoxColumn();
                clientName.Name = "Nombre";
                clientName.HeaderText = "Nombre del Cliente";
                clientName.DataPropertyName = "Nombre";
                clientName.ValueType = typeof(string);
                clientName.Width = 220;
                clientName.ReadOnly = true;
                dgFacturas.Columns.Add(clientName);

                DataGridViewTextBoxColumn invoiceTotal = new DataGridViewTextBoxColumn();
                invoiceTotal.Name = "Total";
                invoiceTotal.HeaderText = "Total de la Factura";
                invoiceTotal.DataPropertyName = "Total";
                invoiceTotal.ValueType = typeof(string);
                invoiceTotal.ReadOnly = true;
                dgFacturas.Columns.Add(invoiceTotal);

                DataGridViewTextBoxColumn estadoFirma = new DataGridViewTextBoxColumn();
                estadoFirma.Name = "Estatus";
                estadoFirma.HeaderText = "Estatus Firma";
                estadoFirma.DataPropertyName = "Estatus";
                estadoFirma.ValueType = typeof(string);
                estadoFirma.ReadOnly = true;
                estadoFirma.Visible = false;
                dgFacturas.Columns.Add(estadoFirma);

                DataGridViewTextBoxColumn estadoFirmaActualizado = new DataGridViewTextBoxColumn();
                estadoFirmaActualizado.Name = "EstatusUpdate";
                estadoFirmaActualizado.HeaderText = "Estatus Firma Digital";
                estadoFirmaActualizado.DataPropertyName = "EstatusUpdate";
                estadoFirmaActualizado.ValueType = typeof(string);
                estadoFirmaActualizado.ReadOnly = true;
                dgFacturas.Columns.Add(estadoFirmaActualizado);

                DataGridViewCheckBoxColumn columnaFirma = new DataGridViewCheckBoxColumn();
                columnaFirma.Name = "Firma";
                columnaFirma.HeaderText = "Firmar ?";
                columnaFirma.Width = 80;
                columnaFirma.ValueType = typeof(bool);
                dgFacturas.Columns.Add(columnaFirma);

                CultureInfo formato = CultureInfo.CreateSpecificCulture("en-US");

                var show = from busqueda in resultadoBusqueda.OrderBy(i => i.ConsecutiveNumber)
                           select new
                           {
                               Id = busqueda.Id,
                               NumeroFactura = busqueda.ConsecutiveNumber,
                               Fecha = busqueda.DueDate.ToString("yyyy-MM-dd"),
                               Nombre = busqueda.Client.Name + " " + busqueda.Client.LastName,
                               Total = busqueda.Total.ToString("###,###,###.00", formato),
                               Estatus = busqueda.StatusFirmaDigital
                           };

                // Asignar el datasource al gridview                
                dgFacturas.DataSource = show.ToList();

                foreach (DataGridViewRow row in dgFacturas.Rows)
                {
                    row.Cells["EstatusUpdate"].Value = row.Cells["Estatus"].Value.ToString();

                    if (row.Cells["EstatusUpdate"].Value.ToString().Contains("Firmada"))
                    {
                        row.Cells["Firma"].ReadOnly = true;
                    }
                }
            }

        }

        private void BuscarButton_Click(object sender, EventArgs e)
        {
            _token = this.RefreshToken(_token);
            BuscarButton.Enabled = false;
            FirmarFacturasButton.Enabled = false;
            _facturas = BuscarFacturas(_token,true);
            this.CrearGrid(_facturas);
            BuscarButton.Enabled = true;
            FirmarFacturasButton.Enabled = true;
        }

        private void FirmarFacturasButton_Click(object sender, EventArgs e)
        {
            SignaturePolicyInfo signaturePolicyInfo = new SignaturePolicyInfo()
            {
                PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4/Resolucion%20Comprobantes%20Electronicos%20%20DGT-R-48-2016.pdf",
                PolicyHash = "V8lVVNGDCPen6VELRD1Ja8HARFk=",
                PolicyUri = ""
            };
            SignatureParameters parameters = new SignatureParameters()
            {
                SignatureMethod = SignatureMethod.RSAwithSHA256,
                SigningDate = new DateTime?(DateTime.Now),
                SignaturePolicyInfo = signaturePolicyInfo,
                SignaturePackaging = SignaturePackaging.ENVELOPED
            };
            try
            {   
                // Para Debug sin Certificado comentar la siguiente linea
                parameters.Signer = new Signer(CertUtil.SelectCertificate((string)null, (string)null));
            }
            catch
            {
                MessageBox.Show("Error con el Hardware o al cargar los certificados, por favor verifique");
                return;
            }
            
            BuscarButton.Enabled = false;
            FirmarFacturasButton.Enabled = false;
            foreach (DataGridViewRow row in dgFacturas.Rows)
            {
                if (row.Cells["Firma"].Value != null)
                {
                    if (row.Index > 0)
                    {
                        int indice = row.Index - 1;
                        if ((dgFacturas.Rows[indice].Cells["Firma"].Value == null) || ( (bool) dgFacturas.Rows[indice].Cells["Firma"].Value == false))
                        {
                            MessageBox.Show("Debe firmar las facturas en orden consecutivo");
                            return;
                        }
                    }
                    if (row.Cells["Firma"].Value.Equals(true) && (!row.Cells["EstatusUpdate"].Value.ToString().Contains("Firmada")))
                    {
                        try
                        {
                            string xmlAFirmar = null;
                            string xmlFirmado = null;
                            string invoiceId = null;
                            invoiceId = row.Cells["Id"].Value.ToString();
                            xmlAFirmar = BuscarXml(invoiceId, _token);
                            if (xmlAFirmar != null)
                            {
                                // Para Debug sin Certificado comentar la siguiente linea y Descomentar la segunda
                                xmlFirmado = new XadesService().Sign((Stream)new MemoryStream(Encoding.UTF8.GetBytes(xmlAFirmar)), parameters).Document.InnerXml;
                                // xmlFirmado = xmlAFirmar;
                            }
                            if (xmlFirmado != null)
                            {                                
                                bool enviado = false;
                                // Para Debug sin Certificado comentar la siguiente linea y Descomentar la segunda
                                enviado = ActualizarXmlFirmado(_token, invoiceId, xmlFirmado);
                                // enviado = true;
                                if (enviado)
                                {
                                    row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Firmada.ToString();
                                    row.Cells["Firma"].ReadOnly = true;
                                }
                                else
                                {
                                    row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Error.ToString();
                                    MessageBox.Show("Error al firmar Factura " + row.Cells["NumeroFactura"].Value.ToString());
                                    return;
                                }
                            }
                            else
                            {
                                row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Error.ToString();
                                MessageBox.Show("Error al firmar Factura " + row.Cells["NumeroFactura"].Value.ToString() + ", problemas de comunicación con el hardware");
                                return;
                            }
                        }
                        catch
                        {
                            row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Error.ToString();
                            MessageBox.Show("Error con el Hardware o al cargar los certificados, por favor verifique");
                            return;
                        }
                    }
                }
                dgFacturas.Update();
                
            }
            MessageBox.Show("Proceso de firma de Facturas terminado");
            _token = this.RefreshToken(_token);
            _facturas = BuscarFacturas(_token, true);
            this.CrearGrid(_facturas);
            dgFacturas.Update();
            BuscarButton.Enabled = true;
            FirmarFacturasButton.Enabled = true;
        }

        private void dgFacturas_Paint(object sender, PaintEventArgs e)
        {
            DataGridView sndr = (DataGridView)sender;

            if (sndr.Rows.Count == 0) // <-- if there are no rows in the DataGridView when it paints, then it will create your message
            {
                using (Graphics grfx = e.Graphics)
                {
                    grfx.FillRectangle(Brushes.White, new Rectangle(new Point(), new Size(sndr.Width, 25)));
                    grfx.DrawString("No existen registros para mostrar", new Font("Arial", 12), Brushes.Black, new PointF(3, 3));
                }
            }
        }

        #endregion

        #region Llamadas al Servicio

        public string RefreshToken(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            RefreshCredentials credenciales = new RefreshCredentials();
            credenciales.Token = _token;
            credenciales.AdditionalTimeType = TimeLapsType.Minutes;
            credenciales.AdditionalTimeAmount = 5;
            respuestaServicio = TicoPayDll.Authentication.Authentication.RefreshTokenAuthenticate(credenciales).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                JsonRefreshToken freshCredentials = JsonConvert.DeserializeObject<JsonRefreshToken>(respuestaServicio.result);
                return freshCredentials.objectResponse.tokenAuthenticate;
            }
            else
            {
                return null;
            }
        }

        public Client[] BuscarClientes(string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Clients.ClientController.GetClients(token,false).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                ClientController.JsonClients clientes = JsonConvert.DeserializeObject<ClientController.JsonClients>(respuestaServicio.result);
                return clientes.listObjectResponse;
            }
            else
            {
                return null;
            }
        }

        private Invoice[] BuscarFacturas(string token, bool pendientes)
        {
            TicoPayDll.Response.Response respuestaServicio;
            InvoiceSearchConfiguration parametrosBusqueda = new InvoiceSearchConfiguration();
            
            parametrosBusqueda.ClientId = null;
            parametrosBusqueda.StartDueDate = null;
            parametrosBusqueda.EndDueDate = null;
            parametrosBusqueda.InvoiceId = null;
            parametrosBusqueda.Status = null;
            parametrosBusqueda.TipoFirma = FirmType.Firma;
            if (pendientes)
            {
                parametrosBusqueda.EstatusFirma = StatusFirmaDigital.Pendiente;
            }
            else
            {
                parametrosBusqueda.EstatusFirma = null;
            }
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.GetInvoices(parametrosBusqueda, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                var settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Include;
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                settings.Formatting = Formatting.Indented;
                settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                settings.FloatParseHandling = FloatParseHandling.Decimal;
                JsonInvoices facturas = JsonConvert.DeserializeObject<JsonInvoices>(respuestaServicio.result, settings);
                return facturas.listObjectResponse;
            }
            else
            {
                return null;
            }
        }

        private Note[] BuscarNotas(string token, bool pendientes)
        {
            TicoPayDll.Response.Response respuestaServicio;
            NotesSearchConfiguration parametrosBusqueda = new NotesSearchConfiguration();
            parametrosBusqueda.NoteId = null;
            parametrosBusqueda.StartDueDate = null;
            parametrosBusqueda.EndDueDate = null;
            parametrosBusqueda.InvoiceId = null;
            parametrosBusqueda.Status = null;
            parametrosBusqueda.TipoFirma = FirmType.Firma;
            if (pendientes)
            {
                parametrosBusqueda.EstatusFirma = StatusFirmaDigital.Pendiente;
            }
            else
            {
                parametrosBusqueda.EstatusFirma = null;
            }           
            respuestaServicio = TicoPayDll.Notes.NoteController.GetNotes(parametrosBusqueda, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                var settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Include;
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                settings.Formatting = Formatting.Indented;
                settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                settings.FloatParseHandling = FloatParseHandling.Decimal;
                JsonNotes notas = JsonConvert.DeserializeObject<JsonNotes>(respuestaServicio.result, settings);
                return notas.listObjectResponse;
            }
            else
            {
                return null;
            }
        }

        public bool ActualizarXmlFirmado(string token, string invoiceId, string xmlFirmado)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.UpdateInvoiceXml(invoiceId, xmlFirmado, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {                
                return true;
            }
            else
            {
                return false;
            }
        }

        public string BuscarXml(string invoiceId, string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Invoices.InvoiceController.GetXml(invoiceId, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                string xml = respuestaServicio.result;
                return xml;
            }
            else
            {
                return null;
            }
        }

        public string BuscarNoteXml(string noteId, string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Notes.NoteController.GetXml(noteId, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                string xml = respuestaServicio.result;
                return xml;
            }
            else
            {
                return null;
            }
        }

        public bool ActualizarNoteXmlFirmado(string token, string noteId, string xmlFirmado)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Notes.NoteController.UpdateInvoiceXml(noteId, xmlFirmado, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Voucher[] BuscarComprobantes(string token, bool pendientes)
        {
            TicoPayDll.Response.Response respuestaServicio;
            SearchVoucher parametrosBusqueda = new SearchVoucher();
            // parametrosBusqueda.ConsecutiveNumber = null;
            // parametrosBusqueda.ConsecutiveNumberInvoice = null;
            // parametrosBusqueda.Identification = null;
            // parametrosBusqueda.Name = null;
            // parametrosBusqueda.StartDueDate = null;
            // parametrosBusqueda.EndDueDate = null;
            // parametrosBusqueda.StatusTribunet = null;
            parametrosBusqueda.TipoFirma = FirmType.Firma;
            if (pendientes)
            {
                parametrosBusqueda.StatusFirmaDigital = StatusFirmaDigital.Pendiente;
            }
            else
            {
                parametrosBusqueda.StatusFirmaDigital = null;
            }
            respuestaServicio = TicoPayDll.Vouchers.VoucherController.GetVouchers(parametrosBusqueda, _token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {                
                JsonVouchers comprobantes = JsonConvert.DeserializeObject<JsonVouchers>(respuestaServicio.result);
                return comprobantes.listObjectResponse;
            }
            else
            {
                return null;
            }
        }

        private string BuscarXmlComprobante(string voucherId,string token)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Vouchers.VoucherController.GetXml(voucherId, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                string xml = respuestaServicio.result;
                return xml;
            }
            else
            {
                return null;
            }
        }

        private bool ActualizarXmlFirmadoComprobante(string token, string voucherId, string xmlFirmado)
        {
            TicoPayDll.Response.Response respuestaServicio;
            respuestaServicio = TicoPayDll.Vouchers.VoucherController.UpdateVoucherXml(voucherId, xmlFirmado, token).GetAwaiter().GetResult();
            if (respuestaServicio.status == ResponseType.Ok)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Panel Notas

        private void notasEmitidasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _token = this.RefreshToken(_token);
            cbNotasType.DataSource = Enum.GetValues(typeof(NoteType));
            pFacturasEmitidas.Visible = false;
            pNotasEmitidas.Visible = true;
            pVouchers.Visible = false;
        }

        private void BuscarNotasButton_Click(object sender, EventArgs e)
        {
            _token = this.RefreshToken(_token);
            BuscarNotasButton.Enabled = false;
            FirmarNotasButton.Enabled = false;
            _notas = BuscarNotas(_token, true);
            this.CrearGridNotas(_notas);
            BuscarNotasButton.Enabled = true;
            FirmarNotasButton.Enabled = true;
        }

        private void CrearGridNotas(Note[] resultadoBusqueda)
        {
            // Inicializar el Grid
            dgwNotas.Columns.Clear();
            dgwNotas.DataSource = null;
            dgwNotas.AutoGenerateColumns = false;

            if (resultadoBusqueda != null)
            {                

                //Agregar las columnas

                DataGridViewTextBoxColumn idNota = new DataGridViewTextBoxColumn();
                idNota.Name = "Id";
                idNota.HeaderText = "Id";
                idNota.DataPropertyName = "Id";
                idNota.ValueType = typeof(string);
                idNota.Visible = false;
                idNota.ReadOnly = true;
                dgwNotas.Columns.Add(idNota);

                DataGridViewTextBoxColumn idFactura = new DataGridViewTextBoxColumn();
                idFactura.Name = "IdFactura";
                idFactura.HeaderText = "IdFactura";
                idFactura.DataPropertyName = "IdFactura";
                idFactura.ValueType = typeof(string);
                idFactura.Visible = false;
                idFactura.ReadOnly = true;
                dgwNotas.Columns.Add(idFactura);

                DataGridViewTextBoxColumn nNota = new DataGridViewTextBoxColumn();
                nNota.Name = "NumeroNota";
                nNota.HeaderText = "Nº de Nota";
                nNota.DataPropertyName = "NumeroNota";
                nNota.ValueType = typeof(string);
                nNota.Width = 190;
                nNota.ReadOnly = true;
                dgwNotas.Columns.Add(nNota);

                DataGridViewTextBoxColumn clientName = new DataGridViewTextBoxColumn();
                clientName.Name = "Nombre";
                clientName.HeaderText = "Nombre del Cliente";
                clientName.DataPropertyName = "Nombre";
                clientName.ValueType = typeof(string);
                clientName.Width = 155;
                clientName.ReadOnly = true;
                dgwNotas.Columns.Add(clientName);

                DataGridViewTextBoxColumn nFacturaAfectada = new DataGridViewTextBoxColumn();
                nFacturaAfectada.Name = "NumeroFacturaAfectada";
                nFacturaAfectada.HeaderText = "Nº de Factura Afectada";
                nFacturaAfectada.DataPropertyName = "NumeroFacturaAfectada";
                nFacturaAfectada.ValueType = typeof(string);
                nFacturaAfectada.Width = 190;
                nFacturaAfectada.ReadOnly = true;
                dgwNotas.Columns.Add(nFacturaAfectada);                

                DataGridViewTextBoxColumn invoiceTotal = new DataGridViewTextBoxColumn();
                invoiceTotal.Name = "Total";
                invoiceTotal.HeaderText = "Total de la Nota";
                invoiceTotal.DataPropertyName = "Total";
                invoiceTotal.ValueType = typeof(string);
                invoiceTotal.ReadOnly = true;
                dgwNotas.Columns.Add(invoiceTotal);

                DataGridViewTextBoxColumn estadoFirma = new DataGridViewTextBoxColumn();
                estadoFirma.Name = "Estatus";
                estadoFirma.HeaderText = "Estatus Firma";
                estadoFirma.DataPropertyName = "Estatus";
                estadoFirma.ValueType = typeof(string);
                estadoFirma.ReadOnly = true;
                estadoFirma.Visible = false;
                dgwNotas.Columns.Add(estadoFirma);

                DataGridViewTextBoxColumn estadoFirmaActualizado = new DataGridViewTextBoxColumn();
                estadoFirmaActualizado.Name = "EstatusUpdate";
                estadoFirmaActualizado.HeaderText = "Estatus Firma Digital";
                estadoFirmaActualizado.DataPropertyName = "EstatusUpdate";
                estadoFirmaActualizado.ValueType = typeof(string);
                estadoFirmaActualizado.ReadOnly = true;
                dgwNotas.Columns.Add(estadoFirmaActualizado);

                DataGridViewCheckBoxColumn columnaFirma = new DataGridViewCheckBoxColumn();
                columnaFirma.Name = "Firma";
                columnaFirma.HeaderText = "Firmar ?";
                columnaFirma.Width = 80;
                columnaFirma.ValueType = typeof(bool);
                dgwNotas.Columns.Add(columnaFirma);

                CultureInfo formato = CultureInfo.CreateSpecificCulture("en-US");
                NoteType valor = NoteType.Débito;
                if(cbNotasType.SelectedValue.ToString() == NoteType.Crédito.ToString())
                {
                    valor = NoteType.Crédito;
                }
                var show = from busqueda in resultadoBusqueda.Where(n => n.NoteType == valor && n.Invoice.SendInvoice == true).OrderBy(n => n.ConsecutiveNumber)
                           select new
                           {
                               Id = busqueda.Id,
                               IdFactura = busqueda.InvoiceId,
                               NumeroNota = busqueda.ConsecutiveNumber,
                               NumeroFacturaAfectada = busqueda.Invoice.ConsecutiveNumber,
                               Nombre = busqueda.Invoice.Client.Name + " " + busqueda.Invoice.Client.LastName,
                               Total = busqueda.Total.ToString("###,###,###.00", formato),
                               Estatus = busqueda.StatusFirmaDigital
                           };

                lCantidadNotas.Text = show.Count().ToString() + " Notas encontradas";

                // Asignar el datasource al gridview
                dgwNotas.DataSource = show.ToList();

                foreach (DataGridViewRow row in dgwNotas.Rows)
                {
                    row.Cells["EstatusUpdate"].Value = row.Cells["Estatus"].Value.ToString();

                    if (row.Cells["EstatusUpdate"].Value.ToString().Contains("Firmada"))
                    {
                        row.Cells["Firma"].ReadOnly = true;
                    }
                }
            }

        }

        private void FirmarNotasButton_Click(object sender, EventArgs e)
        {
            SignaturePolicyInfo signaturePolicyInfo = new SignaturePolicyInfo()
            {
                PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4/Resolucion%20Comprobantes%20Electronicos%20%20DGT-R-48-2016.pdf",
                PolicyHash = "V8lVVNGDCPen6VELRD1Ja8HARFk=",
                PolicyUri = ""
            };
            SignatureParameters parameters = new SignatureParameters()
            {
                SignatureMethod = SignatureMethod.RSAwithSHA256,
                SigningDate = new DateTime?(DateTime.Now),
                SignaturePolicyInfo = signaturePolicyInfo,
                SignaturePackaging = SignaturePackaging.ENVELOPED
            };
            try
            {
                // Para Debug sin Certificado comentar la siguiente linea
                parameters.Signer = new Signer(CertUtil.SelectCertificate((string)null, (string)null));
            }
            catch
            {
                MessageBox.Show("Error con el Hardware o al cargar los certificados, por favor verifique");
                return;
            }            
            BuscarNotasButton.Enabled = false;
            FirmarNotasButton.Enabled = false;
            foreach (DataGridViewRow row in dgwNotas.Rows)
            {
                if (row.Cells["Firma"].Value != null)
                {
                    if (row.Index > 0)
                    {
                        int indice = row.Index - 1;
                        if ((dgwNotas.Rows[indice].Cells["Firma"].Value == null) || ((bool)dgwNotas.Rows[indice].Cells["Firma"].Value == false))
                        {
                            MessageBox.Show("Debe firmar las Notas en orden consecutivo");
                            return;
                        }
                    }
                    if (row.Cells["Firma"].Value.Equals(true) && (!row.Cells["EstatusUpdate"].Value.ToString().Contains("Firmada")))
                    {
                        try
                        {
                            string xmlAFirmar = null;
                            string xmlFirmado = null;
                            string noteId = null;
                            noteId = row.Cells["Id"].Value.ToString();
                            xmlAFirmar = BuscarNoteXml(noteId, _token);
                            if (xmlAFirmar != null)
                            {
                                // Para Debug sin Certificado comentar la siguiente linea y Descomentar la segunda
                                xmlFirmado = new XadesService().Sign((Stream)new MemoryStream(Encoding.UTF8.GetBytes(xmlAFirmar)), parameters).Document.InnerXml;
                                // xmlFirmado = xmlAFirmar;
                            }
                            if (xmlFirmado != null)
                            {
                                bool enviado = false;
                                // Para Debug sin Certificado comentar la siguiente linea y Descomentar la segunda
                                enviado = ActualizarNoteXmlFirmado(_token, noteId, xmlFirmado);
                                // enviado = true;
                                if (enviado)
                                {
                                    row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Firmada.ToString();
                                    row.Cells["Firma"].ReadOnly = true;
                                }
                                else
                                {
                                    row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Error.ToString();
                                    MessageBox.Show("Error al firmar Nota " + row.Cells["NumeroNota"].Value.ToString());
                                    return;
                                }
                            }
                            else
                            {
                                row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Error.ToString();
                                MessageBox.Show("Error al firmar Nota " + row.Cells["NumeroNota"].Value.ToString() + ", problemas de comunicación con el hardware");
                                return;
                            }
                        }
                        catch
                        {
                            row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Error.ToString();
                            MessageBox.Show("Error con el Hardware o al cargar los certificados, por favor verifique");
                            return;
                        }
                    }
                }

            }
            MessageBox.Show("Proceso de firma de Notas terminado");
            _token = this.RefreshToken(_token);
            _notas = BuscarNotas(_token, true);
            this.CrearGridNotas(_notas);
            BuscarNotasButton.Enabled = true;
            FirmarNotasButton.Enabled = true;
        }

        private void dgwNotas_Paint(object sender, PaintEventArgs e)
        {
            DataGridView sndr = (DataGridView)sender;

            if (sndr.Rows.Count == 0) // <-- if there are no rows in the DataGridView when it paints, then it will create your message
            {
                using (Graphics grfx = e.Graphics)
                {
                    grfx.FillRectangle(Brushes.White, new Rectangle(new Point(), new Size(sndr.Width, 25)));
                    grfx.DrawString("No existen registros para mostrar", new Font("Arial", 12), Brushes.Black, new PointF(3, 3));
                }
            }
        }

        #endregion

        #region Panel Comprobantes

        private void VoucherSingButton_Click(object sender, EventArgs e)
        {
            _token = this.RefreshToken(_token);
            pFacturasEmitidas.Visible = false;
            pNotasEmitidas.Visible = false;
            pVouchers.Visible = true;
        }

        private void dgvComprobantes_Paint(object sender, PaintEventArgs e)
        {
            DataGridView sndr = (DataGridView)sender;

            if (sndr.Rows.Count == 0) // <-- if there are no rows in the DataGridView when it paints, then it will create your message
            {
                using (Graphics grfx = e.Graphics)
                {
                    grfx.FillRectangle(Brushes.White, new Rectangle(new Point(), new Size(sndr.Width, 25)));
                    grfx.DrawString("No existen registros para mostrar", new Font("Arial", 12), Brushes.Black, new PointF(3, 3));
                }
            }
        }

        private void CrearGridComprobantes(Voucher[] resultadoBusqueda)
        {
            // Inicializar el Grid
            dgvComprobantes.Columns.Clear();
            dgvComprobantes.DataSource = null;
            dgvComprobantes.AutoGenerateColumns = false;

            if (resultadoBusqueda != null)
            {
                lComprobantesPendientes.Text = resultadoBusqueda.Count().ToString() + " Comprobantes encontrados";
                lComprobantesPendientes.Visible = true;

                //Agregar las columnas

                DataGridViewTextBoxColumn idComprobante = new DataGridViewTextBoxColumn();
                idComprobante.Name = "Id";
                idComprobante.HeaderText = "Id";
                idComprobante.DataPropertyName = "Id";
                idComprobante.ValueType = typeof(string);
                idComprobante.Visible = false;
                idComprobante.ReadOnly = true;
                dgvComprobantes.Columns.Add(idComprobante);

                DataGridViewTextBoxColumn nComprobante = new DataGridViewTextBoxColumn();
                nComprobante.Name = "NumeroComprobante";
                nComprobante.HeaderText = "Nº de Comprobante Electrónico";
                nComprobante.DataPropertyName = "NumeroComprobante";
                nComprobante.ValueType = typeof(string);
                nComprobante.Width = 200;
                nComprobante.ReadOnly = true;
                dgvComprobantes.Columns.Add(nComprobante);

                DataGridViewTextBoxColumn fechaComprobante = new DataGridViewTextBoxColumn();
                fechaComprobante.Name = "Fecha";
                fechaComprobante.HeaderText = "Fecha";
                fechaComprobante.DataPropertyName = "Fecha";
                fechaComprobante.ValueType = typeof(string);
                fechaComprobante.ReadOnly = true;
                dgvComprobantes.Columns.Add(fechaComprobante);

                DataGridViewTextBoxColumn emisorName = new DataGridViewTextBoxColumn();
                emisorName.Name = "Nombre";
                emisorName.HeaderText = "Nombre del Emisor";
                emisorName.DataPropertyName = "Nombre";
                emisorName.ValueType = typeof(string);
                emisorName.Width = 220;
                emisorName.ReadOnly = true;
                dgvComprobantes.Columns.Add(emisorName);

                DataGridViewTextBoxColumn voucherTotal = new DataGridViewTextBoxColumn();
                voucherTotal.Name = "Total";
                voucherTotal.HeaderText = "Total del Comprobante";
                voucherTotal.DataPropertyName = "Total";
                voucherTotal.ValueType = typeof(string);
                voucherTotal.ReadOnly = true;
                dgvComprobantes.Columns.Add(voucherTotal);

                DataGridViewTextBoxColumn estadoFirma = new DataGridViewTextBoxColumn();
                estadoFirma.Name = "Estatus";
                estadoFirma.HeaderText = "Estatus Firma";
                estadoFirma.DataPropertyName = "Estatus";
                estadoFirma.ValueType = typeof(string);
                estadoFirma.ReadOnly = true;
                estadoFirma.Visible = false;
                dgvComprobantes.Columns.Add(estadoFirma);

                DataGridViewTextBoxColumn estadoFirmaActualizado = new DataGridViewTextBoxColumn();
                estadoFirmaActualizado.Name = "EstatusUpdate";
                estadoFirmaActualizado.HeaderText = "Estatus Firma Digital";
                estadoFirmaActualizado.DataPropertyName = "EstatusUpdate";
                estadoFirmaActualizado.ValueType = typeof(string);
                estadoFirmaActualizado.ReadOnly = true;
                dgvComprobantes.Columns.Add(estadoFirmaActualizado);

                DataGridViewCheckBoxColumn columnaFirma = new DataGridViewCheckBoxColumn();
                columnaFirma.Name = "Firma";
                columnaFirma.HeaderText = "Firmar ?";
                columnaFirma.Width = 80;
                columnaFirma.ValueType = typeof(bool);
                dgvComprobantes.Columns.Add(columnaFirma);

                CultureInfo formato = CultureInfo.CreateSpecificCulture("en-US");

                var show = from busqueda in resultadoBusqueda.OrderBy(i => i.ConsecutiveNumber)
                           select new
                           {
                               Id = busqueda.Id,
                               NumeroComprobante = busqueda.ConsecutiveNumber,
                               Fecha = busqueda.creationTime.ToString("yyyy-MM-dd"),
                               Nombre = busqueda.NameSender,
                               Total = busqueda.Totalinvoice.ToString("###,###,###.00", formato),
                               Estatus = busqueda.StatusFirmaDigital
                           };

                // Asignar el datasource al gridview                
                dgvComprobantes.DataSource = show.ToList();

                foreach (DataGridViewRow row in dgvComprobantes.Rows)
                {
                    row.Cells["EstatusUpdate"].Value = row.Cells["Estatus"].Value.ToString();

                    if (row.Cells["EstatusUpdate"].Value.ToString().Contains("Firmada"))
                    {
                        row.Cells["Firma"].ReadOnly = true;
                    }
                }
            }

        }

        private void BuscarVouchersButton_Click(object sender, EventArgs e)
        {
            _token = this.RefreshToken(_token);
            BuscarVouchersButton.Enabled = false;
            FirmarComprobantesButton.Enabled = false;
            _comprobantes = BuscarComprobantes(_token, true);
            this.CrearGridComprobantes(_comprobantes);
            BuscarVouchersButton.Enabled = true;
            FirmarComprobantesButton.Enabled = true;
        }

        private void FirmarComprobantesButton_Click(object sender, EventArgs e)
        {
            SignaturePolicyInfo signaturePolicyInfo = new SignaturePolicyInfo()
            {
                PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4/Resolucion%20Comprobantes%20Electronicos%20%20DGT-R-48-2016.pdf",
                PolicyHash = "V8lVVNGDCPen6VELRD1Ja8HARFk=",
                PolicyUri = ""
            };
            SignatureParameters parameters = new SignatureParameters()
            {
                SignatureMethod = SignatureMethod.RSAwithSHA256,
                SigningDate = new DateTime?(DateTime.Now),
                SignaturePolicyInfo = signaturePolicyInfo,
                SignaturePackaging = SignaturePackaging.ENVELOPED
            };
            try
            {
                // Para Debug sin Certificado comentar la siguiente linea
                parameters.Signer = new Signer(CertUtil.SelectCertificate((string)null, (string)null));
            }
            catch
            {
                MessageBox.Show("Error con el Hardware o al cargar los certificados, por favor verifique");
                return;
            }

            BuscarVouchersButton.Enabled = false;
            FirmarComprobantesButton.Enabled = false;
            foreach (DataGridViewRow row in dgvComprobantes.Rows)
            {
                if (row.Cells["Firma"].Value != null)
                {
                    if (row.Index > 0)
                    {
                        int indice = row.Index - 1;
                        if ((dgvComprobantes.Rows[indice].Cells["Firma"].Value == null) || ((bool)dgvComprobantes.Rows[indice].Cells["Firma"].Value == false))
                        {
                            MessageBox.Show("Debe firmar los comprobantes en orden consecutivo");
                            return;
                        }
                    }
                    if (row.Cells["Firma"].Value.Equals(true) && (!row.Cells["EstatusUpdate"].Value.ToString().Contains("Firmada")))
                    {
                        try
                        {
                            string xmlAFirmar = null;
                            string xmlFirmado = null;
                            string comprobanteId = null;
                            comprobanteId = row.Cells["Id"].Value.ToString();
                            xmlAFirmar = BuscarXmlComprobante(comprobanteId, _token);
                            if (xmlAFirmar != null)
                            {
                                // Para Debug sin Certificado comentar la siguiente linea y Descomentar la segunda
                                xmlFirmado = new XadesService().Sign((Stream)new MemoryStream(Encoding.UTF8.GetBytes(xmlAFirmar)), parameters).Document.InnerXml;
                                // xmlFirmado = xmlAFirmar;
                            }
                            if (xmlFirmado != null)
                            {
                                bool enviado = false;
                                // Para Debug sin Certificado comentar la siguiente linea y Descomentar la segunda
                                enviado = ActualizarXmlFirmadoComprobante(_token, comprobanteId, xmlFirmado);
                                // enviado = true;
                                if (enviado)
                                {
                                    row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Firmada.ToString();
                                    row.Cells["Firma"].ReadOnly = true;
                                }
                                else
                                {
                                    row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Error.ToString();
                                    MessageBox.Show("Error al firmar el comprobante " + row.Cells["NumeroComprobante"].Value.ToString());
                                    return;
                                }
                            }
                            else
                            {
                                row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Error.ToString();
                                MessageBox.Show("Error al firmar el comprobante " + row.Cells["NumeroComprobante"].Value.ToString() + ", problemas de comunicación con el hardware");
                                return;
                            }
                        }
                        catch
                        {
                            row.Cells["EstatusUpdate"].Value = StatusFirmaDigital.Error.ToString();
                            MessageBox.Show("Error con el Hardware o al cargar los certificados, por favor verifique");
                            return;
                        }
                    }
                }
                dgvComprobantes.Update();

            }
            MessageBox.Show("Proceso de firma de comprobantes terminado");
            _token = this.RefreshToken(_token);
            _comprobantes = BuscarComprobantes(_token, true);
            this.CrearGridComprobantes(_comprobantes);
            dgvComprobantes.Update();
            BuscarVouchersButton.Enabled = true;
            FirmarComprobantesButton.Enabled = true;
        }

        #endregion

        private void MenuPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {   
            this.Dispose();
        }

        
    }
}
