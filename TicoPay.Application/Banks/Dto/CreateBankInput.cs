using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TicoPay.Common;
using TicoPay.General;
using TicoPay.Invoices.XSD;

namespace TicoPay.Banks.Dto
{
    /// <summary>
    /// Contiene los Datos para la Creación del Banco
    /// </summary>
    /// <seealso cref="TicoPay.Common.IDtoViewBaseFields" />
    public class CreateBankInput : IDtoViewBaseFields
    {
        /// <summary>
        /// Obtiene o Almacena el Nombre del Banco.
        /// </summary>
        /// <value>
        /// Nombre del Banco.
        /// </value>
        [Required]
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre corto del banco.
        /// </summary>
        /// <value>
        /// nombre corto.
        /// </value>
        [Required]
        public string ShortName { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int? ErrorCode { get; set; }
        
        /// <exclude />
        [JsonIgnore]
        public string ErrorDescription { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Action { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Control { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Query { get; set; }
    }
}
