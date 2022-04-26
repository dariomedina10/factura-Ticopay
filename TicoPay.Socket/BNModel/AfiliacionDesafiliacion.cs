using System;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama # 1 : Afiliación – Desafiliación de servicios  para Pago Automático de Recibos.
    /// </summary>
    public class AfiliacionDesafiliacion : BNConectividad
    {
        /// <summary>
        /// El codigo para este mensaje es 100. Tamaño = 4
        /// </summary>
        public int TipoMensaje { get; set; }

        /// <summary>
        /// Código de la SUGEF asignado al Banco Nacional. Actualmente es 151. Tamaño = 5
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// Código de la agencia del Banco Nacional  desde la cual se envía la consulta. Tamaño = 6
        /// </summary>
        public int CodigoAgencia { get; set; }

        /// <summary>
        /// 100000 : Afiliación, 100001 : Desafiliación, 100002 : Consulta(obtener datos del cliente). Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el que se envíe en la respuesta.
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Código de convenio acordado por la empresa y el  Banco Nacional de Costa Rica para ofrecer el servicio PAR. Tamaño = 6
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// Fecha y hora de la transacción. Formato AAAAMMDDHH24MISS. Tamaño = 14
        /// </summary>
        public DateTime FechaTransaccion { get; set; }

        /// <summary>
        /// Tipo que identifica la llave de servicio que se está afiliando, desafiliando o consultando. Tamaño = 3
        /// </summary>
        public string TipoAcceso { get; set; }

        /// <summary>
        /// Llave de servicio que se está afiliando, desafiliando o consultando. Tamaño = 30
        /// </summary>
        public string LlaveServicio { get; set; }

        /// <summary>
        /// Se refiere al monto máximo del débito que se autoriza para realizar el pago automático del recibo del servicio. Tamaño = 18
        /// </summary>
        public int MontoMaximo { get; set; }

        //TODO:ngonzalez --> Vericar con el BN, 1 = Vencimiento, 0 = fija
        /// <summary>
        /// Forma de pago escogida por el cliente, Vencimiento (fecha o ciclo), Fecha Fija, 1 = Vencimiento, 0 = fija. Tamaño = 1
        /// </summary>
        public int FormaPago { get; set; }

        /// <summary>
        /// Día en que se debe realizar el pago del servicio según el cliente (caso de fecha fija). Tamaño = 2
        /// </summary>
        public int DiaPago { get; set; }

        /// <summary>
        /// Serializa los datos de la trama afiliacion desafiliacion en un objeto de tipo AfiliacionDesafiliacion
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>El objeto AfiliacionDesafiliacion con los datos</returns>
        public static AfiliacionDesafiliacion ParserAfiliacionDesafiliacion(string trama)
        {
            var afiliaciondesafiliacion = new AfiliacionDesafiliacion()
            {
                TipoMensaje = 100,
                CodigoBanco = 151,
                CodigoAgencia = GetCodigoAgencia(trama),
                CodigoTransaccion = GetCodigoTransaccion(trama),
                ConsecutivoTransaccion = GetConsecutivoTransaccion(trama),
                CodigoConvenio = GetCodigoConvenio(trama),
                FechaTransaccion = GetFechaHoraFromTrama(trama, 30),
                TipoAcceso = GetCadena(trama,44, 3),
                LlaveServicio = GetLlaveAcceso(trama, 47),
                MontoMaximo = GetNumero(trama, 77, 18),
                FormaPago = GetNumero(trama, 95, 1),
                DiaPago = GetNumero(trama,96, 2)
            };
            return afiliaciondesafiliacion;
        }
    }
}
