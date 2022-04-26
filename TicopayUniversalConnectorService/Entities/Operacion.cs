using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.Entities
{
    /// <summary>
    /// Contiene los Datos de una operación registrada por el Conector Universal
    /// </summary>
    public class Operacion
    {
        /// <summary>
        /// Obtiene o Almacena el Id de la Operación.
        /// </summary>
        /// <value>
        /// Id de la Operación.
        /// </value>
        [Key]
        public Guid IdOperacion { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID del Documento (Operación) en el sistema a Conectar.
        /// </summary>
        /// <value>
        /// ID del Documento.
        /// </value>
        public string IdDocumento { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID del Cliente al que pertenece la operación en el sistema a Conectar.
        /// </summary>
        /// <value>
        /// ID del Cliente.
        /// </value>
        public string IdCliente { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de la Empresa (Correspondiente al Tenant o Sub Dominio) al que pertenece la operación en el sistema a Conectar.
        /// </summary>
        /// <value>
        /// ID de la Empresa.
        /// </value>
        public string IdEmpresa { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID del Documento afectado (N Factura) al que pertenece la operación en el sistema a Conectar.
        /// </summary>
        /// <value>
        /// ID del Documento afectado.
        /// </value>
        public string IdDocumentoAfectado { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID del Documento Procesado en Ticopay.
        /// </summary>
        /// <value>
        /// ID del Documento Procesado en Ticopay.
        /// </value>
        public string IdTicopayDocument { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Operación realizada o a realizar.
        /// </summary>
        /// <value>
        /// Tipo de Operación realizada.
        /// </value>
        public TipoOperacion TipoDeOperacion { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estado de la Operación realizada o a realizar.
        /// </summary>
        /// <value>
        /// Estado de la Operación realizada.
        /// </value>
        public Estado EstadoOperacion { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de operación Consecutivo del software a Conectar.
        /// </summary>
        /// <value>
        /// Numero de operación Consecutivo del software a Conectar.
        /// </value>
        public string ConsecutivoConector { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de Consecutivo del documento en Ticopay.
        /// </summary>
        /// <value>
        /// Numero de Consecutivo del documento en Ticopay.
        /// </value>
        public string ConsecutivoTicopay { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Voucher Key de Ticopay.
        /// </summary>
        /// <value>
        /// Voucher Key de Ticopay.
        /// </value>
        public string VoucherTicopay { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha y Hora de envió del Documento.
        /// </summary>
        /// <value>
        /// Fecha y Hora de envió del Documento.
        /// </value>
        public DateTime EnviadoEl { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha de registro de la operación.
        /// </summary>
        /// <value>
        /// Fecha de registro de la operación.
        /// </value>
        public DateTime Registrado { get; set; }

        /// <summary>
        /// Obtiene o Almacena el error si la operación no fue exitosa.
        /// </summary>
        /// <value>
        /// Error.
        /// </value>
        public string Errores { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Información de configuración a utilizar para realizar la operación.
        /// </summary>
        /// <value>
        /// Información de configuración.
        /// </value>
        public Configuracion Configuracion { get; set; }

        /// <summary>
        /// Inicia liza un instancia Básica de la clase <see cref="Operacion"/> (Requerida por Entity Framework).
        /// </summary>
        public Operacion()
        {
            IdOperacion = Guid.NewGuid();
        }

        /// <summary>
        /// Inicia liza una instancia de la clase <see cref="Operacion"/>.
        /// </summary>
        /// <param name="idDocumento">ID del Documento (Operación) en el sistema a Conectar.</param>
        /// <param name="idCliente"> ID del Cliente al que pertenece la operación en el sistema a Conectar.</param>
        /// <param name="idEmpresa">ID de la Empresa (Correspondiente al Tenant o Sub Dominio) al que pertenece la operación en el sistema a Conectar.</param>
        /// <param name="tipoOperacion">Tipo de Operación.</param>
        /// <param name="config">Información de configuración a utilizar para realizar la operación.</param>
        public Operacion(string idDocumento, string idCliente, string idEmpresa, TipoOperacion tipoOperacion, Configuracion config)
        {
            IdOperacion = Guid.NewGuid();
            IdDocumento = idDocumento;
            IdCliente = idCliente;
            IdEmpresa = idEmpresa;
            IdDocumentoAfectado = null;
            TipoDeOperacion = tipoOperacion;
            EstadoOperacion = Estado.NoProcesado;
            IdTicopayDocument = null;
            ConsecutivoConector = null;
            ConsecutivoTicopay = null;
            VoucherTicopay = null;
            Registrado = DateTime.Now;
            Configuracion = config;
        }

        /// <summary>
        /// Inicia liza una instancia de la clase <see cref="Operacion"/>, para Notas de Débito, Devoluciones y Reversos.
        /// </summary>
        /// <param name="idDocumento">ID del Documento (Operación) en el sistema a Conectar.</param>
        /// <param name="idCliente"> ID del Cliente al que pertenece la operación en el sistema a Conectar.</param>
        /// <param name="idEmpresa">ID de la Empresa (Correspondiente al Tenant o Sub Dominio) al que pertenece la operación en el sistema a Conectar.</param>
        /// <param name="tipoOperacion">Tipo de Operación.</param>
        /// <param name="config">Información de configuración a utilizar para realizar la operación.</param>
        /// <param name="idDocumentoAfectado">ID del Documento afectado (N Factura).</param>
        public Operacion(string idDocumento, string idCliente, string idEmpresa, TipoOperacion tipoOperacion, Configuracion config, string idDocumentoAfectado)
        {
            IdOperacion = Guid.NewGuid();
            IdDocumento = idDocumento;
            IdCliente = idCliente;
            IdEmpresa = idEmpresa;
            IdDocumentoAfectado = idDocumentoAfectado;
            TipoDeOperacion = tipoOperacion;
            EstadoOperacion = Estado.NoProcesado;
            IdTicopayDocument = null;
            ConsecutivoConector = null;
            ConsecutivoTicopay = null;
            VoucherTicopay = null;
            Registrado = DateTime.Now;
            Configuracion = config;
        }
    }
}
