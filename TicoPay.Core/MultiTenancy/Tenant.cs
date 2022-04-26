using Abp.MultiTenancy;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicoPay.General;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using TicoPay.Users;
using System;
using System.ComponentModel;
using TicoPay.Sellers;

namespace TicoPay.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public const int quantityDecimal = 2;

        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }
        [MaxLength(3)]
        public string local { get; set; }
        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }
        public virtual Country Country { get; set; }
        public int? CountryID { get; set; }
        [MaxLength(80)]
        public string BussinesName { get; set; }
        public IdentificacionTypeTipo IdentificationType { get; set; }

        [MaxLength(12)]
        public string IdentificationNumber { get; set; }

        [MaxLength(80)]
        public string ComercialName { get; set; }
        public virtual Barrio Barrio { get; set; }
        public int? BarrioId { get; set; }

        [MaxLength(160)]
        public string Address { get; set; }

        [StringLength(23)]
        public string PhoneNumber { get; set; }

        [StringLength(23)]
        public string Fax { get; set; }

        [StringLength(60)]
        public string Email { get; set; }

        [StringLength(60)]
        public string AlternativeEmail { get; set; }

        public bool ValidateHacienda { get; set; }

        [DefaultValue(7)]
        public int InvoicesMonthlyLimitNotificationInterval { get; set; }

        [DefaultValue(7)]
        public int NearInvoicesMonthlyLimitNotificationInterval { get; set; }

        public DateTime? LastInvoicesMonthlyLimitNotificationDate { get; set; }

        public DateTime? LastNearInvoicesMonthlyLimitNotificationDate { get; set; }

        public byte[] Logo { get; set; }

        //public virtual ICollection<Invoice> Invoices { get; protected set; }

        public virtual ICollection<Certificate> Certificates { get; protected set; }

        public bool GeneratesAutomaticClientCodeSetting { get; set; }

        public DateTime? LastPayNotificationSendedAt { get; set; }

        public virtual Seller Seller { get; set; }

        [DefaultValue(false)]
        public bool IsTutorialCompania { get; set; }

        [DefaultValue(false)]
        public bool IsTutotialServices { get; set; }

        [DefaultValue(false)]
        public bool IsTutorialClients { get; set; }

        [DefaultValue(false)]
        public bool IsTutorialProduct { get; set; }

        public bool SmsNoficicarFacturaACobro { get; set; }

        public decimal CostoSms { get; set; }

        public bool IsAddressShort { get; set; }

        [MaxLength(160)]
        public string AddressShort { get; set; }

        public SectorTenant? Sector { get; set; }

        public UnidadMedidaType? UnitMeasurementDefault { get; set; } = UnidadMedidaType.Servicios_Profesionales;

        public string UnitMeasurementOthersDefault { get; set; }

        public FirmType? TipoFirma { get; set; } // Los tipos de firma que soportara el tenant

        public FirmType? FirmaRecurrente { get; set; } // El tipo de firmado que utilizara en facturacion recurrente

        public string UserTribunet { get; set; }

        public string PasswordTribunet { get; set; }

        public bool IsConvertUSD { get; set; }
        
        public InactiveReason? MotiveSuspension { get; set; }


        public bool IsPos { get; set; }

        public PrinterTypes? PrinterType { get; set; }

        public bool ShowServiceCodePdf { get; set; }

        public Tenant()
        {

        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }

        public static Tenant Create(string tenancyname, string name, string cnn,
            FacturaElectronicaResumenFacturaCodigoMoneda coins, int? barrioId, string _local, FacturaElectronicaCondicionVenta conditionsale,
            int? countryID, string bussinesName, IdentificacionTypeTipo idenType,
            string idenNumber, string comercialName, string address, string phoneNumber, string fax, string email, int editionID, bool isTutorialClients
            , bool isTutorialCompania, bool isTutotialServices, bool isTutorialProduct)
        {
            var @tenant = new Tenant
            {
                TenancyName = tenancyname,
                Name = name,
                ConnectionString = cnn,
                CodigoMoneda = coins,
                BarrioId = barrioId,
                local = _local,
                ConditionSaleType = conditionsale,
                CountryID = countryID,
                BussinesName = bussinesName,
                IdentificationType = idenType,
                IdentificationNumber = idenNumber,
                ComercialName = comercialName,
                Address = address,
                PhoneNumber = phoneNumber,
                Fax = fax,
                Email = email,
                EditionId = editionID,
                IsTutorialClients = isTutorialClients,
                IsTutorialCompania = isTutorialCompania,
                IsTutotialServices = isTutotialServices,
                IsTutorialProduct = isTutorialProduct,
                IsConvertUSD = false
            };
            return @tenant;
        }

        public enum SectorTenant
        {
            [Display(Name = "Aeroespacial")]
            Aeroespacial,
            [Display(Name = "Agencia de Publicidad")]
            Publicidad,
            [Display(Name = "Agricola")]
            Agricola,
            [Display(Name = "Alfarería y cerámica‎")]
            Alfareria,
            [Display(Name = "Alimentacion")]
            Alimentacion,
            [Display(Name = "Anime y Manga")]
            Anime,
            [Display(Name = "Aseguradoras")]
            Aseguradoras,
            [Display(Name = "Asistencia Sanitaria")]
            Sanitaria,
            [Display(Name = "Bancos")]
            Bancos,
            [Display(Name = "Bienes Raíces")]
            Raices,
            [Display(Name = "Construccion")]
            Construccion,
            [Display(Name = "Cosmética")]
            Cosmetica,
            [Display(Name = "Cristalería")]
            Cristaleria,
            [Display(Name = "Defensa")]
            Defensa,
            [Display(Name = "Editoriales")]
            Editoriales,
            [Display(Name = "Electrodomésticos")]
            Electrodomesticos,
            [Display(Name = "Electrónica")]
            Electronica,
            [Display(Name = "Encuestadoras")]
            Encuestadoras,
            [Display(Name = "Energía")]
            Energia,
            [Display(Name = "Entretenimiento")]
            Entretenimiento,
            [Display(Name = "Envasado")]
            Envasado,
            [Display(Name = "Establecimientos Comerciales")]
            Comerciales,
            [Display(Name = "Fabricación")]
            Fabricacion,
            [Display(Name = "Fabricantes de Herramientas")]
            Herramientas,
            [Display(Name = "Fabricantes de instrumentos musicales‎")]
            Instrumentos,
            [Display(Name = "Farmacéutico")]
            Farmaceutico,
            [Display(Name = "Ferrocarriles")]
            Ferrocarriles,
            [Display(Name = "Forestales")]
            Forestales,
            [Display(Name = "Fotografía")]
            Fotografia,
            [Display(Name = "Franquicias")]
            Franquicias,
            [Display(Name = "Informática")]
            Informatica,
            [Display(Name = "Ingeniería")]
            Ingenieria,
            [Display(Name = "Joyería")]
            Joyeria,
            [Display(Name = "Logísitica")]
            Logisitica,
            [Display(Name = "Medios de Comunicación")]
            Comunicacion,
            [Display(Name = "Metalúrgicas")]
            Metalurgicas,
            [Display(Name = "Minería")]
            Mineria,
            [Display(Name = "Moda")]
            Moda,
            [Display(Name = "Otros")]
            Otros,
            [Display(Name = "Papeleras")]
            Papeleras,
            [Display(Name = "Químicas")]
            Quimicas,
            [Display(Name = "Relaciones Públicas")]
            Relaciones,
            [Display(Name = "Relojeras")]
            Relojeras,
            [Display(Name = "Sector y Continente")]
            Continente,
            [Display(Name = "Seguridad")]
            Seguridad,
            [Display(Name = "Servicios Financieros")]
            Financieros,
            [Display(Name = "Servicios Sanitarios")]
            Sanitarios,
            [Display(Name = "Siderúrgicas‎")]
            Siderurgicas‎,
            [Display(Name = "Tabacaleras")]
            Tabacaleras,
            [Display(Name = "Tecnológicas")]
            Tecnológicas,
            [Display(Name = "Telecomunicaciones")]
            Telecomunicaciones,
            [Display(Name = "Textiles")]
            Textiles,
            [Display(Name = "Turismo")]
            Turismo,
            [Display(Name = "Vinícolas")]
            Vinicolas
        }

        public void SetSectorTenant(SectorTenant sector)
        {
            Sector = sector;
        }

        public string IdentificationTypeToString()
        {
            string result = string.Empty;
            switch (IdentificationType)
            {
                case IdentificacionTypeTipo.Cedula_Fisica:
                    result = "Cédula Física";
                    break;
                case IdentificacionTypeTipo.Cedula_Juridica:
                    result = "Cédula Jurídica";
                    break;
                case IdentificacionTypeTipo.DIMEX:
                    result = "DIMEX";
                    break;
                case IdentificacionTypeTipo.NITE:
                    result = "NITE";
                    break;
            }
            return result;
        }

        public bool IsAddressCompleted()
        {
            var IsAddressCompleted = false;
            if (BarrioId != null && CountryID != null && Address != null)
            {
                IsAddressCompleted = true;
            }
            return IsAddressCompleted;
        }

        public void SetFirmType(FirmType firmType)
        {
            TipoFirma = firmType;
        }

        /// <summary>
        /// Enumerado que especifica los tipos de firma de los documentos / Enum that contains the Signature types
        /// </summary>
        public enum FirmType
        {
            /// <summary>
            /// Llave Criptográfica / Cryptographic Key
            /// </summary>
            [Display(Name = "Llave Criptográfica")]
            [Description("Llave Criptográfica")]
            Llave = 0,
            /// <summary>
            /// Firma Digital / Digital Signature
            /// </summary>
            [Display(Name = "Firma Digital")]
            [Description("Firma Digital")]
            Firma,
            /// <summary>
            /// Ambas (Solo aplica para la configuración del Tenant) / Not valid for Invoices or tickets (Only applies to Tenant configuration)
            /// </summary>
            [Display(Name = "Llave Criptográfica / Firma Digital")]
            [Description("Llave Criptográfica / Firma Digital")]
            Todos
        }

        public enum InactiveReason
        {
          

            [Display(Name = "Cuenta Suspendida")]
            [Description("Cuenta Suspendida")]

            inactive = 0,
            
            [Display(Name = "Posee Facturas vencidas pendientes")]
            [Description("Posee facturas vencidas pendientes")]
            InvoicePending,
           
        }

        public enum FeatureName
        {
            [Display(Name = "Limite de Usuarios")]
            [Description("Limite de Usuarios")]
            UsersLimit,


            [Display(Name = "Limite de Cajas")]
            [Description("Limite de Cajas")]
            DrawerLimit,

        }

        public enum PrinterTypes
        {
            
            [Display(Name = "Matriz de Punto")]
            [Description("Matriz de Punto")]
            MatrizPunto,
            [Display(Name = "Epson TM-U220")]
            [Description("Epson")]
            Epson,
            [Display(Name = "Epson TMT20II")]
            [Description("Epson TMT20II")]
            Epson_TMT20II,
            [Display(Name = "Zebra iMZ320")]
            [Description("Zebra iMZ320")]
            Zebra_iMZ320,
            [Display(Name = "Bematech LR200")]
            [Description("Bematech_LR200")]
            Bematech_LR200,
            [Display(Name = "BTP-R880NP")]
            [Description("BTP_R880NP")]
            BTP_R880NP,
            [Display(Name = "Zebra ZQ320")]
            [Description("Zebra_ZQ320")]
            Zebra_ZQ320,
            [Display(Name = "Xprinter XP-58iih")]
            [Description("XP_58iih")]
            XP_58iih,
            [Display(Name = "N3Star-PPT300BT")]
            [Description("N3Star_PPT300BT")]
            N3Star_PPT300BT,
            [Display(Name = "POS-5890c")]
            [Description("POS_5890c")]
            POS_5890c,
            [Display(Name = "POS-5805DD")]
            [Description("POS_5805DD")]
            POS_5805DD

        }
    }

}