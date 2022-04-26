namespace Ticopay.BNModel.BNArchivoConciliacion
{
    public class NotasConciliacion : BnConectividadConciliacion
    {
        /// <summary>
        /// Indicador del tipo de registro . Tamaño 2
        /// 2 = Indica Nota Débito
        /// 3 = Indica Nota Crédito
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
        /// Número de cuenta del cliente en que se aceditan los pagos. Tamaño = 30
        /// </summary>
        public string NumeroCuenta { get; set; }

        /// <summary>
        /// úmero de la nota de crédito con que se deposita el monto recaudado o número de la nota de 
        /// débito con que se debita la comisión por recaudación (ver tipo de registro).. Tamaño 10
        /// </summary>
        public string NumeroNota { get; set; }

        /// <summary>
        /// Monto a pagar: 13 enteros  y dos decimales.. Tamaño = 20: 18(2)
        /// </summary>
        public double MontoNota { get; set; }

        /// <summary>
        /// Cantidad de pagos que se están acreditando y cuyo desglose se muestra en el archivo de pagos. Tamaño = 6
        /// </summary>
        public int CantidadPagos { get; set; }




        public static NotasConciliacion Parsernotasconciliacion(string trama, ref bool listo)
        {
            var notasconciliacion = new NotasConciliacion();
            notasconciliacion.TipoRegistro = GetTipoRegistro(trama);
            notasconciliacion.CodigoEmpresa = GetCodigoEmpresa(trama);
            notasconciliacion.CodigoConvenio = GetCodigoConvenio(trama);
            notasconciliacion.CodigoBanco = GetCodigoBanco(trama, 8);
            notasconciliacion.NumeroCuenta = GetCadena(trama, 12, 30);
            notasconciliacion.NumeroNota = GetCadena(trama, 42, 10);
            notasconciliacion.MontoNota = GetMontoFromTrama(trama, 52, 20);
            notasconciliacion.CantidadPagos = GetCantidadPagos(trama);



            // regitro pago = 286 caracteres
            // totales =80 letras la trama

            return notasconciliacion;
        }
    }
}