using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using TicoPay.Clients;
using TicoPay.Inventory;
using TicoPay.Services;
using System.Xml.Serialization;
using System.Text;
using ZXing;
using ZXing.Common;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using TicoPay.MultiTenancy;
using TicoPay.Invoices.XSD;
using System.Configuration;
using System.Drawing.Imaging;
using TicoPay.Core.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using TicoPay.Common;
using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature.Parameters;
using System.Xml;
using Microsoft.Xades;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using TicoPay.ReportsSettings;
using TicoPay.Taxes;
using static TicoPay.MultiTenancy.Tenant;
using TicoPay.Core;
using TicoPay.Invoices.Ticket;
using TicoPay.Drawers;
using TicoPay.BranchOffices;
using System.Globalization;
using BCR;

namespace TicoPay.Invoices
{
    public class Invoice : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited, IComprobanteRecepcion
    {
        public const string facturaelectronica = "01";
        public const int _changeType = 1;
        private static Random random = new Random((int)DateTime.Now.Ticks);


        public virtual Tenant Tenant { get; protected set; }
        public int TenantId { get; set; }

        /// <summary>
        /// Gets or sets the Number. 
        /// </summary>
        [Index("IX_Number")]
        public int Number { get; set; }
        //[Index("IX_InvoiceNumber", IsUnique = true)]
        //public long InvoiceNumber { get; set; }
        /// <summary>
        /// Gets or sets the Alphanumeric
        /// </summary>
        [MaxLength(50)]
        public string Alphanumeric { get; set; }

        /// <summary>
        /// Gets or sets the Note. 
        /// </summary>
        /// 
        [MaxLength(500)]
        public string Note { get; set; }
        /// <summary>
        /// Gets or sets the Transaction string or Amount cash. 
        /// </summary>
        /// 
        // [MaxLength(50)]
        //public string Transaction { get; set; }
        /// <summary>
        /// Gets or sets the SubTotal. Sum of all the Lines without tax or discount of the invoice 
        /// </summary>
        public decimal SubTotal { get; set; }
        /// <summary>
        /// Gets or sets the Discount Percentaje. 
        /// </summary>
        public decimal DiscountPercentaje { get; set; }

        /// <summary>
        /// Gets or sets the Discount Percentaje. 
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the TotalTax. 
        /// </summary>
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Gets or sets the Total. 
        /// </summary>
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets the Balance. 
        /// </summary>
        [Index]
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the Due Date
        /// </summary>
        [Index]
        [Display(Name = "Fecha")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the Payment Date
        /// </summary>
        //public DateTime? PaymentDate { get;  set; }

        /// <summary>
        /// Gets or sets the Status. 
        /// </summary>
        [Index]
        public Status Status { get; set; }

        /// <summary>Gets or sets the ClientId. </summary>
        public Guid? ClientId { get; set; }

        /// <summary>Gets or sets the Client. </summary>
        public virtual Client Client { get; set; }

        public Guid? RegisterId { get; set; }

        public virtual Register Register { get; protected set; }

        /// <summary>Gets or sets the User Id. </summary>
        public Guid? UserId { get; set; }

        /// <summary>Gets or sets the User Name. </summary>
        public string UserName { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// Gets or sets the Currency Code.
        /// </summary>
        //public PaymetnMethodType PaymetnMethodType { get;  set; }

        //public virtual PaymentMethod PaymetnMethod { get; set; }
        //public Guid? PaymetnMethodId { get; set; }
        /// <summary>
        /// Clave foranea de condicion de venta
        /// </summary>
        //public virtual Tipos ConditionSales { get; set; }
        //public int TypeId { get; set; }
        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }
        /// <summary>
        /// Si la condicion de venta es otro se debe especificar
        /// </summary>
        [MaxLength(100)]
        public string OtherConditionSale { get; set; }
        /// <summary>
        /// Si la condicion de venta es a credito, se debe indicar a cuantos dias
        /// </summary>
        public int CreditTerm { get; set; }
        /// <summary>
        /// Clave foranea de moneda
        /// </summary>
        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }
        /// <summary>
        /// Estatus del comprobante electronico
        /// </summary>
        public VoucherSituation StatusVoucher { get; set; }
        /// <summary>
        /// Mensaje de respuesta de la administracion triutaria
        /// </summary>
        public string MessageTaxAdministration { get; set; }
        /// <summary>
        /// Mensaje de respuesta del receptor del comprobante
        /// </summary>
        public string MessageReceiver { get; set; }
        /// <summary>
        /// XML de la factura electronica
        /// </summary>
        public string ElectronicBill { get; set; }
        /// <summary>
        /// Representacion PDF factura Electronica
        public string ElectronicBillPDF { get; set; }
        /// </summary>
        //public byte[] ElectronicBillDocPDF { get; set; }
        ///// <summary>
        ///// Codigo QR
        ///// </summary>
        //public string QRCodeGenerator { get; set; }
        public byte[] QRCode { get; set; }
        /// <summary>
        /// Clave del comprobante
        /// </summary>
        [MaxLength(50)]
        public string VoucherKey { get; set; }
        /// <summary>
        /// Numero consecutivo
        /// </summary>
        /// 
        [MaxLength(20)]
        [Display(Name = "No. Factura")]
        public string ConsecutiveNumber { get; set; }
        public decimal ChangeType { get; set; }
        public decimal TotalServGravados { get; set; }
        public decimal TotalServExento { get; set; }
        public decimal TotalProductExento { get; set; }
        public decimal TotalProductGravado { get; set; }
        public decimal TotalGravado { get; set; }
        public decimal TotalExento { get; set; }
        public decimal SaleTotal { get; set; }
        public decimal NetaSale { get; set; }
        /// <summary>
        /// Indica si la factura fue enviada a Hacienda
        /// </summary>
        public bool SendInvoice { get; set; }

        /// <summary>
        /// Indica si el PDF fue grabado correctamente en Azure
        /// </summary>
        public bool SavedInvoiceOrTicketPDF { get; set; }

        /// <summary>
        /// Indica si el PDF fue grabado correctamente en Azure
        /// </summary>
        public bool SavedInvoiceOrTicketXML { get; set; }

        /// <summary>
        /// Se registra la excepcion al enviar a hacienda en caso de falla
        /// </summary>
        public string ResponseTribunetExepcion { get; set; }
        /// <summary>
        /// Estatus de la factura en hacienda
        /// </summary>
        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }
        /// <summary>Gets or sets the InvoiceLines. </summary>

        public virtual ICollection<InvoiceLine> InvoiceLines { get; protected set; }

        /// <summary>Gets or sets the PaymentInvoices. </summary>

        public virtual ICollection<PaymentInvoice> InvoicePaymentTypes { get; protected set; }

        /// <summary>Gets or sets the Credit and Debit Notes. </summary>

        public virtual ICollection<Note> Notes { get; protected set; }

        //public virtual ICollection<PaymentInvoice> PaymentInvoices { get; protected set; }

        /// <summary>Gets or sets the InvoiceHistoryStatuses. </summary>

        public virtual ICollection<InvoiceHistoryStatus> InvoiceHistoryStatuses { get; protected set; }

        [Display(Name = "Estado Recepción")]
        public bool IsInvoiceReceptionConfirmed { get; set; }

        public bool SmsEnviado { get; set; }

        /// <summary>
        /// Indica la fecha de vencimiento de la factura si es a crédito
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        public FirmType? TipoFirma { get; set; }

        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        public TypeDocumentInvoice TypeDocument { get; set; } = TypeDocumentInvoice.Invoice;


        public string ClientName { get; set; }
        
        public XSD.IdentificacionTypeTipo ClientIdentificationType { get; set; }
        /// <summary>
        /// Gets or sets the identification of your client. 
        /// </summary>

        [StringLength(20)]
        public string ClientIdentification { get; set; }

        public string ClientAddress { get; set; }

        public string ClientPhoneNumber { get; set; }

        public string ClientMobilNumber { get; set; }

        public string ClientEmail { get; set; }
        //public virtual ICollection<Moneda> Moneys { get; protected set; }

        /// <summary>Gets or sets the Drawer. </summary>
        public Guid? DrawerId { get; set; }

        /// <summary>Gets or sets the Drawer. </summary>
        public virtual Drawer Drawer { get; set; }

        public bool IsContingency { get; set; } = false;

        [MaxLength(50)]
        public string ConsecutiveNumberContingency { get; set; }

        [MaxLength(180)]
        public string ReasonContingency { get; set; }

        public DateTime? DateContingency { get; set; }


        public string GeneralObservation { get; set; }

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
        protected Invoice()
        {

        }

        public static Invoice Create(int tenantId, DateTime dueDate, Tenant tenant)
        {
            var entity = new Invoice {
                TenantId=tenantId,
                DueDate=dueDate
            };
            entity.InvoiceLines=new Collection<InvoiceLine>();
            return entity;
        }

        public static Invoice Create(int tenantId, string note, DateTime dueDate, Guid? clientId, Tenant tenant, FacturaElectronicaCondicionVenta ConditionSaleType,
            FacturaElectronicaResumenFacturaCodigoMoneda CodeMoney)
        {
            var entity = new Invoice
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Note = note,
                DueDate = dueDate,
                ClientId = clientId,
                Status = Status.Parked,
                ConditionSaleType = ConditionSaleType,
                CodigoMoneda = CodeMoney,
                ChangeType = _changeType,
                SavedInvoiceOrTicketXML = false,
                SavedInvoiceOrTicketPDF = false
            };
            entity.InvoiceLines = new Collection<InvoiceLine>();
            entity.InvoicePaymentTypes = new Collection<PaymentInvoice>();
            entity.Notes = new Collection<Note>();
            entity.InvoiceHistoryStatuses = new Collection<InvoiceHistoryStatus>();
            return entity;
        }

        public static Invoice Create(int tenantId, string note, DateTime dueDate, Guid? clientId, Tenant tenant, FacturaElectronicaCondicionVenta ConditionSaleType,
            FacturaElectronicaResumenFacturaCodigoMoneda CodeMoney, FirmType? firmType, TypeDocumentInvoice type, string externalReference = "N/A", string generalObservation = "")
        {
            var entity = new Invoice
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Note = note,
                DueDate = dueDate,
                ClientId = clientId,
                Status = Status.Parked,
                ConditionSaleType = ConditionSaleType,
                CodigoMoneda = CodeMoney,
                ChangeType = _changeType,
                TipoFirma = firmType,
                TypeDocument = type,
                ExternalReferenceNumber = externalReference,
                GeneralObservation = generalObservation,
                SavedInvoiceOrTicketXML = false,
                SavedInvoiceOrTicketPDF = false
            };
            entity.InvoiceLines = new Collection<InvoiceLine>();
            entity.InvoicePaymentTypes = new Collection<PaymentInvoice>();
            entity.Notes = new Collection<Note>();
            entity.InvoiceHistoryStatuses = new Collection<InvoiceHistoryStatus>();
            return entity;
        }


        public void AssignInvoiceLine(int tenantId, decimal pricePerUnit, decimal taxAmount, decimal discountpercentage, string descriptionDiscount, string note, string title, decimal qty, LineType ltype, Service service, Product product, Invoice invoice, int numberline, Tax tax, Guid? taxId,
            XSD.UnidadMedidaType unitMeasurement, string unitMeasurementOthers)
            {
            decimal precioUnit = GetValor(pricePerUnit);
            decimal cantidad = GetValor(qty);
            decimal descuento = GetValor(discountpercentage);
            decimal subTotal = GetValor(precioUnit * cantidad);
            decimal taxImp = tax.Rate/100;
            var invoiceLine = InvoiceLine.Create(tenantId, precioUnit, note, title, cantidad, ltype, service, product, invoice, numberline, descuento, descriptionDiscount,XSD.CodigoTypeTipo.Codigo_UsoInterno, tax, taxId, unitMeasurement, unitMeasurementOthers);
            var amountdisc = CalcularDescuento(subTotal, descuento);
            invoiceLine.SetLineSubtotal(amountdisc, subTotal);
            var impuesto = GetValor(invoiceLine.SubTotal * taxImp);
            invoiceLine.SetLineTax(impuesto);
            invoiceLine.SetLineLinetotal(invoiceLine.SubTotal, invoiceLine.TaxAmount);
            InvoiceLines.Add(invoiceLine);
        }

        private static decimal CalcularDescuento(decimal total, decimal descuento)
        {
            decimal porcentaje = descuento / 100;
            decimal result = GetValor(total * porcentaje);
            return result;
        }

        //public void AssignPaymentType(int tenantId, decimal amount, Invoice invoice, ExchangeRate exchangeRate, PaymentMethod paymentMethod, PaymentInvoiceType paymentInvoiceType, string reference)
        //{
        //    var paymentInvoice = PaymentInvoice.Create(tenantId, amount, invoice, exchangeRate, paymentMethod, paymentInvoiceType, reference);
        //    InvoicePaymentTypes.Add(paymentInvoice);
        //}

        //public void AssignNote(int tenantId, decimal amount, Invoice invoice, ExchangeRate exchangeRate, string reference, string currencyCode)
        //{
        //    var invoiceNote = Invoices.Note.Create(tenantId, amount, invoice, exchangeRate, reference, currencyCode);
        //    Notes.Add(invoiceNote);
        //}

        public void AssignInvoiceHistoryStatus(int tenantId, Status status, Invoice invoice)
        {
            var invoiceHistoryStatus = InvoiceHistoryStatus.Create(tenantId, status, invoice);
            InvoiceHistoryStatuses.Add(invoiceHistoryStatus);
        }

        //public void PayInvoiceCash()
        //{
        //    PaymetnMethodType = PaymetnMethodType.Cash;

        //}
        //public void PayInvoiceCreditCard()
        //{
        //    PaymetnMethodType = PaymetnMethodType.CreditCard;
        //}
        //public void PayInvoiceCheck()
        //{
        //    PaymetnMethodType = PaymetnMethodType.Check;
        //}
        //public void PayInvoiceDeposit()
        //{
        //    PaymetnMethodType = PaymetnMethodType.Deposit;
        //}


        public void SetInvoiceNumber(int number)
        {
            Number = number;
        }
        public void SetInvoiceTotalCalculate(decimal taxamount, decimal discounttotal, decimal totalgravadoservicio, decimal totalexentoservicio, decimal totalgravadoproducto, decimal totalexentoproducto)
        {
            TotalServGravados = GetValor(totalgravadoservicio);
            TotalServExento = GetValor(totalexentoservicio);
            TotalProductGravado = GetValor(totalgravadoproducto);
            TotalProductExento = GetValor(totalexentoproducto);
            TotalGravado = GetValor(totalgravadoservicio) + GetValor(totalgravadoproducto);
            TotalExento = GetValor(totalexentoservicio) + GetValor(totalexentoproducto);
            SaleTotal = GetValor(TotalExento) + GetValor(TotalGravado);
            DiscountAmount = GetValor(discounttotal);
            TotalTax = GetValor(taxamount);
            NetaSale = GetValor(SaleTotal) - GetValor(DiscountAmount);
            Total = GetValor(NetaSale) + GetValor(TotalTax);
            Balance = GetValor(Total);
        }

        public void SetInvoiceXML(Uri XML, Uri PDF)
        {
            //ElectronicBill = GetStringFromXMLFile(Path.Combine(WorkPaths.GetXmlPath(), voucherkey + ".xml"));
            //ElectronicBillDocPDF = File.ReadAllBytes(Path.Combine(WorkPaths.GetPdfPath(), voucherkey + ".pdf"));
            if (XML != null)
            {
                ElectronicBill = XML.ToString();
                SavedInvoiceOrTicketXML = true;
            }
            if (PDF != null)
            {
                ElectronicBillPDF = PDF.ToString();
                SavedInvoiceOrTicketPDF = true;
            }
        }


        public void CreatePDF(Invoice invoice, Client client, Tenant _tenant, List<FacturaElectronicaMedioPago> mediopago,  List<PaymentInvoice> listInfoPago, List<BranchOffice> infoBranchOffice, ReportSettings reportSettings)
        {
            GeneratePDF PdfInvoie = new GeneratePDF(reportSettings);

            try
            {

                PdfInvoie.CreatePDF(invoice, client, _tenant, mediopago, listInfoPago, infoBranchOffice);
            }
            catch (IOException)
            {
            }
        }

        public void CreatePDF(Invoice invoice, ReportSettings reportSettings)
        {
            GeneratePDF PdfInvoie = new GeneratePDF(reportSettings);

            try
            {
                PdfInvoie.CreatePDF(invoice, invoice.Client, invoice.Tenant, null, null, null);
            }
            catch (IOException)
            {
            }
        }
               
        public void CreateXML(Invoice invoice, Client client, Tenant _tenant, List<FacturaElectronicaMedioPago> mediopago, Certificate certified)
        {
           
            XmlSerializer serializer2 = new XmlSerializer(typeof(FacturaElectronica));
            XmlSerializer serializer3 = new XmlSerializer(typeof(TiqueteElectronico));
            
            string path = Path.Combine(WorkPaths.GetXmlPath(), invoice.VoucherKey + ".xml");

            if (!_tenant.ValidateHacienda)
            {
                path = Path.Combine(WorkPaths.GetXmlSignedPath(), invoice.VoucherKey + ".xml");
            }
           

            if (invoice.TypeDocument == TypeDocumentInvoice.Invoice)
            {
                // arma la clase para el XML
                FacturaElectronica item = CreateInvoiceToSerialize(invoice, client, _tenant, mediopago);
                if (!File.Exists(path))
                {
                    // Con este ajuste no coloca saltos de linea en la serializacion solicitado por Andres Schifter <andresschifter@gmail.com>
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = false;

                    XmlWriter writer = XmlWriter.Create(path, settings);
                    //TextWriter writer = new StreamWriter(path);
                    serializer2.Serialize(writer, item);
                    writer.Close();
                }
            }
            else
            {
                TiqueteElectronico item = CreateTicketToSerialize(invoice, client, _tenant, mediopago);
                if (!File.Exists(path))
                {
                    TextWriter writer = new StreamWriter(path);
                    serializer3.Serialize(writer, item);
                    writer.Close();
                }
            }
                
            if (_tenant.ValidateHacienda && certified != null)
            {
                //Guarda el certificado temporalmente
                string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + certified.FileName).ToLower();
                string certPath = Path.Combine(WorkPaths.GetCertifiedPath(), archivo);
                try
                {
                    File.WriteAllBytes(certPath, certified.CertifiedRoute);
                }
                catch (IOException)
                {
                }
                SignedXMLXADES2(Path.Combine(WorkPaths.GetXmlPath(), invoice.VoucherKey + ".xml"), Path.Combine(WorkPaths.GetXmlSignedPath(), invoice.VoucherKey + ".xml"), Path.Combine(certPath), certified.Password);
            }

        }

        public static FacturaElectronica CreateInvoiceToSerialize(Invoice invoice, Client client, Tenant _tenant, List<FacturaElectronicaMedioPago> mediopago)
        {
            string _codphoneClient = "506", _phoneClient = "", _codfaxClient = "506", _faxCliente = "", _codphoneTenant = "506", _phoneTenant = "", _faxTenant = "", _codfaxTenant = "506";

            decimal rate = _tenant.IsConvertUSD ? invoice.ChangeType : 1;

            FacturaElectronica item = new FacturaElectronica
            {
                Clave = invoice.VoucherKey,
                NumeroConsecutivo = invoice.ConsecutiveNumber,
                FechaEmision = invoice.DueDate,
                CondicionVenta = invoice.ConditionSaleType, // cuando es automatica se toma de Tenant
                PlazoCredito = invoice.CreditTerm.ToString(),
                ResumenFactura = new FacturaElectronicaResumenFactura
                {
                    CodigoMoneda = invoice.CodigoMoneda,  // cuando es automatica se toma de Tenant
                    CodigoMonedaSpecified = true,
                    TipoCambio = rate,
                    TipoCambioSpecified = true,
                    // TotalServGravados = Decimal.Round(invoice.TotalServGravados, 2),
                    TotalServGravadosSpecified = false,
                    TotalServExentos = ConvertCRCToUSDDecimal(invoice.TotalServExento, 1, _tenant,invoice,null),
                    TotalServExentosSpecified = true,

                    TotalMercanciasGravadasSpecified = false,
                    TotalMercanciasExentas = ConvertCRCToUSDDecimal(invoice.TotalProductExento, 1, _tenant, invoice, null),
                    TotalMercanciasExentasSpecified = true,
                    
                    //TotalGravado = Decimal.Round(invoice.TotalGravado, 2),
                    TotalGravadoSpecified = false,
                    TotalExento = ConvertCRCToUSDDecimal(invoice.TotalExento, 1, _tenant, invoice, null),
                    TotalExentoSpecified = true,
                    TotalVenta = ConvertCRCToUSDDecimal(invoice.SaleTotal, 1, _tenant, invoice, null),
                    TotalDescuentos = ConvertCRCToUSDDecimal(invoice.DiscountAmount, 1, _tenant, invoice, null),
                    TotalDescuentosSpecified = true,
                    TotalVentaNeta = ConvertCRCToUSDDecimal(invoice.NetaSale, 1, _tenant, invoice, null),
                    //TotalImpuesto = Decimal.Round(invoice.TotalTax, 2),
                    TotalImpuestoSpecified = false,
                    TotalComprobante = ConvertCRCToUSDDecimal(invoice.Total, 1, _tenant, invoice, null)
                },

                Normativa = new FacturaElectronicaNormativa { NumeroResolucion = ConfigurationManager.AppSettings["XML.NumeroResolucion"], FechaResolucion = ConfigurationManager.AppSettings["XML.FechaResolucion"] }
            };

            if (invoice.TotalTax > 0)
            {
                item.ResumenFactura.TotalImpuesto = ConvertCRCToUSDDecimal(invoice.TotalTax, 1, _tenant, invoice, null);
                item.ResumenFactura.TotalImpuestoSpecified = true;
                item.ResumenFactura.TotalServGravados = ConvertCRCToUSDDecimal(invoice.TotalServGravados, 1, _tenant, invoice, null);
                item.ResumenFactura.TotalServGravadosSpecified = true;
                item.ResumenFactura.TotalMercanciasGravadas = ConvertCRCToUSDDecimal(invoice.TotalProductGravado, 1, _tenant, invoice, null);
                item.ResumenFactura.TotalMercanciasGravadasSpecified = true;
                item.ResumenFactura.TotalGravado = ConvertCRCToUSDDecimal(invoice.TotalGravado, 1, _tenant, invoice, null);
                item.ResumenFactura.TotalGravadoSpecified = true;
            }
            #region pagos
            item.MedioPago = new List<FacturaElectronicaMedioPago>(); // cuando es automatica se envia efectivo
            if (mediopago != null && mediopago.Count > 0)
            {
                foreach (FacturaElectronicaMedioPago facturacion in mediopago)
                    item.MedioPago.Add(facturacion);
            }
            else if (invoice.InvoicePaymentTypes != null && invoice.InvoicePaymentTypes.Count > 0)
            {
                foreach (var payment in invoice.InvoicePaymentTypes)
                {
                    item.MedioPago.Add((XSD.FacturaElectronicaMedioPago)payment.Payment.PaymetnMethodType);
                }
            }
            else
            {
                item.MedioPago.Add(XSD.FacturaElectronicaMedioPago.Efectivo);
            }
            #endregion pagos

            // emisor
            #region emisor
            var emisor = new XSD.EmisorType();

            emisor.Nombre = _tenant.Name;
            emisor.NombreComercial = _tenant.BussinesName;
            emisor.Identificacion = new XSD.IdentificacionType { Tipo = _tenant.IdentificationType, Numero = _tenant.IdentificationNumber };
            emisor.NombreComercial = _tenant.ComercialName;
            emisor.Ubicacion = new XSD.UbicacionType
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
                emisor.Fax = new XSD.TelefonoType { CodigoPais = _codfaxTenant, NumTelefono = _faxTenant/*.PadLeft(20, '0')*/ };
            }

            if (!String.IsNullOrEmpty(_tenant.PhoneNumber))
            {
                _phoneTenant = _tenant.PhoneNumber.Substring(4, _tenant.PhoneNumber.Length - 4);
                _codphoneTenant = _tenant.PhoneNumber.Substring(0, 3);
                emisor.Telefono = new XSD.TelefonoType { CodigoPais = _codphoneTenant, NumTelefono = _phoneTenant/*.PadLeft(20, '0') */};
            }

            item.Emisor = emisor;
            #endregion emisor

            // receptor
            #region receptor
            if (client!=null)
            {
                if (!String.IsNullOrWhiteSpace(invoice.ClientName) || !String.IsNullOrWhiteSpace(invoice.ClientIdentification) || !String.IsNullOrWhiteSpace(invoice.ClientEmail) || !String.IsNullOrEmpty(invoice.ClientPhoneNumber))
                {
                    var receptor = new XSD.ReceptorType();
                    var lastName = client.LastName == "N/D" ? "" : " " + client.LastName;
                    var nombrereceptor = client.Name + lastName;
                    if (nombrereceptor.Length > 80)
                        nombrereceptor = nombrereceptor.Substring(0, 80);

                    receptor.Nombre = nombrereceptor;
                    receptor.Identificacion = client.IdentificationType == XSD.IdentificacionTypeTipo.NoAsiganda ? null : new XSD.IdentificacionType { Tipo = client.IdentificationType, Numero = client.Identification };

                    if (!String.IsNullOrEmpty(client.IdentificacionExtranjero))
                        receptor.IdentificacionExtranjero = client.IdentificacionExtranjero;

                    if ((client.NameComercial != null) && (client.NameComercial.Length > 80))
                        receptor.NombreComercial = client.NameComercial.Substring(0, 80);
                    else
                        receptor.NombreComercial = client.NameComercial;

                    if ((!String.IsNullOrEmpty(client.Address)) && (client.Barrio != null))
                    {
                        receptor.Ubicacion = new XSD.UbicacionType
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
                        receptor.Telefono = new XSD.TelefonoType { CodigoPais = _codphoneClient, NumTelefono = _phoneClient/*.PadLeft(20, '0')*/ };
                    }

                    if (!String.IsNullOrEmpty(client.Fax))
                    {
                        _faxCliente = client.Fax.Substring(4, client.Fax.Length - 4);
                        _codfaxClient = client.Fax.Substring(0, 3);
                        receptor.Fax = new XSD.TelefonoType { CodigoPais = _codfaxClient, NumTelefono = _faxCliente/*.PadLeft(20, '0')*/ };
                    }

                    if (!String.IsNullOrWhiteSpace(client.Email))
                        receptor.CorreoElectronico = client.Email;

                    item.Receptor = receptor;
                } 
            }
            #endregion receptor

            #region detalle
            var lineDetail = new FacturaElectronicaLineaDetalle[invoice.InvoiceLines.Count];
            int i = 0;
            foreach (var line in invoice.InvoiceLines)
            {
                var list = new FacturaElectronicaLineaDetalle();
                list.NumeroLinea = line.LineNumber.ToString();
                string codigo = line.ObtenerCodigo(line);
                list.Codigo = new XSD.CodigoType[] { new XSD.CodigoType { Tipo = line.CodeTypes, Codigo = codigo } }; // se corta a 20 el id porque es lo permirtido por hacienda
                list.Cantidad = line.Quantity;
                /// CAMBIAR ESTO
                list.UnidadMedida = line.UnitMeasurement;
                list.UnidadMedidaComercial = ((line.UnitMeasurementOthers != null && line.UnitMeasurementOthers.Length > 20) ? line.UnitMeasurementOthers.Substring(0, 20) : line.UnitMeasurementOthers); //line.Service.UnitMeasurementOthers;
                list.Detalle = line.Title;// verificar esto
                list.PrecioUnitario = ConvertCRCToUSDDecimal(line.PricePerUnit, 1, _tenant, invoice, null);
                list.MontoTotal = ConvertCRCToUSDDecimal(line.Total, 1, _tenant, invoice, null);

                if (line.DiscountPercentage > 0)
                {
                    //list.MontoDescuento = Decimal.Round(((line.Quantity * line.PricePerUnit) * line.DiscountPercentage) / 100, 2, MidpointRounding.AwayFromZero);
                    list.MontoDescuento = GetValor(((line.Quantity * line.PricePerUnit) * line.DiscountPercentage) / 100);
                    list.NaturalezaDescuento = line.DescriptionDiscount ?? "Descuento en servicio";
                    list.MontoDescuentoSpecified = true;
                }
                else
                    list.MontoDescuentoSpecified = false;

                list.SubTotal = ConvertCRCToUSDDecimal(line.SubTotal, 1, _tenant, invoice, null);
                // tipo de impuesto


                if (invoice.TotalTax > 0)
                    list.Impuesto = new XSD.ImpuestoType[] { new XSD.ImpuestoType {Codigo = line.Tax.TaxTypes, Tarifa = line.Tax.Rate,
               
               //list.Impuesto = new ImpuestoType[] { new ImpuestoType {Codigo = ImpuestoTypeCodigo.Exento, Tarifa = 0,

                Monto =  ConvertCRCToUSDDecimal(line.TaxAmount, 1, _tenant, invoice, null)}};
                //Exoneracion = new ExoneracionType {TipoDocumento = ExoneracionTypeTipoDocumento.Item01, }} 
                list.MontoTotalLinea = ConvertCRCToUSDDecimal(line.LineTotal, 1, _tenant, invoice, null);

                lineDetail[i] = list;
                i++;
            }
            item.DetalleServicio = lineDetail;
            #endregion detalle

            #region Contingencia
            if (invoice.IsContingency)
            {
                var contingencia = new XSD.FacturaElectronicaInformacionReferencia[] {
                new FacturaElectronicaInformacionReferencia {
                        Codigo = FacturaElectronicaInformacionReferenciaCodigo.Item05,
                        Numero = invoice.ConsecutiveNumberContingency,
                        Razon= invoice.ReasonContingency,
                        FechaEmision= Convert.ToDateTime(invoice.DateContingency),
                        TipoDoc=  FacturaElectronicaInformacionReferenciaTipoDoc.Item08
                    }
                };

                item.InformacionReferencia = contingencia;
            }
            
            #endregion Contigencia

            return item;
        }
        
        public static TiqueteElectronico CreateTicketToSerialize(Invoice invoice, Client client, Tenant _tenant, List<FacturaElectronicaMedioPago> mediopago)
        {
            string _codphoneClient = "506", _phoneClient = "", _codfaxClient = "506", _faxCliente = "", _codphoneTenant = "506", _phoneTenant = "", _faxTenant = "", _codfaxTenant = "506";

            decimal rate = _tenant.IsConvertUSD ? invoice.ChangeType : 1;

            TiqueteElectronico item = new TiqueteElectronico
            {
                Clave = invoice.VoucherKey,
                NumeroConsecutivo = invoice.ConsecutiveNumber,
                FechaEmision = invoice.DueDate,
                CondicionVenta = (TiqueteElectronicoCondicionVenta)invoice.ConditionSaleType, // cuando es automatica se toma de Tenant
                PlazoCredito = invoice.CreditTerm.ToString(),
                ResumenFactura = new TiqueteElectronicoResumenFactura
                {
                    CodigoMoneda = (TiqueteElectronicoResumenFacturaCodigoMoneda)invoice.CodigoMoneda,  
                    CodigoMonedaSpecified = true,
                    TipoCambio = rate,
                    TipoCambioSpecified = true,
                    TotalServGravados = ConvertCRCToUSDDecimal(invoice.TotalServGravados, 1, _tenant, invoice, null),
                    TotalServGravadosSpecified = true,
                    TotalServExentos = ConvertCRCToUSDDecimal(invoice.TotalServExento, 1, _tenant, invoice, null),
                    TotalServExentosSpecified = true,
                    TotalMercanciasExentasSpecified = false,
                    TotalMercanciasGravadas = ConvertCRCToUSDDecimal(invoice.TotalProductGravado, 1, _tenant, invoice, null),
                    TotalMercanciasGravadasSpecified = true,
                    TotalGravado = ConvertCRCToUSDDecimal(invoice.TotalGravado, 1, _tenant, invoice, null),
                    TotalGravadoSpecified = true,
                    TotalExento = ConvertCRCToUSDDecimal(invoice.TotalExento, 1, _tenant, invoice, null),
                    TotalExentoSpecified = true,
                    TotalVenta = ConvertCRCToUSDDecimal(invoice.SaleTotal, 1, _tenant, invoice, null),
                    TotalDescuentos = ConvertCRCToUSDDecimal(invoice.DiscountAmount, 1, _tenant, invoice, null),
                    TotalDescuentosSpecified = true,
                    TotalVentaNeta = ConvertCRCToUSDDecimal(invoice.NetaSale, 1, _tenant, invoice, null),
                    TotalImpuesto = ConvertCRCToUSDDecimal(invoice.TotalTax, 1, _tenant, invoice, null),
                    TotalImpuestoSpecified = true,
                    TotalComprobante = ConvertCRCToUSDDecimal(invoice.Total, 1, _tenant, invoice, null)
                },
                Normativa = new  TiqueteElectronicoNormativa { NumeroResolucion = ConfigurationManager.AppSettings["XML.NumeroResolucion"], FechaResolucion = ConfigurationManager.AppSettings["XML.FechaResolucion"] }
            };

            if (invoice.TotalTax > 0)
            {
                item.ResumenFactura.TotalImpuesto = Decimal.Round(invoice.TotalTax, 2);
                item.ResumenFactura.TotalImpuestoSpecified = true;
                item.ResumenFactura.TotalServGravados = Decimal.Round(invoice.TotalServGravados, 2);
                item.ResumenFactura.TotalServGravadosSpecified = true;
                item.ResumenFactura.TotalMercanciasGravadas = Decimal.Round(invoice.TotalProductGravado, 2);
                item.ResumenFactura.TotalMercanciasGravadasSpecified = true;
                item.ResumenFactura.TotalGravado = Decimal.Round(invoice.TotalGravado, 2);
                item.ResumenFactura.TotalGravadoSpecified = true;
            }

            #region pagos

            // item.MedioPago = new List<FacturaElectronicaMedioPago>(); // cuando es automatica se envia efectivo
            if (mediopago != null && mediopago.Count > 0)
            {
                var pagos = new TiqueteElectronicoMedioPago[mediopago.Count];
                var n = 0;
                foreach (TiqueteElectronicoMedioPago facturacion in mediopago)
                {
                    pagos[n] = facturacion;
                   n++;
                    //item.MedioPago.Add(facturacion);
                }
                item.MedioPago = pagos;
            }
            else {
                if (invoice.InvoicePaymentTypes != null && invoice.InvoicePaymentTypes.Count > 0)
                {
                    var pagos = new TiqueteElectronicoMedioPago[invoice.InvoicePaymentTypes.Count];
                    var n = 0;
                    foreach (var payment in invoice.InvoicePaymentTypes)
                    {
                        pagos[n] = (TiqueteElectronicoMedioPago)payment.Payment.PaymetnMethodType;
                        //item.MedioPago.Add((XSD.FacturaElectronicaMedioPago)payment.Payment.PaymetnMethodType);
                        n++;
                    }
                    item.MedioPago = pagos;
                }
                else
                {                    
                    item.MedioPago= new TiqueteElectronicoMedioPago[1] { TiqueteElectronicoMedioPago.Item01 };
                }
            }

            #endregion pagos

            // emisor
            #region emisor
            var emisor = new Ticket.EmisorType(); 
            emisor.Nombre = _tenant.Name;
            emisor.NombreComercial = _tenant.BussinesName;
            emisor.Identificacion = new TicoPay.Invoices.Ticket.IdentificacionType { Tipo = (Ticket.IdentificacionTypeTipo)_tenant.IdentificationType, Numero = _tenant.IdentificationNumber };
            emisor.NombreComercial = _tenant.ComercialName;
            emisor.Ubicacion = new TicoPay.Invoices.Ticket.UbicacionType
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
                emisor.Fax = new Ticket.TelefonoType { CodigoPais = _codfaxTenant, NumTelefono = _faxTenant/*.PadLeft(20, '0')*/ };
            }

            if (!String.IsNullOrEmpty(_tenant.PhoneNumber))
            {
                _phoneTenant = _tenant.PhoneNumber.Substring(4, _tenant.PhoneNumber.Length - 4);
                _codphoneTenant = _tenant.PhoneNumber.Substring(0, 3);
                emisor.Telefono = new Ticket.TelefonoType { CodigoPais = _codphoneTenant, NumTelefono = _phoneTenant/*.PadLeft(20, '0')*/ };
            }

            item.Emisor = emisor;
            #endregion emisor

            // receptor
            #region receptor

            if (!String.IsNullOrWhiteSpace(invoice.ClientName)|| !String.IsNullOrWhiteSpace(invoice.ClientIdentification)|| !String.IsNullOrWhiteSpace(invoice.ClientEmail)|| !String.IsNullOrEmpty(invoice.ClientPhoneNumber))
            {
                var receptor = new Ticket.ReceptorType();
                //var lastName = client.LastName == "N/D" ? "" : " " + client.LastName;
                var nombrereceptor = invoice.ClientName;
                if (!string.IsNullOrEmpty(nombrereceptor) && nombrereceptor.Length > 80)
                    nombrereceptor = nombrereceptor.Substring(0, 80);

                if (!String.IsNullOrWhiteSpace(invoice.ClientName))
                    receptor.Nombre = nombrereceptor;

                if (!String.IsNullOrWhiteSpace(invoice.ClientIdentification))
                {
                    receptor.Identificacion = (invoice.ClientIdentificationType == XSD.IdentificacionTypeTipo.NoAsiganda ? null : new Ticket.IdentificacionType { Tipo = (Ticket.IdentificacionTypeTipo)invoice.ClientIdentificationType, Numero = invoice.ClientIdentification });

                    if (invoice.ClientIdentificationType == XSD.IdentificacionTypeTipo.NoAsiganda)
                        receptor.IdentificacionExtranjero = invoice.ClientIdentification;

                }

                if ((client != null) && (!String.IsNullOrEmpty(client.Address)) && (client.Barrio != null))
                {
                    receptor.Ubicacion = new Ticket.UbicacionType
                    {
                        Provincia = client.Barrio.Distrito.Canton.Provincia.Id.ToString(),
                        Canton = client.Barrio.Distrito.Canton.codigocanton.PadLeft(2, '0'),
                        Distrito = client.Barrio.Distrito.codigodistrito.PadLeft(2, '0'),
                        Barrio = client.Barrio.codigobarrio.PadLeft(2, '0'),
                        OtrasSenas = client.Address
                    };
                }


                if (!String.IsNullOrEmpty(invoice.ClientPhoneNumber))
                {
                    _phoneClient = invoice.ClientPhoneNumber.Substring(4, invoice.ClientPhoneNumber.Length - 4);
                    _codphoneClient = invoice.ClientPhoneNumber.Substring(0, 3);
                    receptor.Telefono = new Ticket.TelefonoType { CodigoPais = _codphoneClient, NumTelefono = _phoneClient/*.PadLeft(20, '0')*/ };
                }

                if ((client != null) && (!String.IsNullOrEmpty(client.Fax)))
                {
                    _faxCliente = client.Fax.Substring(4, client.Fax.Length - 4);
                    _codfaxClient = client.Fax.Substring(0, 3);
                    receptor.Fax = new Ticket.TelefonoType { CodigoPais = _codfaxClient, NumTelefono = _faxCliente/*.PadLeft(20, '0')*/ };
                }

                if (!String.IsNullOrWhiteSpace(invoice.ClientEmail))
                    receptor.CorreoElectronico = invoice.ClientEmail;
                item.Receptor = receptor;
            }
           

            #endregion receptor

            #region detalle
            var lineDetail = new TiqueteElectronicoLineaDetalle[invoice.InvoiceLines.Count];
            int i = 0;
            foreach (var line in invoice.InvoiceLines)
            {
                var list = new TiqueteElectronicoLineaDetalle();
                list.NumeroLinea = line.LineNumber.ToString();
                string codigo = line.ObtenerCodigo(line);
                list.Codigo = new Ticket.CodigoType[] { new Ticket.CodigoType { Tipo = (Ticket.CodigoTypeTipo) line.CodeTypes, Codigo = codigo } }; // se corta a 20 el id porque es lo permirtido por hacienda
                list.Cantidad = line.Quantity;
                /// CAMBIAR ESTO
                list.UnidadMedida = (Ticket.UnidadMedidaType) line.UnitMeasurement;
                list.UnidadMedidaComercial = ((line.UnitMeasurementOthers != null && line.UnitMeasurementOthers.Length > 20) ? line.UnitMeasurementOthers.Substring(0, 20) : line.UnitMeasurementOthers); //line.Service.UnitMeasurementOthers;
                list.Detalle = line.Title;// verificar esto
                list.PrecioUnitario = line.PricePerUnit;
                list.MontoTotal = line.Total;

                if (line.DiscountPercentage > 0)
                {
                    list.MontoDescuento = Decimal.Round(((line.Quantity* line.PricePerUnit) * line.DiscountPercentage) / 100, 2, MidpointRounding.AwayFromZero);
                    list.NaturalezaDescuento = line.DescriptionDiscount ?? "Descuento en servicio";
                    list.MontoDescuentoSpecified = true;
                }
                else
                    list.MontoDescuentoSpecified = false;

                list.SubTotal = ConvertCRCToUSDDecimal(line.SubTotal, 1, _tenant, invoice, null);
                // tipo de impuesto
             
                list.Impuesto = new Ticket.ImpuestoType[] { new Ticket.ImpuestoType {Codigo = (Ticket.ImpuestoTypeCodigo) line.Tax.TaxTypes, Tarifa = line.Tax.Rate,
               //list.Impuesto = new ImpuestoType[] { new ImpuestoType {Codigo = ImpuestoTypeCodigo.Exento, Tarifa = 0,

                Monto =  ConvertCRCToUSDDecimal(line.TaxAmount,1, _tenant, invoice, null)}};
                //Exoneracion = new ExoneracionType {TipoDocumento = ExoneracionTypeTipoDocumento.Item01, }} 
                list.MontoTotalLinea = ConvertCRCToUSDDecimal(line.LineTotal, 1, _tenant, invoice, null);

                lineDetail[i] = list;
                i++;
            }
            item.DetalleServicio = lineDetail;
            #endregion detalle

            #region Contingencia
            if (invoice.IsContingency)
            {
                var contingencia = new TiqueteElectronicoInformacionReferencia [] {
                new TiqueteElectronicoInformacionReferencia {
                        Codigo = TiqueteElectronicoInformacionReferenciaCodigo.Item05,
                        Numero = invoice.ConsecutiveNumberContingency,
                        Razon= invoice.ReasonContingency,
                        FechaEmision= Convert.ToDateTime(invoice.DateContingency),
                        TipoDoc=  TiqueteElectronicoInformacionReferenciaTipoDoc.Item08
                    }
                };

                item.InformacionReferencia = contingencia;
            }

            #endregion Contigencia

            return item;
        }

        public static MemoryStream SerializeToStream(FacturaElectronica o)
        {
            var serializer = new XmlSerializer(typeof(FacturaElectronica));

            MemoryStream stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
            serializer.Serialize(stream, o);

            return stream;
        }

        public static Stream GetXML(FacturaElectronica invoice)
        {
            MemoryStream stream = SerializeToStream(invoice);
            return stream;
        }

        public void SignedXMLXADES2(string RutaXML, string RutaXMLSigned, string archivo, string password)
        {
            XadesService xadesService = new XadesService();
            SignatureParameters parametros = new SignatureParameters();

            //parametros.SignatureMethod = SignatureMethod.RSAwithSHA256;
            parametros.SigningDate = DateTime.Now;

            // Política de firma de factura-e 3.1
            parametros.SignaturePolicyInfo = new SignaturePolicyInfo();
            parametros.SignaturePolicyInfo.PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4.1/Resolucion_Comprobantes_Electronicos_DGT-R-48-2016.pdf";
            parametros.SignaturePolicyInfo.PolicyHash = "NmI5Njk1ZThkNzI0MmIzMGJmZDAyNDc4YjUwNzkzODM2NTBiOWUxNTBkMmI2YjgzYzZjM2I5NTZlNDQ4OWQzMQ==";
            parametros.SignaturePackaging = SignaturePackaging.ENVELOPED;
            parametros.InputMimeType = "text/xml";

            X509Certificate2 selectedCertificate = new X509Certificate2(@archivo, password, X509KeyStorageFlags.Exportable);

            //using (parametros.Signer = new Signer(CertsHelper.Load("TicoPay.Common." + archivo, password)))
            using (parametros.Signer = new Signer(selectedCertificate))
            {
                if (File.Exists(RutaXML))
                {
                    using (FileStream fs = new FileStream(@RutaXML, FileMode.Open))
                    {
                        var docFirmado = xadesService.Sign(fs, parametros);
                        docFirmado.Save(@RutaXMLSigned);
                    }

                }
            }

            // borra el certificado del directorio temporal
            FileInfo filecert = new FileInfo(archivo);
            filecert.Delete();
        }

        private static XmlDsigXPathTransform CreateXPathTransform(string xpath)
        {
            // create the XML that represents the transform
            XmlDocument doc = new XmlDocument();
            XmlElement xpathElem = doc.CreateElement("XPath");
            xpathElem.InnerText = xpath;

            XmlDsigXPathTransform xform = new XmlDsigXPathTransform();
            xform.LoadInnerXml(xpathElem.SelectNodes("."));

            return xform;
        }

        public void SignedXMLXADES(string RutaXML, string RutaXMLSigned)
        {
            // var selectedCertificate = GetSelectedCertificate(@"C:\Users\Usuario\Documents\TICOPAY\Facturacion Electronica\Firma Electronica\310174178825.p12", "4563");
            //Firmado digital con Microsoft Xades
            //var selectedCertificate = CertsHelper.Load("TicoPay.Common.310174178825.p12", "4563");

            CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");

            X509Certificate2 selectedCertificate = new X509Certificate2(@"C:\Users\Usuario\Documents\TICOPAY\newticopay\TicoPay.Core\Common\310174178825.p12", "4563", X509KeyStorageFlags.Exportable);
            var exportedKeyMaterial = selectedCertificate.PrivateKey.ToXmlString(true);
            var key = new RSACryptoServiceProvider(new CspParameters(24));
            key.PersistKeyInCsp = false;
            key.FromXmlString(exportedKeyMaterial);

            XmlDocument envelopedSignatureXmlDocument = new XmlDocument();

            envelopedSignatureXmlDocument.PreserveWhitespace = true;
            envelopedSignatureXmlDocument.Load(@RutaXML);

            SignedXml signedXml = new SignedXml(envelopedSignatureXmlDocument);
            signedXml.SigningKey = key;
            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

            //var root = envelopedSignatureXmlDocument.FirstChild;
            //root.Attributes.Remove(root.Attributes["xmlns:xsi"]);
            //root.Attributes.Remove(root.Attributes["xmlns:xsd"]);

            XadesSignedXml xadesSignedXml = new XadesSignedXml(envelopedSignatureXmlDocument);

            //xadesSignedXml.SignedInfo.SignatureMethod = SignatureMethod.RSAwithSHA256.URI;
            Reference reference;

            reference = new Reference();
            reference.Uri = "";
            XmlDsigC14NTransform xmlDsigC14NTransform = new XmlDsigC14NTransform();
            reference.AddTransform(xmlDsigC14NTransform);
            reference.AddTransform(CreateXPathTransform("ancestor-or-self::Signature"));
            //XmlDsigEnvelopedSignatureTransform xmlDsigEnvelopedSignatureTransform = new XmlDsigEnvelopedSignatureTransform();
            //reference.AddTransform(xmlDsigEnvelopedSignatureTransform);

            xadesSignedXml.AddReference(reference);

            RSACryptoServiceProvider rsaKey = (RSACryptoServiceProvider)selectedCertificate.PrivateKey;
            xadesSignedXml.SigningKey = rsaKey;

            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data((X509Certificate)selectedCertificate));
            keyInfo.AddClause(new RSAKeyValue(rsaKey));
            xadesSignedXml.KeyInfo = keyInfo;

            try
            {
                xadesSignedXml.Signature.Id = "SignatureId";
                XadesObject xadesObject = new XadesObject();
                xadesObject.Id = "XadesObject";
                xadesObject.QualifyingProperties.Target = "#" + "SignatureId";
                this.AddSignedSignatureProperties(
                    xadesObject.QualifyingProperties.SignedProperties.SignedSignatureProperties, selectedCertificate);

                xadesSignedXml.AddXadesObject(xadesObject);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            try
            {
                xadesSignedXml.ComputeSignature();
                xadesSignedXml.SignatureValueId = "SignatureValueId";

                envelopedSignatureXmlDocument.DocumentElement.AppendChild(envelopedSignatureXmlDocument.ImportNode(xadesSignedXml.GetXml(), true));

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.PreserveWhitespace = true; //Needed
                xmlDocument.LoadXml(envelopedSignatureXmlDocument.OuterXml);
                xmlDocument.Save(@RutaXMLSigned);
            }
            catch (Exception exception)
            {
                new Exception("Problem during signature computation (Did you select a valid certificate?): " + exception.Message);
            }
        }
        private void AddSignedSignatureProperties(SignedSignatureProperties signedSignatureProperties, X509Certificate2 selectedCertificate)
        {
            XmlDocument xmlDocument;
            Cert cert;

            xmlDocument = new XmlDocument();

            cert = new Cert();
            cert.IssuerSerial.X509IssuerName = selectedCertificate.IssuerName.Name;
            cert.IssuerSerial.X509SerialNumber = selectedCertificate.SerialNumber;
            cert.CertDigest.DigestMethod.Algorithm = SignedXml.XmlDsigSHA1Url;
            cert.CertDigest.DigestValue = selectedCertificate.GetCertHash();
            signedSignatureProperties.SigningCertificate.CertCollection.Add(cert);

            signedSignatureProperties.SigningTime = DateTime.Now;

            signedSignatureProperties.SignaturePolicyIdentifier.SignaturePolicyImplied = true;
        }

        //private X509Certificate2 GetSelectedCertificate(string certificatePath, string password)
        //{
        //    return (new X509Certificate2(certificatePath, password));
        //}

        public string GetStringFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        public void byteArrayToImage(byte[] byteArrayIn, string voucherkey)
        {
            byte[] bitmap = byteArrayIn;
            using (Image image = Image.FromStream(new MemoryStream(bitmap)))
            {
                image.Save(Path.Combine(WorkPaths.GetQRPath(), voucherkey + ".png"), ImageFormat.Png);
            }
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
        /// <summary>
        /// Genera el consecutivo exigido para la FE
        /// </summary>
        /// <param name="tenantRegisters"></param>
        public void SetInvoiceConsecutivo(Register tenantRegisters, Tenant tenant, bool incrementLastInvoiceNumber = true, Drawer drawer = null)
        {
            string consecutivo = null;
            if (TypeDocument == TypeDocumentInvoice.Invoice)
            {
                if (incrementLastInvoiceNumber)
                {
               
                    if (tenantRegisters.LastInvoiceNumber == 9999999999)
                        tenantRegisters.LastInvoiceNumber = 0;

                    tenantRegisters.LastInvoiceNumber = tenantRegisters.LastInvoiceNumber + 1;
                    consecutivo = tenantRegisters.LastInvoiceNumber.ToString();
               
               
               
                }
                else
                {
                    consecutivo = ((tenantRegisters.LastInvoiceNumber == 9999999999) ? 0 : tenantRegisters.LastInvoiceNumber + 1).ToString();
                }

                //ConsecutiveNumber = tenant.local.ToString() + tenantRegisters.RegisterCode.ToString() + facturaelectronica + consecutivo.PadLeft(10, '0');
            }
             else{

                if (incrementLastInvoiceNumber)
                {

                    if (tenantRegisters.LastTicketNumber == 9999999999)
                        tenantRegisters.LastTicketNumber = 0;

                    tenantRegisters.LastTicketNumber = tenantRegisters.LastTicketNumber + 1;
                    consecutivo = tenantRegisters.LastTicketNumber.ToString();
                }
                else
                {
                    consecutivo = ((tenantRegisters.LastTicketNumber == 9999999999) ? 0 : tenantRegisters.LastTicketNumber + 1).ToString();
                }

               
            }
            if (drawer==null)
                ConsecutiveNumber = tenant.local.ToString() + tenantRegisters.RegisterCode.ToString() + Convert.ToInt32(TypeDocument).ToString().PadLeft(2, '0') + consecutivo.PadLeft(10, '0');
            else
            {
                DrawerId = drawer.Id;
                ConsecutiveNumber = drawer.BranchOffice.Code.PadLeft(3, '0') + drawer.Code.PadLeft(5, '0') + Convert.ToInt32(TypeDocument).ToString().PadLeft(2, '0') + consecutivo.PadLeft(10, '0');
            }
                
            //var writer = new ZXing.Presentation.BarcodeWriter
            //{
            //    Format = BarcodeFormat.QR_CODE,
            //    //Options = new EncodingOptions
            //    //{
            //    //    PureBarcode = true,
            //    //    Height = 100,
            //    //    Width = 300
            //    //}
            //};

            //var image = writer.Write(ConsecutiveNumber);

            //QRCode = ConvertWriteableBitmapToByte(image);
        }

        public void SetInvoiceConsecutivo(Register tenantRegisters)
        {
            if (TypeDocument== TypeDocumentInvoice.Ticket) {
                if (tenantRegisters.LastTicketNumber == 9999999999)
                    tenantRegisters.LastTicketNumber = 0;

                tenantRegisters.LastTicketNumber = tenantRegisters.LastTicketNumber + 1;
            } else
            {
                if (tenantRegisters.LastInvoiceNumber == 9999999999)
                    tenantRegisters.LastInvoiceNumber = 0;

                tenantRegisters.LastInvoiceNumber = tenantRegisters.LastInvoiceNumber + 1;
            }
            
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

        /// <summary>
        /// Genera la clave numerica exigida para la FE
        /// </summary>
        /// <param name="tenantRegisters"></param>
        public void SetInvoiceNumberKey(DateTime fechafactura, string consecutive, Tenant tenant, int vouchersituation)
        {
            string dia = fechafactura.Day.ToString();
            string anio = fechafactura.Year.ToString().Substring(2, 2);
            string mes = fechafactura.Month.ToString();
            string securityCode = SecurityCode(8);

            VoucherKey = tenant.Country.CountryCode.ToString() + dia.PadLeft(2, '0') + mes.PadLeft(2, '0') + anio + tenant.IdentificationNumber.PadLeft(12, '0') + consecutive + vouchersituation.ToString() + securityCode;

            ReGenerateQrFromVoucherKey();
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

        //public void SetTransaction(string trans)
        //{
        //    Transaction = trans;
        //}

        //public void SetPaymentDate(DateTime date)
        //{
        //    PaymentDate = date;
        //}

        public void SetStatus(Status status)
        {
            Status = status;
        }

        public void SetUserNamePayment(string username)
        {
            UserName = username;
        }

        public string ConditionSaleTypeToString()
        {
            string result = string.Empty;
            switch (ConditionSaleType)
            {
                case FacturaElectronicaCondicionVenta.Contado:
                    result = "Contado";
                    break;
                case FacturaElectronicaCondicionVenta.Credito:
                    result = "Crédito";
                    break;
                case FacturaElectronicaCondicionVenta.Consignacion:
                    result = "Consignación";
                    break;
                case FacturaElectronicaCondicionVenta.Apartado:
                    result = "Apartado";
                    break;
                case FacturaElectronicaCondicionVenta.Arrendamiento_Opcion_de_Compra:
                    result = "Arrendamiento Opción de Compra";
                    break;
                case FacturaElectronicaCondicionVenta.Arrendamiento_Funcion_Financiera:
                    result = "Arrendamiento Función Financiera";
                    break;
                case FacturaElectronicaCondicionVenta.Otros:
                    result = "Otros";
                    break;
            }
            return result;
        }

        public decimal GetTotal()
        {
            decimal totalDebito = this.Notes.ToList().Where(i => i.NoteType == NoteType.Debito).Sum(i => i.Total);
            decimal totalCredito = this.Notes.ToList().Where(i => i.NoteType == NoteType.Credito).Sum(i => i.Total);
            decimal total = (Total + totalDebito) - totalCredito;
            return total;
        }

        public static decimal GetValor(decimal valorDecimal, int adicional = 0)
        {
            return Decimal.Round(valorDecimal, quantityDecimal + adicional, MidpointRounding.AwayFromZero);
        }
        

        public decimal GetRate()
        {
            RateOfDay _rateOfDay = new RateOfDay();
            return _rateOfDay.RateDate(DueDate).rate;
        }

        public static string ConvertCRCToUSD(decimal valor, decimal rate, Tenant tenant, Invoice invoice, FacturaElectronicaResumenFacturaCodigoMoneda? codigoMoneda)
        {
            string convert = "";
            if(RunConversion(tenant,invoice))
                switch (codigoMoneda)
                {
                    case FacturaElectronicaResumenFacturaCodigoMoneda.USD:
                        {
                            convert = ConvertUSD(valor, rate);
                            break;
                        }
                    case FacturaElectronicaResumenFacturaCodigoMoneda.CRC:
                        {
                            convert = ConvertCRC(valor, rate);
                            break;
                        }
                    default:
                        {
                            if (invoice.CodigoMoneda.Equals(FacturaElectronicaResumenFacturaCodigoMoneda.CRC))
                            {
                                convert = ConvertCRC(valor, 1);
                            }
                            else if (invoice.CodigoMoneda.Equals(FacturaElectronicaResumenFacturaCodigoMoneda.USD))
                            {
                                convert = ConvertCRC(valor, rate);
                            }
                            break;
                        }
                }
            else
            {
                convert = String.Format("{0:n2}", valor);
            }
            return convert;
        }

        private static decimal ConvertCRCToUSDDecimal(decimal valor, decimal rate, Tenant tenant, Invoice invoice,FacturaElectronicaResumenFacturaCodigoMoneda? codigoMoneda)
        {
            if (RunConversion(tenant, invoice) == true)
            {
                string rateString = ConvertCRCToUSD(valor, rate, tenant, invoice, codigoMoneda);
                return decimal.Parse(rateString);
            }
            else
            {
                return GetValor(valor);
            }                
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

        private static string CalcularMontosConver(string convert,decimal valorConvert)
        {
            if (valorConvert.ToString().Length > 1)
            {
                string convertUSDString = String.Format("{0:n7}", valorConvert);
                
                var position = convertUSDString.IndexOf('.');
                string stringEntero = convertUSDString.Substring(0, position);
                string stringDecimal = convertUSDString.ToString().Substring(position, 6);
                
                convert = stringEntero + stringDecimal;
            }
            else
            {
                convert = String.Format("{0:n5}", valorConvert);
            }


            return convert;
        }

        public static bool RunConversion(Tenant tenant, Invoice invoice)
        {
            if (tenant.IsConvertUSD)
            {
                return true;
            }
            return false;
        }

        public decimal TypeConversion(decimal valor,decimal tasa, FacturaElectronicaResumenFacturaCodigoMoneda monedaTenant)
        {
            decimal convertion = 0;
            string tipoConversion = string.Format("{0}-{1}",monedaTenant.ToString(),CodigoMoneda.ToString());
            switch (tipoConversion)
            {
                case "USD-CRC":
                    convertion = decimal.Parse(ConvertCRC(valor, tasa));
                    break;
                case "CRC-USD":
                    convertion = decimal.Parse(ConvertUSD(valor, tasa));
                    break;
                default:
                    convertion = valor;
                    break;
            }
            return convertion;
        }

    }

    /// <summary>
    /// Enumerado que describe el estatus de la factura en Ticopay / Enum that contains the Status of a Document in Ticopay
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Pagada / Payed
        /// </summary>
        [Description("Pagada")]
        Completed,
        /// <summary>
        /// Provisional / Lay by
        /// </summary>
        [Description("Provisional")]
        LayBy,
        /// <summary>
        /// Contabilizada / On Account
        /// </summary>
        [Description("Contabilizada")]
        OnAccount,
        /// <summary>
        /// Reversada / Returned
        /// </summary>
        [Description("Reversada")]
        Returned,
        /// <summary>
        /// Pendiente / Parked
        /// </summary>
        [Description("Pendiente")]
        Parked,
        /// <summary>
        /// Anulada / Voided
        /// </summary>
        [Description("Anulada")]
        Voided
    }

    /// <summary>
    /// Enumerado que describe los Medio de Pago de los Documentos / Enum that Contains the Payment Method Types
    /// </summary>
    public enum PaymetnMethodType
    {
        /// <summary>
        /// Efectivo / Cash
        /// </summary>
        [Description("Efectivo")]
        Cash,
        /// <summary>
        /// Tarjeta de DEbito o Crédito / Credit or Debit Card
        /// </summary>
        [Description("Tarjeta")]
        Card,
        /// <summary>
        /// Cheque / Check
        /// </summary>
        [Description("Cheque")]
        Check,
        /// <summary>
        /// Deposito o Transferencia / Deposit or Electronic Transfer
        /// </summary>
        [Description("Depósito/Transferencia")]
        Deposit,
        /// <summary>
        /// Nota de Crédito / Credit Memo
        /// </summary>
        [Description("Nota de Crédito")]
        PositiveBalance
    }

    /// <summary>
    /// Enumerado que contiene los tipos de documentos / Enum that contains the document type information
    /// </summary>
    public enum TypeDocumentInvoice
    {
        /// <summary>
        /// Factura / Invoice
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        [Description("Factura Electrónica")]
        Invoice =1,

        /// <summary>
        /// Nota de Débito / Debit Memo
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        [Description("Nota de Débito")]
        NotaDebito,

        /// <summary>
        /// Nota de Crédito / Credit Memo
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        [Description("Nota de Crédito")]
        NotaCredito,

        /// <summary>
        /// Tiquete / Ticket
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        [Description("Tiquete Electrónico")]
        Ticket
    }

    public enum VoucherSituation
    {
        Normal = 1,
        Contigency,
        withoutInternet
    }

    /// <summary>
    /// Enumerado que indica el estado de un documento con respecto a Hacienda / Enum that contains the State according to Hacienda
    /// </summary>
    public enum StatusTaxAdministration
    {
        /// <summary>
        /// Sin Enviar a Hacienda / Not Send or With Response
        /// </summary>
        [Description("Sin Respuesta")]
        NoEnviada = 0,
        /// <summary>
        /// Recibido por Hacienda / Received
        /// </summary>
        [Description("Recibido")]
        Recibido,
        /// <summary>
        /// Procesado por Hacienda / In Process
        /// </summary>
        [Description("Procesando")]
        Procesando,
        /// <summary>
        /// Aceptado por Hacienda / Accepted
        /// </summary>
        [Description("Aceptado")]
        Aceptado,
        /// <summary>
        /// Rechazado por Hacienda / Rejected
        /// </summary>
        [Description("Rechazado")]
        Rechazado,
        /// <summary>
        /// Error / Error
        /// </summary>
        [Description("Error")]
        Error
    }

    public enum StatusReception
    {
        Pendiente = 0,
        Confirmada

    }

    /// <summary>
    /// Enumerado que especifica los estados de la firma de los documentos / Enum that contains the Sing status
    /// </summary>
    public enum StatusFirmaDigital
    {
        /// <summary>
        /// Pendiente por firmar / Sing Pending
        /// </summary>
        [Description("Pendiente por firmar")]
        Pendiente = 0,
        /// <summary>
        /// Firmado / Signed
        /// </summary>
        [Description("Firmada")]
        Firmada,
        /// <summary>
        /// Error al firmar / Signing Error
        /// </summary>
        [Description("Error")]
        Error,
    }
}
