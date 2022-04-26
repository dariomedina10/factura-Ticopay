using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;
using TicoPay.Users;

namespace TicoPay.BranchOffices.Dto
{
    [AutoMapFrom(typeof(BranchOffice))]
    public class BranchOfficesDetailOutput : IDtoViewBaseFields
    {
        public Guid Id { get; set; }
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        /// <summary>
        /// Código de sucursal
        /// </summary>
        [Display(Name = "Codigo")]
        public string Code { get; set; }
        /// <summary>
        /// Ubicacion de la sucursal
        /// </summary>
        [Display(Name = "Ubicacion")]
        public string Location { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }

        public int LastModifierUserId { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }
        /// <summary>
        /// cajas asocidas a la sucursal 
        /// </summary>

        public IList<User> Users { get; set; }

    

        public int CreatorUserId { get; set; }


        public int? ErrorCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ErrorDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Action { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Control { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Query { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
