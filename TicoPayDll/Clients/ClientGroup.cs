using System;
using System.Collections.Generic;
using System.Text;

namespace TicoPayDll.Clients
{
    public class ClientGroup
    {
        /// <summary>
        /// Obtiene o Almacena el Id de un Cliente.
        /// </summary>
        /// <value>
        /// Id de un Cliente.
        /// </value>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Lista de Grupos de un cliente.
        /// </summary>
        /// <value>
        /// Lista de Grupos de un cliente.
        /// </value>
        public IList<Group> Groups { get; set; }
    }
}
