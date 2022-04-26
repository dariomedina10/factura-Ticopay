using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace TicoPay.General
{
    public class RecordHistory : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public Guid ReferenceToRecordId { get; set; }

        public string ReferenceToTableName { get; set; }

        public Guid? UserId { get; set; }

        public string UserName { get; set; }
        
        public string Note { get; set; } 

        public ChangeType ChangeType { get; set; }

        public string OldValue { get; set; }
    }

    public enum ChangeType
    {
        Created,
        Updated,
        DeletedSoft,
        DeletedHard
    }
}
