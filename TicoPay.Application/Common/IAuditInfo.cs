using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Common
{
    public interface IAuditInfo
    {
        string CreatorUserUserName { get; set; }
        string LastModifierUserName { get; set; }
        string DeleterUserName { get; set; }
    }
}
