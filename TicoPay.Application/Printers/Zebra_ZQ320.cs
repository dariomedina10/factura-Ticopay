using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Printers
{
    public class Zebra_ZQ320 : Epson
    {
        private const string bR = "----------------------------------------------------\r\n";
        private const int lineLength = 52;
        private const int intLength = 26;
        private const int strLength = 26;
        public const int maxLineDetails = -1;
        public override int LineLength => lineLength;
        public override int IntLength => intLength;
        public override int StrLength => strLength;
        public override int MaxLineDetails => maxLineDetails;
        public override string BR => bR;
        public override string HeaderDetails(string str)
        {
            str += "====================================================" + CommonPrinter.NEW_LINE;
            str += "DESCRIPCION                                    TOTAL" + CommonPrinter.NEW_LINE;
            str += "==============================    ==================" + CommonPrinter.NEW_LINE;
            return str;
        }
        public override string IncludeResolucionMessage(string str)
        {
            str += BR;
            str += "       Incluido en el Registro de Facturacion       " + CommonPrinter.NEW_LINE;
            str += "            Electronica, segun normativa            " + CommonPrinter.NEW_LINE;
            str += "                    DGT-R-48-2016                   " + CommonPrinter.NEW_LINE;
            str += BR;
            return str;
        }
    }
}
