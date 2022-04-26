using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices;

namespace TicoPay.Drawers.Dto
{
    /// <summary>
    /// Clase que contiene los datos de las Cajas en Ticopays / Class that contains the Drawer information in Ticopays
    /// </summary>
    [AutoMapFrom(typeof(Drawer))]
    public class DrawerDto
    {
        /// <summary>
        /// Obtiene el Id de la Caja / Gets the Drawer ID.
        /// </summary>
        /// <value>
        /// Id de la Caja / The Drawer ID.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Obtiene el Código de la Caja / Gets the Drawer Code.
        /// </summary>
        /// <value>
        /// Código de la Caja / The Drawer Code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Obtiene la Descripción de la Caja / Gets the Drawer Description
        /// </summary>
        /// <value>
        /// Descripción de la Caja / The Drawer Description.
        /// </value>
        public string Description { get; set; }

        [JsonIgnore]
        public string CodeBranchOffice { get; set; }

        [JsonIgnore]
        public long? DeleterUserId { get; set; }
        [JsonIgnore]
        public DateTime? DeletionTime { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene el Id de la Sucursal / Gets the Branch Office Id
        /// </summary>
        /// <value>
        /// Id de la Sucursal / The Branch Office Id.
        /// </value>
        public Guid BranchOfficeId { get; set; }

        /// <summary>
        /// Obtiene la Sucursal / Gets the Branch Office
        /// </summary>
        /// <value>
        /// La Sucursal / The Branch Office.
        /// </value>
        [JsonIgnore]
        public BranchOffice BranchOffice { get; set; }

        [JsonIgnore]
        public List<BranchOffice> BranchOffices { get; set; }

        /// <summary>
        /// Obtiene el Indicador si la caja esta Abierta / Gets the Status of the Drawer.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si esta abierta / If its Open; Sino / otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen { get; set; }

        /// <summary>
        /// Obtiene el Id del usuario que Aperturo la Caja / Gets the Id of the User that opened the Drawer.
        /// </summary>
        /// <value>
        /// Id del usuario que Aperturo la Caja / Id of the User that opened the Drawer.
        /// </value>
        public long? UserIdOpen { get; set; }

        [JsonIgnore]
        public long? LastUserIdOpen { get; set; }

        /// <summary>
        /// Obtiene la fecha de Apertura de la Caja / Gets the Date when the Drawer was Opened.
        /// </summary>
        /// <value>
        /// Fecha de Apertura de la Caja / Date when the Drawer was Opened.
        /// </value>
        public DateTime? OpenUserDate { get; set; }

        /// <summary>
        /// Obtiene la fecha de Cierre de la Caja / Gets the Date when the Drawer was closed.
        /// </summary>
        /// <value>
        /// Fecha de Cierre de la Caja / Date when the Drawer was closed.
        /// </value>
        public DateTime? CloseUserDate { get; set; }

        // public virtual ICollection<DrawerUser> DrawerUse
    }
}
