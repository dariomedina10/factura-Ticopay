using System;

namespace Ticopay.BNModel
{
    public class ConsultaRecibo : BNConectividad
    {
        /// <summary>
        /// Tipo Mensaje = 200. Tamaño = 4
        /// </summary>
        public int TipoMensaje { get; set; }

        /// <summary>
        /// Código de la SUGEF asignado al Banco Nacional. Actualmente 151. Tamaño = 5
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// Código de la agencia de donde se envió la consulta. Tamaño = 6
        /// </summary>
        public int CodigoAgencia { get; set; }

        /// <summary>
        /// 310000 Tamaño = 6 
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envíe en la respuesta. Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Número que identifica al convenio y es asignado por el Banco Nacional. Tamaño = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// Tipo de búsqueda a realizar, 0 = Cédula o identificación, 1 = Servicio. Tamaño = 1
        /// </summary>
        public string TipoBusqueda { get; set; }

        /// <summary>
        /// Tipo que identifica a la llave de búsqueda,  ya sea cédula o identificación (jurídica, física, pasaporte) o servicio(teléfono, localización, cuenta, no. cliente, etc). Tamaño = 3
        /// </summary>
        public string TipoAcceso { get; set; }

        /// <summary>
        /// Valor de la llave de acceso. Tamaño = 30
        /// </summary>
        public string LlaveAcceso { get; set; }

        /// <summary>
        /// Fecha y hora de la transacción, utilizando el formato AAAAMMDDHH24MISS. Tamaño = 14
        /// </summary>
        public DateTime FechaTransaccion { get; set; }

        /// <summary>
        /// Número de cuotas a consultar. En caso de que se requiera cancelar todo, se envía un cero(0). Se utiliza en convenios en los que no se maneja facturas si no cuotas. Tamaño = 4
        /// </summary>
        public int NumeroCouta { get; set; }

        /// <summary>
        /// Serializa la trama al objeto tipo ConsultaRecibo
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Objeto Tipo ConsultaRecibo con los datos de la trama</returns>
        public static ConsultaRecibo ParserConsultaRecibo(string trama)
        {
            var consultaRecibo = new ConsultaRecibo
            {
                TipoMensaje = 200,
                CodigoBanco = GetCodigoBanco(trama),
                CodigoAgencia = GetCodigoAgencia(trama),
                CodigoTransaccion = 310000,
                ConsecutivoTransaccion = GetConsecutivoTransaccion(trama),
                CodigoConvenio = GetCodigoConvenio(trama),
                TipoBusqueda = GetCadena(trama,30,1),
                TipoAcceso = GetCadena(trama, 31, 3),
                LlaveAcceso = GetLlaveAcceso(trama, 34),
                FechaTransaccion = GetFechaHoraFromTrama(trama, 64),
                NumeroCouta = GetNumero(trama, 78, 4),
            };
            return consultaRecibo;
        }
    }

    public enum TipoBusqueda
    {
        Identificacion = 0,
        Servicio = 1

    }
}
