using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using TicoPay.Services;
using TicoPay.Invoices.NCR;
using ZXing;
using ZXing.Common;
using TicoPay.MultiTenancy;
using TicoPay.Clients;
using System.Xml.Serialization;
using System.IO;
using TicoPay.Invoices.NotaDebito;
using System.Configuration;
using TicoPay.Invoices.XSD;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using TicoPay.Core.Common;
using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature.Parameters;
using TicoPay.Common;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using TicoPay.ReportsSettings;
using TicoPay.Taxes;
using static TicoPay.MultiTenancy.Tenant;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TicoPay.Inventory;
using System.ComponentModel.DataAnnotations.Schema;
using TicoPay.Drawers;
using System.Globalization;
using BCR;

namespace TicoPay.Invoices
{
    public class Note : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited, IComprobanteRecepcion
    {
        public const string subject = "Nueva Nota ";
        public const string emailbody = "<p>Adjunto al correo encontrará su nota electrónica en formato PDF y XML. Para garantizar la seguridad y confidencialidad de sus datos, esta dirección de e-mail será utilizada únicamente para enviar la información solicitada, por lo tanto le agradecemos no responder los correos enviados, ni utilizar esta vía de comunicación para realizar consultas personales referentes a sus facturas o notas de credito o debito.</p>";
        public const string emailfooter = "<p>Para cualquier ayuda contáctenos a, soporte@ticopays.com</p> <p>Gracias</p> <p><b>Equipo TicoPay</b></p>";
        private static Random random = new Random((int)DateTime.Now.Ticks);
        public const int cantidadDecimal = MultiTenancy.Tenant.quantityDecimal;

        public int TenantId { get; set; }
        [Display(Name = "Monto")]
        /// <summary>
        /// Gets or sets the Amount. 
        /// </summary>
        public decimal Amount { get; protected set; }

        /// <summary>
        /// Gets or sets the Discount Percentaje. 
        /// </summary>
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Impuesto")]
        /// <summary>
        /// Porcion de impuesto a devolver o sumar
        /// </summary>
        public decimal TaxAmount { get; protected set; }

        [Display(Name = "Monto Total")]
        /// <summary>
        /// Monto total de la nota
        /// </summary>
        public decimal Total { get; protected set; }

        public decimal TotalServGravados { get; set; }

        public decimal TotalServExento { get; set; }

        public decimal TotalProductExento { get; set; }

        public decimal TotalProductGravado { get; set; }

        public decimal TotalGravado { get; set; }

        public decimal TotalExento { get; set; }

        public decimal SaleTotal { get; set; }

        /// <summary>Gets or sets the InvoiceId. </summary>
        public Guid InvoiceId { get; protected set; }

        /// <summary>Gets or sets the Invoice. </summary>
        public virtual Invoice Invoice { get; protected set; }

        /// <summary>Gets or sets the ExchangeRate Id. </summary>
        public Guid? ExchangeRateId { get; protected set; }

        /// <summary>Gets or sets the ExchangeRate. </summary>
        public virtual ExchangeRate ExchangeRate { get; protected set; }

        /// <summary>Gets or sets the Reference. </summary>
        public string Reference { get; protected set; }

        //public string Description { get; protected set; }

        /// <summary>Gets or sets the CurrencyCode. </summary>
        public NoteCodigoMoneda CodigoMoneda { get; set; }

        //public Guid? ServiceId { get; protected set; }

        ///// <summary>Gets or sets the ExchangeRate. </summary>
        //public virtual Service Services { get; protected set; }
        public NoteReason NoteReasons { get; set; }

        public string NoteReasonsOthers { get; set; }
        /// <summary>
        /// Mensaje de respuesta de la administracion triutaria
        /// </summary>
        public string MessageTaxAdministration { get; set; }
        /// <summary>
        /// Mensaje de respuesta del receptor del comprobante
        /// </summary>
        public string MessageReceiver { get; set; }
        /// <summary>
        /// XML de la Note
        /// </summary>
        public string ElectronicBill { get; set; }
        /// <summary>
        /// Representacion PDF Note
        /// </summary>
        //public byte[] ElectronicBillDocPDF { get; set; }
        public string ElectronicBillPDF { get; set; }
        ///// <summary>
        ///// Codigo QR
        ///// </summary>
        public byte[] QRCode { get; set; }
        /// <summary>
        /// Clave del comprobante
        /// </summary>
        [MaxLength(50)]
        public string VoucherKey { get; set; }
        [Display(Name = "No. Nota")]
        /// <summary>
        /// Numero consecutivo
        /// </summary>
        [MaxLength(20)]
        public string ConsecutiveNumber { get; set; }
        public decimal ChangeType { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        [Display(Name = "Tipo")]
        public NoteType NoteType { get; set; }

        ///// <summary>
        ///// Indica el tipo de documento
        ///// </summary>
        //public TypeDocuments TypeDocuments { get; set; }

        
       [Display(Name = "Estado")]
        /// <summary>
        /// Indica si la factura fue enviada a Hacienda
        /// </summary>
        public bool SendInvoice { get; set; }
        /// <summary>
        /// Estatus de la factura en hacienda
        /// </summary>
        public StatusTaxAdministration StatusTribunet { get; set; }
        /// <summary>
        /// Estatus del comprobante electronico
        /// </summary>
        public VoucherSituation StatusVoucher { get; set; }

        [Display(Name = "Estado Recepción")]
        public bool IsNoteReceptionConfirmed { get; set; }

        public string ConsecutiveNumberReference { get; set; }


        public FirmType? TipoFirma { get; set; }

        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        [Index]
        public Status Status { get; set; }

        [Index]
        [Display(Name = "Fecha vencimiento")]
        public DateTime DueDate { get; set; }

        public int CreditTerm { get; set; }

        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }

        public decimal Balance { get; set; }

        public bool IsContingency { get; set; } = false;

        [MaxLength(50)]
        public string ConsecutiveNumberContingency { get; set; }

        [MaxLength(180)]
        public string ReasonContingency { get; set; }

        public DateTime? DateContingency { get; set; }

        public virtual ICollection<NoteLine> NotesLines { get; protected set; }

        public virtual ICollection<PaymentNote> NotePaymentTypes { get; protected set; }

        /// <summary>Gets or sets the Drawer. </summary>
        public Guid? DrawerId { get; set; }

        /// <summary>Gets or sets the Drawer. </summary>
        public virtual Drawer Drawer { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de Referencia Externa (Usuarios del Api).
        /// </summary>
        /// <value>
        /// Numero de Referencia Externa (Usuarios del Api).
        /// </value>
        public string ExternalReferenceNumber { get; set; }

        /// <summary>
        /// We don't make constructor public
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        /// 
        protected Note()
        {

        }

        public static Note Create(int tenantId, decimal amount, Guid invoiceId, string reference, NoteCodigoMoneda codigoMoneda,
            NoteReason reason, NoteType noteType, decimal montotax, decimal totalNota, DateTime dueDate, Status status, FacturaElectronicaCondicionVenta conditionSaleType, int creditTerm, Invoice invoice
            , string externalReference = "N/A")
        {
            decimal totalAmount = Invoice.GetValor(amount);
            decimal taxAmount = Invoice.GetValor(montotax);
            decimal total = Invoice.GetValor(totalNota);

            var entity = new Note
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Amount = totalAmount,
                InvoiceId = invoiceId,
                Reference = reference,
                CodigoMoneda = codigoMoneda,
                NoteReasons = reason,
                NoteType = noteType,
                ChangeType = 1,
                TaxAmount = taxAmount,
                Total = total,
                Status = status,
                DueDate = dueDate,
                ConditionSaleType = conditionSaleType,
                CreditTerm = creditTerm,
                Invoice = invoice,
                Balance = creditTerm == 0 ? 0 : total,
                ExternalReferenceNumber = externalReference
            };
            entity.NotesLines = new Collection<NoteLine>();
            return entity;
        }

        public void SetInvoiceTotalCalculate(decimal taxamount, decimal discounttotal, decimal totalgravado, decimal totalexento)
        {
            TotalGravado = Invoice.GetValor(totalgravado);
            TotalServGravados = Invoice.GetValor(totalgravado);
            TotalExento = Invoice.GetValor(totalexento);
            TotalServExento = Invoice.GetValor(totalexento);
            DiscountAmount = Invoice.GetValor(discounttotal);
            TaxAmount = Invoice.GetValor(taxamount);
            SaleTotal = Invoice.GetValor(TotalExento + TotalGravado);
           // NetaSale = Decimal.Round(SaleTotal - DiscountAmount, 2);
            Total = Invoice.GetValor((SaleTotal - DiscountAmount) + TaxAmount);
            // Balance = Decimal.Round(Total, 2);            
        }

        public void AssignNoteLine(int tenantId, decimal pricePerUnit, decimal taxAmount, decimal discountpercentage, string descriptionDiscount, string notes, string title, decimal qty, LineType ltype, Service service, Product product, Note note, int numberline, Tax tax, Guid? taxId,
            TicoPay.Invoices.XSD.UnidadMedidaType unitMeasurement, string unitMeasurementOthers)
        {
            decimal precioUnit = Invoice.GetValor(pricePerUnit);
            decimal cantidad = Invoice.GetValor(qty);
            decimal descuento = Invoice.GetValor(discountpercentage);
            decimal subTotal = Invoice.GetValor(precioUnit * cantidad);
            decimal taxImp = tax.Rate / 100;
            var noteLine = NoteLine.Create(tenantId, precioUnit, notes, title, cantidad, ltype, service, product, note, numberline, descuento, descriptionDiscount,TicoPay.Invoices.XSD.CodigoTypeTipo.Codigo_UsoInterno, tax, taxId, unitMeasurement, unitMeasurementOthers);
            decimal amountdisc = CalcularDescuento(subTotal, descuento);
            noteLine.SetLineSubtotal(amountdisc, subTotal);
            var impuesto = Invoice.GetValor(noteLine.SubTotal * taxImp);
            noteLine.SetLineTax(impuesto);
            noteLine.SetLineLinetotal(noteLine.SubTotal, noteLine.TaxAmount);
            NotesLines.Add(noteLine);
        }
        private decimal CalcularDescuento(decimal subTotal, decimal descuento)
        {
            decimal porcentaje = descuento / 100;
            decimal result = Invoice.GetValor(subTotal * porcentaje);
            return result;
        }


        public void SetStatus(Status status)
        {
            Status = status;
        }

        public string GetStringFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        public void SetInvoiceXMLPDF(Uri XML, Uri PDF)
        {
            //ElectronicBill = GetStringFromXMLFile(Path.Combine(WorkPaths.GetXmlPath(), "note_" + voucherkey + ".xml"));
            //ElectronicBillDocPDF = File.ReadAllBytes(Path.Combine(WorkPaths.GetPdfPath(), "note_" + voucherkey + ".pdf"));
            ElectronicBill = XML.ToString();
            ElectronicBillPDF = PDF.ToString();
        }

        public void CreateXMLNote(Invoice invoice, Client client, Tenant _tenant, Note note, Certificate certified)
        {
            XmlSerializer serializer2 = new XmlSerializer(typeof(NotaCreditoElectronica));
            XmlSerializer serializer3 = new XmlSerializer(typeof(NotaDebitoElectronica));
            string path = Path.Combine(WorkPaths.GetXmlPath(), "note_" + note.VoucherKey + ".xml");


            // arma la clase para el XML para nota de debito
            if (note.NoteType == NoteType.Debito)
            {
                NotaDebitoElectronica item = CreateNoteDebitoToSerialize(invoice, client, _tenant, note);

                if (!_tenant.ValidateHacienda)
                {
                    path = Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml");
                }

                TextWriter writer2 = new StreamWriter(path);
                serializer3.Serialize(writer2, item);
                writer2.Close();
            }

            // nota de credito
            if (note.NoteType == NoteType.Credito)
            {
                NotaCreditoElectronica item = CreateNoteCreditoToSerialize(invoice, client, _tenant, note);

                if (!_tenant.ValidateHacienda)
                {
                    path = Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml");
                }

                TextWriter writer = new StreamWriter(path);
                serializer2.Serialize(writer, item);
                writer.Close();
            }

            if (_tenant.ValidateHacienda && certified != null)
            {

                //Guarda el certificado temporalmente

                string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + certified.FileName).ToLower();

                var _certifiedPath = Path.GetFullPath("Common" + "\\" + archivo);

                File.WriteAllBytes(Path.Combine(WorkPaths.GetCertifiedPath(), archivo), certified.CertifiedRoute);

                // Firmar note
                SignedXMLXADES(Path.Combine(WorkPaths.GetXmlPath(), "note_" + note.VoucherKey + ".xml"),
                Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml"),
                Path.Combine(WorkPaths.GetCertifiedPath(), archivo), certified.Password);
            }
        }

        public static string getNoteReasons(NoteReason reason){

            string razonote = string.Empty;
            int codreason = (int)reason;
            switch (codreason)
            {
               case 0:
                    razonote = "Reversa Documento de Referencia";
                    break;
              case 1:
                    razonote = "Corrige monto a Documento de Referencia";
                    break;
                default:
                    razonote = "Otros";
                    break;
            }           
            return razonote;
        }

        public static NotaDebitoElectronica CreateNoteDebito(Client client, Tenant _tenant, Note note, string ConsecutiveNumber, DateTime dateDoc)
        {
            string _codphoneClient = "506", _phoneClient = string.Empty, _codfaxClient = "506", _faxCliente = string.Empty, _codphoneTenant = "506", _phoneTenant = string.Empty, _faxTenant = string.Empty, _codfaxTenant = "506";

            decimal rate = _tenant.IsConvertUSD ? note.ChangeType : 1;

            string razonote = getNoteReasons(note.NoteReasons);                       

            var tipoDoc = (note.ConsecutiveNumberReference == null) ? (NotaDebitoElectronicaInformacionReferenciaTipoDoc)note.Invoice.TypeDocument :
              ((note.NoteType == NoteType.Credito) ? NotaDebitoElectronicaInformacionReferenciaTipoDoc.Item02 : NotaDebitoElectronicaInformacionReferenciaTipoDoc.Item03);


            NotaDebitoElectronica item = new NotaDebitoElectronica
            {
                Clave = note.VoucherKey,
                NumeroConsecutivo = note.ConsecutiveNumber,
                FechaEmision = note.CreationTime,
                CondicionVenta = (NotaDebitoElectronicaCondicionVenta)note.ConditionSaleType,// NotaDebitoElectronicaCondicionVenta.Item01, // se coloca de contado fijo hasta que se puedan pagar las notas
                                                                                              //CondicionVenta = (NotaDebitoElectronicaCondicionVenta)invoice.ConditionSaleType, // se toma de la factura
               PlazoCredito = note.CreditTerm.ToString(),
               //MedioPago = new NotaDebitoElectronicaMedioPago[] { (NotaDebitoElectronicaMedioPago)invoice.PaymetnMethodType }, // se toma de la factura                        

                ResumenFactura = new NotaDebitoElectronicaResumenFactura
                {
                    CodigoMoneda = (TicoPay.Invoices.NotaDebito.NotaDebitoElectronicaResumenFacturaCodigoMoneda)note.CodigoMoneda,  // se toma de la nota
                    CodigoMonedaSpecified = true,
                    TipoCambio = rate,
                    TipoCambioSpecified = true,
                    TotalServGravados = ConvertCRCToUSDDecimal(note.TotalServGravados, 1, _tenant, note, null),
                    TotalServGravadosSpecified = true,
                    TotalServExentos = ConvertCRCToUSDDecimal(note.TotalServExento, 1, _tenant, note, null),
                    TotalServExentosSpecified = true,
                    TotalMercanciasExentasSpecified = false,
                    TotalMercanciasGravadasSpecified = false,
                    TotalGravado = ConvertCRCToUSDDecimal(note.TotalGravado, 1, _tenant, note, null),
                    TotalGravadoSpecified = true,
                    TotalExento = ConvertCRCToUSDDecimal(note.TotalExento, 1, _tenant, note, null),
                    TotalExentoSpecified = true,
                    TotalVenta = ConvertCRCToUSDDecimal(note.SaleTotal, 1, _tenant, note, null),
                    TotalDescuentosSpecified = true,
                    TotalVentaNeta = ConvertCRCToUSDDecimal(note.Amount, 1, _tenant, note, null),
                    TotalImpuesto = ConvertCRCToUSDDecimal(note.TaxAmount, 1, _tenant, note, null),
                    TotalDescuentos = ConvertCRCToUSDDecimal(note.DiscountAmount, 1, _tenant, note, null),
                    TotalImpuestoSpecified = true,
                    TotalComprobante = ConvertCRCToUSDDecimal(note.Total, 1, _tenant, note, null)
                },
               
                Normativa = new NotaDebitoElectronicaNormativa { NumeroResolucion = ConfigurationManager.AppSettings["XML.NumeroResolucion"], FechaResolucion = ConfigurationManager.AppSettings["XML.FechaResolucion"] }
                //,InformacionReferencia = new NotaDebitoElectronicaInformacionReferencia[] {
                //        new NotaDebitoElectronicaInformacionReferencia {
                //             TipoDoc = NotaDebitoElectronicaInformacionReferenciaTipoDoc.Item01, // aplica a factura
                //             Numero = ConsecutiveNumber,
                //             FechaEmision = dateDoc,
                //             Codigo = (NotaDebitoElectronicaInformacionReferenciaCodigo) note.NoteReasons,
                //             Razon =  razonote
                //          }
                //    }
            };

            #region emisor
            // emisor
            var emisor = new NotaDebito.EmisorType();

            emisor.Nombre = _tenant.Name;
            emisor.NombreComercial = _tenant.BussinesName;
            emisor.Identificacion = new NotaDebito.IdentificacionType { Tipo = (NotaDebito.IdentificacionTypeTipo)_tenant.IdentificationType, Numero = _tenant.IdentificationNumber };
            emisor.NombreComercial = _tenant.ComercialName;
            emisor.Ubicacion = new NotaDebito.UbicacionType
            {
                Provincia = _tenant.Barrio.Distrito.Canton.Provincia.Id.ToString(),
                Canton = _tenant.Barrio.Distrito.Canton.codigocanton.PadLeft(2, '0'),
                Distrito = _tenant.Barrio.Distrito.codigodistrito.PadLeft(2, '0'),
                Barrio = _tenant.Barrio.codigobarrio.PadLeft(2, '0'),
                OtrasSenas = _tenant.Address
            };
            emisor.CorreoElectronico = _tenant.Email;

            if (!String.IsNullOrEmpty(_tenant.Fax))
            {
                _faxTenant = _tenant.Fax.Substring(4, _tenant.Fax.Length - 4);
                _codfaxTenant = _tenant.Fax.Substring(0, 3);
                emisor.Fax = new NotaDebito.TelefonoType { CodigoPais = _codfaxTenant, NumTelefono = _faxTenant.PadLeft(20, '0') };
            }

            if (!String.IsNullOrEmpty(_tenant.PhoneNumber))
            {
                _phoneTenant = _tenant.PhoneNumber.Substring(4, _tenant.PhoneNumber.Length - 4);
                _codphoneTenant = _tenant.PhoneNumber.Substring(0, 3);
                emisor.Telefono = new NotaDebito.TelefonoType { CodigoPais = _codphoneTenant, NumTelefono = _phoneTenant.PadLeft(20, '0') };
            }

            item.Emisor = emisor;

            #endregion

            #region receptor
            // receptor
            if (client != null)
            {
                var receptor = new NotaDebito.ReceptorType();


                var nombrereceptor = note.Invoice.ClientName ?? "";
                if (nombrereceptor.Length > 80)
                    nombrereceptor = nombrereceptor.Substring(0, 80);

                receptor.Nombre = nombrereceptor;
                if (!String.IsNullOrWhiteSpace(note.Invoice.ClientIdentification))
                {
                    receptor.Identificacion = (note.Invoice.ClientIdentificationType == XSD.IdentificacionTypeTipo.NoAsiganda ? null : new NotaDebito.IdentificacionType { Tipo = (NotaDebito.IdentificacionTypeTipo)note.Invoice.ClientIdentificationType, Numero = note.Invoice.ClientIdentification });

                    if (note.Invoice.ClientIdentificationType == XSD.IdentificacionTypeTipo.NoAsiganda)
                        receptor.IdentificacionExtranjero = note.Invoice.ClientIdentification;
                }

                if (!String.IsNullOrEmpty(note.Invoice.ClientPhoneNumber))
                {
                    _phoneClient = note.Invoice.ClientPhoneNumber.Substring(4, note.Invoice.ClientPhoneNumber.Length - 4);
                    _codphoneClient = note.Invoice.ClientPhoneNumber.Substring(0, 3);
                    receptor.Telefono = new NotaDebito.TelefonoType { CodigoPais = _codphoneClient, NumTelefono = _phoneClient.PadLeft(20, '0') };
                }

                if ((client.NameComercial != null) && (client.NameComercial.Length > 80))
                    receptor.NombreComercial = client.NameComercial.Substring(0, 80);
                else
                    receptor.NombreComercial = client.NameComercial;


                if ((!String.IsNullOrEmpty(client.Address)) && (client.Barrio != null))
                {
                    receptor.Ubicacion = new NotaDebito.UbicacionType
                    {
                        Provincia = client.Barrio.Distrito.Canton.Provincia.Id.ToString(),
                        Canton = client.Barrio.Distrito.Canton.codigocanton.PadLeft(2, '0'),
                        Distrito = client.Barrio.Distrito.codigodistrito.PadLeft(2, '0'),
                        Barrio = client.Barrio.codigobarrio.PadLeft(2, '0'),
                        OtrasSenas = client.Address
                    };
                }

                if (!String.IsNullOrEmpty(client.PhoneNumber))
                {
                    _phoneClient = client.PhoneNumber.Substring(4, client.PhoneNumber.Length - 4);
                    _codphoneClient = client.PhoneNumber.Substring(0, 3);
                    receptor.Telefono = new NotaDebito.TelefonoType { CodigoPais = _codphoneClient, NumTelefono = _phoneClient/*.PadLeft(20, '0') */};
                }

                if (!String.IsNullOrEmpty(client.Fax))
                {
                    _faxCliente = client.Fax.Substring(4, client.Fax.Length - 4);
                    _codfaxClient = client.Fax.Substring(0, 3);
                    receptor.Fax = new NotaDebito.TelefonoType { CodigoPais = _codfaxClient, NumTelefono = _faxCliente/*.PadLeft(20, '0')*/ };
                }

                if (!string.IsNullOrEmpty(note.Invoice.ClientEmail))
                {
                    receptor.CorreoElectronico = note.Invoice.ClientEmail;
                }

                item.Receptor = receptor;
            }
            #endregion receptor

            #region detalle
            var lineDetail = new NotaDebitoElectronicaLineaDetalle[note.NotesLines.Count];
            int i = 0;
            foreach (var line in note.NotesLines)
            {
                var list = new NotaDebitoElectronicaLineaDetalle();
                list.NumeroLinea = line.LineNumber.ToString();
                //Codigo = new CodigoTypeND []{new CodigoTypeND {Tipo = CodigoTypeTipoND.Item04, Codigo = service.Id.ToString().Substring(0,20)}}, // se corta a 20 el id porque es lo permirtido por hacienda
                list.Cantidad = line.Quantity;
                list.UnidadMedida = (NotaDebito.UnidadMedidaType)line.UnitMeasurement;
                list.UnidadMedidaComercial = ((line.UnitMeasurementOthers != null && line.UnitMeasurementOthers.Length > 20) ? line.UnitMeasurementOthers.Substring(0, 20) : line.UnitMeasurementOthers);
                list.Detalle = line.Title;// verificar esto
                list.PrecioUnitario = ConvertCRCToUSDDecimal(line.PricePerUnit, 1, _tenant, note, null);
                list.MontoTotal = ConvertCRCToUSDDecimal(line.Total, 1, _tenant, note, null);

                if (line.DiscountPercentage > 0)
                {
                    list.MontoDescuento = ConvertCRCToUSDDecimal((line.Total * line.DiscountPercentage) / 100, 1, _tenant, note, null);
                    list.NaturalezaDescuento = line.DescriptionDiscount ?? "Descuento en Servicio";
                    list.MontoDescuentoSpecified = true;
                }
                else
                    list.MontoDescuentoSpecified = false;

                list.SubTotal = ConvertCRCToUSDDecimal(line.SubTotal, 1, _tenant, note, null);
                // tipo de impuesto

                /// CAMBIAR ESTO
                list.Impuesto = new NotaDebito.ImpuestoType[] { new NotaDebito.ImpuestoType {Codigo = (NotaDebito.ImpuestoTypeCodigo) line.Tax.TaxTypes, Tarifa = line.Tax.Rate,

                Monto =  ConvertCRCToUSDDecimal(line.TaxAmount,1,_tenant,note,null)}};
                //Exoneracion = new ExoneracionType {TipoDocumento = ExoneracionTypeTipoDocumento.Item01, }} 
                list.MontoTotalLinea = ConvertCRCToUSDDecimal(line.LineTotal, 1, _tenant, note, null);

                lineDetail[i] = list;
                i++;
            }
            item.DetalleServicio = lineDetail;

            #endregion detalle

            #region Otros

            var Otros = note.NoteReasons == NoteReason.Otros ? new NotaDebitoElectronicaOtros { OtroTexto = new NotaDebitoElectronicaOtrosOtroTexto[] { new NotaDebitoElectronicaOtrosOtroTexto { codigo = "99", Value = note.NoteReasonsOthers } } } : null;
            if (Otros != null)
                item.Otros = Otros;

            #endregion Otros

            #region Referencias y Contingencia
            int n = 1;
            NotaDebitoElectronicaInformacionReferencia referencia = new NotaDebitoElectronicaInformacionReferencia
            {
                TipoDoc = tipoDoc, // aplica a factura
                Numero = ConsecutiveNumber,
                FechaEmision = dateDoc,
                Codigo = (NotaDebitoElectronicaInformacionReferenciaCodigo)note.NoteReasons,
                Razon = razonote
            };
            NotaDebitoElectronicaInformacionReferencia contingencia = null;
            if (note.IsContingency)
            {
                n = 2;
                contingencia =
               new NotaDebitoElectronicaInformacionReferencia
               {
                   Codigo = NotaDebitoElectronicaInformacionReferenciaCodigo.Item05,
                   Numero = note.ConsecutiveNumberContingency,
                   Razon = note.ReasonContingency,
                   FechaEmision = Convert.ToDateTime(note.DateContingency),
                   TipoDoc = NotaDebitoElectronicaInformacionReferenciaTipoDoc.Item08
               };

               
            }

            var referencias = new NotaDebitoElectronicaInformacionReferencia[n];

            referencias[0] = referencia;

            if (note.IsContingency)
                referencias[1] = contingencia;

            item.InformacionReferencia = referencias;
            #endregion Referencias y Contingencia

            return item;
        }

        public static NotaCreditoElectronica CreateNoteCredito(Client client, Tenant _tenant, Note note, string ConsecutiveNumber, DateTime dateDoc)
        {
            string _codphoneClient = "506", _phoneClient = string.Empty, _codfaxClient = "506", _faxCliente = string.Empty, _codphoneTenant = "506", _phoneTenant = string.Empty, _faxTenant = string.Empty, _codfaxTenant = "506";

            string razonote = getNoteReasons(note.NoteReasons);
            var tipoDoc = (note.ConsecutiveNumberReference == null) ? (NotaCreditoElectronicaInformacionReferenciaTipoDoc)note.Invoice.TypeDocument :
               ((note.NoteType == NoteType.Credito) ? NotaCreditoElectronicaInformacionReferenciaTipoDoc.Item02 : NotaCreditoElectronicaInformacionReferenciaTipoDoc.Item03);

            decimal rate = _tenant.IsConvertUSD ? note.ChangeType : 1;

            NotaCreditoElectronica item = new NotaCreditoElectronica
            {
                Clave = note.VoucherKey,
                NumeroConsecutivo = note.ConsecutiveNumber,
                FechaEmision = note.CreationTime,
                CondicionVenta = (NotaCreditoElectronicaCondicionVenta)note.ConditionSaleType,//NotaCreditoElectronicaCondicionVenta.Item01, // se toma de factura
                //CondicionVenta = (NotaCreditoElectronicaCondicionVenta)invoice.ConditionSaleType, // se toma de factura
                PlazoCredito = note.CreditTerm.ToString(),
                //MedioPago = new NotaCreditoElectronicaMedioPago[] { (NotaCreditoElectronicaMedioPago)invoice.PaymetnMethodType }, //se toma de factura

                ResumenFactura = new NotaCreditoElectronicaResumenFactura
                {
                    CodigoMoneda = (TicoPay.Invoices.NCR.NotaCreditoElectronicaResumenFacturaCodigoMoneda)note.CodigoMoneda,  // se toma de nota
                    CodigoMonedaSpecified = true,
                    TipoCambio = rate,
                    TipoCambioSpecified = true,
                    TotalServGravados = ConvertCRCToUSDDecimal(note.TotalServGravados,1,_tenant,note,null),
                    TotalServGravadosSpecified = true,
                    TotalServExentos = ConvertCRCToUSDDecimal(note.TotalServExento, 1, _tenant, note, null),
                    TotalServExentosSpecified = true,
                    TotalMercanciasExentasSpecified = true,
                    TotalMercanciasExentas= ConvertCRCToUSDDecimal(note.TotalProductExento, 1, _tenant, note, null),
                    TotalMercanciasGravadasSpecified = true,
                    TotalMercanciasGravadas= ConvertCRCToUSDDecimal(note.TotalProductGravado, 1, _tenant, note, null),
                    TotalGravado = ConvertCRCToUSDDecimal(note.TotalGravado, 1, _tenant, note, null),
                    TotalGravadoSpecified = true,
                    TotalExento = ConvertCRCToUSDDecimal(note.TotalExento, 1, _tenant, note, null),
                    TotalExentoSpecified = true,
                    TotalVenta = ConvertCRCToUSDDecimal(note.Amount, 1, _tenant, note, null),
                    TotalDescuentosSpecified = false,
                    TotalVentaNeta = ConvertCRCToUSDDecimal(note.Amount, 1, _tenant, note, null),
                    TotalImpuesto = ConvertCRCToUSDDecimal(note.TaxAmount, 1, _tenant, note, null),
                    TotalImpuestoSpecified = true,
                    TotalComprobante = ConvertCRCToUSDDecimal(note.Total, 1, _tenant, note, null)
                },
                Normativa = new NotaCreditoElectronicaNormativa { NumeroResolucion = ConfigurationManager.AppSettings["XML.NumeroResolucion"], FechaResolucion = ConfigurationManager.AppSettings["XML.FechaResolucion"] }
                //,InformacionReferencia = new NotaCreditoElectronicaInformacionReferencia[] {
                //        new NotaCreditoElectronicaInformacionReferencia {
                //             TipoDoc =  tipoDoc, // aplica a factura
                //             Numero = ConsecutiveNumber,
                //             FechaEmision = dateDoc,
                //             Codigo = (NotaCreditoElectronicaInformacionReferenciaCodigo)note.NoteReasons,
                //             Razon = razonote
                //          }
                //    }
            };
            #region emisor
            // emisor
            var emisor = new NCR.EmisorType();

            emisor.Nombre = _tenant.Name;
            emisor.NombreComercial = _tenant.BussinesName;
            emisor.Identificacion = new NCR.IdentificacionType { Tipo = (NCR.IdentificacionTypeTipo)_tenant.IdentificationType, Numero = _tenant.IdentificationNumber };
            emisor.NombreComercial = _tenant.ComercialName;
            emisor.Ubicacion = new NCR.UbicacionType
            {
                Provincia = _tenant.Barrio.Distrito.Canton.Provincia.Id.ToString(),
                Canton = _tenant.Barrio.Distrito.Canton.codigocanton.PadLeft(2, '0'),
                Distrito = _tenant.Barrio.Distrito.codigodistrito.PadLeft(2, '0'),
                Barrio = _tenant.Barrio.codigobarrio.PadLeft(2, '0'),
                OtrasSenas = _tenant.Address
            };
            emisor.CorreoElectronico = _tenant.Email;

            if (!String.IsNullOrEmpty(_tenant.Fax))
            {
                _faxTenant = _tenant.Fax.Substring(4, _tenant.Fax.Length - 4);
                _codfaxTenant = _tenant.Fax.Substring(0, 3);
                emisor.Fax = new NCR.TelefonoType { CodigoPais = _codfaxTenant, NumTelefono = _faxTenant/*.PadLeft(20, '0')*/ };
            }

            if (!String.IsNullOrEmpty(_tenant.PhoneNumber))
            {
                _phoneTenant = _tenant.PhoneNumber.Substring(4, _tenant.PhoneNumber.Length - 4);
                _codphoneTenant = _tenant.PhoneNumber.Substring(0, 3);
                emisor.Telefono = new NCR.TelefonoType { CodigoPais = _codphoneTenant, NumTelefono = _phoneTenant/*.PadLeft(20, '0')*/ };
            }

            item.Emisor = emisor;
            #endregion emisor

            #region receptor
            // receptor
            if (client != null)
            {
                var receptor = new NCR.ReceptorType();


                var nombrereceptor = note.Invoice.ClientName ?? "";
                if (nombrereceptor.Length > 80)
                    nombrereceptor = nombrereceptor.Substring(0, 80);

                receptor.Nombre = nombrereceptor;
                if (!String.IsNullOrWhiteSpace(note.Invoice.ClientIdentification))
                {
                    receptor.Identificacion = (note.Invoice.ClientIdentificationType == XSD.IdentificacionTypeTipo.NoAsiganda ? null : new NCR.IdentificacionType { Tipo = (NCR.IdentificacionTypeTipo)note.Invoice.ClientIdentificationType, Numero = note.Invoice.ClientIdentification });

                    if (note.Invoice.ClientIdentificationType == XSD.IdentificacionTypeTipo.NoAsiganda)
                        receptor.IdentificacionExtranjero = note.Invoice.ClientIdentification;
                }

                if (!String.IsNullOrEmpty(note.Invoice.ClientPhoneNumber))
                {
                    _phoneClient = note.Invoice.ClientPhoneNumber.Substring(4, note.Invoice.ClientPhoneNumber.Length - 4);
                    _codphoneClient = note.Invoice.ClientPhoneNumber.Substring(0, 3);
                    receptor.Telefono = new NCR.TelefonoType { CodigoPais = _codphoneClient, NumTelefono = _phoneClient.PadLeft(20, '0') };
                }

                if ((client.NameComercial != null) && (client.NameComercial.Length > 80))
                    receptor.NombreComercial = client.NameComercial.Substring(0, 80);
                else
                    receptor.NombreComercial = client.NameComercial;


                if ((!String.IsNullOrEmpty(client.Address)) && (client.Barrio != null))
                {
                    receptor.Ubicacion = new NCR.UbicacionType
                    {
                        Provincia = client.Barrio.Distrito.Canton.Provincia.Id.ToString(),
                        Canton = client.Barrio.Distrito.Canton.codigocanton.PadLeft(2, '0'),
                        Distrito = client.Barrio.Distrito.codigodistrito.PadLeft(2, '0'),
                        Barrio = client.Barrio.codigobarrio.PadLeft(2, '0'),
                        OtrasSenas = client.Address
                    };
                }

                if (!String.IsNullOrEmpty(client.PhoneNumber))
                {
                    _phoneClient = client.PhoneNumber.Substring(4, client.PhoneNumber.Length - 4);
                    _codphoneClient = client.PhoneNumber.Substring(0, 3);
                    receptor.Telefono = new NCR.TelefonoType { CodigoPais = _codphoneClient, NumTelefono = _phoneClient/*.PadLeft(20, '0')*/ };
                }


                if (!String.IsNullOrEmpty(client.Fax))
                {
                    _faxCliente = client.Fax.Substring(4, client.Fax.Length - 4);
                    _codfaxClient = client.Fax.Substring(0, 3);
                    receptor.Fax = new NCR.TelefonoType { CodigoPais = _codfaxClient, NumTelefono = _faxCliente/*.PadLeft(20, '0')*/ };
                }

                if (!string.IsNullOrEmpty(note.Invoice.ClientEmail))
                {
                    receptor.CorreoElectronico = note.Invoice.ClientEmail;
                }

                item.Receptor = receptor;
            }
            #endregion receptor


            #region detalle
            var lineDetail = new NotaCreditoElectronicaLineaDetalle[note.NotesLines.Count];
            int i = 0;
            foreach (var line in note.NotesLines)
            {
                var list = new NotaCreditoElectronicaLineaDetalle();
                list.NumeroLinea = line.LineNumber.ToString();
                //Codigo = new CodigoTypeND []{new CodigoTypeND {Tipo = CodigoTypeTipoND.Item04, Codigo = service.Id.ToString().Substring(0,20)}}, // se corta a 20 el id porque es lo permirtido por hacienda
                list.Cantidad = line.Quantity;
                list.UnidadMedida = (NCR.UnidadMedidaType)line.UnitMeasurement;
                list.UnidadMedidaComercial = ((line.UnitMeasurementOthers != null && line.UnitMeasurementOthers.Length > 20) ? line.UnitMeasurementOthers.Substring(0, 20) : line.UnitMeasurementOthers);
                list.Detalle = line.Title;// verificar esto
                list.PrecioUnitario = ConvertCRCToUSDDecimal(line.PricePerUnit, 1, _tenant, note, null);
                list.MontoTotal = ConvertCRCToUSDDecimal(line.Total, 1, _tenant, note, null);

                if (line.DiscountPercentage > 0)
                {
                    list.MontoDescuento = ConvertCRCToUSDDecimal((line.Total * line.DiscountPercentage) / 100, 1, _tenant, note, null);
                    list.NaturalezaDescuento = line.DescriptionDiscount ?? "Descuento en Servicio";
                    list.MontoDescuentoSpecified = true;
                }
                else
                    list.MontoDescuentoSpecified = false;

                list.SubTotal = ConvertCRCToUSDDecimal(line.SubTotal, 1, _tenant, note, null);
                // tipo de impuesto

                /// CAMBIAR ESTO
                list.Impuesto = new NCR.ImpuestoType[] { new NCR.ImpuestoType {Codigo = (NCR.ImpuestoTypeCodigo) line.Tax.TaxTypes, Tarifa = line.Tax.Rate,
                Monto =  ConvertCRCToUSDDecimal(line.TaxAmount,rate,_tenant,note,null)}};
                //Exoneracion = new ExoneracionType {TipoDocumento = ExoneracionTypeTipoDocumento.Item01, }} 
                list.MontoTotalLinea = ConvertCRCToUSDDecimal(line.LineTotal, 1, _tenant, note, null);
                lineDetail[i] = list;
                i++;
            }
            item.DetalleServicio = lineDetail;
            #endregion detalle

            #region Otros

            var Otros = note.NoteReasons == NoteReason.Otros ? new NotaCreditoElectronicaOtros { OtroTexto = new NotaCreditoElectronicaOtrosOtroTexto[] { new NotaCreditoElectronicaOtrosOtroTexto { codigo = "99", Value = note.NoteReasonsOthers } } } : null;
            if (Otros != null)
                item.Otros = Otros;
            #endregion Otros

            #region Referencias y Contingencia
            int n = 1;
            NotaCreditoElectronicaInformacionReferencia referencia = new NotaCreditoElectronicaInformacionReferencia
            {
                TipoDoc = tipoDoc, // aplica a factura
                Numero = ConsecutiveNumber,
                FechaEmision = dateDoc,
                Codigo = (NotaCreditoElectronicaInformacionReferenciaCodigo)note.NoteReasons,
                Razon = razonote
            };
            NotaCreditoElectronicaInformacionReferencia contingencia = null;
            if (note.IsContingency)
            {
                n = 2;
                 contingencia = 
                new NotaCreditoElectronicaInformacionReferencia{
                        Codigo = NotaCreditoElectronicaInformacionReferenciaCodigo.Item05,
                        Numero = note.ConsecutiveNumberContingency,
                        Razon= note.ReasonContingency,
                        FechaEmision= Convert.ToDateTime(note.DateContingency),
                        TipoDoc= NotaCreditoElectronicaInformacionReferenciaTipoDoc.Item08
                    };               
            }

            var referencias = new NotaCreditoElectronicaInformacionReferencia[n];

            referencias[0] = referencia;

            if (note.IsContingency)
                referencias[1] = contingencia;

            item.InformacionReferencia = referencias;
            #endregion Referencias y Contingencia

            return item;
        }

        public static NotaDebitoElectronica CreateNoteDebitoToSerialize(Invoice invoice, Client client, Tenant _tenant, Note note)
        {
            NotaDebitoElectronica item = CreateNoteDebito(client, _tenant, note, invoice.ConsecutiveNumber, invoice.DueDate);         
             return item;
        }

        public static NotaCreditoElectronica CreateNoteCreditoToSerialize(Invoice invoice, Client client, Tenant _tenant, Note note)
        {
            NotaCreditoElectronica item = CreateNoteCredito(client, _tenant, note, invoice.ConsecutiveNumber, invoice.DueDate);
            return item;         
        }

        public static Stream GetXML(NotaCreditoElectronica note)
        {
            MemoryStream stream = SerializeToStream(note);
            return stream;
        }

        public static MemoryStream SerializeToStream(NotaCreditoElectronica o)
        {
            var serializer = new XmlSerializer(typeof(NotaCreditoElectronica));

            MemoryStream stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
            serializer.Serialize(stream, o);

            return stream;
        }


        public static Stream GetXML(NotaDebitoElectronica note)
        {
            MemoryStream stream = SerializeToStream(note);
            return stream;
        }

        public static MemoryStream SerializeToStream(NotaDebitoElectronica o)
        {
            var serializer = new XmlSerializer(typeof(NotaDebitoElectronica));

            MemoryStream stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
            serializer.Serialize(stream, o);

            return stream;
        }

        public void CreateXMLAnulaNoteCR(Note _noteOriginal, Client client, Tenant _tenant, Note note, Certificate certified)
        {
            XmlSerializer serializer2 = new XmlSerializer(typeof(NotaCreditoElectronica));
            XmlSerializer serializer3 = new XmlSerializer(typeof(NotaDebitoElectronica));
            string path = Path.Combine(WorkPaths.GetXmlPath(), "note_" + note.VoucherKey + ".xml");

          


            // arma la clase para el XML para nota de debito
            if (note.NoteType == NoteType.Debito)
            {
               
                NotaDebitoElectronica item = CreateNoteDebito(client, _tenant, note, _noteOriginal.ConsecutiveNumber, _noteOriginal.CreationTime);               
              
                if (!_tenant.ValidateHacienda)
                {
                    path = Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml");
                }

                TextWriter writer2 = new StreamWriter(path);
                serializer3.Serialize(writer2, item);
                writer2.Close();
            }

            if (note.NoteType == NoteType.Credito)
            {
                NotaCreditoElectronica item = CreateNoteCredito(client, _tenant, note, _noteOriginal.ConsecutiveNumber, _noteOriginal.CreationTime);
               
                if (!_tenant.ValidateHacienda)
                {
                    path = Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml");
                }

                TextWriter writer = new StreamWriter(path);
                serializer2.Serialize(writer, item);
                writer.Close();
            }

            if (_tenant.ValidateHacienda && certified != null)
            {

                //Guarda el certificado temporalmente

                string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + certified.FileName).ToLower();

                //var _certifiedPath = Path.GetFullPath("Common" + "\\" + archivo);

                File.WriteAllBytes(Path.Combine(WorkPaths.GetCertifiedPath(), archivo), certified.CertifiedRoute);

                // Firmar note
                SignedXMLXADES(Path.Combine(WorkPaths.GetXmlPath(), "note_" + note.VoucherKey + ".xml"),
                Path.Combine(WorkPaths.GetXmlSignedPath(), "note_" + note.VoucherKey + ".xml"),
                Path.Combine(WorkPaths.GetCertifiedPath(), archivo), certified.Password);
            }
        }

        public void SignedXMLXADES(string RutaXML, string RutaXMLSigned, string archivo, string password)
        {
            XadesService xadesService = new XadesService();
            SignatureParameters parametros = new SignatureParameters();

            //parametros.SignatureMethod = SignatureMethod.RSAwithSHA256;
            parametros.SigningDate = DateTime.Now;

            // Política de firma de factura-e
            parametros.SignaturePolicyInfo = new SignaturePolicyInfo();
            parametros.SignaturePolicyInfo.PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4/Resolucion_Comprobantes_Electronicos_DGT-R-482016.pdf";
            parametros.SignaturePolicyInfo.PolicyHash = "NmI5Njk1ZThkNzI0MmIzMGJmZDAyNDc4YjUwNzkzODM2NTBiOWUxNTBkMmI2YjgzYzZjM2I5NTZlNDQ4OWQzMQ==";
            parametros.SignaturePackaging = SignaturePackaging.ENVELOPED;
            parametros.InputMimeType = "text/xml";

            X509Certificate2 selectedCertificate = new X509Certificate2(@archivo, password, X509KeyStorageFlags.Exportable);

            //using (parametros.Signer = new Signer(CertsHelper.Load("TicoPay.Common." + archivo, password)))
            using (parametros.Signer = new Signer(selectedCertificate))
            {
                using (FileStream fs = new FileStream(@RutaXML, FileMode.Open))
                {
                    var docFirmado = xadesService.Sign(fs, parametros);
                    docFirmado.Save(@RutaXMLSigned);
                }
            }

            // borra el certificado del directorio temporal
            FileInfo filecert = new FileInfo(archivo);
            filecert.Delete();
        }
        /// <summary>
        /// Genera un codigo de seguridad a la FE
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string SecurityCode(int size)
        {
            System.Random randomGenerate = new System.Random();
            System.String sPassword = "";
            sPassword = System.Convert.ToString(randomGenerate.Next(00000001, 99999999));
            sPassword = sPassword.PadLeft(size, '0');
            return sPassword.Substring(sPassword.Length - size, size);
        }

        //public void SetTaxAmount(Service service, decimal amountNote)
        //{

        //    decimal rate = service.Tax.Rate;

        //    TaxAmount = (amountNote * rate) / 100;

        //    Total = amountNote + TaxAmount;
        //}
        public void SetInvoiceNumberKey(DateTime fechafactura, string consecutive, Tenant tenant, int vouchersituation)
        {
            string dia = fechafactura.Day.ToString();
            string anio = fechafactura.Year.ToString().Substring(2, 2);
            string mes = fechafactura.Month.ToString();
            string securityCode = SecurityCode(8);

            VoucherKey = tenant.Country.CountryCode.ToString() + dia.PadLeft(2, '0') + mes.PadLeft(2, '0') + anio + tenant.IdentificationNumber.PadLeft(12, '0') + consecutive + vouchersituation.ToString() + securityCode;

            var writer = new ZXing.Presentation.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    PureBarcode = true,
                    Height = 100,
                    Width = 100
                }
            };

            var image = writer.Write(VoucherKey);
            QRCode = ConvertWriteableBitmapToByte(image);
        }

        public byte[] ConvertWriteableBitmapToByte(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }

            byte[] data;
            PngBitmapEncoder encoder2 = new PngBitmapEncoder();
            encoder2.Frames.Add(BitmapFrame.Create(bmImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder2.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        public static string ConvertToString(Enum e)
        {
            // Get the Type of the enum
            Type t = e.GetType();

            // Get the FieldInfo for the member field with the enums name
            FieldInfo info = t.GetField(e.ToString("G"));

            // Check to see if the XmlEnumAttribute is defined on this field
            if (!info.IsDefined(typeof(XmlEnumAttribute), false))
            {
                // If no XmlEnumAttribute then return the string version of the enum.
                return e.ToString("G");
            }

            // Get the XmlEnumAttribute
            object[] o = info.GetCustomAttributes(typeof(XmlEnumAttribute), false);
            XmlEnumAttribute att = (XmlEnumAttribute)o[0];
            return att.Name;
        }

        public void SetInvoiceConsecutivo(Register tenantRegisters, Tenant tenant, NoteType notetype, bool incrementLastInvoiceNumber = true, Drawer drawer=null)
        {
            string consecutivo = null;

            if (incrementLastInvoiceNumber)
            {
                if (notetype == NoteType.Debito)
                {
                    if (tenantRegisters.LastNoteDebitNumber == 9999999999)
                        tenantRegisters.LastNoteDebitNumber = 0;

                    tenantRegisters.LastNoteDebitNumber = tenantRegisters.LastNoteDebitNumber + 1;
                    consecutivo = tenantRegisters.LastNoteDebitNumber.ToString();
                }
                else
                {
                    if (tenantRegisters.LastNoteCreditNumber == 9999999999)
                        tenantRegisters.LastNoteCreditNumber = 0;

                    tenantRegisters.LastNoteCreditNumber = tenantRegisters.LastNoteCreditNumber + 1;
                    consecutivo = tenantRegisters.LastNoteCreditNumber.ToString();
                } 
            }
            else
            {
                if (notetype == NoteType.Debito)
                {
                    consecutivo = ((tenantRegisters.LastNoteDebitNumber == 9999999999) ? 0 : tenantRegisters.LastNoteDebitNumber + 1).ToString();
                }
                else
                {
                    consecutivo = ((tenantRegisters.LastNoteCreditNumber == 9999999999) ? 0 : tenantRegisters.LastNoteCreditNumber + 1).ToString();
                }
            }

            string note = ConvertToString(notetype);
            if (drawer==null) 
                ConsecutiveNumber = tenant.local.ToString() + tenantRegisters.RegisterCode.ToString() + note + consecutivo.PadLeft(10, '0'); // cambiar const
            else
                ConsecutiveNumber = drawer.BranchOffice.Code.PadLeft(3,'0') + drawer.Code.PadLeft(5,'0') + note + consecutivo.PadLeft(10, '0'); // cambiar const
        }

        public void byteArrayToImage(byte[] byteArrayIn, string voucherkey)
        {
            byte[] bitmap = byteArrayIn;
            using (Image image = Image.FromStream(new MemoryStream(bitmap)))
            {
                image.Save(Path.Combine(WorkPaths.GetQRPath(), "note_" + voucherkey + ".png"), ImageFormat.Png);
            }

        }

        public void ReGenerateQrFromVoucherKey()
        {
            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                barcodeWriter.Write(VoucherKey).Save(memoryStream, ImageFormat.Png);
                QRCode = memoryStream.ToArray();
            }
        }

        public void CreatePDFNote(Invoice invoice, Client client, Tenant _tenant, Note note, ReportSettings reportSettings)
        {
            GeneratePDF PdfInvoie = new GeneratePDF(reportSettings);

            PdfInvoie.CreatePDFNote(invoice, client, _tenant, note);

        }

        public void CreatePDFNote(Note note, ReportSettings reportSettings)
        {
            GeneratePDF PdfInvoie = new GeneratePDF(reportSettings);

            PdfInvoie.CreatePDFNote(note.Invoice, note.Invoice.Client, note.Invoice.Tenant, note);

        }

        public decimal GetRate()
        {
            RateOfDay _rateOfDay = new RateOfDay();
            return _rateOfDay.RateDate(DueDate).rate;
    }

        public static string ConvertUSDToCRC(decimal valor, decimal rate, Tenant tenant, Note note,NoteCodigoMoneda? codigoMoneda)
        {
            string convert = "";
            if (RunConversion(tenant, note))
            {
                switch (codigoMoneda)
                {
                    case NoteCodigoMoneda.USD:
                        {
                            convert = ConvertUSD(valor, rate);
                            break;
                        }
                    case NoteCodigoMoneda.CRC:
                        {
                            convert = ConvertCRC(valor, rate);
                            break;
                        }
                    default:
                        {
                            if (note.CodigoMoneda.Equals(NoteCodigoMoneda.CRC))
                            {
                                convert = ConvertCRC(valor, 1);
                            }
                            else if (note.CodigoMoneda.Equals(NoteCodigoMoneda.USD))
                            {
                                convert = ConvertCRC(valor, rate);
                            }
                            break;
                        }
                }
            }
            else
            {
                convert = String.Format("{0:n2}", valor);
            }
            return convert;
        }

        private static decimal ConvertCRCToUSDDecimal(decimal valor, decimal rate, Tenant tenant, Note note, NoteCodigoMoneda? codigoMoneda)
        {
            string rateString = ConvertUSDToCRC(valor, rate, tenant, note,codigoMoneda);
            return decimal.Parse(rateString);
        }

        private static string ConvertCRC(decimal valor, decimal rate)
        {
            string convert = "";
            decimal convertUSD = valor / rate;
            if (convertUSD != 0)
            {
                convert = CalcularMontosConver(convert, convertUSD);
            }
            else
            {
                convert = "0.00000";
            }
            return convert;
        }

        private static string ConvertUSD(decimal valor, decimal rate)
        {
            string convert = "";
            decimal convertUSD = valor * rate;
            if (convertUSD != 0)
            {
                convert = CalcularMontosConver(convert, convertUSD);
            }
            else
            {
                convert = "0.00000";
            }
            return convert;
        }

        private static string CalcularMontosConver(string convert, decimal valorConvert)
        {
            string convertUSDString = String.Format("{0:n7}", valorConvert);

            var position = convertUSDString.IndexOf('.');
            string stringEntero = convertUSDString.Substring(0, position);
            string stringDecimal = convertUSDString.ToString().Substring(position, 6);
            convert = stringEntero + stringDecimal;
            return convert;
        }

        public static bool RunConversion(Tenant tenant, Note note)
        {
            if (tenant.IsConvertUSD)
            {
                return true;
            }
            return false;
        }

        public void TotalNote(Note note, Invoice invoice)
        {

        }

    }

     /// <summary>
    /// Enumerado que contiene los Tipos de Nota / Enum that contains the Memo Types
    /// </summary>
    public enum NoteType
    {
        /// <summary>
        /// Nota de Débito / Debit Memo
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        [Description("Débito")]
        [Display(Name = "Débito")]
        Debito,
        /// <summary>
        /// Nota de Crédito / Credit Memo
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        [Description("Crédito")]
        [Display(Name = "Crédito")]
        Credito,

       

    }

    //public enum TypeDocuments
    //{
    //    /// <summary>
    //    /// Nota de Débito
    //    /// </summary>
    //    /// 
    //    [Description("Nota de Débito")]
    //    NoteDebito = 0,
    //    /// <summary>
    //    /// Nota de Crédito
    //    /// </summary>
    //    /// 
    //    [Description("Nota de Crédito")]
    //    NoteCredito,
    //    /// <summary>
    //    /// Factura
    //    /// </summary>
    //    /// 
    //    [Description("Factura")]
    //    Invoice

    //}

    /// <summary>
    /// Enumerado que describe la Razón de la Nota / Enum that contains the Note Reasons
    /// </summary>
    public enum NoteReason
    {
        /// <summary>
        /// Reversa un Documento (Por el monto completo) / Voided Document (Memo for the complete Amount)
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        [Display(Name = "Reversar Documento")]
        [Description("Reversar Documento")]
        Reversa_documento,
        /// <summary>
        /// Corregir Texto del Documento (Corrección de Errores) / Amend the Document Text (Error Correction)
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        [Display(Name = "Corregir Texto Documento")]
        [Description("Corregir Texto Documento")]
        Corregir_Texto_Documento,
        /// <summary>
        /// Corregir Monto de Factura (Cambios a lineas de la factura) / Amend lines of the Document (Changes in the Invoice Lines)
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        [Display(Name = "Corregir Monto Factura")]
        [Description("Corregir Monto Factura")]
        Corregir_Monto_Factura,
        /// <summary>
        /// Referencia de Documento / Document Reference
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        [Display(Name = "Referencia documento")]
        [Description("Referencia documento")]
        Referencia_documento,
        /// <summary>
        /// Sustituye Comprobante Provisional / Replace Temporary Document
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        [Display(Name = "Sustituye comprobante provisional por contingencia.")]
        [Description("Sustituye comprobante provisional por contingencia.")]
        Sustituye_comprobante,
        /// <summary>
        /// Otros / Other
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("99")]
        [Display(Name = "Otros")]
        [Description("Otros")]
        Otros
    }

    /// <summary>
    /// Enumerado que Describe los Códigos de Moneda según Hacienda
    /// </summary>
    public enum NoteCodigoMoneda
    {
        /// <comentarios/>
        AED,

        /// <comentarios/>
        AFN,

        /// <comentarios/>
        ALL,

        /// <comentarios/>
        AMD,

        /// <comentarios/>
        ANG,

        /// <comentarios/>
        AOA,

        /// <comentarios/>
        ARS,

        /// <comentarios/>
        AUD,

        /// <comentarios/>
        AWG,

        /// <comentarios/>
        AZN,

        /// <comentarios/>
        BAM,

        /// <comentarios/>
        BBD,

        /// <comentarios/>
        BDT,

        /// <comentarios/>
        BGN,

        /// <comentarios/>
        BHD,

        /// <comentarios/>
        BIF,

        /// <comentarios/>
        BMD,

        /// <comentarios/>
        BND,

        /// <comentarios/>
        BOB,

        /// <comentarios/>
        BOV,

        /// <comentarios/>
        BRL,

        /// <comentarios/>
        BSD,

        /// <comentarios/>
        BTN,

        /// <comentarios/>
        BWP,

        /// <comentarios/>
        BYR,

        /// <comentarios/>
        BZD,

        /// <comentarios/>
        CAD,

        /// <comentarios/>
        CDF,

        /// <comentarios/>
        CHE,

        /// <comentarios/>
        CHF,

        /// <comentarios/>
        CHW,

        /// <comentarios/>
        CLF,

        /// <comentarios/>
        CLP,

        /// <comentarios/>
        CNY,

        /// <comentarios/>
        COP,

        /// <comentarios/>
        COU,

        /// <summary>
        /// Colones
        /// </summary>
        /// <comentarios />
        CRC,

        /// <comentarios/>
        CUC,

        /// <comentarios/>
        CUP,

        /// <comentarios/>
        CVE,

        /// <comentarios/>
        CZK,

        /// <comentarios/>
        DJF,

        /// <comentarios/>
        DKK,

        /// <comentarios/>
        DOP,

        /// <comentarios/>
        DZD,

        /// <comentarios/>
        EGP,

        /// <comentarios/>
        ERN,

        /// <comentarios/>
        ETB,

        /// <comentarios/>
        EUR,

        /// <comentarios/>
        FJD,

        /// <comentarios/>
        FKP,

        /// <comentarios/>
        GBP,

        /// <comentarios/>
        GEL,

        /// <comentarios/>
        GHS,

        /// <comentarios/>
        GIP,

        /// <comentarios/>
        GMD,

        /// <comentarios/>
        GNF,

        /// <comentarios/>
        GTQ,

        /// <comentarios/>
        GYD,

        /// <comentarios/>
        HKD,

        /// <comentarios/>
        HNL,

        /// <comentarios/>
        HRK,

        /// <comentarios/>
        HTG,

        /// <comentarios/>
        HUF,

        /// <comentarios/>
        IDR,

        /// <comentarios/>
        ILS,

        /// <comentarios/>
        INR,

        /// <comentarios/>
        IQD,

        /// <comentarios/>
        IRR,

        /// <comentarios/>
        ISK,

        /// <comentarios/>
        JMD,

        /// <comentarios/>
        JOD,

        /// <comentarios/>
        JPY,

        /// <comentarios/>
        KES,

        /// <comentarios/>
        KGS,

        /// <comentarios/>
        KHR,

        /// <comentarios/>
        KMF,

        /// <comentarios/>
        KPW,

        /// <comentarios/>
        KRW,

        /// <comentarios/>
        KWD,

        /// <comentarios/>
        KYD,

        /// <comentarios/>
        KZT,

        /// <comentarios/>
        LAK,

        /// <comentarios/>
        LBP,

        /// <comentarios/>
        LKR,

        /// <comentarios/>
        LRD,

        /// <comentarios/>
        LSL,

        /// <comentarios/>
        LYD,

        /// <comentarios/>
        MAD,

        /// <comentarios/>
        MDL,

        /// <comentarios/>
        MGA,

        /// <comentarios/>
        MKD,

        /// <comentarios/>
        MMK,

        /// <comentarios/>
        MNT,

        /// <comentarios/>
        MOP,

        /// <comentarios/>
        MRO,

        /// <comentarios/>
        MUR,

        /// <comentarios/>
        MVR,

        /// <comentarios/>
        MWK,

        /// <comentarios/>
        MXN,

        /// <comentarios/>
        MXV,

        /// <comentarios/>
        MYR,

        /// <comentarios/>
        MZN,

        /// <comentarios/>
        NAD,

        /// <comentarios/>
        NGN,

        /// <comentarios/>
        NIO,

        /// <comentarios/>
        NOK,

        /// <comentarios/>
        NPR,

        /// <comentarios/>
        NZD,

        /// <comentarios/>
        OMR,

        /// <comentarios/>
        PAB,

        /// <comentarios/>
        PEN,

        /// <comentarios/>
        PGK,

        /// <comentarios/>
        PHP,

        /// <comentarios/>
        PKR,

        /// <comentarios/>
        PLN,

        /// <comentarios/>
        PYG,

        /// <comentarios/>
        QAR,

        /// <comentarios/>
        RON,

        /// <comentarios/>
        RSD,

        /// <comentarios/>
        RUB,

        /// <comentarios/>
        RWF,

        /// <comentarios/>
        SAR,

        /// <comentarios/>
        SBD,

        /// <comentarios/>
        SCR,

        /// <comentarios/>
        SDG,

        /// <comentarios/>
        SEK,

        /// <comentarios/>
        SGD,

        /// <comentarios/>
        SHP,

        /// <comentarios/>
        SLL,

        /// <comentarios/>
        SOS,

        /// <comentarios/>
        SRD,

        /// <comentarios/>
        SSP,

        /// <comentarios/>
        STD,

        /// <comentarios/>
        SVC,

        /// <comentarios/>
        SYP,

        /// <comentarios/>
        SZL,

        /// <comentarios/>
        THB,

        /// <comentarios/>
        TJS,

        /// <comentarios/>
        TMT,

        /// <comentarios/>
        TND,

        /// <comentarios/>
        TOP,

        /// <comentarios/>
        TRY,

        /// <comentarios/>
        TTD,

        /// <comentarios/>
        TWD,

        /// <comentarios/>
        TZS,

        /// <comentarios/>
        UAH,

        /// <comentarios/>
        UGX,

        /// <comentarios/>
        USD,

        /// <comentarios/>
        USN,

        /// <comentarios/>
        UYI,

        /// <comentarios/>
        UYU,

        /// <comentarios/>
        UZS,

        /// <comentarios/>
        VEF,

        /// <comentarios/>
        VND,

        /// <comentarios/>
        VUV,

        /// <comentarios/>
        WST,

        /// <comentarios/>
        XAF,

        /// <comentarios/>
        XAG,

        /// <comentarios/>
        XAU,

        /// <comentarios/>
        XBA,

        /// <comentarios/>
        XBB,

        /// <comentarios/>
        XBC,

        /// <comentarios/>
        XBD,

        /// <comentarios/>
        XCD,

        /// <comentarios/>
        XDR,

        /// <comentarios/>
        XOF,

        /// <comentarios/>
        XPD,

        /// <comentarios/>
        XPF,

        /// <comentarios/>
        XPT,

        /// <comentarios/>
        XSU,

        /// <comentarios/>
        XTS,

        /// <comentarios/>
        XUA,

        /// <comentarios/>
        XXX,

        /// <comentarios/>
        YER,

        /// <comentarios/>
        ZAR,

        /// <comentarios/>
        ZMW,
        /// <comentarios/>
        ZWL,
    }

    //public enum VoucherSituation
    //{
    //    Normal,
    //    Contingencia,
    //    Sin_Internet
    //}
}
