using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;
using TicoPay.Services;

namespace TicoPay.Clients
{
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public class GroupConcepts : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }

        /// <summary>Gets or sets the Name. </summary>
        [MaxLength(60)]
        public string Name { get; set; }

        /// <summary>Gets or sets the descripcion del grupo. </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        public string CronExpression { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public virtual ICollection<ClientGroupConcept> Clients { get;  set; }

        public virtual ICollection<Service> Services { get; set; }
        
        public DateTime? WorkerFirstEjecutionDate { get; set; }

        public DateTime? WorkerLastEjecutionDate { get; set; }

        public DateTime? WorkerNextEjecutionDate { get; set; }

        public int? LastGeneratedInvoice { get; set; }

        public decimal Quantity { get; set; } = 1;

        public decimal DiscountPercentage { get; set; } = 0;

        public static GroupConcepts Create(int tenantId, string name, string description, string cronExpression)
        {
            var group = new GroupConcepts
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = name,
                Description = description,
                CronExpression = cronExpression,
                Services = new List<Service>()
            };
            return group;
        }

        public bool CanCreateInvoice()
        {
            bool cantCreateInvoice = true;
            if (string.IsNullOrWhiteSpace(CronExpression) || WorkerLastEjecutionDate == null)
            {
                cantCreateInvoice = true;
                return cantCreateInvoice;
            }
            var nextExecutionDate = new CronExpression(CronExpression).GetNextValidTimeAfter(WorkerLastEjecutionDate.Value).Value.LocalDateTime;
            if (nextExecutionDate != null)
            {
                WorkerNextEjecutionDate = nextExecutionDate;
                if (DateTimeZone.Now() >= WorkerNextEjecutionDate)
                {
                    cantCreateInvoice = true;
                }
            }
            if (DateTimeZone.Now() < WorkerNextEjecutionDate)
            {
                cantCreateInvoice = false;
            }
            return cantCreateInvoice;
        }

        public void SetNewEjecutionDates()
        {
            if (WorkerFirstEjecutionDate == null)
            {
                WorkerFirstEjecutionDate = DateTimeZone.Now();
            }
            WorkerLastEjecutionDate = DateTimeZone.Now();
            var nextExecutionDate = new CronExpression(CronExpression).GetNextValidTimeAfter(DateTimeZone.Now()).Value.LocalDateTime;
            if (nextExecutionDate != null)
            {
                WorkerNextEjecutionDate = nextExecutionDate;
            }

            
        }
    }
}
