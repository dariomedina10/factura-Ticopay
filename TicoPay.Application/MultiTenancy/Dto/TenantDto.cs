using Abp.AutoMapper;
using Abp.MultiTenancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices.XSD;

namespace TicoPay.MultiTenancy.Dto
{
    /// <summary>
    /// Clase que contiene la información actualizada del Tenant
    /// </summary>
    [AutoMapFrom(typeof(Tenant))]
    public class TenantDto
    {
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
        [Required]
        [MaxLength(80)]
        public string ComercialName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Identificación del Tenant.
        /// </summary>
        /// <value>
        /// Tipo de Identificación del Tenant.
        /// </value>
        [JsonIgnore]
        public IdentificacionTypeTipo IdentificationType { get; set; }

        /// <summary>
        /// Obtiene el Número de Identificación del Tenant.
        /// </summary>
        /// <value>
        /// Número de Identificación del Tenant
        /// </value>
        [JsonIgnore]
        [MaxLength(12)]
        public string IdentificationNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Teléfono del Tenant.
        /// </summary>
        /// <value>
        /// Número de Teléfono del Tenant.
        /// </value>
        [Required]
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
        [Required]
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
        [Required]
        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de Barrio del Tenant.
        /// </summary>
        /// <value>
        /// ID de Barrio del Tenant.
        /// </value>
        [Required]
        public int? BarrioId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Dirección del Tenant.
        /// </summary>
        /// <value>
        /// Dirección del Tenant.
        /// </value>
        [Required]
        [MaxLength(160)]
        public string Address { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de País del Tenant.
        /// </summary>
        /// <value>
        /// ID de País del Tenant.
        /// </value>
        [Required]
        public int? CountryID { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int EditionId { get; set; }

        /// <summary>
        /// Obtiene el Id de Registro del Tenant
        /// </summary>
        /// <value>
        /// Id de Registro del Tenant
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

        /// <exclude />
        [JsonIgnore]
        public bool SmsNoficicarFacturaACobro { get; set; }

        /// <exclude />
        [JsonIgnore]
        public decimal CostoSms { get; set; }

        /// <exclude />
        [JsonIgnore]
        public DateTime? LastPayNotificationSendedAt { get; set; }
    }
}
