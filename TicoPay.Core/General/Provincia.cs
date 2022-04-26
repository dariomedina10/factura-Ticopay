using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.General
{
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public class Provincia : Entity<int>
    {
        // public const int MaxNameLength = 50;

        [Required]
        [StringLength(50)]
        public string NombreProvincia { get; set; }

        public virtual ICollection<Canton> Cantones { get; protected set; }

        //protected Provincia()
        //{

        //}
    }
}
