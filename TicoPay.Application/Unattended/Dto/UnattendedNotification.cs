using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices;
using static TicoPay.Invoices.UnattendedApi;

namespace TicoPay.Unattended.Dto
{
    public class UnattendedNotification
    {
        public string Emisor { get; set; }

        public TipoDocumento TipoDocumento { get; set; }

        public StatusDocumentUnattended Estado { get; set; }

        public string MensajeRespuesta { get; set; }

        public byte[] DocumentoXML { get; set; }

        public string Clave { get; set; }

        public string ConsecutivoXML { get; set; }

        public UnattendedApiDto UnattendedApiDto { get; set; }
    }

    public enum StatusDocumentUnattended
    {
        [Description("Enviado a Hacienda")]
        Enviado,
        [Description("Rechazado por Plataforma por error de formato")]
        Rechazado,
        [Description("Otros")]
        Otros
    }
}
