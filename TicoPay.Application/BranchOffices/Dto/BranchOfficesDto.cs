using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Application.Services.Dto;
using TicoPay.Drawers;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TicoPay.BranchOffices.Dto
{
    [AutoMapFrom(typeof(BranchOffice))]
    public class BranchOfficesDto: EntityDto<Guid>
    {
        /// <summary>
        /// Obtiene el nombre de la Sucursal / Gets the Branch office Name.
        /// </summary>
        /// <value>
        /// Nombre de la Sucursal / Branch office Name.
        /// </value>
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        /// <summary>
        /// Obtiene el Código de sucursal / Gets the Branch Office Code
        /// </summary>
        /// <value>
        /// Código de sucursal / Branch Office Code.
        /// </value>
        [Display(Name = "Codigo")]
        public string Code { get; set; }
        /// <summary>
        /// Obtiene la Ubicación de la sucursal / Gets the Branch Office Location
        /// </summary>
        /// <value>
        /// Ubicación de la sucursal / Branch Office Location.
        /// </value>
        [Display(Name = "Ubicacion")]
        public string Location { get; set; }

        [JsonIgnore]
        public long? DeleterUserId { get; set; }
        [JsonIgnore]
        public DateTime? DeletionTime { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene las Cajas asociadas a la sucursal / Gets the Drawers that belong to a Branch Office
        /// </summary>
        /// <value>
        /// Cajas asociadas a la sucursal / Drawers that belong to a Branch Office.
        /// </value>
        [JsonIgnore]
        public virtual ICollection<Drawer> Drawers { get; protected set; }

    }
}
