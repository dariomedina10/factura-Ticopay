using System;
using System.Collections.Generic;
using System.Text;
using TicoPayDll.Addresses.Dto;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Services;

namespace TicoPayDll.Tenants.Dto
{
    public class Tenant
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
        public string TenancyName { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Razón Social del Tenant.
        /// </summary>
        /// <value>
        /// Razón Social del Tenant.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre de Negocio del Tenant (Usualmente el mismo que ComercialName).
        /// </summary>
        /// <value>
        /// Nombre de Negocio del Tenant.
        /// </value>
        public string BussinesName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre Comercial del Tenant.
        /// </summary>
        /// <value>
        /// Nombre Comercial del Tenant.
        /// </value>
        public string ComercialName { get; set; }

        /// <summary>
        /// Obtiene el Plan que tiene el Tenant.
        /// </summary>
        /// <value>
        /// Plan del Tenant.
        /// </value>
        public TicoPayEdition Edition { get; set; }

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
        public string IdentificationNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Teléfono del Tenant.
        /// </summary>
        /// <value>
        /// Número de Teléfono del Tenant.
        /// </value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Fax del Tenant.
        /// </summary>
        /// <value>
        /// Número de Fax del Tenant.
        /// </value>
        public string Fax { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Email del Tenant.
        /// </summary>
        /// <value>
        /// Email del Tenant.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Email Alternativo del Tenant.
        /// </summary>
        /// <value>
        /// Email Alternativo del Tenant.
        /// </value>
        public string AlternativeEmail { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Moneda por Defecto del Tenant.
        /// </summary>
        /// <value>
        /// Moneda por Defecto del Tenant.
        /// </value>
        public CodigoMoneda CodigoMoneda { get; set; }                       

        /// <summary>
        /// Obtiene o Almacena el Barrio del Tenant.
        /// </summary>
        /// <value>
        /// Barrio del Tenant.
        /// </value>        
        public virtual Barrio Barrio { get; set; }                

        /// <summary>
        /// Obtiene o Almacena la Dirección del Tenant.
        /// </summary>
        /// <value>
        /// Dirección del Tenant.
        /// </value>
        public string Address { get; set; }
        
        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Factura
        /// </summary>
        /// <value>
        /// Ultimo Número de Factura
        /// </value>
        public long LastInvoiceNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Nota de Débito
        /// </summary>
        /// <value>
        /// Ultimo Número de Nota de Débito
        /// </value>
        public long LastNoteDebitNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Nota de Crédito
        /// </summary>
        /// <value>
        /// Ultimo Número de Nota de Crédito
        /// </value>
        public long LastNoteCreditNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Comprobante Electrónico
        /// </summary>
        /// <value>
        /// Ultimo Número de Comprobante Electrónico
        /// </value>
        public long LastVoucherNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Firma del Tenant.
        /// </summary>
        /// <value>
        /// Tipo de Firma del Tenant.
        /// </value>
        public FirmType? TipoFirma { get; set; }        

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
    }
}
