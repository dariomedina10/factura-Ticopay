using Abp.Application.Editions;
using Abp.AutoMapper;
using Abp.MultiTenancy;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using TicoPay.Common;
using TicoPay.Editions;
using TicoPay.General;
using TicoPay.Invoices.XSD;
using TicoPay.Services.Dto;
using TicoPay.Users;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.MultiTenancy.Dto
{
    /// <summary>
    /// Clase que contiene los Datos del Tenant
    /// </summary>
    [AutoMapFrom(typeof(Tenant))]
    public class UpdateTenantInput : BaseServiceDTO, IDtoViewBaseFields, ICustomValidate
    {
        //public const int groupConditionSale = 2;
        /// <summary>
        /// Obtiene el ID del Tenant o Sub Dominio.
        /// </summary>
        /// <value>
        /// Obtiene el ID del Tenant o Sub Dominio.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene el Nombre del Tenant o Sub Dominio (Usado en el URL y la Autentificación).
        /// </summary>
        /// <value>
        /// Obtiene el Nombre del Tenant o Sub Dominio (Usado en el URL y la Autentificación).
        /// </value>
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(Tenant.TenancyNameRegex)]
        public string TenancyName { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Razón Social del Tenant.
        /// </summary>
        /// <value>
        /// Razón Social del Tenant.
        /// </value>
        [Required]
        [StringLength(Tenant.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre de Negocio del Tenant (Usualmente el mismo que ComercialName).
        /// </summary>
        /// <value>
        /// Nombre de Negocio del Tenant.
        /// </value>
        [MaxLength(80)]
        public string BussinesName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre Comercial del Tenant.
        /// </summary>
        /// <value>
        /// Nombre Comercial del Tenant.
        /// </value>
        [MaxLength(80)]
        public string ComercialName { get; set; }

        /// <summary>
        /// Obtiene el Tipo de Identificación del Tenant.
        /// </summary>
        /// <value>
        /// Tipo de Identificación del Tenant.
        /// </value>
        public IdentificacionTypeTipo IdentificationType { get; set; }

        /// <summary>
        /// Obtiene el Número de Identificación del Tenant.
        /// </summary>
        /// <value>
        /// Número de Identificación del Tenant
        /// </value>
        [MaxLength(12)]
        public string IdentificationNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Teléfono del Tenant.
        /// </summary>
        /// <value>
        /// Número de Teléfono del Tenant.
        /// </value>
        [StringLength(23)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Fax del Tenant.
        /// </summary>
        /// <value>
        /// Número de Fax del Tenant.
        /// </value>
        [StringLength(23)]
        public string Fax { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Email del Tenant.
        /// </summary>
        /// <value>
        /// Email del Tenant.
        /// </value>
        [StringLength(60)]
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Email Alternativo del Tenant.
        /// </summary>
        /// <value>
        /// Email Alternativo del Tenant.
        /// </value>
        [StringLength(60)]
        public string AlternativeEmail { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Moneda por Defecto del Tenant.
        /// </summary>
        /// <value>
        /// Moneda por Defecto del Tenant.
        /// </value>
        [Range(0, 177, ErrorMessage = "El tipo de moneda debe estar entre 0 y 177")]
        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }


        /// <summary>
        /// Obtiene o Almacena el Código de Moneda por Defecto del Tenant.
        /// </summary>
        /// <value>
        /// Código de Moneda por Defecto del Tenant.
        /// </value>
        public string currencyCode { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Barrio del Tenant.
        /// </summary>
        /// <value>
        /// Barrio del Tenant.
        /// </value>        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual Barrio Barrio { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de Barrio del Tenant.
        /// </summary>
        /// <value>
        /// ID de Barrio del Tenant.
        /// </value>
        [JsonIgnore]
        public int? BarrioId { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<Barrio> BarriosList { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<Provincia> Province { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int ProvinciaID { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int CantonID { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<Canton> Cantons { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<Distrito> Distritos { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int DistritoID { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Dirección del Tenant.
        /// </summary>
        /// <value>
        /// Dirección del Tenant.
        /// </value>
        [MaxLength(160)]
        public string Address { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual Country Country { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int? CountryID { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int EditionId { get; set; }

        /// <summary>
        /// Obtiene la lista de Planes que tiene el tenant.
        /// </summary>
        /// <value>
        /// Lista de planes que tiene el tenant.
        /// </value>
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<TicoPayEdition> EditionSelect { get; set; }

        /// <summary>
        /// Obtiene el Plan que tiene el Tenant.
        /// </summary>
        /// <value>
        /// Plan del Tenant.
        /// </value>
        public TicoPayEdition Edition { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de Registro del Tenant.
        /// </summary>
        /// <value>
        /// Id de Registro del Tenant.
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Guid RegisterID { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Factura
        /// </summary>
        /// <value>
        /// Ultimo Número de Factura
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long LastInvoiceNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Nota de Débito
        /// </summary>
        /// <value>
        /// Ultimo Número de Nota de Débito
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long LastNoteDebitNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Nota de Crédito
        /// </summary>
        /// <value>
        /// Ultimo Número de Nota de Crédito
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long LastNoteCreditNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Comprobante Electrónico
        /// </summary>
        /// <value>
        /// Ultimo Número de Comprobante Electrónico
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long LastVoucherNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Tiquete Electrónico
        /// </summary>
        /// <value>
        /// Ultimo Número de Tiquete Electrónico
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long LastTicketNumber { get; set; }

        //[Required]
        //[StringLength(User.MaxEmailAddressLength)]
        //public string AdminEmailAddress { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [MaxLength(AbpTenantBase.MaxConnectionStringLength)]
        public string ConnectionString { get; set; }

        /// <exclude />
        [JsonIgnore]
        [MaxLength(3)]
        public string local { get; set; }

        /// <exclude />
        [JsonIgnore]
        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }

        /// <exclude />
        [JsonIgnore]
        public SectorTenant? Sector { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Firma del Tenant.
        /// </summary>
        /// <value>
        /// Tipo de Firma del Tenant.
        /// </value>
        public FirmType? TipoFirma { get; set; }

        /// <exclude />
        [JsonIgnore]
        public FirmType? FirmaRecurrente { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<Country> CountrySelect { get; set; }

        /// <exclude />
        [JsonIgnore]
        public bool ValidateHacienda { get; set; }

        [JsonIgnore]
        public string UserTribunet { get; set; }

        [JsonIgnore]
        public string PasswordTribunet { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CertifiedID { get; set; }

        /// <exclude />
        [JsonIgnore]
        public HttpPostedFileBase file { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string CertifiedPath { get; set; }

        /// <exclude />
        [JsonIgnore]
        public byte[] CertifiedRoute { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string CertifiedPassword { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string FileName { get; set; }

        /// <exclude />
        [JsonIgnore]
        public bool CertifiedChange { get; set; }

        /// <exclude />
        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> FirmaRecurrentes
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = FirmType.Llave.ToString(), Text = "Llave Criptográfica" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = FirmType.Firma.ToString(), Text = "Firma Digital" });
                return list;
            }

        }

        /// <exclude />
        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> CodigoMonedas
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = FacturaElectronicaResumenFacturaCodigoMoneda.CRC.ToString(), Text = "CRC" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = FacturaElectronicaResumenFacturaCodigoMoneda.USD.ToString(), Text = "USD" });
                return list;
            }

        }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ErrorCode { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorDescription { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Control { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Query { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HttpPostedFileBase LogoFile { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] LogoData { get; set; }

        /// <exclude />
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LogoBase64String { get; set; }

        /// <exclude />
        [JsonIgnore]
        public bool? IsTutorialCompania { get; set; }

        /// <exclude />
        [JsonIgnore]
        public bool? IsTutorialServices { get; set; }

        /// <exclude />
        [JsonIgnore]
        public bool? IsTutorialClients { get; set; }

        /// <exclude />
        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> IdentificationTypes
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = IdentificacionTypeTipo.Cedula_Fisica.ToString(), Text = "Cédula Física" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = IdentificacionTypeTipo.Cedula_Juridica.ToString(), Text = "Cédula Jurídica" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = IdentificacionTypeTipo.DIMEX.ToString(), Text = "DIMEX" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = IdentificacionTypeTipo.NITE.ToString(), Text = "NITE" });
                return list;
            }

        }

        /// <exclude />
        [JsonIgnore]
        public bool SmsNoficicarFacturaACobro { get; set; }

        /// <exclude />
        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public decimal CostoSms { get; set; }

        /// <exclude />
        [JsonIgnore]
        public bool IsAddressShort { get; set; }

        /// <exclude />
        [JsonIgnore]
        [MaxLength(160)]
        public string AddressShort { get; set; }

        /// <exclude />
        [JsonIgnore]
        public bool DeleteLogo { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Unidad de Medida por Defecto del Tenant.
        /// </summary>
        /// <value>
        /// Unidad de Medida por Defecto del Tenant.
        /// </value>
        public UnidadMedidaType? UnitMeasurementDefault { get; set; } = UnidadMedidaType.Servicios_Profesionales;


        /// <summary>
        /// Obtiene o Almacena la Descripción de la Unidad de Medida Otros por Defecto del Tenant.
        /// </summary>
        /// <value>
        /// Descripción de la Unidad de Medida Otros por Defecto del Tenant.
        /// </value>
        public string UnitMeasurementOthersDefault { get; set; }

        /// <summary>
        /// Obtiene o almacena el indicador de punto de venta para el Tenant
        /// </summary>
        /// <value>
        /// Indica si el tenant funciona como punto de venta
        /// </value>
        public bool IsPos { get; set; }

        /// <summary>
        /// Obtiene o Almacena el tipo de impresora usada por el tenant en caso de ser punto de venta
        /// </summary>
        /// <value>Tipo de empresora utilizada por el tenanten caso de ser punto de venta </value>
        public PrinterTypes? PrinterType { get; set; }

        /// <summary>
        /// Indica si muestra el el codigo de servicio en el pdf de factura
        /// </summary>
        public bool ShowServiceCodePdf { get; set; }

        /// <exclude />
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (ProvinciaID <= 0)
                context.Results.Add(new ValidationResult("Debe seleccionar una Provincia", new string[] { "ProvinciaID" }));

            if (BarrioId <= 0)
                context.Results.Add(new ValidationResult("Debe seleccionar un Barrio", new string[] { "BarrioId" }));

            if (DistritoID <= 0)
                context.Results.Add(new ValidationResult("Debe seleccionar un Distrito", new string[] { "DistritoID" }));

            if (CantonID <= 0)
                context.Results.Add(new ValidationResult("Debe seleccionar un Canton", new string[] { "CantonID" }));

            // validacion de cedula fisica

            if ((IdentificationNumber.Length == 9) && (IdentificationType == IdentificacionTypeTipo.Cedula_Fisica))
            {

                if (IdentificationNumber.IndexOf("0") == 0)
                    context.Results.Add(new ValidationResult("El número de Cédula física, no debe tener un 0 al inicio", new string[] { "IdentificationNumber" }));

            }
            else
                if (((IdentificationNumber.Length > 9) || (IdentificationNumber.Length < 9)) && (IdentificationType == IdentificacionTypeTipo.Cedula_Fisica))
                context.Results.Add(new ValidationResult("El número de Cédula física, debe contener 9 dígitos", new string[] { "IdentificationNumber" }));

            // validacion de cedula juridica

            if (((IdentificationNumber.Length > 10) || (IdentificationNumber.Length < 10)) && (IdentificationType == IdentificacionTypeTipo.Cedula_Juridica))
                context.Results.Add(new ValidationResult("El número de cédula jurídica, debe contener 10 dígitos", new string[] { "IdentificationNumber" }));

            // validacion de DIMEX

            if (((IdentificationNumber.Length == 11) && (IdentificationType == IdentificacionTypeTipo.DIMEX)) || ((IdentificationNumber.Length == 12) && (IdentificationType == IdentificacionTypeTipo.DIMEX)))
            {
                if (IdentificationNumber.IndexOf("0") == 0)
                    context.Results.Add(new ValidationResult("El número DIMEX, no debe tener un 0 al inicio", new string[] { "IdentificationNumber" }));

            }
            else
                if (((IdentificationNumber.Length < 11) && (IdentificationType == IdentificacionTypeTipo.DIMEX)) || ((IdentificationNumber.Length > 12) && (IdentificationType == IdentificacionTypeTipo.DIMEX)))
                context.Results.Add(new ValidationResult("El número DIMEX, debe contener 11 0 12 dígitos", new string[] { "IdentificationNumber" }));

            // Validacion NITE
            if (((IdentificationNumber.Length < 10) || (IdentificationNumber.Length > 10)) && (IdentificationType == IdentificacionTypeTipo.NITE))
                context.Results.Add(new ValidationResult("El número NITE, debe contener 10 dígitos", new string[] { "IdentificationNumber" }));

            //Validacion de campos Certificado

            if (((CertifiedID == null) && (ValidateHacienda)) || ((CertifiedChange) && (ValidateHacienda)))
            {
                //if (CertifiedPath == null)
                //    context.Results.Add(new ValidationResult("Debe seleccionar un certificado", new string[] { "file" }));

                //if (CertifiedPassword == null)
                //    context.Results.Add(new ValidationResult("Debe ingresar la contraseña del certificado", new string[] { "CertifiedPassword" }));

                //if ((CertifiedPath != null) && (CertifiedPassword != null))
                //{
                //    bool valido = ValidateCertified(CertifiedPath, CertifiedPassword);

                //    //if (!ValidateCertified(CertifiedPath, CertifiedPassword))
                //    //context.Results.Add(new ValidationResult("El certificado no es valido o la contraseña no es correcta!", new string[] { "file" }));
                //}

            }
        }

        private X509Certificate2 GetSelectedCertificate(string certificatePath, string password)
        {
            return (new X509Certificate2(certificatePath, password));
        }

        /// <exclude />
        public bool ValidateCertified(string certificatePath, string password)
        {
            try
            {
                var selectedCertificate = GetSelectedCertificate(@certificatePath, password);

                X509Certificate2 objCert = selectedCertificate;
                X509Chain objChain = new X509Chain();

                //Verifico toda la cadena de revocación
                objChain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                objChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                //Timeout para las listas de revocación
                objChain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 30);

                //Verificar todo
                objChain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                //Se puede cambiar la fecha de verificación
                //objChain.ChainPolicy.VerificationTime = new DateTime(1999, 1, 1);
                objChain.Build(objCert);

                if (objChain.ChainStatus.Length != 0)
                {
                    string estatus = string.Empty;
                    foreach (X509ChainStatus objChainStatus in objChain.ChainStatus)
                        estatus = estatus + objChainStatus.Status.ToString() + " - " + objChainStatus.StatusInformation;

                    return false;

                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <exclude />
        [JsonIgnore]
        public bool IsFreeEdition { get; set; }

        [JsonIgnore]
        public bool IsConvertUSD { get; set; }

    }
}
