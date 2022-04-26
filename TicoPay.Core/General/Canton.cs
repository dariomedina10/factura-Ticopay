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
    public class Canton : Entity<int>
    {
        [Required]
        [StringLength(50)]
        public string NombreCanton { get; set; }

        [Required]
        [StringLength(2)]
        public string codigocanton { get; set; }

        public virtual Provincia Provincia { get; set; }

        public int ProvinciaID { get; set; }

        public virtual ICollection<Distrito> Distrito { get; protected set; }

        //protected Canton()
        //{

        //}

    }
}
