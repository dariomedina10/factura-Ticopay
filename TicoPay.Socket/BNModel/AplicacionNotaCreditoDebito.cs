using System;
using System.Collections.Generic;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama N°7:  Aplicación de notas de crédito (depósito) y débito (comisión).
    /// </summary>
    public class AplicacionNotaCreditoDebito : BNConectividad
    {
        /// <summary>
        /// El codigo para este mensaje es 500, Tamaño = 4
        /// </summary>
        public int TipoMensaje { get; set; }

        /// <summary>
        /// Código de la SUGEF asignado al Banco Nacional. Actualmente 151. Tamaño = 5
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// Código de la oficina desde donde se envió los movimientos. Tamaño = 6
        /// </summary>
        public int CodigoAgencia { get; set; }

        /// <summary>
        /// 210000. Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envíe en la respuesta de transacción de aplicación. Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Número que identifica al convenio y es asignado por el Banco Nacional. Tamaño = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// Fecha correspondiente a las notas de crédito y débito con las que se realiza los movimientos. Tamaño = 8
        /// </summary>
        public DateTime FechaNota { get; set; }

        /// <summary>
        /// Total de pagos recaudados. Tamaño = 6
        /// </summary>
        public int TotalPagos { get; set; }

        /// <summary>
        /// Monto total recaudado. El punto decimal no debe ser especificado. Tamaño = 20(2)
        /// </summary>
        public double TotalMontoPagos { get; set; }

        /// <summary>
        /// Número de la nota del débito por comisión. Tamaño = 15
        /// </summary>
        public int NumeroNotaDebito { get; set; }

        /// <summary>
        /// Monto total por comisiones. El punto decimal no debe ser especificado. Tamaño = 15(2)
        /// </summary>
        public double TotalMontoComisiones { get; set; }

        /// <summary>
        /// Cuenta bancaria a la que se le realizó el débito de las comisión ganada por el recaudador.Tamaño = 25
        /// </summary>
        public int CuentaBancariaNotaDebito { get; set; }

        /// <summary>
        /// Número de movimientos de crédito para depósitos (es factible realizar el depósito de fracciones del total a diversas cuentas). Tamaño = 2
        /// </summary>
        public int NumeroMovimientosCredito { get; set; }

        /// <summary>
        /// Código del primer grupo de rubros que sumariza el depósito de recaudación. Estos son definidos por acuerdo común. Tamaño = 3 
        /// </summary>
        public int CodigoGrupoRubros { get; set; }

        /// <summary>
        /// Recaudaciones
        /// </summary>
        public List<Recaudacion> Recaudaciones { get; set; }

        public static AplicacionNotaCreditoDebito ParserAplicacionNotaCD(string trama)
        {
            var aplicacionNotaCD = new AplicacionNotaCreditoDebito
            {
                TipoMensaje = 500,
                CodigoBanco = 151,
                CodigoAgencia = GetCodigoAgencia(trama),
                CodigoTransaccion = 210000,
                ConsecutivoTransaccion = GetConsecutivoTransaccion(trama),
                CodigoConvenio = GetCodigoConvenio(trama),
                FechaNota = GetFechaFromTrama(trama, 30),
                TotalPagos = GetNumero(trama, 38, 6),
                TotalMontoPagos = GetMontoFromTrama(trama, 44, 20),
                NumeroNotaDebito = GetNumero(trama, 64, 15),
                TotalMontoComisiones = GetMontoFromTrama(trama, 79, 15),
                CuentaBancariaNotaDebito = GetNumero(trama, 94, 25),
                NumeroMovimientosCredito = GetNumero(trama, 119, 2),
                CodigoGrupoRubros = GetNumero(trama, 121, 3),
            };
            return aplicacionNotaCD;
        }
    }
}
