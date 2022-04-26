using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ticopay.BNModel.BNArchivoConciliacion
{
    /// <summary>
    /// Archivo de Pagos:  Archivo de Conciliación.
    /// </summary>
    public class ArchivoConciliacion : BnConectividadConciliacion
    {

        /// <summary>
        /// Indicador del tipo de registro = 1 Indica descripción de un pago. Tamaño 2
        /// </summary>
        public int TipoRegistro { get; set; }

        /// <summary>
        /// Código de la empresa (ASADA) al que pertenece el convenio. Tamaño = 3
        /// </summary>
        public int CodigoEmpresa { get; set; }

        /// <summary>
        /// Código de convenio del archivo batch. Tamaño = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// Código del ente recaudador hacia el cual es dirigido el archivo. Tamaño = 4
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// Agencia o canal de recaudación desde el cual se realizó el pago. Tamaño = 4
        /// </summary>
        public int CodigoAgencia { get; set; }

        /// <summary>
        /// Número de factura asignado al pago por la empresa externa. Tamaño 20
        /// </summary>
        public int NumeroFactura { get; set; }

        /// <summary>
        ///Self del número de factura. Tamaño = 1
        /// </summary>
        public int SelfVerificacion { get; set; }

        /// <summary>
        ///Tipo de Moeda. Tamaño = 3
        /// 01: Colones
        /// 02: Dolares
        /// 03: Dolares
        /// </summary>
        public int CodigoMoneda { get; set; }

        /// <summary>
        /// Llave de acceso (Valor de la llave de acceso del recibo). Tamaño = 30
        /// </summary>
        public string LlaveAcceso { get; set; }

        /// <summary>
        /// Periodo del recibo a pagar. Debe enviarse un formato de periodo válido ya sea (AAAAMMDD), (AAAAMM), (AAAA). Tamaño = 8
        /// </summary>
        public string PeriodoRecibo { get; set; }

        /// <summary>
        /// Fecha en que se realizo el pago, utilizar .shorter. Formato AAAAMMDD. Tamaño = 8
        /// </summary>
        public DateTime FechaPago { get; set; }

        /// <summary>
        /// Monto a pagar: 13 enteros  y dos decimales.. Tamaño = 15: 13(2)
        /// </summary>
        public double MontoImpuesto { get; set; }

        /// <summary>
        /// Monto Total del pago: 13 enteros  y dos decimales.. Tamaño = 15: 13(2)
        /// </summary>
        public double MontoPago { get; set; }

        /// <summary>
        /// Monto de la comision: 11 enteros  y dos decimales.. Tamaño = 13: 11(2)
        /// </summary>
        public double MontoComision { get; set; }

        /// <summary>
        /// Código de la SUGEF asignado al Banco Emisor del cheque. Tamaño = 4
        /// </summary>
        public int CodigoBancoEmisor { get; set; }

        /// <summary>
        /// Número de cuenta del cheque. Tamaño = 19
        /// </summary>
        public int NumeroCuenta { get; set; }

        /// <summary>
        /// Número del cheque con que se realiza el pago. Tamaño = 14
        /// </summary>
        public int NumeroCheque { get; set; }

        /// <summary>
        /// Nombre del cliente (persona física o jurídica) dueño del servicio.. Tamaño = 110
        /// </summary>
        public string NombreCliente { get; set; }

        /// <summary>
        /// Nombre del cliente (persona física o jurídica) dueño del servicio.. Tamaño = 10
        /// </summary>
        public string NumeroComprobante { get; set; }


        public static ArchivoConciliacion ParserArchivoConciliacion(string trama, ref bool listo)
        {
            var archivoconciliacion = new ArchivoConciliacion();
            archivoconciliacion.TipoRegistro = GetTipoRegistro(trama);
            if (archivoconciliacion.TipoRegistro ==1)
            {
                archivoconciliacion.CodigoEmpresa = GetCodigoEmpresa(trama);
                archivoconciliacion.CodigoConvenio = GetCodigoConvenio(trama);
                archivoconciliacion.CodigoBanco = GetCodigoBanco(trama, 8);
                archivoconciliacion.CodigoAgencia = GetCodigoAgencia(trama);
                archivoconciliacion.NumeroFactura = GetNumeroFactura(trama);
                archivoconciliacion.SelfVerificacion = GetSelfVerificacion(trama);
                archivoconciliacion.CodigoMoneda = GetCodigoMoneda(trama);
                archivoconciliacion.LlaveAcceso = GetLlaveAcceso(trama, 40);
                archivoconciliacion.PeriodoRecibo = GetCadena(trama, 70, 8);
                archivoconciliacion.FechaPago = GetFechaHoraFromTrama(trama, 78);
                archivoconciliacion.MontoImpuesto = GetMontoFromTrama(trama, 86, 15);
                archivoconciliacion.MontoPago = GetMontoFromTrama(trama, 101, 15);
                archivoconciliacion.MontoComision = GetMontoFromTrama(trama, 116, 13);
                archivoconciliacion.CodigoBancoEmisor = GetCodigoBanco(trama, 129);
                archivoconciliacion.NumeroCuenta = GetNumeroCuenta(trama);
                archivoconciliacion.NumeroCheque = GetNumeroCheque(trama);
                archivoconciliacion.NombreCliente = GetCadena(trama, 166, 110);
                archivoconciliacion.NumeroComprobante = GetCadena(trama, 276, 10);
            }
            else
            {
                listo = true;
            }


            // regitro pago = 286 caracteres
            // totales =80 letras la trama

            return archivoconciliacion;
        }

    }
}
