using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Services;
using TicoPay.Users;

namespace TicoPay.GroupConcept.Dto
{
    [AutoMapFrom(typeof(GroupConcepts))]
    public class GroupConceptsDetailOutput : IDtoViewBaseFields, IAuditInfo
    {
        public Guid Id { get; set; }
        /// <summary>Gets or sets the Name. </summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the Code. </summary>
        public string Description { get; set; }

        [Display(Name = "Servicios")]
        public ICollection<Service> Services { get; set; }

        [Display(Name = "Frecuencia de Facturación:")]
        public string CronExpression { get; internal set; }

        public List<string> ServiceIds
        {
            get
            {
                if(Services == null)
                {
                    return new List<string>();
                }
                return Services.Select(s => s.Id.ToString()).ToList();
            }
        }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

        public int LastModifierUserId { get; set; }

        public int CreatorUserId { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public string CreatorUserUserName { get; set; }
        public string LastModifierUserName { get; set; }
        public string DeleterUserName { get; set; }
    }
}
