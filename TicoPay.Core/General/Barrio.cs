using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;

namespace TicoPay.General
{
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public class Barrio : Entity<int>
    {
        [Required]
        [StringLength(50)]
        public string NombreBarrio { get; set; }

        [Required]
        [StringLength(2)]
        public string codigobarrio { get; set; }

        [JsonIgnore]
        public virtual Distrito Distrito { get; set; }

        public int DistritoID { get; set; }

        [JsonIgnore]
        public virtual ICollection<Client> Clientes { get; protected set; }

    }
}
