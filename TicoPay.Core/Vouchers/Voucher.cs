using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TicoPay.Common;
using TicoPay.Core.Common;
using TicoPay.Drawers;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using TicoPay.MultiTenancy;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Vouchers
{
    public class Voucher : AuditedEntity<Guid>, IComprobanteRecepcion, IMustHaveTenant, IFullAudited
    {
        //public const string numbervoucher = "99";
        public const int MaxNameLength = 80;
        public const int MaxIdentificationLength = 12;
        public const int MaxIdentificationExtranjeroLength = 20;
        public const int MaxConsecutiveNumberLength = 20;
        public const int MaxKeyLength = 50;

        public virtual Tenant Tenant { get; protected set; }
        public int TenantId { get; set; }

        ///// <summary>
        ///// Gets or sets the identification of your client. 
        ///// </summary>
        //public IdentificacionTypeTipo IdentificationType { get; set; }

        /// <summary>
        /// Gets or sets the identification sender.
        /// </summary>
        /// <value>
        /// Número de identificación del Emisor del Documento.
        /// </value>
        [Required]
        [StringLength(MaxIdentificationLength)]
        [Display(Name = "No. Identificación")]
        public string IdentificationSender { get; set; }

        /// <summary>
        /// Gets or sets the name sender.
        /// </summary>
        /// <value>
        /// Nombre del Emisor del Documento.
        /// </value>
        [Required]
        [StringLength(MaxNameLength)]
        [Display(Name = "Nombre Emisor")]
        public string NameSender { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// Correo del Emisor del Documento.
        /// </value>
        [Display(Name = "Correo")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name receiver.
        /// </summary>
        /// <value>
        /// Nombre del Receptor del Documento.
        /// </value>
        [Display(Name = "Nombre Receptor")]
        public string NameReceiver { get; set; }

        /// <summary>
        /// Gets or sets the identification receiver.
        /// </summary>
        /// <value>
        /// Número de identificación del Receptor del Documento.
        /// </value>
        [Display(Name = "No. Identificación Receptor")]
        public string IdentificationReceiver { get; set; }

        [MaxLength(MaxConsecutiveNumberLength)]
        [Display(Name = "No. Factura")]
        public string ConsecutiveNumberInvoice { get; set; }

     
        [Display(Name = "Fecha Factura")]
        public DateTime DateInvoice { get; set; }

        [Display(Name = "Moneda")]
        public FacturaElectronicaResumenFacturaCodigoMoneda Coin { get; set; }

        [Required]
        [Display(Name = "Total Factura")]
        public decimal Totalinvoice { get; set; }

        [Display(Name = "Total Impuesto")]
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Clave del comprobante
        /// </summary>
        [MaxLength(MaxKeyLength)]
        [Display(Name = "Clave")]
        public string VoucherKey { get; set; }

        [Display(Name = "Clave Referido")]
        public string VoucherKeyRef { get; set; }
        /// <summary>
        /// Consecutivo de mensases de confirmación
        /// </summary>
        [MaxLength(MaxConsecutiveNumberLength)]
        [Display(Name = "No. Comprobante")]
        public string ConsecutiveNumber { get; set; }

        /// <summary>
        /// Código del mensaje de respuesta
        /// </summary>
        public MessageVoucher Message { get; set; }

        /// <summary>
        /// Detalle del mensaje
        /// </summary>
        [StringLength(MaxNameLength)]
        [Display(Name = "Detalle del Mensaje")]
        public string DetailsMessage { get; set; }
        /// <summary>
        /// XML del comprobante
        /// </summary>
        public string ElectronicBill { get; set; }
        /// <summary>
        /// Indica si el comprobante fue enviado a Hacienda
        /// </summary>
        public bool SendVoucher { get; set; }
        /// <summary>
        /// Estatus del comprobate en hacienda
        /// </summary>
        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }

        public VoucherSituation StatusVoucher { get; set; }

        public FirmType? TipoFirma { get; set; }

        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        /// <summary>Gets or sets the Drawer. </summary>
        public Guid? DrawerId { get; set; }

        /// <summary>Gets or sets the Drawer. </summary>
        public virtual Drawer Drawer { get; set; }
        /// <summary>
        /// Mensaje de respuesta de la administracion triutaria
        /// </summary>
        public string MessageTaxAdministration { get; set; }
        public long? DeleterUserId { get ; set ; }
        public DateTime? DeletionTime { get ; set; }
        public bool IsDeleted { get ; set ; }

        public TypeVoucher TypeVoucher { get; set; }
        public MessageSupplier? MessageSupplier { get; set; }
        /// <summary>
        /// Mensaje de respuesta del docuemnto de aceptacion del proveedor
        /// </summary>
        public string MessageTaxAdministrationSupplier { get; set; }

        // Retorna el Xml sin grabarlo en Azure
        public Stream GenerateXML(Voucher voucher, Tenant _tenant)
        {
            XmlSerializer serializer2 = new XmlSerializer(typeof(MensajeReceptor));
            string path = Path.Combine(WorkPaths.GetXmlPath(), "voucher_" + voucher.VoucherKey + ".xml");

            // arma la clase para el XML
            MensajeReceptor item = CreateVoucherToSerialize(voucher, _tenant);

            if (!_tenant.ValidateHacienda)
            {
                path = Path.Combine(WorkPaths.GetXmlSignedPath(), voucher.VoucherKey + ".xml");
            }

            if (!File.Exists(path))
            {
                var stream = new MemoryStream();
                TextWriter writer = new StreamWriter(stream);
                serializer2.Serialize(writer, item);                                
                stream.Position = 0;
                // writer.Close();
                return stream;
            }

            return null;
        }


        public void CreateXML(Voucher voucher, Tenant _tenant, Certificate certified)
        {
            XmlSerializer serializer2 = new XmlSerializer(typeof(MensajeReceptor));
            string path = Path.Combine(WorkPaths.GetXmlPath(), "voucher_" + voucher.VoucherKey + ".xml");

            // arma la clase para el XML
            MensajeReceptor item = CreateVoucherToSerialize(voucher, _tenant);

            if (!_tenant.ValidateHacienda)
            {
                path = Path.Combine(WorkPaths.GetXmlSignedPath(), voucher.VoucherKey + ".xml");
            }

            if (!File.Exists(path))
            {
                TextWriter writer = new StreamWriter(path);
                serializer2.Serialize(writer, item);
                writer.Close();
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
                SignedXMLXADES2(Path.Combine(WorkPaths.GetXmlPath(), "voucher_"+voucher.VoucherKey + ".xml"), Path.Combine(WorkPaths.GetXmlSignedPath(), "voucher_"+voucher.VoucherKey + ".xml"), Path.Combine(certPath), certified.Password);
            }

        }

        public static MensajeReceptor CreateVoucherToSerialize(Voucher voucher, Tenant _tenant)
        {
            //string _codphoneClient = "506", _phoneClient = "", _codfaxClient = "506", _faxCliente = "", _codphoneTenant = "506", _phoneTenant = "", _faxTenant = "", _codfaxTenant = "506";
            MensajeReceptor item = new MensajeReceptor
            {
                Clave = voucher.VoucherKeyRef,
                NumeroCedulaEmisor = voucher.IdentificationSender.PadLeft(12, '0'),
                FechaEmisionDoc = voucher.CreationTime, // la fecha d ela factura o d la confirmación?,
                Mensaje = (MensajeReceptorMensaje) voucher.Message, 
                DetalleMensaje = voucher.DetailsMessage,                
                MontoTotalImpuestoSpecified = false,
                //MontoTotalImpuesto = Decimal.Round(voucher.TotalTax,2),
                TotalFactura = Decimal.Round(voucher.Totalinvoice, 5),
                NumeroCedulaReceptor = voucher.IdentificationReceiver.PadLeft(12,'0'),
                NumeroConsecutivoReceptor= voucher.ConsecutiveNumber
                
            };
            if (voucher.TotalTax > 0)
            {
                item.MontoTotalImpuestoSpecified = true;
                item.MontoTotalImpuesto = Decimal.Round(voucher.TotalTax, 5);
            }
            return item;
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

        public static Voucher Create(int tenantId, string identificationSender, string nameSender, string email, string nameReceiver, string identificationReceiver, string consecutiveNumberInvoice, DateTime dateInvoice,
                    FacturaElectronicaResumenFacturaCodigoMoneda coin, decimal totalinvoice, decimal totalTax, MessageVoucher message,
                    string detailsMessage,  bool sendVoucher,  FirmType? tipoFirma, TypeVoucher typeVoucher, MessageSupplier? messageSupplier, string messageTaxAdministrationSupplier)
        {
            Voucher newvoucher = new Voucher() {
                TenantId=tenantId,
                IdentificationSender=identificationSender,
                NameSender=nameSender,
                Email=email,
                NameReceiver=nameReceiver,
                IdentificationReceiver=identificationReceiver,
                ConsecutiveNumberInvoice=consecutiveNumberInvoice,
                DateInvoice=dateInvoice,
                Coin=coin,
                Totalinvoice=totalinvoice,
                TotalTax=totalTax,
                Message=message,
                DetailsMessage=detailsMessage,
                SendVoucher=sendVoucher,
                StatusTribunet= StatusTaxAdministration.NoEnviada,
                TipoFirma= tipoFirma,
                TypeVoucher = typeVoucher,
                MessageSupplier = messageSupplier,
                MessageTaxAdministrationSupplier = messageTaxAdministrationSupplier
            };

            return newvoucher;
        }


        public void SetVoucherXML(Uri XML)
        {
            
            if (XML != null)
            {
                ElectronicBill = XML.ToString();
            }
            
        }

        public void SetVoucherConsecutivo(Register allRegisters, Tenant tenant, string numbervoucher, bool incrementLastInvoiceNumber = true, Drawer drawer=null)
        {
            string consecutivo = null;
            if (incrementLastInvoiceNumber)
            {
                if (allRegisters.LastVoucherNumber == 9999999999)
                    allRegisters.LastVoucherNumber = 0;

                allRegisters.LastVoucherNumber = allRegisters.LastVoucherNumber + 1;
                consecutivo = allRegisters.LastVoucherNumber.ToString();
            }
            else
            {
                consecutivo = ((allRegisters.LastVoucherNumber == 9999999999) ? 0 : allRegisters.LastVoucherNumber + 1).ToString();
            }

            if (drawer == null)
                ConsecutiveNumber = tenant.local.ToString() + allRegisters.RegisterCode.ToString() + numbervoucher + consecutivo.PadLeft(10, '0');
            else
                ConsecutiveNumber = drawer.BranchOffice.Code.PadLeft(3,'0') + drawer.Code.PadLeft(5,'0') + numbervoucher + consecutivo.PadLeft(10, '0');


        }

        public void SetInvoiceConsecutivo(Register tenantRegisters)
        {
            if (tenantRegisters.LastVoucherNumber == 9999999999)
                tenantRegisters.LastVoucherNumber = 0;

            tenantRegisters.LastVoucherNumber = tenantRegisters.LastVoucherNumber + 1;
        }

        /// <summary>
        /// Genera la clave numerica exigida para la FE
        /// </summary>
        /// <param name="tenantRegisters"></param>
        public void SetVoucherNumberKey(DateTime fechafactura, string consecutive, Tenant tenant, int vouchersituation)
        {
            string dia = fechafactura.Day.ToString();
            string anio = fechafactura.Year.ToString().Substring(2, 2);
            string mes = fechafactura.Month.ToString();
            string securityCode = SecurityCode(8);

            VoucherKey = tenant.Country.CountryCode.ToString() + dia.PadLeft(2, '0') + mes.PadLeft(2, '0') + anio + tenant.IdentificationNumber.PadLeft(12, '0') + consecutive + vouchersituation.ToString() + securityCode;
                        
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

        public static MemoryStream SerializeToStream(MensajeReceptor o)
        {
            var serializer = new XmlSerializer(typeof(MensajeReceptor));

            MemoryStream stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
            serializer.Serialize(stream, o);

            return stream;
        }

        public static Stream GetXML(MensajeReceptor voucher)
        {
            MemoryStream stream = SerializeToStream(voucher);
            return stream;
        }
    }

    /// <summary>
    /// Enumerado que contiene la respuesta al documento del comprobante electrónico / Enum that contains the Documents Responses
    /// </summary>
    public enum MessageVoucher
    {
        /// <summary>
        /// Documento Aceptado / Document Accepted
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        [Description("Aceptado")]
        Aceptado=0,
        /// <summary>
        /// Documento Parcialmente Aceptado / Document Partially Accepted
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        [Description("Aceptado Parcialmente")]
        AceptadoParcial,
        /// <summary>
        /// Documento Rechazado / Document Rejected
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        [Description("Rechazado")]
        Rechazado,
    }

    /// <summary>
    /// Enumerado que contiene la respuesta al documento del comprobante electrónico / Enum that contains the Documents Responses
    /// </summary>
    public enum MessageSupplier
    {
        /// <summary>
        /// Documento Aceptado / Document Accepted
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        [Description("Aceptado")]
        Aceptado = 0,
        /// <summary>
        /// Documento Rechazado / Document Rejected
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        [Description("Rechazado")]
        Rechazado
    }

    /// <summary>
    /// Enumerado que contiene los tipos de voucer / Enum that contains the voucher type information
    /// </summary>
    public enum TypeVoucher
    {
        /// <summary>
        /// Gastos / Purchases
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        [Description("Compras")]
        Purchases = 1,

        /// <summary>
        /// Gastos / Expenses
        /// </summary>
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        [Description("Gastos")]
        Expenses
    }

}
