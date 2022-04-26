using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.Entities
{
    /// <summary>
    /// Almacena la configuración del conector
    /// </summary>
    public class Configuracion
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Sub dominio o Tenant de Ticopay.
        /// </summary>
        /// <value>
        /// Sub dominio o Tenant.
        /// </value>
        public string SubDominioTicopay { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Usuario de Ticopay.
        /// </summary>
        /// <value>
        /// Usuario.
        /// </value>
        public string UsuarioTicopay { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Contraseña de Ticopay.
        /// </summary>
        /// <value>
        /// Contraseña.
        /// </value>
        public string ClaveTicopay { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de la Empresa a la que esta ligada la cuenta de Ticopay.
        /// </summary>
        /// <value>
        /// Id de la Empresa a la que esta ligada la cuenta de Ticopay.
        /// </value>
        public string IdEmpresa { get; set; }

        /// <summary>
        /// Obtiene o Almacena los Datos utilizados para conectarse al software externo (Cadena de Conexión u otros datos separados por ;).
        /// </summary>
        /// <value>
        /// Datos utilizados para conectarse al software externo.
        /// </value>
        public string DatosConexion { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de conector a utilizar.
        /// </summary>
        /// <value>
        /// Tipo de conector a utilizar.
        /// </value>
        public Conector TipoConector { get; set; }

        /// <summary>
        /// Obtiene o Almacena las Operaciones creadas con esa Configuración.
        /// </summary>
        /// <value>
        /// Operaciones creadas con esa Configuración.
        /// </value>
        public virtual ICollection<Operacion> Operaciones { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha de creación del Job.
        /// </summary>
        /// <value>
        /// Fecha de Creación.
        /// </value>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Inicia liza una nueva instancia de la clase <see cref="Configuracion"/> (Solo para Uso del Entity Framework).
        /// </summary>
        public Configuracion()
        {
            Id = Guid.NewGuid();
            Operaciones = new List<Operacion>();
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Inicia liza una nueva instancia de la clase <see cref="Configuracion"/>.
        /// </summary>
        /// <param name="subDominio">Sub dominio o Tenant.</param>
        /// <param name="usuario">Usuario.</param>
        /// <param name="clave">Contraseña.</param>
        /// <param name="empresa">ID de la Empresa.</param>
        /// <param name="datos">Datos utilizados para conectarse al software externo.</param>
        /// <param name="conector">Tipo de Conector.</param>
        public Configuracion(string subDominio, string usuario, string clave, string empresa, string datos, Conector conector)
        {
            Id = Guid.NewGuid();
            SubDominioTicopay = subDominio;
            UsuarioTicopay = usuario;
            ClaveTicopay = clave;
            IdEmpresa = empresa;
            DatosConexion = datos;
            TipoConector = conector;
            Operaciones = new List<Operacion>();
            FechaCreacion = DateTime.Now;
        }
    }
}
