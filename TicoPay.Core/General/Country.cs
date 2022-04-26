using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.General
{
    public class Country : Entity<int>
    {
        [Required]
        [StringLength(50)]
        public string CountryName { get; set; }

        [Required]
        [StringLength(3)]
        public string CountryCode { get; set; }

        [StringLength(13)]
        public string ResolutionNumber { get; set; }
        public DateTime ResolutionDate { get; set; }

    }
}
