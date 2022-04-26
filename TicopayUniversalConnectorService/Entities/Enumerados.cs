using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.Entities
{
    /// <summary>
    /// Tipo de Operación a Realizar en Ticopay
    /// </summary>
    public enum TipoOperacion
    {
        /// <summary>
        /// Factura Electrónica
        /// </summary>
        Factura,
        /// <summary>
        /// Ticket Electrónico
        /// </summary>
        Ticket,
        /// <summary>
        /// Nota de Crédito (Reversa de Factura o Ticket)
        /// </summary>
        Reverso,
        /// <summary>
        /// Nota de Débito
        /// </summary>
        NotaDebito,
        /// <summary>
        /// Nota de Crédito (Devolución factura o Ticket)
        /// </summary>
        DevolucionFactura,
        /// <summary>
        /// Pago de Factura
        /// </summary>
        PagoFactura,
        /// <summary>
        /// Factura de Contado (Para sistemas que separan la facturación)
        /// </summary>
        FacturaContado,
        /// <summary>
        /// Factura de Crédito (Para sistemas que separan la facturación)
        /// </summary>
        FacturaCredito,            
    }

    /// <summary>
    /// Estado de la Operación en el conector
    /// </summary>
    public enum Estado
    {
        /// <summary>
        /// Operación agregada al Conector pero que todavía no ha sido Procesada y enviada a Ticopay
        /// </summary>
        NoProcesado,
        /// <summary>
        /// Operación Procesada que ya fue enviada a Ticopay
        /// </summary>
        Procesado,
        /// <summary>
        /// Operación que sufrió un Error durante su procesamiento
        /// </summary>
        Error,
    }

    /// <summary>
    /// Tipo de Conector a Utilizar
    /// </summary>
    public enum Conector
    {
        /// <summary>
        /// Conector de ContaPyme (Version 4.7 A 47)
        /// </summary>
        Contapyme,
        /// <summary>
        /// Conector de Quickbooks Enterprise
        /// </summary>
        QuickbooksEnterprise,
        /// <summary>
        /// Conector De ticopay a Contapyme (Version 4.7 A 47)
        /// </summary>
        TicopayContapyme,
    }

    /// <summary>
    /// Tiempo en minutos para la Sincronizacion
    /// </summary>
    public enum PeriodoSincronizacion
    {
        Dos = 2,
        Tres = 3,
        Cuatro = 4,
        Cinco = 5,
        Diez = 10
    }
}
