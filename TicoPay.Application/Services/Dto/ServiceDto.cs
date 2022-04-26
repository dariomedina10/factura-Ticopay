using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using TicoPay.Taxes;
using Newtonsoft.Json;
using TicoPay.Invoices.XSD;
using System.ComponentModel;
using TicoPay.Taxes.Dto;
using TicoPay.Invoices;

namespace TicoPay.Services.Dto
{
    /// <summary>
    /// Contiene los Datos de los Servicios a facturar le al cliente / Contains all the Service information / Contains all the Service Information
    /// </summary>
    [AutoMapFrom(typeof(Service))]   
    public class ServiceDto : EntityDto<Guid>
    {
        /// <summary>
        /// Obtiene o Almacena el ID del Servicio / Gets the Service ID.
        /// </summary>
        /// <value>
        /// ID del Servicio Recurrente / Service ID.
        /// </value>
        public Guid? IdDetails { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre del Servicio / Gets or Sets the Service Name.
        /// </summary>
        /// <value>
        /// Nombre del Servicio Recurrente / Service Name.
        /// </value>
        [Required]
        [MaxLength(Service.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Precio del Servicio / Gets or Sets the Service Price.
        /// </summary>
        /// <value>
        /// Precio del Servicio Recurrente / Service Price.
        /// </value>
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "#.#,00")]
        public decimal Price { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID del Impuesto del Servicio / Gets or Sets the Tax Id of the Service.
        /// </summary>
        /// <value>
        /// Id del Impuesto del Servicio Recurrente / Tax Id of the Service.
        /// </value>
        [Required]
        public Guid? TaxId { get; set; }


        /// <summary>
        /// Obtiene o Almacena el Impuesto del Servicio / Gets or Sets the Tax.
        /// </summary>
        /// <value>
        /// Impuesto del Servicio Recurrente / Tax.
        /// </value>
        public virtual TaxDto Tax { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Unidad de medida del Servicio / Gets or Sets the Measurement Unit of the Service.
        /// </summary>
        /// <value>
        /// Unidad de medida del Servicio Recurrente / Measurement Unit of the Service.
        /// </value>
        [Required]
        [Range(0, 86, ErrorMessage = "El tipo de unidad de medida debe estar entre 0 y 86")]
        public UnidadMedidaType UnitMeasurement { get; set; }

        public string UnidadMedida { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la Unidad de media otros del Servicio / Gets or Sets the alternative Measurement Unit of the Service.
        /// </summary>
        /// <value>
        /// Descripción de la Unidad de medida otros / Alternative Measurement Unit of the Service.
        /// </value>
        public string UnitMeasurementOthers { get; set; }


        /// <summary>
        /// Obtiene o Almacena Si el Servicio es Recurrente / Gets or Sets if the Service is Scheduled .
        /// </summary>
        /// <value>
        /// <c>true</c> Si es recurrente ; sino, <c>false</c> / <c>true</c> If Scheduled ; If not, <c>false</c>.
        /// </value>
        public bool IsRecurrent { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Expresión Cron utilizada para la Planificación de la tarea (Debe ser Valida) / Gets or Sets a Cron expression to Schedule the Service invoice.
        /// </summary>
        /// <value>
        /// Expresión Cron utilizada para la Planificación de la tarea (Debe ser Valida) / Cron expression to Schedule the Service invoice.
        /// </value>
        public string CronExpression { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Cantidad del Servicio Recurrente / Gets or Sets the Scheduled Service Quantity (Minimum 1).
        /// </summary>
        /// <value>
        /// Cantidad del Servicio Recurrente / Scheduled Service Quantity.
        /// </value>
        [Range(1, 99999, ErrorMessage = "La cantidad debe estar entre 0 y 99999")]
        public decimal Quantity { get; set; }

        /// <exclude />
        [JsonIgnore]
        [Range(0, 100, ErrorMessage = "El porcentaje de descuento debe estar entre 0 y 100")]
        public decimal DiscountPercentage { get; set; }

        /// <exclude />
        [JsonIgnore]
        public LineType Tipo { get; set; } = LineType.Service;

    }

    
}
