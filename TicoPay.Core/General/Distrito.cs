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
    public class Distrito : Entity<int>
    {
        [Required]
        [StringLength(50)]
        public string NombreDistrito { get; set; }

        [Required]
        [StringLength(2)]
        public string codigodistrito { get; set; }

        public virtual Canton Canton { get; set; }

        public int CantonID { get; set; }

        public virtual ICollection<Barrio> Barrios { get; protected set; }

    }
}
