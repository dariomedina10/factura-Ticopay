using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Users;


namespace TicoPay.Groups.Dto
{
    [AutoMapFrom(typeof(Group))]
    public class GroupDetailOutput : IDtoViewBaseFields
    {
        public Guid Id { get; set; }
        /// <summary>Gets or sets the Name. </summary>
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        /// <summary>Gets or sets the Code. </summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the Subscribers. </summary>
        public virtual ICollection<Client> Subscribers { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

        public int LastModifierUserId { get; set; }

        public int CreatorUserId { get; set; }

        public IList<User> Users { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
