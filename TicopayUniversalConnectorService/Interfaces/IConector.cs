using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Clients;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Taxes;
using TicopayUniversalConnectorService.Entities;

namespace TicopayUniversalConnectorService.Interfaces
{
    /// <summary>
    /// Interfaz que Especifica los métodos que deben ser implementados en los Jobs de los conectores para poder integrar el sistema a Ticopay
    /// </summary>
    /// <seealso cref="Quartz.IJob" />
    public interface IConector : IJob
    {
        /// <summary>
        /// Busca en Ticopay si el cliente existe y sino lo Crea.
        /// </summary>
        /// <param name="operacion">Información de la Operación a realizar.</param>
        /// <param name="configuracion">Configuración del conector a utilizar.</param>
        /// <returns></returns>
        Client IngresarCliente(Operacion operacion, Configuracion configuracion);

        /// <summary>
        /// Recopila los datos de la factura en el sistema a conectar y prepara la factura para ser ingresada en Ticopay.
        /// </summary>
        /// <param name="operacion">Información de la Operación a realizar.</param>
        /// <param name="configuracion">Configuración del conector a utilizar.</param>
        /// <returns></returns>
        CreateInvoice ElaborarFactura(Operacion operacion, Configuracion configuracion, Client cliente);

        /// <summary>
        /// Recopila los datos del tiquete en el sistema a conectar y prepara el tiquete para ser ingresado en Ticopay.
        /// </summary>
        /// <param name="operacion">Información de la Operación a realizar.</param>
        /// <param name="configuracion">Configuración del conector a utilizar.</param>
        /// <returns></returns>
        CreateInvoice ElaborarTiquete(Operacion operacion, Configuracion configuracion, Client cliente);


        /// <summary>
        /// Recopila los datos de la Devolución en el sistema a conectar y prepara la nota de crédito para ser ingresada en Ticopay.
        /// </summary>
        /// <param name="operacion">Información de la Operación a realizar.</param>
        /// <param name="configuracion">Configuración del conector a utilizar.</param>
        /// <returns></returns>
        CompleteNote ElaborarNotaCreditoDevolucion(Operacion operacion, Configuracion configuracion);


        /// <summary>
        /// Recopila los datos de la factura a reversar en el sistema a conectar y prepara la nota de crédito para ser ingresada en Ticopay.
        /// </summary>
        /// <param name="operacion">Información de la Operación a realizar.</param>
        /// <param name="configuracion">Configuración del conector a utilizar.</param>
        /// <returns></returns>
        CompleteNote ElaborarNotaCreditoReverso(Operacion operacion, Configuracion configuracion);


        /// <summary>
        /// Recopila los datos de la nota de Débito en el sistema a conectar y prepara la nota de débito para ser ingresada en Ticopay.
        /// </summary>
        /// <param name="operacion">Información de la Operación a realizar.</param>
        /// <param name="configuracion">Configuración del conector a utilizar.</param>
        /// <returns></returns>
        CompleteNote ElaborarNotaDebito(Operacion operacion, Configuracion configuracion);

        /// <summary>
        /// Revisa las operaciones a realizar, ejecuta los métodos para procesarlas, enviarlas y actualiza su estatus en el Servicio.
        /// </summary>
        /// <param name="operacionesPendientes">Lista de Operaciones pendientes.</param>
        /// <param name="configuracion">Configuración del conector a utilizar.</param>
        void ProcesarOperaciones(List<Operacion> operacionesPendientes, Configuracion configuracion);

        /// <summary>
        /// Busca un impuesto utilizado en las operaciones en Ticopay.
        /// </summary>
        /// <param name="operacion">Información de la Operación a realizar.</param>
        /// <param name="configuracion">Configuración del conector a utilizar.</param>
        /// <param name="porcentajeImpuesto">Porcentaje del impuesto a buscar.</param>
        /// <param name="nombreImpuesto">Nombre del Impuesto a buscar.</param>
        /// <returns></returns>
        Tax BuscarImpuesto(Operacion operacion, Configuracion configuracion, string porcentajeImpuesto = null, string nombreImpuesto = null);
    }
}
