using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Services
{
    public class UnitMeasurement:  Entity<int>
    {
        [MaxLength(10)]
        public string Symbol { get; set; }

        [MaxLength(60)]
        public string Description { get; set; }

        //public virtual ICollection<Service> Services { get; protected set; }

    }
}
