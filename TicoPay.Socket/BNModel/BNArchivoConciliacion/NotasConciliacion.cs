namespace Ticopay.BNModel.BNArchivoConciliacion
{
    public class NotasConciliacion : BnConectividadConciliacion
    {
        /// <summary>
        /// Indicador del tipo de registro . Tama�o 2
        /// 2 = Indica Nota D�bito
        /// 3 = Indica Nota Cr�dito
        /// </summary>
        public int TipoRegistro { get; set; }

        /// <summary>
        /// C�digo de la empresa (ASADA) al que pertenece el convenio. Tama�o = 3
        /// </summary>
        public int CodigoEmpresa { get; set; }

        /// <summary>
        /// C�digo de convenio del archivo batch. Tama�o = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// C�digo del ente recaudador hacia el cual es dirigido el archivo. Tama�o = 4
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// N�mero de cuenta del cliente en que se aceditan los pagos. Tama�o = 30
        /// </summary>
        public string NumeroCuenta { get; set; }

        /// <summary>
        /// �mero de la nota de cr�dito con que se deposita el monto recaudado o n�mero de la nota de 
        /// d�bito con que se debita la comisi�n por recaudaci�n (ver tipo de registro).. Tama�o 10
        /// </summary>
        public string NumeroNota { get; set; }

        /// <summary>
        /// Monto a pagar: 13 enteros  y dos decimales.. Tama�o = 20: 18(2)
        /// </summary>
        public double MontoNota { get; set; }

        /// <summary>
        /// Cantidad de pagos que se est�n acreditando y cuyo desglose se muestra en el archivo de pagos. Tama�o = 6
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