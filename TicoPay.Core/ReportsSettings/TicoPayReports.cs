using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.ReportsSettings
{
    public enum TicoPayReports
    {
        [Description("Factura Electrónica")]
        Factura,
        [Description("Nota Crédito / Débito")]
        Nota,
    }
}
