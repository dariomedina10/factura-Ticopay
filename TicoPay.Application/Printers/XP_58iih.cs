using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Printers
{
    public class XP_58iih : Epson
    {
        private const string bR = "--------------------------------\r\n";
        private const int lineLength = 32;
        private const int intLength = 16;
        private const int strLength = 16;
        public const int maxLineDetails = -1;
        public override int LineLength => lineLength;
        public override int IntLength => intLength;
        public override int StrLength => strLength;
        public override int MaxLineDetails => maxLineDetails;
        public override string BR => bR;
        public override string HeaderDetails(string str)
        {
            str += "================================" + CommonPrinter.NEW_LINE;
            str += "DESCRIPCION                TOTAL" + CommonPrinter.NEW_LINE;
            str += "================   =============" + CommonPrinter.NEW_LINE;
            return str;
        }
        public override string IncludeResolucionMessage(string str)
        {
            str += BR;
            str += "   Incluido en el Registro de   " + CommonPrinter.NEW_LINE;
            str += "    Facturacion Electronica,    " + CommonPrinter.NEW_LINE;
            str += "         segun normativa        " + CommonPrinter.NEW_LINE;
            str += "          DGT-R-48-2016         " + CommonPrinter.NEW_LINE;
            str += BR;
            return str;
        }
    }
}
