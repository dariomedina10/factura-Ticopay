using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices;
using TicoPay.Common;

namespace TicoPay.Drawers.Dto
{
    [AutoMapFrom(typeof(Drawer))]
    public class UpdateDrawerInput : IDtoViewBaseFields
    {
        public Guid Id { get; set; }
        [Display(Name = "Código:")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Codigo Tiene que ser longitud 5")]
        public string Code { get; set; }
        /// <summary>
        /// descripción
        /// </summary>
        [Display(Name = "Descripción:")]
        public string Description { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }
        /// <summary>
        /// Id Sucursal
        /// </summary>
        [Display(Name = "Sucursal:")]
        public Guid BranchOfficeId { get; set; }
        /// <summary>
        /// Sucursal
        /// </summary>
        public BranchOffice BranchOffice { get; set; }

        [Display(Name = "Sucursal:")]
        public List<BranchOffice> BranchOffices { get; set; }

        public bool IsOpen { get; set; }
        public long? UserIdOpen { get; set; }
        public long? LastUserIdOpen { get; set; }
        public DateTime? OpenUserDate { get; set; }
        public DateTime? CloseUserDate { get; set; }
        // public virtual ICollection<DrawerUser> DrawerUsers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Guid RegisterID { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Factura
        /// </summary>
        /// <value>
        /// Ultimo Número de Factura
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Último #Factura:")]
        public long LastInvoiceNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Nota de Débito
        /// </summary>
        /// <value>
        /// Ultimo Número de Nota de Débito
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Último #Nota de Debito:")]
        public long LastNoteDebitNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Nota de Crédito
        /// </summary>
        /// <value>
        /// Ultimo Número de Nota de Crédito
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Último #Nota de Crédito:")]
        public long LastNoteCreditNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Comprobante Electrónico
        /// </summary>
        /// <value>
        /// Ultimo Número de Comprobante Electrónico
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Último #Comprobante de recepción:")]
        public long LastVoucherNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Ultimo Número de Tiquete Electrónico
        /// </summary>
        /// <value>
        /// Ultimo Número de Tiquete Electrónico
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Último #Tiquete:")]
        public long LastTicketNumber { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
