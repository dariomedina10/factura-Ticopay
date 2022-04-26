using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TicoPayDll.Clients
{
    public class Group
    {
        public Guid Id { get; set; }

        [MaxLength(60)]
        public string Name { get; set; }

        /// <summary>Gets or sets the Code. </summary>
        [MaxLength(1024)]
        public string Description { get; set; }        
    }
}
