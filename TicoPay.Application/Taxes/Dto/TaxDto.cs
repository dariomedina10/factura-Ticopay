using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicoPay.General;
using TicoPay.Invoices.XSD;
using TicoPay.Users;

namespace TicoPay.Taxes.Dto
{
    /// <summary>
    /// Clase que contiene la información de un Impuesto / Contains the Tax information
    /// </summary>
    [AutoMapFrom(typeof(Tax))]
    public class TaxDto : EntityDto<Guid>
    {
        //public Guid Id { get; set; }
        /// <summary>
        /// Obtiene o Almacena el Nombre del Impuesto / Gets or Sets the Tax Name.
        /// </summary>
        /// <value>
        /// Nombre del Impuesto / Tax Name.
        /// </value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Tasa del Impuesto (Porcentaje) / Gets or Sets the Tax Rate (Percentage).
        /// </summary>
        /// <value>
        /// Tasa del Impuesto / Tax Rate.
        /// </value>
        [Required]
        public decimal Rate { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Impuesto / Gets or Sets the Tax Type.
        /// </summary>
        /// <value>
        /// Tipo de Impuesto / Tax Type.
        /// </value>
        [Required]
        [Range(0, 12, ErrorMessage = "El tipo de impuesto debe estar entre 0 y 12")]
        public ImpuestoTypeCodigo TaxTypes { get; set; }

        /// <exclude />
        [JsonIgnore]
        public DateTime CreationTime { get; set; }

        /// <exclude />
        [JsonIgnore]
        public DateTime LastModificationTime { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int LastModifierUserId { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int CreatorUserId { get; set; }

        /// <exclude />
        [JsonIgnore]
        IList<User> Users { get; set; }
        

    }
}
